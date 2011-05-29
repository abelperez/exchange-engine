
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
    public class DebtLeadEvent : GenericStrategyEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private DebtLead lead;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public DebtLeadEvent()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public DebtLead Lead
        {
            get { return lead; }
            set { lead = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="actor"></param>
        /// 
        public DebtLeadEvent(string actor)
            : base(actor, typeof(DebtLeadEvent).ToString(), "Captures all details about the incoming debt lead.")
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
            xml.Append("<lead>");
            xml.AppendFormat("<guid>{0}</guid>", lead.Guid);
            xml.AppendFormat("<aid>{0}</aid>", lead.Aid);
            xml.AppendFormat("<cid>{0}</cid>", lead.Cid);
            xml.AppendFormat("<tid>{0}</tid>", lead.Tid);
            xml.AppendFormat("<sid>{0}</sid>", lead.Sid);
            xml.AppendFormat("<firstname>{0}</firstname>", lead.FirstName);
            xml.AppendFormat("<lastname>{0}</lastname>", lead.LastName);
            xml.AppendFormat("<email>{0}</email>", lead.Email);
            xml.AppendFormat("<phone>{0}</phone>", lead.Phone);
            xml.AppendFormat("<secondaryphone>{0}</secondaryphone>", lead.SecondaryPhone);
            xml.AppendFormat("<street>{0}</street>", lead.Street);
            xml.AppendFormat("<city>{0}</city>", lead.City);
            xml.AppendFormat("<zipcode>{0}</zipcode>", lead.Zip);
            xml.AppendFormat("<state>{0}</state>", lead.State);
            xml.AppendFormat("<created>{0}</created>", lead.Created);
            xml.Append("<debt>");
            xml.AppendFormat("<creditcardamount>{0}</creditcardamount>", lead.CreditCardDebtAmount);
            xml.AppendFormat("<monthlypayments>{0}</monthlypayments>", lead.MonthlyPayments);
            xml.AppendFormat("<paymentstatus>{0}</paymentstatus>", lead.PaymentStatus);
            xml.AppendFormat("<unsecuredamount>{0}</unsecuredamount>", lead.UnsecuredDebtAmount);
            xml.Append("</debt>");
            xml.Append("</lead>");
            return xml.ToString();
        }
    }
}
