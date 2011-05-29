
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

using log4net;

using Exchange.Vintage;
using Exchange.Engine.Strategy.Events;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class VintageStrategy : GenericStrategy, IExchangeStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private static ILog logger = LogManager.GetLogger(typeof(VintageStrategy));

        /// <summary>
        /// 
        /// </summary>
        /// 
        public VintageStrategy()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="name"></param>
        /// 
        public VintageStrategy(string name)
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

            VintageEvent @event = new VintageEvent(this.GetType().Name);

            if (runtime.AllocationCount == 0)
            {
                runtime.Status = ExchangeRuntime.PENDING_MATCH;
            }

            // TODO: extract to method.
            if (runtime.AllocationCount < 4)
            {
                VintageDebtLead lead = new VintageDebtLead();
                lead.LeadId = runtime.GetLead().Id;
                lead.Status = "PENDING";

                ExchangeService.SaveVintageDebtLead(lead);
                @event.BuyRate = runtime.AllocationCount;
                @event.Vintage = true;
            }

            timer.Stop();
            @event.ElapsedTime = timer.ElapsedMilliseconds;
            runtime.AddStrategyEvent(@event);
            runtime.Continue();
        }
    }
}
