
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
    public class NotificationEvent : GenericStrategyEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private string from;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string to;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string subject;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string body;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public NotificationEvent()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public NotificationEvent(string actor)
            : base(actor, typeof(NotificationEvent).ToString(), "Notification strategy that sends an email from any point in the exchange runtime.")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string From
        {
            get { return from; }
            set { from = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string To
        {
            get { return to; }
            set { to = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Body
        {
            get { return body; }
            set { body = value; }
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
            xml.AppendFormat("<notification type=\"email\" email=\"{1}\" created=\"{2}\" guid=\"{0}\" >", Guid, Email, Created);
            xml.AppendFormat("<from>{0}</from>", From);
            xml.AppendFormat("<to>{0}</to>", To);
            xml.AppendFormat("<subject>{0}</subject>", Subject);
            xml.Append("<body />");
            xml.AppendFormat("<time unit=\"ms\">{0}</time>", ElapsedTime);
            xml.Append("</notification>");
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
                    .AppendFormat("From={0},", From)
                    .AppendFormat("To={0},", To)
                    .AppendFormat("Subject={0},", Subject)
                    .Append("Body=").ToString();
        }
    }
}
