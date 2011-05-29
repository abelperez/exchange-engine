
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

using Mindplex.Commons.Model;

using Exchange.Distance;
using Exchange.Engine.Strategy.Events;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class OutboundDuplicateStrategy : GenericStrategy, IExchangeStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private static ILog logger = LogManager.GetLogger(typeof(OutboundDuplicateStrategy));

        /// <summary>
        /// 
        /// </summary>
        /// 
        public OutboundDuplicateStrategy()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="name"></param>
        /// 
        public OutboundDuplicateStrategy(string name)
            : base(name)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="runtime"></param>
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

            List<long> prospects = new List<long>();
            EntityCollection<OrderStateRadius> orders = runtime.GetData(ExchangeRuntime.ORDER_STATES) as EntityCollection<OrderStateRadius>;
            OutboundDuplicateEvent @event = new OutboundDuplicateEvent(this.GetType().Name);
            
            foreach (OrderStateRadius radius in orders)
            {
                if (ExchangeService.IsOutboundDuplicate(runtime.GetLead().Email, radius.Aid))
                {
                    @event.Aid = radius.Aid;
                    @event.Duplicate = true;
                    continue;
                }
                prospects.Add(radius.OrderId);
            }

            timer.Stop();
            @event.ElapsedTime = timer.ElapsedMilliseconds;
            runtime.StoreData(ExchangeRuntime.PROSPECTS, prospects);
            runtime.AddStrategyEvent(@event);
            runtime.Continue();
        }
    }
}
