
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
    public class MessageGatewayEvent : GenericStrategyEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private string destination;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string content;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public MessageGatewayEvent()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="actor"></param>
        /// 
        public MessageGatewayEvent(string actor)
            : base(actor, typeof(MessageGatewayEvent).ToString(), "Message Gateway that delivers xml messages to specified destination queue.")
        {
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
            xml.Append("<gateway type=\"messaging\">");
            xml.AppendFormat("<destination>{0}</destination>", Destination);
            //xml.AppendFormat("<result type=\"text\"><![CDATA[{0}]]></result>", Content);
            xml.AppendFormat("<time unit=\"ms\">{0}</time>", ElapsedTime);
            xml.Append("</gateway>");
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
                    .AppendFormat("Destination = {0}, ", Destination).ToString();
        }
    }
}
