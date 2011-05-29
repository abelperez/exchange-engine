
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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

using Exchange.Engine.Strategy.Events;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class TransformerStrategy : GenericStrategy, IExchangeStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private string template;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string resultKey;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public TransformerStrategy()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="name"></param>
        /// 
        public TransformerStrategy(string name) 
            : base(name)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Template
        {
            get 
            { 
                return template; 
            }
            set 
            { 
                template = string.Format(@"{0}\Xsl\{1}", new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, value); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string ResultKey
        {
            get { return resultKey; }
            set { resultKey = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="context"></param>
        /// 
        public override void Invoke(ExchangeRuntime runtime)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            string xml = ExchangeTracker.CreateSoapMessage(runtime);
            
            XslCompiledTransform transformer = new XslCompiledTransform();
            transformer.Load(template);
            
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            StringWriter writer = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(writer);
            transformer.Transform(doc, xmlWriter);
            string result = writer.ToString();

            runtime.StoreData(ResultKey, result);

            TransformerEvent @event = new TransformerEvent(this.GetType().Name);
            @event.Template = template;
            @event.Result = result;
            timer.Stop();
            @event.ElapsedTime = timer.ElapsedMilliseconds;
            runtime.AddStrategyEvent(@event);
            runtime.Continue();
        }
    }
}
