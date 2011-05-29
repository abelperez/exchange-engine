
// Copyright (C) 2011 Mindplex Media, LLC.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
// file except in compliance with the License. You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.

#region Imports

using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Messaging;
using System.Web;

using Threading = System.Threading;
using Collections = System.Collections;
using Regex = System.Text.RegularExpressions;
using Cryptography = System.Security.Cryptography;
using DirectoryServices = System.DirectoryServices;
using Compilers = _1800Communications.Utils.Compilers;
using System.Globalization;
using Microsoft.Win32;
using HexValidEmailLib;
using Base32Encoding;

using _1800Communications.AggregationSystem._CoreLibrary;

#endregion

namespace InternetBrands.Core.Event.Rule
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public abstract class EventLanguageCompiler : EventResource
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        protected Random randomNumberGenerator = new Random();

        /// <summary>
        /// The node all configuration of this element is stored in.  Typically this
        /// would be a direct child of the DocumentElement, though that is not necessary.
        /// </summary>
        /// 
        protected XmlElement ruleNode = null;

        /// <summary>
        /// SOAP Namespace URN
        /// </summary>
        /// 
        protected const string SoapURN = "http://schemas.xmlsoap.org/soap/envelope/";

        /// <summary>
        /// Encryption key used when none is provided to an Encrypt/Decrypt routine
        /// </summary>
        /// 
        private const string DefaultEncryptionKey = "a8gR3Ge+";

        /// <summary>
        /// This is what's reported any time the tool needs to identify itself
        /// (for logging, monitoring, etc.)
        /// </summary>
        /// 
        protected string ruleName = "Tool Name Not Set";

        /// <summary>
        /// This is set by the ErrorHandler so that it doesn't overwrite any LastQueue values
        /// </summary>
        /// 
        protected bool silent = false;

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="ruleNode"></param>
        /// 
        public EventLanguageCompiler(XmlElement ruleNode)
        {
            this.ruleNode = ruleNode;
        }

        #region GetContext

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        private EventRuleContext GetContext()
        {
            EventRuleContext context = new EventRuleContext(null);

            // Set up a name table so we can query based on namespaces
            NameTable SoapNameTable = new NameTable();
            XmlNamespaceManager SoapNamespaceManager = new XmlNamespaceManager(SoapNameTable);
            SoapNamespaceManager.AddNamespace(String.Empty, "urn:none");
            SoapNamespaceManager.AddNamespace("soap", SoapURN);

            // This allows custom namespaces as defined in the config xml
            foreach (XmlElement NamespaceNode in ruleNode.SelectNodes("namespaces/namespace"))
            {
                string NamespaceName = "";
                string NamespaceURN = "urn:none";
                if (NamespaceNode.GetAttributeNode("name") != null)
                {
                    NamespaceName = NamespaceNode.GetAttribute("name");
                }
                if (NamespaceNode.InnerText != "")
                {
                    NamespaceURN = NamespaceNode.InnerText;
                }
                SoapNamespaceManager.AddNamespace(NamespaceName, NamespaceURN);
            }

            context.SoapNameTable = SoapNameTable;
            context.SoapNamespaceManager = SoapNamespaceManager;
            return context;
        }

        #endregion

        #region Invoke

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="EventNode"></param>
        /// 
        public virtual void Invoke(XmlElement EventNode)
        {
            string ruleLabel = HttpUtility.HtmlAttributeEncode(EventNode.GetAttribute("id"));
            string messageBody = "<soap:Envelope xmlns:soap='" + SoapURN + "' soap:id='" + NewGuid + "'><soap:Header/><soap:Body><dataset id='" + ruleLabel + "'>" + EventNode.InnerXml + "</dataset></soap:Body></soap:Envelope>";

            Invoke(messageBody, ruleLabel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="eventXml"></param>
        /// <param name="ruleLabel"></param>
        /// 
        public virtual void Invoke(string eventXml, string ruleLabel)
        {
            EventRuleContext context = GetContext();
            XmlDocument SoapMessage = new XmlDocument(context.SoapNameTable);
            context.Label = ruleLabel;

            try
            {
                // Change this from a loop to get single attribute in one call.
                foreach (ListenerAttribute Att in this.GetType().GetCustomAttributes(typeof(ListenerAttribute), true))
                {
                    SoapMessage.LoadXml(eventXml);
                    context.SoapMessage = SoapMessage;

                    if (Att.Contributes) // default is true when attribute value is not specified.
                    {
                        if (ApplyRule(context))
                        {
                            SendApplication(context);
                        }
                    }
                    else
                    {
                        SendApplication(context);
                    }
                }
            }
            catch (Exception exception)
            {
                if (Logger.IsErrorEnabled)
                {
                    LogError(exception, "failed to receive message: {0}.", exception.Message);
                }
            }
        }

        #endregion

        #region Apply Rule

        /// <summary>
        /// This routine takes a message, finds the appropriate actions, executes
        /// any necessary stylesheet, and executes each in turn.
        /// </summary>
        /// 
        /// <param name="context"></param>
        /// 
        /// <returns></returns>
        /// 
        private bool ApplyRule(EventRuleContext context)
        {
            XmlNodeList actions = ruleNode.SelectNodes("action");
            bool OKToSend = true;
            if (actions.Count == 0)
            {
                OKToSend = ExecuteAction(context, null, null);
            }
            else
            {
                foreach (XmlElement action in actions)
                {
                    if (CheckTests(action, context.SoapMessage, context.SoapNamespaceManager))
                    {
                        foreach (XmlElement actionDescription in action.SelectNodes("*"))
                        {
                            XmlElement actionCommand = null;
                            switch (actionDescription.Name)
                            {
                                case "test":
                                    break;
                                case "xsl:stylesheet":
                                    try
                                    {
                                        actionCommand = Transform(context.SoapMessage, actionDescription);
                                    }
                                    catch (System.Exception exception)
                                    {
                                        if (Logger.IsErrorEnabled)
                                        {
                                            LogError(exception, "failed to transform: {0}.", exception.Message);
                                        }
                                        actionCommand = null;
                                    }
                                    break;
                                default:
                                    actionCommand = actionDescription;
                                    break;
                            }

                            if (actionCommand != null)
                            {
                                // look for nested action node.
                                if (actionCommand.Name == "action")
                                {
                                    foreach (XmlElement tempAction in actionCommand.SelectNodes("*"))
                                    {
                                        if (CheckTests(tempAction, context.SoapMessage, context.SoapNamespaceManager))
                                        {
                                            // process nested action
                                            OKToSend = OKToSend && ExecuteAction(context, tempAction, action);
                                        }
                                    }
                                }
                                else if (CheckTests(actionCommand, context.SoapMessage, context.SoapNamespaceManager))
                                {
                                    // process action
                                    OKToSend = OKToSend && ExecuteAction(context, actionCommand, action);
                                }
                            }
                        }
                    }
                }
            }
            return OKToSend;
        }

        #endregion

        #region Execute Action

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="context"></param>
        /// <param name="Action"></param>
        /// <param name="ActionParent"></param>
        /// 
        /// <returns></returns>
        /// 
        public virtual bool ExecuteAction(EventRuleContext context, XmlElement InnerAction, XmlElement ActionParent)
        {
            XmlElement NewChunk = null;
            string UpdateMessage = "";

            Exception Error = null;

            int retryCount = 1;

            // Find out how many times to try this action in case of exception (default is 1)
            if (ActionParent != null && ActionParent.GetAttributeNode("retrycount") != null)
            {
                try
                {
                    retryCount = int.Parse(ActionParent.GetAttribute("retrycount"));
                }
                catch
                {
                }
            }

            // Try to process the application
            for (int x = 0; x < retryCount; x++)
            {
                try
                {
                    Error = null;
                    NewChunk = ProcessApplication(context, InnerAction, ref UpdateMessage);
                    break;
                }
                catch (Exception exception)
                {
                    Error = exception;
                    if (Logger.IsErrorEnabled)
                    {
                        LogError(exception, "failed to process application: {0}.", exception.Message);
                    }
                }
            }

            if (Error != null)
            {
                // If there were errors all [RetryCount] times, send an error message
                //Error.Send(_ErrorQueueName);
                //MonitorSend(MessageType.Error, "", _ErrorQueueName, null);
                return (ActionParent.GetAttribute("failure") == "continue");
            }
            else
            {
                // If it succeeded at all...
                string IncomingQueuePath = "";

                //if (IncomingQueue != null)
                //{
                //    IncomingQueuePath = IncomingQueue.Path;
                //}

                // if there was data back, stamp it in the app
                if (NewChunk != null)
                {
                    // Console.WriteLine("CHUNK: " + NewChunk.OuterXml);

                    string actionId = null;
                    bool blankDatasetId = false;

                    if (ActionParent != null)
                    {
                        actionId = ActionParent.GetAttribute("id");
                        if (ActionParent.HasAttribute("allowblankdatasetid"))
                        {
                            try
                            {
                                blankDatasetId = Convert.ToBoolean(ActionParent.GetAttribute("allowblankdatasetid"));
                            }
                            catch
                            {
                                //ignore, default to false
                            }
                        }
                    }
                    StampApplication(context, actionId, blankDatasetId, UpdateMessage, IncomingQueuePath, NewChunk, true, (ActionParent.GetAttribute("results").ToLower() != "ignore"));
                }
                return true;
            }
        }

        #endregion

        #region Send Applications

        /// <remarks>
        /// Once the processing of a message is complete, it needs to be sent to the next
        /// component via another message queue.  SendApplication() does just that.  The
        /// message is sent out to each queue as specified by the configuration  At this 
        /// point a brief description of the queue naming system is merited:
        /// 
        /// All queues are by default expected to by Private queues on the local machine.  
        /// This is set via the _BaseQueue class level variable, which should be done in
        /// the inherited class configuration method.  The default _BaseQueue is:
        ///		".\\PRIVATE$\\Aggregator."
        /// 
        /// Message queue names consist of a path delineated by "." characters.  Thus,
        ///		".\\PRIVATE$\\Aggregator.Incoming.Complete.AutoLoan"
        ///	is a typical queue name.  By default, when outputing to a queue, the name of the
        ///	queue is determined by apending a new name onto the incoming queue.  Thus, if a
        ///	step of the process outputs to "Processed" and the incoming queue was the one
        ///	specified above, then the message would be output to:
        ///		".\\PRIVATE$\\Aggregator.Incoming.Complete.AutoLoan.Processed"
        ///	This allows "queue affinity".  Suppose we have some step which is very generic, and
        ///	executed on several different queues.  This step defines it's output queue as "C"
        ///	and listens to queues "A" and "B".  With an absolute path, the results would be put
        ///	out to queue "C".  However, with relative paths, the results go out to either "A.C"
        ///	or "B.C", thus it's tied to the incoming queue.
        ///	
        ///	The same logic applies as the number of incoming and outgoing queues increases.  If 
        ///	this same process listens to queues "A", "B", and "C" and outputs to queues "D" and 
        ///	"E", then the results would be output to ("A.D" and "A.E") or ("B.D" and "B.E") or 
        ///	("C.D" and "C.E").
        ///	
        ///	This behavior can be overridden by specifying that the output queue is an absolute 
        ///	path.
        /// 
        /// This routine may be overridden as necessary, though it is not recommended.
        /// </remarks>
        protected virtual void SendApplication(EventRuleContext context)
        {
            // Gotta look at all nodes so we catch any XSL
            // Note, this is uber-gay and needs to be done differently.  Maybe XSL in the 
            // destinations, but that breaks some aspects of the overall schema
            foreach (XmlElement ChildElement in ruleNode.SelectNodes("*"))
            {
                XmlElement xeDestination = ChildElement;
                // If this is XSL, transform it and use that as our context
                if (xeDestination.Name == "xsl:stylesheet")
                {
                    xeDestination = Transform(context.SoapMessage, ChildElement);
                }
                if (xeDestination.Name == "destinations")
                {
                    SendApplication(xeDestination, context);
                }
            }
        }

        protected virtual void SendApplication(XmlElement xeDestinationNode, EventRuleContext context)
        {
            // if we're looking at a "destinations" node, we should find info on delivery
            if (CheckTests(xeDestinationNode, context.SoapMessage, context.SoapNamespaceManager))
            {
                // timeout stuff here
                TimeSpan tsTimeout = TimeSpan.Zero;
                XmlElement xeTimeout = (XmlElement)xeDestinationNode.SelectSingleNode("timeout");
                if (xeTimeout != null)
                {
                    tsTimeout = new TimeSpan(
                        int.Parse("0" + GetParameter("days", xeTimeout, context.SoapMessage, context.SoapNamespaceManager)),
                        int.Parse("0" + GetParameter("hours", xeTimeout, context.SoapMessage, context.SoapNamespaceManager)),
                        int.Parse("0" + GetParameter("minutes", xeTimeout, context.SoapMessage, context.SoapNamespaceManager)),
                        int.Parse("0" + GetParameter("seconds", xeTimeout, context.SoapMessage, context.SoapNamespaceManager)),
                        int.Parse("0" + GetParameter("milliseconds", xeTimeout, context.SoapMessage, context.SoapNamespaceManager))
                        );
                    XmlElement SoapRouting = context.SoapMessage.CreateElement("soap", "Routing", SoapURN);
                    XmlElement SoapLabel = context.SoapMessage.CreateElement("soap", "Label", SoapURN);
                    SoapLabel.AppendChild(context.SoapMessage.CreateTextNode(context.Label));
                    SoapRouting.AppendChild(SoapLabel);
                    context.SoapMessage.SelectSingleNode("soap:Envelope", context.SoapNamespaceManager).AppendChild(SoapRouting);
                    foreach (XmlElement DestinationQueue in xeTimeout.SelectNodes("queue"))
                    {
                        if (CheckTests(DestinationQueue, context.SoapMessage, context.SoapNamespaceManager))
                        {
                            string QueuePath = GetParameter("", DestinationQueue, context.SoapMessage.SelectSingleNode("*"), context.SoapNamespaceManager);
                            //QueuePath = ResolveQueuePath(QueuePath, DestinationQueue, sIncomingQueueName);
                            //if (QueuePath != "")
                            //{
                            XmlElement DestinationNode = context.SoapMessage.CreateElement("soap", "Destination", SoapURN);
                            DestinationNode.AppendChild(context.SoapMessage.CreateTextNode("QueuePath")); // me
                            SoapRouting.AppendChild(DestinationNode);
                            //}
                        }
                    }
                }

                foreach (XmlElement DestinationQueue in xeDestinationNode.SelectNodes("queue"))
                {
                    // loop over all queues in the destination. All test conditions on the destinations element
                    // have already been performed so get the queue path.
                    string sQueuePath = GetParameter("", DestinationQueue, context.SoapMessage.SelectSingleNode("*"), context.SoapNamespaceManager).Trim();
                    if (Logger.IsInfoEnabled)
                    {
                        LogInfo("destination: {0}.", sQueuePath);
                    }

                    // If we actually have a path...
                    if (sQueuePath != "")
                    {
                        //sQueuePath = ResolveQueuePath(sQueuePath, DestinationQueue, sIncomingQueueName);
                        // Locate the queue we've settled on
                        //MessageQueue CorrectQueue = null;
                        //try
                        //{
                        //    CorrectQueue = FindQueue(sQueuePath);
                        //    // This would only be null if there's a catastrophic queue creation or naming error
                        //    if (CorrectQueue != null)
                        //    {
                        //        lock (CorrectQueue)
                        //        {
                        try
                        {
                            // The Last Queue is useful for running errors back through
                            context.SoapMessage.DocumentElement.RemoveAttribute("lastqueue");
                            if (!silent)
                            {
                                context.SoapMessage.DocumentElement.SetAttribute("lastqueue", sQueuePath);
                            }
                            
                            // Create and send the message
                            //Message AppMessage = new Message();
                            //try
                            //{
                            //    //set timeout here
                            //    if (tsTimeout != TimeSpan.Zero)
                            //    {
                            //        AppMessage.TimeToBeReceived = tsTimeout;
                            //        AppMessage.UseDeadLetterQueue = true;
                            //    }
                            //    AppMessage.Body = context.SoapMessage.DocumentElement;
                            //    AppMessage.Label = context.Label;
                            //    AppMessage.Recoverable = true;
                            //    CorrectQueue.Send(AppMessage);
                            if (Logger.IsInfoEnabled)
                            {
                                LogInfo("sending message: {0}.", context.SoapMessage.DocumentElement.OuterXml);
                            }
                        }
                        finally
                        {
                            //AppMessage.Dispose();
                        }
                    }
                }
            }
        }

        #endregion

        #region Stamp Application Header

        /// <summary>
        /// Stamps the application with Header information and associates that with the data
        /// being added to the application.
        /// </summary>
        /// 
        /// <param name="SoapMessage">The Application</param>
        /// <param name="SoapNamespaceManager">Namespace Manager for the Application</param>
        /// <param name="ActionID">ID for this action</param>
        /// <param name="bBlankDatasetID">A flag that says whether to allow a blank dataset ID</param>
        /// <param name="UpdateMessage">Components Message about what was done</param>
        /// <param name="IncomingQueuePath">Queue this message was read from</param>
        /// <param name="NewChunk">The chunk to add to the application</param>
        /// <param name="WriteHeader">Should the header be written</param>
        /// <param name="WriteData">Should the data be written</param>
        /// 
        /// <returns></returns>
        /// 
        public XmlElement StampApplication(EventRuleContext context, string ActionID, bool bBlankDatasetID, string UpdateMessage, string IncomingQueuePath, XmlNode NewChunk, bool WriteHeader, bool WriteData)
        {
            // Don't bother if the app doesn't exist
            if (context.SoapMessage != null)
            {
                if (ActionID == null)
                {
                    ActionID = "";
                }
                if (ActionID == "" && !bBlankDatasetID)
                {
                    ActionID = "[Unspecified]";
                }

                // Set an index so this ActionID can be uniquely identified
                int Index = context.SoapMessage.SelectNodes("/soap:Envelope/soap:Body/dataset[@id='" + ActionID + "']", context.SoapNamespaceManager).Count + 1;

                // Try to write the header data
                int ActorID = -1;
                if (WriteHeader)
                {
                    ActorID = MarkUsersApplication(context.SoapMessage, context.SoapNamespaceManager, UpdateMessage, IncomingQueuePath);
                }
                // Try to write the NewChunk
                if (WriteData)
                {
                    XmlElement NewDataSet = context.SoapMessage.CreateElement("dataset");
                    NewDataSet.SetAttribute("id", ActionID);
                    NewDataSet.SetAttribute("index", Index.ToString());
                    NewDataSet.SetAttribute("tool", ruleName);
                    // If there was no actor, don't write the attribute
                    if (ActorID > 0)
                    {
                        NewDataSet.SetAttribute("actor", ActorID.ToString());
                    }
                    // If there was actually a new chunk, slap it in there
                    if (NewChunk != null)
                    {
                        try
                        {
                            XmlNode xnNamespacesStripped = NewDataSet.OwnerDocument.ReadNode(new XmlTextReader(RemoveNamespaces(NewChunk), XmlNodeType.Element, null));
                            NewDataSet.AppendChild(xnNamespacesStripped);
                        }
                        catch (Exception exx)
                        {
                            //ReportErrorNow(SoapMessage, exx);
                            NewDataSet.AppendChild(NewDataSet.OwnerDocument.ImportNode(NewChunk, true));
                        }
                    }
                    // Add to the Soap Message
                    XmlNode Target = context.SoapMessage.SelectSingleNode("/soap:Envelope/soap:Body", context.SoapNamespaceManager);
                    if (Target == null)
                    {
                        Target = context.SoapMessage.DocumentElement;
                    }
                    Target.AppendChild(NewDataSet);
                    return NewDataSet;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Get Parameters

        /// <summary>
        /// This runs GetParameter over all Nodes which match the ParameterName XPath and returns an array of results
        /// </summary>
        /// <param name="ParameterName">XPath to search</param>
        /// <param name="Context">The XML to look at</param>
        /// <param name="Application">The Soap Message being processed</param>
        /// <param name="SoapNamespaceManager">The Namespace Manager for any Namespaced XPaths</param>
        /// <returns></returns>
        public string[] GetParameters(string ParameterName, XmlNode Context, XmlNode Application, XmlNamespaceManager SoapNamespaceManager)
        {
            return GetParameters("", Context.SelectNodes(ParameterName), Application, SoapNamespaceManager);
        }


        /// <summary>
        /// This runs GetParameter over all Nodes passed in after following the supplied XPath and returns an array of results
        /// </summary>
        /// <param name="ParameterName">Relative XPath to look at</param>
        /// <param name="Context">All nodes to check</param>
        /// <param name="Application">The Soap Message being processed</param>
        /// <param name="SoapNamespaceManager">The Namespace Manager for any Namespaced XPaths</param>
        /// <returns></returns>
        public string[] GetParameters(string ParameterName, XmlNodeList Context, XmlNode Application, XmlNamespaceManager SoapNamespaceManager)
        {
            string[] Result = new string[Context.Count];

            for (int I = 0; I < Context.Count; I++)
                Result[I] = GetParameter(ParameterName, Context[I], Application, SoapNamespaceManager);

            return Result;
        }

        /// <summary>
        /// GetParameter() is used pretty widely.  It's a generic "GetString" style routine 
        /// which builds a string from text data as well as various instructions found in various
        /// xml files.
        /// 
        /// This routine can not be overridden, but should be expanded as necessary while 
        /// maintaining backwards compatibility to add necessary functionality.  Should a 
        /// particular tool need custom extensions, these can be added to GetCustomParameter().
        /// </summary>
        /// 
        /// <param name="ParameterName">XPath to search</param>
        /// <param name="Context">The XML to look at</param>
        /// <param name="Application">The Soap Message being processed</param>
        /// <param name="SoapNamespaceManager">The Namespace Manager for any Namespaced XPaths</param>
        /// 
        /// <returns></returns>
        /// 
        public string GetParameter(string ParameterName, XmlNode Context, XmlNode Application, XmlNamespaceManager SoapNamespaceManager)
        {
            string Result = "";
            XmlNode TargetNode = null;
            // If a parameter is passed, target that XPath search, otherwise use the current context
            if (ParameterName != "")
            {
                TargetNode = Context.SelectSingleNode(ParameterName);
            }
            else
            {
                TargetNode = Context;
            }
            // Found no data so don't bother going on
            if (TargetNode == null)
            {
                return "";
            }
            // Look at all children for data
            foreach (XmlNode ChildNode in TargetNode.ChildNodes)
            {
                try
                {
                    // If this is an element which is (or represents) plain text, just copy that text to the results
                    if ((ChildNode.NodeType == XmlNodeType.Text) || (ChildNode.NodeType == XmlNodeType.CDATA) || (ChildNode.NodeType == XmlNodeType.EntityReference))
                    {
                        Result = Result + ChildNode.InnerText;
                    }
                    else if (ChildNode.NodeType == XmlNodeType.Element)
                    {
                        // Elements all need to be processed
                        string Format = "";
                        string ChildText = "";
                        string XPath = "";
                        XmlNode XPathResultNode = null;
                        XmlNodeList TempNodeListResult = null;
                        XmlElement ChildElement = (XmlElement)ChildNode;
                        switch (ChildNode.Name)
                        {
                            #region xpath
                            // This runs an XPath against the Application and gets a single text result
                            case "xpath":
                                {
                                    XPath = GetParameter("", ChildElement, Application, SoapNamespaceManager).Trim();
                                    switch (ChildElement.GetAttribute("index").ToLower())
                                    {
                                        case "":
                                        case "first":
                                        case "1":
                                            // Optimizations for the first item
                                            XPathResultNode = Application.SelectSingleNode(XPath, SoapNamespaceManager);
                                            break;
                                        case "last":
                                            // The Last Item
                                            TempNodeListResult = Application.SelectNodes(XPath, SoapNamespaceManager);
                                            XPathResultNode = TempNodeListResult[TempNodeListResult.Count - 1];
                                            break;
                                        default:
                                            // This Nth Item
                                            TempNodeListResult = Application.SelectNodes(XPath, SoapNamespaceManager);
                                            try
                                            {
                                                int Index = int.Parse(ChildElement.GetAttribute("index"));
                                                if (Index < 0) Index = TempNodeListResult.Count + Index;
                                                XPathResultNode = TempNodeListResult[Index];
                                            }
                                            catch
                                            {
                                                XPathResultNode = null;
                                            }
                                            break;
                                    }
                                    if (XPathResultNode != null)
                                    {
                                        Result = Result + XPathResultNode.InnerText;
                                    }
                                    break;
                                }
                            #endregion
                            #region node
                            case "node":
                                XPath = GetParameter("", ChildElement, Application, SoapNamespaceManager).Trim();
                                switch (ChildElement.GetAttribute("index").ToLower())
                                {
                                    case "":
                                    case "first":
                                    case "1":
                                        XPathResultNode = Application.SelectSingleNode(XPath, SoapNamespaceManager);
                                        break;
                                    case "last":
                                        TempNodeListResult = Application.SelectNodes(XPath, SoapNamespaceManager);
                                        XPathResultNode = TempNodeListResult[TempNodeListResult.Count - 1];
                                        break;
                                    default:
                                        TempNodeListResult = Application.SelectNodes(XPath, SoapNamespaceManager);
                                        try
                                        {
                                            XPathResultNode = TempNodeListResult[int.Parse(ChildElement.GetAttribute("index"))];
                                        }
                                        catch
                                        {
                                            XPathResultNode = null;
                                        }
                                        break;
                                }

                                if (XPathResultNode != null)
                                {
                                    switch (ChildElement.GetAttribute("method").ToLower())
                                    {
                                        case "inner":
                                        case "innerxml":
                                            Result = Result + XPathResultNode.InnerXml;
                                            break;
                                        case "innertext":
                                            Result = Result + XPathResultNode.InnerText;
                                            break;
                                        case "outer":
                                        case "outerxml":
                                        default:
                                            Result = Result + XPathResultNode.OuterXml;
                                            break;
                                    }
                                }
                                break;
                            #endregion
                            #region count
                            case "count":
                                XPath = GetParameter("", ChildElement, Application, SoapNamespaceManager).Trim();
                                Result = Result + Application.SelectNodes(XPath, SoapNamespaceManager).Count;
                                break;
                            #endregion
                            #region machine
                            case "machine":
                                Result = Result + System.Net.Dns.GetHostName();
                                break;
                            #endregion
                            #region hash
                            case "hash":
                                Result = Result + System.Convert.ToBase64String(new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(GetParameter("", ChildElement, Application, SoapNamespaceManager).Trim().ToLower())));
                                break;
                            #endregion
                            #region date
                            case "date":
                                Format = ChildElement.GetAttribute("format");
                                if (Format == null || Format == "") Format = "yyyymmdd";
                                goto case "datetime";
                            #endregion
                            #region time
                            case "time":
                                Format = ChildElement.GetAttribute("format");
                                if (Format == null || Format == "") Format = "hhnnss";
                                goto case "datetime";
                            #endregion
                            #region datetime
                            case "datetime":
                                if (Format == null || Format == "") Format = ChildElement.GetAttribute("format");
                                if (Format == null || Format == "") Format = "yyyymmddhhnnss";
                                DateTime TargetDate;
                                ChildText = GetParameter("", ChildElement, Application, SoapNamespaceManager);
                                try
                                {
                                    if (ChildText.Length == 14)
                                    {
                                        TargetDate = new DateTime(
                                            int.Parse(ChildText.Substring(0, 4)),
                                            int.Parse(ChildText.Substring(4, 2)),
                                            int.Parse(ChildText.Substring(6, 2)),
                                            int.Parse(ChildText.Substring(8, 2)),
                                            int.Parse(ChildText.Substring(10, 2)),
                                            int.Parse(ChildText.Substring(12, 2)));
                                    }
                                    else
                                    {
                                        TargetDate = DateTime.Parse(ChildText);
                                    }
                                }
                                catch
                                {
                                    if ((ChildText != "") && (ChildText != null))
                                    {
                                        try
                                        {
                                            TargetDate = new DateTime(int.Parse(ChildText.Substring(0, 4)),
                                                int.Parse(ChildText.Substring(4, 2)),
                                                int.Parse(ChildText.Substring(6, 2)),
                                                int.Parse(ChildText.Substring(8, 2)),
                                                int.Parse(ChildText.Substring(10, 2)),
                                                int.Parse(ChildText.Substring(12, 2)));
                                        }
                                        catch
                                        {
                                            TargetDate = DateTime.Now;
                                        }
                                    }
                                    else
                                    {
                                        TargetDate = DateTime.Now;
                                    }
                                }
                                Format = Format.Replace("yyyy", String.Format("{0:000#}", TargetDate.Year));
                                Format = Format.Replace("yy", String.Format("{0:0#}", TargetDate.Year % 100));
                                Format = Format.Replace("mm", String.Format("{0:0#}", TargetDate.Month));
                                Format = Format.Replace("dd", String.Format("{0:0#}", TargetDate.Day));
                                Format = Format.Replace("hh", String.Format("{0:0#}", TargetDate.Hour));
                                Format = Format.Replace("nn", String.Format("{0:0#}", TargetDate.Minute));
                                Format = Format.Replace("ss", String.Format("{0:0#}", TargetDate.Second));
                                Result = Result + Format;
                                break;
                            #endregion
                            #region section
                            case "section":
                                if (CheckTests(ChildElement, Application, SoapNamespaceManager))
                                {
                                    string NewContext = GetParameter("context", ChildElement, Application, SoapNamespaceManager);
                                    if ((NewContext == "") || (Context == null))
                                    {
                                        Result = Result + GetParameter("", ChildElement, Application, SoapNamespaceManager);
                                    }
                                    else
                                    {
                                        XmlDocument TempXml = new XmlDocument();
                                        TempXml.LoadXml(NewContext);

                                        Result = Result + GetParameter("", ChildElement, TempXml, null);
                                    }
                                }
                                break;
                            #endregion
                            #region replace
                            //								This was a pretty heterosexually challenged element
                            //								It was too hard to document, so it was removed
                            //
                            //								case "replace":
                            //								{
                            //									Xml.XmlNode TempDoc = Application.CloneNode(true);
                            //									XPath = GetParameter("location", ChildElement, Application, SoapNamespaceManager);
                            //									string NewText = GetParameter("newdata", ChildElement, Application, SoapNamespaceManager);
                            //									string XMLContext = GetParameter("context", ChildElement, Application, SoapNamespaceManager);
                            //									string Method = ChildElement.GetAttribute("method");
                            //									string Index = ChildElement.GetAttribute("index");
                            //									Xml.XmlDocument NewDoc = new Xml.XmlDocument();
                            //
                            //									Xml.NameTable           ResultSoapNameTable        = new Xml.NameTable();
                            //									Xml.XmlNamespaceManager ResultSoapNamespaceManager = new Xml.XmlNamespaceManager(ResultSoapNameTable);
                            //			
                            //									ResultSoapNamespaceManager.AddNamespace(String.Empty, "urn:none");
                            //									ResultSoapNamespaceManager.AddNamespace("soap",       _SoapURN);
                            //
                            //									Xml.XmlDocument ResultDoc = new Xml.XmlDocument(ResultSoapNameTable);
                            //
                            //									if (Method.ToLower() == "xml")
                            //										NewDoc.LoadXml(NewText);
                            //
                            //									if ((XMLContext == "") || (XMLContext == null))
                            //										ResultDoc.LoadXml(Application.OuterXml);
                            //									else
                            //										ResultDoc.LoadXml(XMLContext);
                            //
                            //									switch (Index.ToLower())
                            //									{
                            //										case "first":
                            //										case "1":
                            //										{
                            //											Xml.XmlNode BadNode = ResultDoc.SelectSingleNode(XPath, SoapNamespaceManager);
                            //											if (Method == "xml")
                            //												BadNode.ParentNode.ReplaceChild(BadNode.OwnerDocument.ImportNode(NewDoc.DocumentElement, true), BadNode);
                            //											else
                            //											{
                            //												switch (BadNode.NodeType)
                            //												{
                            //													case Xml.XmlNodeType.Attribute:
                            //														((Xml.XmlAttribute)BadNode).Value = NewText;
                            //														break;
                            //													default:
                            //														foreach (Xml.XmlNode BadChild in BadNode.ChildNodes)
                            //															BadNode.RemoveChild(BadChild);
                            //														BadNode.AppendChild(BadNode.OwnerDocument.CreateTextNode(NewText));
                            //														break;
                            //												}
                            //											}
                            //
                            //											break;
                            //										}
                            //										case "all":
                            //										case "*":
                            //										case "":
                            //										{
                            //											/*
                            //												if (Method == "xml")
                            //													Application.SelectSingleNode(XPath, SoapNamespaceManager).InnerXml = NewText;
                            //												else
                            //													Application.SelectSingleNode(XPath, SoapNamespaceManager).InnerText = NewText;
                            //												*/
                            //											break;
                            //										}
                            //									}
                            //
                            //									Result = Result + TempDoc.OuterXml;
                            //
                            //									break;
                            //								}
                            #endregion
                            #region text
                            case "text":
                                Result = Result + GetParameter("", ChildElement, Application, SoapNamespaceManager);
                                break;
                            #endregion
                            #region format
                            case "format":
                                Result = Result + String.Format(ChildElement.GetAttribute("spec"), GetParameter("", ChildElement, Application, SoapNamespaceManager));
                                break;
                            #endregion
                            #region literal
                            case "literal":
                                Result = Result + ChildElement.InnerXml;
                                break;
                            #endregion
                            #region encode
                            case "encode":
                                {
                                    switch (ChildElement.GetAttribute("method").ToLower())
                                    {
                                        case "":
                                        case "url":
                                            Result = Result + System.Web.HttpUtility.UrlEncode(GetParameter("", ChildElement, Application, SoapNamespaceManager));
                                            break;
                                        case "html":
                                        case "xml":
                                            Result = Result + System.Web.HttpUtility.HtmlEncode(GetParameter("", ChildElement, Application, SoapNamespaceManager));
                                            break;
                                        case "attribute":
                                        case "att":
                                            Result = Result + System.Web.HttpUtility.HtmlAttributeEncode(GetParameter("", ChildElement, Application, SoapNamespaceManager));
                                            break;
                                        case "base64":
                                            Result = Result + System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(GetParameter("", ChildElement, Application, SoapNamespaceManager)));
                                            break;
                                        case "qp":
                                        case "quoted-printable":
                                            Result += QuotedPrintable.Encode(GetParameter("", ChildElement, Application, SoapNamespaceManager));
                                            break;
                                        case "base32":
                                            {
                                                Result += Base32Encoder.Encode(System.Text.ASCIIEncoding.ASCII.GetBytes(GetParameter("", ChildElement, Application, SoapNamespaceManager)));
                                                break;
                                            }
                                        default:
                                            Result = Result + "Not Supported";
                                            break;
                                    }
                                    break;
                                }
                            #endregion
                            #region decode
                            case "decode":
                                {
                                    switch (ChildElement.GetAttribute("method"))
                                    {
                                        case "":
                                        case "url":
                                            Result = Result + System.Web.HttpUtility.UrlDecode(GetParameter("", ChildElement, Application, SoapNamespaceManager));
                                            break;
                                        case "html":
                                        case "xml":
                                        case "attribute":
                                        case "att":
                                            Result = Result + System.Web.HttpUtility.HtmlDecode(GetParameter("", ChildElement, Application, SoapNamespaceManager));
                                            break;
                                        case "base64":
                                            Result = Result + System.Text.Encoding.ASCII.GetString(System.Convert.FromBase64String(GetParameter("", ChildElement, Application, SoapNamespaceManager)));
                                            break;
                                        case "base32":
                                            {
                                                Result += System.Text.Encoding.ASCII.GetString(Base32Encoder.Decode(GetParameter("", ChildElement, Application, SoapNamespaceManager)));
                                                break;
                                            }
                                        default:
                                            Result = Result + "Not Supported";
                                            break;
                                    }
                                    break;
                                }
                            #endregion
                            #region regex
                            case "regex":
                                string MatchSpec = ChildElement.GetAttribute("match");
                                if (ChildElement.GetAttributeNode("replace") != null)
                                {
                                    Result = Result + Regex.Regex.Replace(GetParameter("", ChildElement, Application, SoapNamespaceManager), MatchSpec, ChildElement.GetAttribute("replace"));
                                }
                                else
                                {
                                    string sMatchData = GetParameter("", ChildElement, Application, SoapNamespaceManager);
                                    Regex.Match RegexMatches = Regex.Regex.Match(sMatchData, MatchSpec);
                                    if (RegexMatches.Success)
                                    {
                                        if (ChildElement.GetAttributeNode("capture") != null)
                                        {
                                            Result = Result + RegexMatches.Groups[ChildElement.GetAttribute("capture")];
                                        }
                                        else
                                        {
                                            Result = Result + RegexMatches.Value;
                                        }
                                    }
                                }
                                break;
                            #endregion
                            #region switch/select
                            case "switch":
                            case "select":
                                {
                                    XmlNode OldDataNode = ChildElement.SelectSingleNode("value");
                                    string OldData = GetParameter("value", ChildElement, Application, SoapNamespaceManager).ToLower();
                                    string NewResult = GetParameter("else", ChildElement, Application, SoapNamespaceManager);
                                    XmlNodeList Pairs = ChildElement.SelectNodes("case");
                                    foreach (XmlElement Pair in Pairs)
                                    {
                                        foreach (XmlElement When in Pair.SelectNodes("when"))
                                        {
                                            if (OldDataNode == null)
                                            {
                                                if (CheckTests(When, Application, SoapNamespaceManager))
                                                {
                                                    NewResult = GetParameter("then", Pair, Application, SoapNamespaceManager);
                                                    goto FoundResult;
                                                }
                                            }
                                            else
                                            {
                                                string PotentialMatch = GetParameter("", When, Application, SoapNamespaceManager).ToLower();
                                                if (PotentialMatch == OldData)
                                                {
                                                    NewResult = GetParameter("then", Pair, Application, SoapNamespaceManager);
                                                    goto FoundResult;
                                                }
                                            }
                                        }
                                    }

                                FoundResult:

                                    Result = Result + NewResult;

                                    break;
                                }
                            #endregion
                            #region registry
                            case "registry":
                                string HiveName = GetParameter("hive", ChildElement, Application, SoapNamespaceManager);
                                string Path = GetParameter("path", ChildElement, Application, SoapNamespaceManager);
                                string RegKey = GetParameter("key", ChildElement, Application, SoapNamespaceManager);

                                Microsoft.Win32.RegistryKey Hive = null;
                                switch (HiveName.ToUpper())
                                {
                                    case "HKEY_CLASSES_ROOT":
                                        Hive = Microsoft.Win32.Registry.ClassesRoot;
                                        break;
                                    case "HKEY_CURRENT_CONFIG":
                                        Hive = Microsoft.Win32.Registry.CurrentConfig;
                                        break;
                                    case "HKEY_CURRENT_USER":
                                        Hive = Microsoft.Win32.Registry.CurrentUser;
                                        break;
                                    case "HKEY_DYN_DATA":
                                        Hive = Microsoft.Win32.Registry.DynData;
                                        break;
                                    case "HKEY_LOCAL_MACHINE":
                                        Hive = Microsoft.Win32.Registry.LocalMachine;
                                        break;
                                    case "HKEY_PERFORMANCE_DATA":
                                        Hive = Microsoft.Win32.Registry.PerformanceData;
                                        break;
                                    case "HKEY_USERS":
                                        Hive = Microsoft.Win32.Registry.Users;
                                        break;
                                }

                                Microsoft.Win32.RegistryKey TargetKey = Hive.OpenSubKey(Path);

                                if (TargetKey != null)
                                {
                                    Result = Result + (string)TargetKey.GetValue(RegKey);
                                }
                                break;
                            #endregion
                            #region xsl:stylesheet
                            case "xsl:stylesheet":
                                Result = Result + GetParameter("", Transform((XmlDocument)Application, ChildElement), Application, SoapNamespaceManager);
                                break;
                            #endregion
                            #region debug
                            case "debug":
                                // Abel
                                //string NewSegment = GetParameter("", ChildElement, Application, SoapNamespaceManager);
                                //Result = Result + NewSegment;
                                //Debug(ChildElement.GetAttribute("label"), NewSegment, DebugMessageType.User, ChildElement.GetAttribute("debugqueue"));
                                //if (ChildElement.GetAttribute("label") != "")
                                //{
                                //    System.Console.Write(ChildElement.GetAttribute("label") + ": ");
                                //    System.Diagnostics.Debug.Write(ChildElement.GetAttribute("label") + ": ");
                                //}
                                //System.Console.WriteLine(NewSegment);
                                //System.Diagnostics.Debug.WriteLine(NewSegment);
                                break;
                            #endregion
                            #region encrypt
                            case "encrypt":
                                {
                                    byte[] Data = System.Text.ASCIIEncoding.ASCII.GetBytes(GetParameter("", ChildElement, Application, SoapNamespaceManager));

                                    if (Data.Length > 0)
                                    {
                                        Cryptography.SymmetricAlgorithm Provider = null;

                                        switch (ChildElement.GetAttribute("method").ToLower())
                                        {
                                            case "rc2":
                                                Provider = new Cryptography.RC2CryptoServiceProvider();
                                                break;
                                            case "rijndael":
                                                Provider = new Cryptography.RijndaelManaged();
                                                break;
                                            default:
                                            case "3des":
                                            case "tripledes":
                                                Provider = new Cryptography.TripleDESCryptoServiceProvider();
                                                break;
                                            case "des":
                                                Provider = new Cryptography.DESCryptoServiceProvider();
                                                break;
                                        }

                                        string Key = ChildElement.GetAttribute("key");
                                        string IV = ChildElement.GetAttribute("iv");

                                        if (Key == "") Key = DefaultEncryptionKey;

                                        if (ChildElement.GetAttribute("encoding").ToLower() == "base64")
                                        {
                                            Provider.Key = System.Convert.FromBase64String(Key);
                                            if (IV != "")
                                                Provider.IV = System.Convert.FromBase64String(IV);
                                        }
                                        else
                                        {
                                            Provider.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(Key);
                                            Provider.IV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV);
                                        }

                                        System.IO.MemoryStream DataStream = new System.IO.MemoryStream();
                                        Cryptography.CryptoStream EncryptionStream = new System.Security.Cryptography.CryptoStream(DataStream, Provider.CreateEncryptor(), Cryptography.CryptoStreamMode.Write);

                                        EncryptionStream.Write(Data, 0, Data.Length);
                                        EncryptionStream.FlushFinalBlock();

                                        DataStream.Seek(0, System.IO.SeekOrigin.Begin);

                                        byte[] DataBuffer = new byte[DataStream.Length];
                                        DataStream.Read(DataBuffer, 0, (int)DataStream.Length);

                                        Result = Result + System.Convert.ToBase64String(DataBuffer);
                                    }

                                    break;
                                }
                            #endregion
                            #region decrypt
                            case "decrypt":
                                {
                                    byte[] Data = System.Convert.FromBase64String(GetParameter("", ChildElement, Application, SoapNamespaceManager));

                                    if (Data.Length > 0)
                                    {
                                        Cryptography.SymmetricAlgorithm Provider = null;

                                        switch (ChildElement.GetAttribute("method").ToLower())
                                        {
                                            case "rc2":
                                                Provider = new Cryptography.RC2CryptoServiceProvider();
                                                break;
                                            case "rijndael":
                                                Provider = new Cryptography.RijndaelManaged();
                                                break;
                                            default:
                                            case "3des":
                                            case "tripledes":
                                                Provider = new Cryptography.TripleDESCryptoServiceProvider();
                                                break;
                                            case "des":
                                                Provider = new Cryptography.DESCryptoServiceProvider();
                                                break;
                                        }

                                        string Key = ChildElement.GetAttribute("key");
                                        string IV = ChildElement.GetAttribute("iv");

                                        if (Key == "") Key = DefaultEncryptionKey;

                                        if (ChildElement.GetAttribute("encoding").ToLower() == "base64")
                                        {
                                            Provider.Key = System.Convert.FromBase64String(Key);
                                            if (IV != "")
                                                Provider.IV = System.Convert.FromBase64String(IV);
                                        }
                                        else
                                        {
                                            Provider.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(Key);
                                            Provider.IV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV);
                                        }

                                        System.IO.MemoryStream DataStream = new System.IO.MemoryStream();
                                        Cryptography.CryptoStream DecryptionStream = new System.Security.Cryptography.CryptoStream(DataStream, Provider.CreateDecryptor(), Cryptography.CryptoStreamMode.Write);

                                        DecryptionStream.Write(Data, 0, Data.Length);
                                        DecryptionStream.FlushFinalBlock();

                                        DataStream.Seek(0, System.IO.SeekOrigin.Begin);

                                        byte[] DataBuffer = new byte[DataStream.Length];
                                        DataStream.Read(DataBuffer, 0, (int)DataStream.Length);

                                        Result = Result + System.Text.ASCIIEncoding.ASCII.GetString(DataBuffer);
                                    }

                                    break;
                                }
                            #endregion
                            #region code
                            case "code":
                                {
                                    Compilers.ICodeEngine Compiler = null;
                                    switch (ChildElement.GetAttribute("language").ToLower())
                                    {
                                        case "vb":
                                        case "vb.net":
                                        case "visualbasic":
                                        case "visual basic":
                                            Compiler = new Compilers.VisualBasic();
                                            break;
                                        case "":
                                        case "cs":
                                        case "c#":
                                        case "csharp":
                                        case "c-sharp":
                                            Compiler = new Compilers.CSharp();
                                            break;
                                        default:
                                            // TODO: Abel
                                            // ReportError(Application.OwnerDocument, "GetParameter() - 'code' Element", "Language '" + ChildElement.GetAttribute("language") + "' not recognized");
                                            break;
                                    }
                                    string[] Imports = new string[ChildElement.SelectNodes("assembly").Count + 1];
                                    int I = 0;
                                    foreach (XmlElement AssemblyNode in ChildElement.SelectNodes("assembly"))
                                    {
                                        Imports[I++] = GetParameter("", AssemblyNode, Application, SoapNamespaceManager);
                                    }
                                    Imports[I] = "System.Xml.dll";
                                    string Code = GetParameter("source", ChildElement, Application, SoapNamespaceManager);
                                    int CodeHash = Code.GetHashCode();
                                    System.Reflection.Assembly TempAssembly = Compiler.Compile(Code, true, Imports);
                                    string ClassName = ChildElement.GetAttribute("class");
                                    if (ChildElement.GetAttribute("class") == "")
                                    {
                                        if (TempAssembly.GetTypes().Length == 1)
                                        {
                                            ClassName = TempAssembly.GetTypes()[0].FullName;
                                        }
                                        else
                                        {
                                            // TODO: Abel
                                            // ReportError(Application.OwnerDocument, "GetParameter() - 'code' Element", "Ambiguous Which Class Should Be Used");
                                        }
                                    }
                                    System.Type TempType = TempAssembly.GetType(ClassName);
                                    System.Reflection.ConstructorInfo CI = TempType.GetConstructor(new Type[] { typeof(XmlDocument), typeof(XmlNamespaceManager) });
                                    if (CI != null)
                                    {
                                        object Temp = CI.Invoke(System.Reflection.BindingFlags.InvokeMethod, null, new object[2] { Application.OwnerDocument, SoapNamespaceManager }, null);
                                        Result += Temp.ToString();
                                    }
                                    else
                                    {
                                        CI = TempType.GetConstructor(Type.EmptyTypes);
                                        if (CI != null)
                                        {
                                            Result += CI.Invoke(System.Reflection.BindingFlags.InvokeMethod, null, null, null).ToString();
                                        }
                                        else
                                        {
                                            // TODO: Abel 
                                            //ReportErrorNow((Application is XmlDocument ? (XmlDocument)Application : (Application != null ? Application.OwnerDocument : null)), "Appropriate Constructor Not Found", "No Appropriate Constructor Found in '" + ClassName + "'");
                                        }
                                    }
                                    break;
                                }
                            #endregion
                            #region guid
                            case "guid":
                                if (ChildElement.GetAttribute("type").ToLower() == "full")
                                    Result = Result + System.Guid.NewGuid().ToString();
                                else
                                    Result = Result + NewGuid;
                                break;
                            #endregion
                            #region evaluate
                            case "evaluate":
                                {
                                    XPath = GetParameter("", ChildElement, Application, SoapNamespaceManager).Trim();
                                    switch (ChildElement.GetAttribute("index").ToLower())
                                    {
                                        case "":
                                        case "first":
                                        case "1":
                                            XPathResultNode = Application.SelectSingleNode(XPath, SoapNamespaceManager);
                                            break;
                                        case "last":
                                            TempNodeListResult = Application.SelectNodes(XPath, SoapNamespaceManager);
                                            XPathResultNode = TempNodeListResult[TempNodeListResult.Count - 1];
                                            break;
                                        default:
                                            TempNodeListResult = Application.SelectNodes(XPath, SoapNamespaceManager);
                                            try
                                            {
                                                XPathResultNode = TempNodeListResult[int.Parse(ChildElement.GetAttribute("index"))];
                                            }
                                            catch
                                            {
                                                XPathResultNode = null;
                                            }
                                            break;
                                    }
                                    if (XPathResultNode != null)
                                    {
                                        Result = Result + GetParameter("", XPathResultNode, Application, SoapNamespaceManager);
                                    }
                                    break;
                                }
                            #endregion
                            #region validate
                            /*
									 * <validate> validates its innards for well-formedness or against a DTD or schema.
									 * if the validation passes, the evaluated insides get passed right along
									 * if the validation fails, an exception is thrown.
									 *	type should be "wellformed", "schema", or "dtd". defaults to "wellformed".
									 *	href is the schema or dtd location 
									 *	for schema validation, the default namespace in the xml fragment will be the targetNamespace of the schema
									 *	for dtd validation, the dtd declaration is prepended to the xml fragment, and the root element is the doctype identifier
									 * example:
									 *	<!-- dtd validation -->
									 *		<validate type="dtd" href="c:\testes.dtd">
									 *			<literal>
									 *				<!-- comment to mess things up -->
									 *				<testes name="yo">
									 *					<subtestes>hello</subtestes>
									 *				</testes>
									 *			</literal>
									 *		</validate>
									 *	the actual xml that will be validated is:
									 *		<!DOCTYPE testes SYSTEM 'c:\testes.dtd'>
									 *		<testes name="yo">
									 *			<subtestes>hello</subtestes>
									 *		</testes>
									 * 
									 *	<!-- schema validation -->
									 *		<validate type="schema" href="c:\testes.xsd">
									 *			<literal>
									 *				<!-- comment to mess things up -->
									 *				<testes name="yo">
									 *					<subtestes>hello</subtestes>
									 *				</testes>
									 *			</literal>
									 *		</validate>
									 *	the actual xml that will be validated is:
									 *		<testes name="yo" xmlns="(the targetNamespace from c:\testes.xsd)">
									 *			<subtestes>hello</subtestes>
									 *		</testes>
									*/
                            case "validate":
                                {
                                    string sValidationType = ChildElement.GetAttribute("type").ToLower().Trim();
                                    if (sValidationType.Length == 0)
                                    {
                                        sValidationType = "wellformed";
                                    }
                                    string sValidationDocument = ChildElement.GetAttribute("href").Trim();
                                    string sXml = GetParameter("", ChildElement, Application, SoapNamespaceManager).Trim();
                                    XmlValidator xvValidator = new XmlValidator(sXml, sValidationType, sValidationDocument);
                                    switch (xvValidator.ValidationType)
                                    {
                                        case XmlValidator.XmlValidationType.WellFormed:
                                            {
                                                //don't need to do anything
                                                break;
                                            }
                                        case XmlValidator.XmlValidationType.Schema:
                                            {
                                                //still don't need to do anything
                                                break;
                                            }
                                        case XmlValidator.XmlValidationType.DTD:
                                            {
                                                break;
                                            }
                                    }
                                    if (xvValidator.IsValid)
                                    {
                                        Result += sXml;
                                    }
                                    else
                                    {
                                        throw new Exception("The XML fragment did not pass validation.\n" + xvValidator.ErrorMessages);
                                    }
                                    break;
                                }
                            #endregion
                            #region weekofyear
                            case "weekofyear":
                                {
                                    string sContents = GetParameter("", ChildElement, Application, SoapNamespaceManager);
                                    DateTime dtDate;
                                    try
                                    {
                                        dtDate = Convert.ToDateTime(sContents);
                                    }
                                    catch (Exception e)
                                    {
                                        //if there is a problem parsing the date, default to today
                                        System.Diagnostics.Debug.WriteLine(e.ToString());
                                        dtDate = DateTime.Today;
                                    }
                                    int iWeekOfYear = CultureInfo.CurrentCulture.DateTimeFormat.Calendar.GetWeekOfYear(dtDate, CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
                                    Result += iWeekOfYear.ToString();
                                    break;
                                }
                            #endregion
                            #region weekofmonth
                            case "weekofmonth":
                                {
                                    string sContents = GetParameter("", ChildElement, Application, SoapNamespaceManager);
                                    DateTime dtDate;
                                    try
                                    {
                                        dtDate = Convert.ToDateTime(sContents);
                                    }
                                    catch (Exception e)
                                    {
                                        //if there is a problem parsing the date, default to today
                                        System.Diagnostics.Debug.WriteLine(e.ToString());
                                        dtDate = DateTime.Today;
                                    }
                                    int iWeekOfMonth = (dtDate.Day / 7) + (dtDate.Day % 7 == 0 ? 0 : 1);
                                    Result += iWeekOfMonth.ToString();
                                    break;
                                }
                            #endregion
                            #region transform
                            case "transform":
                                {
                                    string sXslFilename = "";
                                    string sXml = "";
                                    XmlAttribute xaXslFilename = (XmlAttribute)ChildNode.SelectSingleNode("@xslfilename", SoapNamespaceManager);
                                    if (xaXslFilename != null)
                                    {
                                        sXslFilename = xaXslFilename.Value.Trim();
                                        sXml = GetParameter("", ChildNode, Application, SoapNamespaceManager);
                                    }
                                    else
                                    {
                                        sXslFilename = GetParameter("xslfilename", ChildNode, Application, SoapNamespaceManager);
                                        sXml = GetParameter("content", ChildNode, Application, SoapNamespaceManager);
                                    }
                                    //get the full path to the filename
                                    if (sXslFilename.IndexOf("://") == -1 && sXslFilename.Substring(1, 1) != ":")
                                    {
                                        RegistryKey rkConfig = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(MessageQueueSpawner._RegistryKeyConfigFilePath);
                                        if (rkConfig != null)
                                        {
                                            sXslFilename = System.IO.Path.Combine(Convert.ToString(rkConfig.GetValue("ConfigPath")), sXslFilename);
                                        }
                                    }
                                    //the XsltArgumentList is for xslt extension functions
                                    XsltArgumentList xalArguments = new XsltArgumentList();
                                    xalArguments.AddExtensionObject("http://www.1800communications.com/schemas", new XsltExtensionFunctions());
                                    XslTransform xslTransform = new XslTransform();
                                    xslTransform.Load(sXslFilename, new XmlUrlResolver());
                                    XmlDocument xdToTransform = new XmlDocument();
                                    xdToTransform.LoadXml(sXml);
                                    StringWriter swResults = new StringWriter();
                                    xslTransform.Transform(xdToTransform, xalArguments, swResults, null);
                                    Result += swResults.ToString();
                                    break;
                                }
                            #endregion
                            #region random
                            case "random":
                                {
                                    XmlElement xeChildNode = (XmlElement)ChildNode;
                                    string sMin = xeChildNode.GetAttribute("min");
                                    string sMax = xeChildNode.GetAttribute("max");
                                    int iRandom = 0;
                                    if (sMax.Length > 0)
                                    {
                                        if (sMin.Length > 0)
                                        {
                                            iRandom = randomNumberGenerator.Next(Convert.ToInt32(sMin), Convert.ToInt32(sMax));
                                        }
                                        else
                                        {
                                            iRandom = randomNumberGenerator.Next(Convert.ToInt32(sMax));
                                        }
                                    }
                                    else
                                    {
                                        iRandom = randomNumberGenerator.Next();
                                    }
                                    Result += iRandom.ToString();
                                    break;
                                }
                            #endregion
                            #region queuesize
                            case "queuesize":
                                {
                                    // Abel
                                    //string sQueuePath = GetParameter("", ChildElement, Application, SoapNamespaceManager);
                                    //sQueuePath = ResolveQueuePath(sQueuePath, ChildElement, m_sIncomingQueuePath);
                                    //long lMessageCount = 0;
                                    //try
                                    //{
                                    //    lMessageCount = QueueMonitorClient.MessageCount(sQueuePath);
                                    //}
                                    //catch (Exception e)
                                    //{
                                    //    System.Diagnostics.Debug.WriteLine(e.ToString());
                                    //}
                                    //Result += Convert.ToString(lMessageCount);
                                    break;
                                }
                            #endregion
                            #region validemail
                            case "validemail":
                                {
                                    string sEmail = GetParameter("", ChildElement, Application, SoapNamespaceManager);
                                    //get the timeout
                                    int iTimeout = 60000;
                                    if (ChildElement.HasAttribute("timeout"))
                                    {
                                        try
                                        {
                                            iTimeout = Convert.ToInt32(ChildElement.GetAttribute("timeout"));
                                        }
                                        catch (Exception e)
                                        {
                                            System.Diagnostics.Debug.WriteLine(e.ToString());
                                        }
                                    }
                                    //get the target validation level
                                    HexValidEmailLevel hvelTargetLevel = HexValidEmailLevel.hexVeLevelSmtp;
                                    if (ChildElement.HasAttribute("level"))
                                    {
                                        switch (ChildElement.GetAttribute("level"))
                                        {
                                            case "smtp":
                                                {
                                                    hvelTargetLevel = HexValidEmailLevel.hexVeLevelSmtp;
                                                    break;
                                                }
                                            case "dns":
                                                {
                                                    hvelTargetLevel = HexValidEmailLevel.hexVeLevelDns;
                                                    break;
                                                }
                                            case "syntax":
                                                {
                                                    hvelTargetLevel = HexValidEmailLevel.hexVeLevelSyntax;
                                                    break;
                                                }
                                        }
                                    }
                                    HexValidEmailLib.Connection hcConnection = new HexValidEmailLib.ConnectionClass();
                                    hcConnection.FromDomain = "1800communications.com";
                                    hcConnection.FromEmail = "sdallamura@1800communications.com";
                                    hcConnection.Timeouts.Item(HexValidEmailTimeout.hexVeTimeoutSmtpTotal).Value = iTimeout;
                                    HexValidEmailLevel hvelResult = (HexValidEmailLevel)hcConnection.Validate(sEmail, hvelTargetLevel);
                                    if (hvelResult == hvelTargetLevel)
                                    {
                                        Result += "true";
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.WriteLine(((HexValidEmailErrors)hcConnection.Error).ToString());
                                        Result += "false";
                                    }
                                    break;
                                }
                            #endregion
                            #region [Custom Parameters]
                            default:
                                Result = Result + GetCustomParameter((XmlElement)ChildNode, Application, SoapNamespaceManager);
                                break;
                            #endregion
                        }
                    }
                }
                catch (Compilers.CompilationException cexx)
                {
                    // Compilation of the Code Block Jacked Up
                    string[] Errors = new string[cexx.Errors.Count];
                    int I = 0;
                    foreach (System.CodeDom.Compiler.CompilerError Error in cexx.Errors)
                    {
                        Errors[I++] = Error.ErrorText;
                    }
                    // ReportErrorNow((Application is XmlDocument ? (XmlDocument)Application : (Application != null ? Application.OwnerDocument : null)), cexx.GetType().ToString(), "Errors During Compilation", null, Errors);
                }
                catch (Exception exx)
                {
                    //Report any other error we found and keep going
                    //Application can be an XmlDocument. if it is, it has no OwnerDocument. so check.
                    // ReportErrorNow((Application is XmlDocument ? (XmlDocument)Application : (Application != null ? Application.OwnerDocument : null)), exx);
                }
            }

            return Result;
        }

        /// <summary>
        /// This routine may be overridden by any class inheriting the MessageQueueListener
        /// class.  The idea is to provide additional functionality to that defined in the
        /// GetParameter() routine.  For example, the Emailer can understand an "attachment"
        /// element which attaches a file and returns the ID assigned to that attachment.
        /// This is useful when creating HTML emails which display images which are attached.
        /// </summary>
        /// <param name="UnrecognizedElement">The element which is not recognized</param>
        /// <param name="Application">The user's application</param>
        /// <param name="SoapNamespaceManager">The Namespace Manager for the user's Application</param>
        /// <returns>The string represented by the unrecognized parameter</returns>
        public virtual string GetCustomParameter(XmlElement UnrecognizedElement, XmlNode Application, XmlNamespaceManager SoapNamespaceManager)
        {
            // Default functionality, return a blank string for unrecognized elements
            return "";
        }

        #endregion

        #region Transforms

        protected XmlElement Transform(XmlDocument OldDocument, XslTransform CommandXsl)
        {
            XmlDocument NewDocument = new XmlDocument();
            NewDocument.Load(CommandXsl.Transform(OldDocument, (XsltArgumentList)null, (XmlResolver)null));
            return NewDocument.DocumentElement;
        }

        protected XmlElement Transform(XmlDocument OldDocument, XmlDocument XSLDocument)
        {
            XslTransform CommandXsl = new XslTransform();
            CommandXsl.Load(XSLDocument, null, null);
            return Transform(OldDocument, CommandXsl);
        }

        protected XmlElement Transform(XmlElement OldChunk, XmlDocument XSLDocument)
        {
            XmlDocument OldDocument = new XmlDocument();
            OldDocument.AppendChild(OldDocument.ImportNode(OldChunk, true));
            //OldDocument.LoadXml(OldChunk.OuterXml);

            return Transform(OldDocument, XSLDocument);
        }

        protected XmlElement Transform(XmlElement OldChunk, string XSLSource)
        {
            XmlDocument XSLDocument = new XmlDocument();

            XSLDocument.LoadXml(XSLSource);

            return Transform(OldChunk, XSLDocument);
        }

        protected XmlElement Transform(XmlDocument OldDocument, string XSLSource)
        {
            XmlDocument XSLDocument = new XmlDocument();

            XSLDocument.LoadXml(XSLSource);

            return Transform(OldDocument, XSLDocument);
        }

        protected XmlElement Transform(XmlDocument OldDocument, XmlElement XSLNode)
        {
            XslTransform XSLDocument = new XslTransform();
            XSLDocument.Load(XSLNode, (XmlResolver)null, (System.Security.Policy.Evidence)null);
            return Transform(OldDocument, XSLDocument);
        }

        protected XmlElement Transform(XmlElement OldChunk, XmlElement XSLNode)
        {
            XmlDocument XSLDocument = new XmlDocument();
            XSLDocument.LoadXml(XSLNode.OuterXml);
            return Transform(OldChunk, XSLDocument);
        }

        #endregion

        #region Check Tests

        /// <summary>
        /// This determines whether the designer intended for a given piece
        /// of the xml to be read depending on various factors including data
        /// in the user's application.
        /// </summary>
        /// 
        /// <param name="Context">The node whose processing is in question</param>
        /// <param name="Application">The entire user's application</param>
        /// <param name="SoapNamespaceManager">Namespace manager for users application</param>
        /// <returns>Returns a boolean determing whether to ignore the node or not</returns>
        /// 
        protected bool CheckTests(XmlNode Context, XmlNode Application, XmlNamespaceManager SoapNamespaceManager)
        {
            // If we've got an operator...
            if (((XmlElement)Context).GetAttribute("operator") != "")
            {
                string NextValue = null;

                object LastObject = null;
                object NextObject = null;

                string DataTypeName = null;

                XmlNodeList OperandSet = Context.SelectNodes("operand");

                switch (((XmlElement)Context).GetAttribute("operator").ToLower())
                {
                    case "equals":
                    case "==":
                    case "=":
                    case "eq":
                    case "e":
                        // This is just a normal equals comparison.  Use a default datatype 
                        // of system.string and use the regular comparison code

                        DataTypeName = "system.string";
                        goto default;
                    case "exists":
                    case "exist":
                    case "?":
                        // This just tests that each operand is non-blank

                        foreach (XmlElement NextOperand in OperandSet)
                        {
                            NextValue = GetParameter("", NextOperand, Application, SoapNamespaceManager);
                            if (NextValue == "") return false;
                        }

                        return true;
                    default:
                        {
                            // For whatever comparison, we may need to do something very tricky
                            // Hopefully it's a string comparison.  If it's not a string and
                            // the object has no Parse(string) method, we're screwed

                            // Get the desired type (if specified).  If not use the default
                            // of system.double (note: the default is set to system.string
                            // for equality comparisons up above).
                            if (((XmlElement)Context).GetAttributeNode("type") != null)
                            {
                                DataTypeName = ((XmlElement)Context).GetAttribute("type");
                            }
                            if (DataTypeName == null)
                            {
                                DataTypeName = "system.double";
                            }
                            if (DataTypeName.IndexOf(".") < 0)
                            {
                                DataTypeName = "System." + DataTypeName;
                            }
                            // Try to find the Parse(string) and Comparison(DataType) methods
                            Type DataType = Type.GetType(DataTypeName, true, true);
                            System.Reflection.MethodInfo ParseMethod = DataType.GetMethod("Parse", new Type[] { Type.GetType("System.String") });
                            System.Reflection.MethodInfo ComparisonMethod = DataType.GetMethod("CompareTo", new Type[] { DataType });

                            // Look at each operand after the first and...
                            foreach (XmlElement NextOperand in OperandSet)
                            {
                                NextValue = GetParameter("", NextOperand, Application, SoapNamespaceManager).ToLower();

                                // If we can't parse it, try to cast it
                                try
                                {
                                    if (ParseMethod != null)
                                    {
                                        NextObject = ParseMethod.Invoke(null, new object[] { NextValue });
                                    }
                                    else
                                    {
                                        NextObject = NextValue;
                                    }
                                }
                                catch
                                {
                                    return false;
                                }

                                // If nothing could be done then "Oh Well", but if we can...

                                if (LastObject != null)
                                {
                                    // Try to run the comparison routine
                                    // < 0 : Less than
                                    // > 0 : Greater than
                                    // = 0 : Equal to
                                    int CompareResult = (int)ComparisonMethod.Invoke(LastObject, new object[] { NextObject });


                                    // If the situation is ever false, the whole run will be false, so junk out.
                                    switch (((XmlElement)Context).GetAttribute("operator").ToLower())
                                    {
                                        case "equals":
                                        case "==":
                                        case "=":
                                        case "eq":
                                        case "e":
                                            if (CompareResult != 0)
                                                return false;
                                            break;
                                        case "greater-than":
                                        case "greaterthan":
                                        case "greater":
                                        case "gt":
                                        case ">":
                                            if (CompareResult <= 0)
                                                return false;
                                            break;
                                        case "less-than":
                                        case "lessthan":
                                        case "less":
                                        case "lt":
                                        case "<":
                                            if (CompareResult >= 0)
                                                return false;
                                            break;
                                        case "less-than-or-equal":
                                        case "lessthanorequal":
                                        case "lessequal":
                                        case "lte":
                                        case "le":
                                        case "<=":
                                            if (CompareResult > 0)
                                                return false;
                                            break;
                                        case "greater-than-or-equal":
                                        case "greaterthanorequal":
                                        case "greaterequal":
                                        case "gte":
                                        case "ge":
                                        case ">=":
                                            if (CompareResult < 0)
                                                return false;
                                            break;
                                        default:
                                            // Abel
                                            //ReportError(Application.OwnerDocument, "Comparison Operator not Recognized");
                                            break;
                                    }
                                }

                                LastObject = NextObject;
                            }

                            // If nothing failed, then the test is true
                            return true;
                        }
                }
            }
            else
            {
                // We've got some sub-tests to examine.  First get the boolean operation used

                int TrueCount = 0;
                int FalseCount = 0;

                string BooleanMethod = ((XmlElement)Context).GetAttribute("boolean");

                if ((Context.Name != "test") || (BooleanMethod == ""))
                    BooleanMethod = "and";

                // Find and run all sub-tests
                XmlNodeList TestNodeSet = Context.SelectNodes("test");

                foreach (XmlElement TestNode in TestNodeSet)
                {
                    bool NextCheck = true;

                    // Get the next test's result
                    NextCheck = CheckTests(TestNode, Application, SoapNamespaceManager);

                    if (NextCheck)
                        TrueCount++;
                    else
                        FalseCount++;

                    // If at any time we know the response, don't bother figuring anything else out

                    if ((BooleanMethod == "and") && (!NextCheck))
                        // False and (*) == False
                        return false;
                    else if ((BooleanMethod == "nand") && (!NextCheck))
                        // False nand (*) == True
                        return true;
                    else if ((BooleanMethod == "not") && (NextCheck))
                        // not True == False
                        return false;
                    else if ((BooleanMethod == "or") && (NextCheck))
                        // True or (*) == True
                        return true;
                    else if ((BooleanMethod == "nor") && (NextCheck))
                        // True nor (*) == False
                        return false;
                    else if ((BooleanMethod == "xor") && (NextCheck))
                        // Count(True) > 1 == False
                        if (TrueCount > 1)
                            return false;
                }

                // We ran through all the tests, return the opposite of above

                if (BooleanMethod == "and")
                    return true;
                else if (BooleanMethod == "nand")
                    return false;
                else if (BooleanMethod == "not")
                    return true;
                else if (BooleanMethod == "or")
                    return false;
                else if (BooleanMethod == "nor")
                    return true;
                else if (BooleanMethod == "xor")
                    return (TrueCount == 1);
            }

            // No test was actually found, just return true

            return true;
        }

        #endregion

        #region MarkUsersApplication

        /// <summary>
        /// Marks the Header section of the user's application with the 
        /// activity just performed
        /// </summary>
        /// <param name="SoapMessage">User's Application</param>
        /// <param name="SoapNamespaceManager">Namespace Manager for User's Application</param>
        /// <param name="UpdateMessage">The text returned by the update code</param>
        /// <param name="Source">Queue this message was read from</param>
        /// <returns></returns>
        private int MarkUsersApplication(XmlDocument SoapMessage, XmlNamespaceManager SoapNamespaceManager, string UpdateMessage, string Source)
        {
            // Find the header block
            XmlElement Header = (XmlElement)SoapMessage.SelectSingleNode("/soap:Envelope/soap:Header", SoapNamespaceManager);
            if (Header == null)
            {
                // If none exists and there is a soap:Envelope, create the header
                XmlElement Envelope = (XmlElement)SoapMessage.SelectSingleNode("/soap:Envelope", SoapNamespaceManager);
                if (Envelope == null)
                    return -1;
                Header = SoapMessage.CreateElement("soap", "Header", SoapURN);
                Envelope.AppendChild(Header);
            }

            // Create, populate and add various pieces of data
            XmlElement ActionNode = SoapMessage.CreateElement("action");
            XmlElement ActorNode = SoapMessage.CreateElement("actor");
            XmlElement FromNode = SoapMessage.CreateElement("from");
            XmlElement TimeNode = SoapMessage.CreateElement("timestamp");
            XmlElement MachineNode = SoapMessage.CreateElement("machine");
            XmlElement DescNode = SoapMessage.CreateElement("description");

            ActorNode.AppendChild(SoapMessage.CreateTextNode(ruleName));
            FromNode.AppendChild(SoapMessage.CreateTextNode(Source));
            TimeNode.AppendChild(SoapMessage.CreateTextNode(DateTime.Now.ToString()));
            MachineNode.AppendChild(SoapMessage.CreateTextNode(System.Net.Dns.GetHostName()));
            DescNode.AppendChild(SoapMessage.CreateTextNode(UpdateMessage));

            ActionNode.AppendChild(ActorNode);
            ActionNode.AppendChild(FromNode);
            ActionNode.AppendChild(TimeNode);
            ActionNode.AppendChild(MachineNode);
            ActionNode.AppendChild(DescNode);

            // Try to set an index up
            try
            {
                ActionNode.SetAttribute("index", (Header.SelectNodes("action").Count + 1).ToString());
            }
            catch
            {
                ActionNode.SetAttribute("index", "1");
            }

            // Add this header to the document
            Header.AppendChild(ActionNode);

            // Return the Index of this guy to be put in the new data chunk
            return Header.SelectNodes("action").Count;
        }

        #endregion

        #region  Remove Namespace

        /// <summary>
        /// This removes namespaces from an XML Element (Scott)
        /// </summary>
        /// <param name="xeElement"></param>
        /// <returns></returns>
        protected string RemoveNamespaces(XmlNode xeElement)
        {
            StringWriter swOutput = new System.IO.StringWriter();
            XmlTextWriter xtwWriter = new XmlTextWriter(swOutput);
            //xtwWriter.WriteStartDocument();
            XmlNodeReader xnrReader = new XmlNodeReader(xeElement);
            while (xnrReader.Read())
            {
                switch (xnrReader.NodeType)
                {
                    case XmlNodeType.Element:
                        xtwWriter.WriteStartElement(xnrReader.Name);
                        if (xnrReader.HasAttributes)
                        {
                            while (xnrReader.MoveToNextAttribute())
                            {
                                if (xnrReader.Name != "xmlns")
                                {
                                    xtwWriter.WriteAttributeString(xnrReader.Name, xnrReader.Value);
                                }
                            }
                            xnrReader.MoveToElement();
                        }
                        if (xnrReader.IsEmptyElement)
                        {
                            xtwWriter.WriteEndElement();
                        }
                        break;
                    case XmlNodeType.Text:
                        xtwWriter.WriteString(xnrReader.Value);
                        break;
                    case XmlNodeType.CDATA:
                        xtwWriter.WriteCData(xnrReader.Value);
                        break;
                    case XmlNodeType.ProcessingInstruction:
                        xtwWriter.WriteProcessingInstruction(xnrReader.Name, xnrReader.Value);
                        break;
                    case XmlNodeType.Comment:
                        xtwWriter.WriteComment(xnrReader.Value);
                        break;
                    case XmlNodeType.EntityReference:
                        xtwWriter.WriteEntityRef(xnrReader.Name);
                        break;
                    case XmlNodeType.EndElement:
                        xtwWriter.WriteEndElement();
                        break;
                }
            }
            //xtwWriter.WriteEndDocument();
            xtwWriter.Flush();
            xtwWriter.Close();
            xnrReader.Close();
            string sOutput = swOutput.ToString();
            return sOutput;
        }

        #endregion

        #region Process Application

        /// <summary>
        /// ProcessApplication() is the heart of the processing of individual messages.  It takes
        /// the original message along with the appropriate action node from the config file.  It
        /// uses these to create a third XML document which specifies how to handle the message.
        /// Finally, it creates an optional XML fragment.  This fragment is then integrated into
        /// the SOAP message so that the results are available during later processes
        /// 
        /// This routine is expected to be overridden.
        /// </summary>
        /// 
        public abstract XmlElement ProcessApplication(EventRuleContext context, XmlElement ActionNode, ref string Actions);

        #endregion

        /// <summary>
        /// Internally used to create a new GUID
        /// </summary>
        /// 
        private string NewGuid
        {
            get
            {
                return System.Guid.NewGuid().ToString("D");
            }
        }

        /// <summary>
        /// Returns the config file this tool is using
        /// </summary>
        /// 
        public XmlNode Config
        {
            get
            {
                return this.ruleNode;
            }

        }

        #region Spawner properties

        /// <summary>
        /// Provides this object with the element in the XML corresponding to this tool.
        /// </summary>
        /// <returns>
        /// String denoting XML element name
        /// </returns>
        protected virtual string GetToolNodeName()
        {
            foreach (SpawnerAttribute Att in this.GetType().GetCustomAttributes(typeof(SpawnerAttribute), true))
            {
                if (Att.ToolNode != null)
                {
                    return Att.ToolNode;
                }
            }
            throw new Exception(this.GetType().ToString() + " class does not have a Spawner attribute or implement GetToolNodeName()");
        }

        #endregion
    }
}
