
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
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Mindplex.Commons.Model;

#endregion

namespace Exchange.Engine.Strategy.Events
{
	/// <summary>
	///
	/// </summary>
	///
	[Serializable]
    public partial class DistanceEvent : GenericStrategyEvent
    {
        /// <summary>
	    ///
	    /// </summary>
	    ///
        private string state;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private List<Bid> bidOrders = new List<Bid>();

        /// <summary>
        ///
        /// </summary>
        ///
        public DistanceEvent(string actor)
            : base(actor, typeof(DistanceEvent).ToString(), "Distance strategy finds all orders that map to the current state.")
        {
        }

        /// <summary>
	    ///
	    /// </summary>
	    ///
        public string State
        {
            get { return state; }
            set { state = value; }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        public List<Bid> BidOrders
        {
            get { return bidOrders; }
            set { bidOrders = value; }
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
            xml.AppendFormat("<distance state=\"{1}\" email=\"{2}\" created=\"{3}\" guid=\"{0}\" >", Guid, State, Email, Created);
            xml.Append("<bids>");

            foreach (Bid bid in bidOrders)
            {
                xml.AppendFormat("<bid><aid>{0}</aid><oid>{1}</oid></bid>", bid.Aid, bid.Oid);
            }

            xml.Append("</bids>");
            xml.AppendFormat("<time unit=\"ms\">{0}</time>", ElapsedTime);
            xml.Append("</distance>");
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
                    .AppendFormat("State = {0}, ", State)
                    .AppendFormat("Orders = {0}, ", BidOrders).ToString();
        }
    }
}