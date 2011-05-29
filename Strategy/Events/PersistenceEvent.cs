
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
using Exchange.Debt;

#endregion

namespace Exchange.Engine.Strategy.Events
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [Serializable]
    public class PersistenceEvent : GenericStrategyEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private Lead lead;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public PersistenceEvent()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="actor"></param>
        /// 
        public PersistenceEvent(string actor)
            : base(actor, typeof(PersistenceEvent).ToString(), "Persistence strategy persists leads into the exchange lead store.")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public Lead Lead
        {
            get { return lead; }
            set { lead = value; }
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
            xml.AppendFormat("<persist email=\"{0}\" created=\"{1}\" guid=\"{2}\" site=\"\" version=\"\" status=\"\" type=\"debt\" server=\"\" >", Email, Created, Guid);
            xml.AppendFormat("<firstname>{0}</firstname>", Lead.FirstName);
            xml.AppendFormat("<lastname>{0}</lastname>", Lead.LastName);
            xml.AppendFormat("<phone>{0}</phone>", Lead.Phone);
            xml.AppendFormat("<secondaryphone>{0}</secondaryphone>", Lead.SecondaryPhone);
            xml.AppendFormat("<street>{0}</street>", Lead.Street);
            xml.AppendFormat("<city>{0}</city>", Lead.City);
            xml.AppendFormat("<state>{0}</state>", Lead.State);
            xml.AppendFormat("<zipcode>{0}</zipcode>", Lead.Zip);
            xml.AppendFormat("<aid>{0}</aid>", Lead.Aid);
            xml.AppendFormat("<cid>{0}</cid>", Lead.Cid);
            xml.AppendFormat("<sid>{0}</sid>", Lead.Sid);
            xml.AppendFormat("<tid>{0}</tid>", Lead.Tid);
            xml.Append("<debt>");
            xml.AppendFormat("<creditcardamount>{0}</creditcardamount>", ((DebtLead)Lead).CreditCardDebtAmount);
            xml.AppendFormat("<monthlypayments>{0}</monthlypayments>", ((DebtLead)Lead).MonthlyPayments);
            xml.AppendFormat("<paymentstatus>{0}</paymentstatus>", ((DebtLead)Lead).PaymentStatus);
            xml.AppendFormat("<unsecuredamount>{0}</unsecuredamount>", ((DebtLead)Lead).UnsecuredDebtAmount);
            xml.Append("</debt>");
            xml.AppendFormat("<time unit=\"ms\">{0}</time>", ElapsedTime);
            xml.Append("</persist>");
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
                    .AppendFormat("Lead = {0}, ", Lead).ToString();
        }
    }
}
