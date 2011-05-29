
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
    public class DistributionEvent : GenericStrategyEvent
    {
        /// <summary>
        ///
        /// </summary>
        ///
        private List<long> advertisers = new List<long>();

        /// <summary>
        /// 
        /// </summary>
        /// 
        public DistributionEvent(string actor)
            : base(actor, typeof(DistributionEvent).ToString(), "Distribution strategy delivers allocated leads into the delivery queue.")
        {
        }

        /// <summary>
        ///
        /// </summary>
        ///
        public List<long> Advertisers
        {
            get { return advertisers; }
            set { advertisers = value; }
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
            xml.AppendFormat("<distribution email=\"{1}\" created=\"{2}\" guid=\"{0}\">", Guid, Email, Created);

            foreach (long advertiserid in advertisers)
            {
                xml.AppendFormat("<advertiserid>{0}</advertiserid>", advertiserid);
            }
            xml.AppendFormat("<time unit=\"ms\">{0}</time>", ElapsedTime);
            xml.Append("</distribution>");
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
                    .AppendFormat("Advertisers = {0}, ", Advertisers).ToString();
        }
    }
}
