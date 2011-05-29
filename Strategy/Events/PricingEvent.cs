
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
    public class PricingEvent : GenericStrategyEvent
    {
        /// <summary>
        ///
        /// </summary>
        ///
        private List<Bid> bids = new List<Bid>();

        /// <summary>
        /// 
        /// </summary>
        /// 
        public PricingEvent()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="actor"></param>
        /// 
        public PricingEvent(string actor)
            : base(actor, typeof(PricingEvent).ToString(), "Pricing strategy runs allocation plan.")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public List<Bid> Bids
        {
            get { return bids; }
            set { bids = value; }
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
            xml.AppendFormat("<pricemap email=\"{0}\" created=\"{1}\" guid=\"{2}\" >", Email, Created, Guid);

            foreach (Bid bid in bids)
            {
                xml.Append("<bid>");
                xml.AppendFormat("<advertiserid>{0}</advertiserid>", bid.Aid);
                xml.AppendFormat("<orderid>{0}</orderid>", bid.Oid);
                xml.Append("</bid>");
            }
            xml.AppendFormat("<time unit=\"ms\">{0}</time>", ElapsedTime);
            xml.Append("</pricemap>");
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
                    .AppendFormat("Bids = {0}, ", Bids).ToString();
        }
    }
}
