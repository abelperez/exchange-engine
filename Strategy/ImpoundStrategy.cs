
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
using System.Text;

using log4net;

using Exchange.Debt;
using Exchange.Impound;
using Exchange.Engine.Strategy.Events;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class ImpoundStrategy : GenericStrategy, IExchangeStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private static ILog logger = LogManager.GetLogger(typeof(ImpoundStrategy));

        /// <summary>
        /// 
        /// </summary>
        /// 
        public ImpoundStrategy()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="name"></param>
        /// 
        public ImpoundStrategy(string name) 
            : base(name)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="context"></param>
        /// 
        public override void Invoke(ExchangeRuntime runtime)
        {
            if (runtime.Abort)
            {
                runtime.Continue();
                return;
            }

            Stopwatch timer = new Stopwatch();
            timer.Start();

            ImpoundEvent @event = new ImpoundEvent(this.GetType().Name);
            @event.Impounded = false;

            if (ExchangeService.IsSuspectPublisher(runtime.GetLead().Aid))
            {
                SuspectDebtLead lead = new SuspectDebtLead();
                lead.Guid = runtime.GetLead().Guid;
                lead.Aid = runtime.GetLead().Aid;
                lead.Cid = runtime.GetLead().Cid;
                lead.Sid = runtime.GetLead().Sid;
                lead.Tid = runtime.GetLead().Tid;
                lead.FirstName = runtime.GetLead().FirstName;
                lead.LastName = runtime.GetLead().LastName;
                lead.Email = runtime.GetLead().Email;
                lead.Phone = runtime.GetLead().Phone;
                lead.SecondaryPhone = runtime.GetLead().SecondaryPhone;
                lead.Street = runtime.GetLead().Street;
                lead.City = runtime.GetLead().City;
                lead.Zip = runtime.GetLead().Zip;
                lead.State = runtime.GetLead().State;
                lead.Created = runtime.GetLead().Created;
                lead.CreditCardDebtAmount = ((DebtLead) runtime.GetLead()).CreditCardDebtAmount;
                lead.UnsecuredDebtAmount = ((DebtLead)runtime.GetLead()).UnsecuredDebtAmount;
                lead.PaymentStatus = ((DebtLead)runtime.GetLead()).PaymentStatus;
                lead.MonthlyPayments = ((DebtLead)runtime.GetLead()).MonthlyPayments;

                ExchangeService.SaveImpound(lead);
                runtime.IsSuspect = true;
                runtime.Status = ExchangeRuntime.PENDING_VERIFICATION;
                @event.Impounded = true;
                @event.Aid = Convert.ToInt64(runtime.GetLead().Aid);
                return;
            }

            timer.Stop();
            @event.ElapsedTime = timer.ElapsedMilliseconds;
            runtime.AddStrategyEvent(@event);
            runtime.Continue();
        }
    }
}
