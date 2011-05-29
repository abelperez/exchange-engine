
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
using System.Linq;
using System.Text;

#endregion

namespace Exchange.Engine.Strategy.Events
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [Serializable]
    public class TransformerEvent : GenericStrategyEvent
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
        private string result;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public TransformerEvent()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="actor"></param>
        /// 
        public TransformerEvent(string actor)
            : base(actor, typeof(TransformerEvent).ToString(), "XSL Transformer that converts from XML to other formats.")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Template
        {
            get { return template; }
            set { template = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Result
        {
            get { return result; }
            set { result = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        public override string ToXml()
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<transformer>");
            xml.AppendFormat("<template>{0}</template>", Template);
            xml.AppendFormat("<result type=\"text\"><![CDATA[{0}]]></result>", Result);
            xml.AppendFormat("<time unit=\"ms\">{0}</time>", ElapsedTime);
            xml.Append("</transformer>");
            return xml.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        ///
        /// <returns></returns>
        ///
        public override string ToString()
        {
            return new StringBuilder(base.ToString())
                    .AppendFormat("Template = {0}, ", template).ToString();
        }
    }
}
