
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

using Exchange.Engine.Strategy.Events;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// Removes xml content from specified xml
    /// messages based on a xpath expression.
    /// </summary>
    /// 
    public class CleanerStrategy : GenericStrategy
    {
        /// <summary>
        /// XPath expression that denotes elements to be removed.
        /// </summary>
        /// 
        private string axis;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string target;
        
        /// <summary>
        /// 
        /// </summary>
        /// 
        private string[] values;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string key;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public CleanerStrategy()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Axis
        {
            get { return axis; }
            set { axis = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string[] Values
        {
            get { return values; }
            set { values = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Target
        {
            get { return target; }
            set { target = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="context"></param>
        /// 
        public override void Invoke(ExchangeRuntime runtime)
        {
            CleanerEvent @event = new CleanerEvent();

            Stopwatch timer = new Stopwatch();
            timer.Start();

            XmlDocument xml = runtime.GetData(key) as XmlDocument;

            foreach (XmlNode node in xml.SelectNodes(Axis))
            {
                string targetValue = node.SelectSingleNode(Target).InnerText;

                foreach (string value in Values)
                {
                    if (value.ToLower() == targetValue.ToLower())
                    {
                        node.ParentNode.RemoveChild(node);
                        break;
                    }
                }
            }

            timer.Stop();
            @event.ElapsedTime = timer.ElapsedMilliseconds;
            runtime.Continue();
        }
    }
}