
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
    public class InboundDuplicateEvent : GenericStrategyEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private long aid;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private bool duplicate;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public InboundDuplicateEvent()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public InboundDuplicateEvent(string actor)
            : base(actor, typeof(InboundDuplicateEvent).ToString(), "Inbound duplicate strategy checks if lead is an incoming duplicate.")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public long Aid
        {
            get { return aid; }
            set { aid = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public bool Duplicate
        {
            get { return duplicate; }
            set { duplicate = value; }
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
            xml.AppendFormat("<duplicate type=\"inbound\" email=\"{0}\" created=\"{1}\" guid=\"{2}\" >", Email, Created, Guid);
            xml.AppendFormat("<aid>{0}</aid>", Aid);
            xml.AppendFormat("<email>{0}</email>", Email);
            xml.AppendFormat("<isduplicate>{0}</isduplicate>", Duplicate);
            xml.AppendFormat("<time unit=\"ms\">{0}</time>", ElapsedTime);
            xml.Append("</duplicate>");
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
                    .AppendFormat("Aid = {0}, ", Aid) 
                    .AppendFormat("Duplicate = {0}, ", Duplicate).ToString();
        }
    }
}
