
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
using System.Text;
using System.Linq;

using log4net;

using Mindplex.Commons.Model;

using Exchange.Duplicate;
using Exchange.Pricing;
using Exchange.Engine.Strategy.Events;
using Exchange.Debt;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// TODO: Constrain this strategy to Debt vertical.
    /// </summary>
    /// 
    public class PricingStrategy : GenericStrategy, IExchangeStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private static ILog logger = LogManager.GetLogger(typeof(PricingStrategy));

        /// <summary>
        /// 
        /// </summary>
        /// 
        public PricingStrategy()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="name"></param>
        /// 
        public PricingStrategy(string name) 
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
                // runtime.Status = ExchangeEngine.
                runtime.Continue();
                return;
            }

            Stopwatch timer = new Stopwatch();
            timer.Start();

            PricingEvent @event = new PricingEvent(this.GetType().Name);
            string debtamount = runtime.GetDebtLead().CreditCardDebtAmount;
            string daysbehind = runtime.GetDebtLead().PaymentStatus;
            Console.WriteLine("amount={0}, days={1}.", debtamount, daysbehind);
            List<long> prospects = runtime.GetData(ExchangeRuntime.PROSPECTS) as List<long>;

            EntityCollection<Allocation> allocations = ExchangeService.GetAllocationPlan(prospects, debtamount, daysbehind);
            if (allocations == null)
            {
                runtime.Status = ExchangeRuntime.PENDING_MATCH;
                timer.Stop();
                @event.ElapsedTime = timer.ElapsedMilliseconds;
                runtime.AllocationCount = 0;
                runtime.AddStrategyEvent(@event);
                runtime.Continue();
                return;
            }

            List<long> distributions = new List<long>();

            foreach (Allocation prospect in allocations)
            {
                OutboundDuplicate outDupe = new OutboundDuplicate();
                outDupe.Aid = Convert.ToString(prospect.Aid);
                outDupe.Oid = Convert.ToString(prospect.OrderId);
                outDupe.Email = runtime.GetLead().Email;
                outDupe.Created = runtime.GetLead().Created;
                ExchangeService.SaveOutboundDuplicate(outDupe);

                distributions.Add(prospect.Aid);
                @event.Bids.Add(new Bid(prospect.Aid, prospect.OrderId));
            }

            if (allocations.Count() > 0)
            {
                runtime.Status = ExchangeRuntime.ACCEPTED;
            }
            
            timer.Stop();
            @event.ElapsedTime = timer.ElapsedMilliseconds;
            runtime.StoreData(ExchangeRuntime.DISTRIBUTIONS, distributions);
            runtime.AllocationCount = allocations.Count();
            runtime.AddStrategyEvent(@event);
            runtime.Continue();
        }
    }
}
