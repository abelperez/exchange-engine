
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

using Mindplex.Commons;
using Mindplex.Commons.Model;
using Mindplex.Commons.DAO;

using Exchange.Debt;
using Exchange.Distance;
using Exchange.Engine.Strategy.Events;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class DistanceStrategy : GenericStrategy, IExchangeStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private static ILog logger = LogManager.GetLogger(typeof(DistanceStrategy));

        /// <summary>
        /// 
        /// </summary>
        /// 
        public DistanceStrategy()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="name"></param>
        /// 
        public DistanceStrategy(string name) 
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

            EntityCollection<OrderStateRadius> radius = ExchangeService.GetOrderStateRadiusByState(runtime.GetLead().State);

            if (radius.Count() == 0)
            {
                runtime.PendingMatch = true;
                return;
            }
   
            DistanceEvent @event = new DistanceEvent(this.GetType().Name);
            @event.State = runtime.GetLead().State;
            
            foreach (OrderStateRadius state in radius)
            {
                // @event.Orders.Add(state.OrderId);
                @event.BidOrders.Add(new Bid(state.Aid, state.OrderId));
            }

            timer.Stop();
            @event.ElapsedTime = timer.ElapsedMilliseconds;
            runtime.StoreData(ExchangeRuntime.ORDER_STATES, radius);
            runtime.AddStrategyEvent(@event);
            runtime.Continue();
        }
    }
}