
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using Mindplex.Commons;

using Exchange.Engine.Strategy.Events;

#endregion

namespace Exchange.Engine
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class ExchangeTracker
    {
        /// <summary>
        /// SOAP Namespace URN
        /// </summary>
        /// 
        public static readonly string SoapURN = "http://schemas.xmlsoap.org/soap/envelope/";

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="soapMessage"></param>
        /// <param name="actor"></param>
        /// <param name="description"></param>
        /// <param name="source"></param>
        /// 
        /// <returns></returns>
        ///
        public static int CreateSoapHeader(XmlDocument soapMessage, string actor, string description, string source, long elapsed)
        {
            // Set up a name table so we can query based on namespaces
            NameTable SoapNameTable = new NameTable();
            XmlNamespaceManager soapNamespaceManager = new XmlNamespaceManager(SoapNameTable);
            soapNamespaceManager.AddNamespace(String.Empty, "urn:none");
            soapNamespaceManager.AddNamespace("soap", SoapURN);

            // Find the header block
            XmlElement header = (XmlElement)soapMessage.SelectSingleNode("/soap:Envelope/soap:Header", soapNamespaceManager);
            if (Guard.IsNull(header))
            {
                // If none exists and there is a soap:Envelope, create the header
                XmlElement envelope = (XmlElement)soapMessage.SelectSingleNode("/soap:Envelope", soapNamespaceManager);
                if (Guard.IsNull(envelope))
                {
                    return -1;
                }
                header = soapMessage.CreateElement("soap", "Header", SoapURN);
                envelope.AppendChild(header);
            }

            // Create, populate and add various pieces of data
            XmlElement action = soapMessage.CreateElement("action");
            XmlElement actorElement = soapMessage.CreateElement("actor");
            XmlElement from = soapMessage.CreateElement("from");
            XmlElement time = soapMessage.CreateElement("timestamp");
            XmlElement machine = soapMessage.CreateElement("machine");
            XmlElement descriptionElement = soapMessage.CreateElement("description");
            XmlElement timeElement = soapMessage.CreateElement("elapsed");

            actorElement.AppendChild(soapMessage.CreateTextNode(actor));
            from.AppendChild(soapMessage.CreateTextNode(source));
            time.AppendChild(soapMessage.CreateTextNode(DateTime.Now.ToString()));
            machine.AppendChild(soapMessage.CreateTextNode(System.Net.Dns.GetHostName()));
            descriptionElement.AppendChild(soapMessage.CreateTextNode(description));
            timeElement.AppendChild(soapMessage.CreateTextNode(elapsed.ToString()));

            action.AppendChild(actorElement);
            action.AppendChild(from);
            action.AppendChild(time);
            action.AppendChild(machine);
            action.AppendChild(descriptionElement);
            action.AppendChild(timeElement);

            // Try to set an index up
            try
            {
                action.SetAttribute("index", (header.SelectNodes("action").Count + 1).ToString());
            }
            catch
            {
                action.SetAttribute("index", "1");
            }

            // Add this header to the document
            header.AppendChild(action);
            return header.SelectNodes("action").Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="status"></param>
        /// <param name="runtime"></param>
        /// 
        public static string CreateSoapMessage(ExchangeRuntime runtime)
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>");
            xml.Append("<soap:Envelope xmlns:soap='" + SoapURN + "' soap:id='" + runtime.GetLead().Guid + "'><soap:Header/><soap:Body>");//<dataset id='" + ruleLabel + "'>" + EventNode.InnerXml + "</dataset></soap:Body></soap:Envelope>";
            xml.AppendFormat("<ExchangeMessage vertical=\"{0}\" type=\"{1}\" status=\"{2}\">", runtime.Vertical, runtime.VerticalType, runtime.Status);
            
            //xml.Append("<dataset id=\"\">");
            //xml.AppendFormat("<created>{0}</created>", runtime.GetLead().Created);
            //xml.AppendFormat("<status>{0}</status>", runtime.Status);
            //xml.AppendFormat("<source><aid>{0}</aid></source>", runtime.GetLead().Aid);
            //xml.AppendFormat("<lead><email>{0}</email></lead>", runtime.GetLead().Email);
            //xml.Append("</dataset>");

            foreach (GenericStrategyEvent @event in runtime.StrategyEvents)
            {
                xml.AppendFormat("<dataset id=\"{0}\" source=\"{1}\" >{2}</dataset>", @event.Actor, @event.Source, @event.ToXml());
            }

            if (runtime.Errors.Count > 0)
            {
                xml.Append("<errors>");
                foreach (Exception exception in runtime.Errors)
                {
                    xml.AppendFormat("<error>{0}</error>", exception.Message);
                }
                xml.Append("</errors>");
            }

            xml.Append("</ExchangeMessage>");
            xml.Append("</soap:Body></soap:Envelope>");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.ToString());
            foreach (GenericStrategyEvent @event in runtime.StrategyEvents)
            {
                CreateSoapHeader(doc, @event.Actor, @event.Description, @event.Source, @event.ElapsedTime);
            }

            return doc.OuterXml;
        }

        public XmlElement AddDataSet(XmlDocument soapMessage, XmlNode data, string actionID, XmlNamespaceManager soapNamespaceManager)
        {
            XmlElement dataSet = soapMessage.CreateElement("dataset");
            dataSet.SetAttribute("id", actionID);

            //NewDataSet.SetAttribute("index", Index.ToString());
            //NewDataSet.SetAttribute("tool", ruleName);
            // If there was no actor, don't write the attribute
            //if (ActorID > 0)
            //{
            //    NewDataSet.SetAttribute("actor", ActorID.ToString());
            //}
            // If there was actually a new chunk, slap it in there

            if (data != null)
            {
                try
                {
                    XmlNode strippedNamespaces = dataSet.OwnerDocument.ReadNode(new XmlTextReader(RemoveNamespaces(data), XmlNodeType.Element, null));
                    dataSet.AppendChild(strippedNamespaces);
                }
                catch (Exception exception)
                {
                    dataSet.AppendChild(dataSet.OwnerDocument.ImportNode(data, true));
                }
            }

            // Add to the Soap Message
            XmlNode target = soapMessage.SelectSingleNode("/soap:Envelope/soap:Body", soapNamespaceManager);
            if (target == null)
            {
                target = soapMessage.DocumentElement;
            }
            target.AppendChild(dataSet);
            return dataSet;
        }

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

        /// <summary>
        /// TODO: move status into runtime.
        /// </summary>
        /// 
        /// <param name="status"></param>
        /// <param name="runtime"></param>
        /// 
        public static void Track(string context, string status, ExchangeRuntime runtime)
        {
            string xml = CreateSoapMessage(runtime);
            FileInfo file = new FileInfo(Utility.CreateFileName(DateTime.Now, context, status, runtime.GetLead().Guid));
            StreamWriter writer = file.CreateText();
            writer.Write(xml);
            writer.Close();
        }

        /// <summary>
        /// TODO: move status into runtime.
        /// </summary>
        /// 
        /// <param name="status"></param>
        /// <param name="runtime"></param>
        /// 
        public static void Report(string context, string status, ExchangeRuntime runtime, string html)
        {
            string xml = CreateSoapMessage(runtime);
            FileInfo file = new FileInfo(Utility.CreateFileName(DateTime.Now, context, status, runtime.GetLead().Guid, "html"));
            StreamWriter writer = file.CreateText();
            writer.Write(html);
            writer.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="context"></param>
        /// <param name="status"></param>
        /// <param name="runtime"></param>
        /// <param name="html"></param>
        /// 
        public static void TrackComplete(string context, string status, ExchangeRuntime runtime, string html)
        {
            Track(context, status, runtime);
            Report(context, status, runtime, html);
        }
    }
}
