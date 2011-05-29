
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

using Exchange.Delivery;
using Exchange.Engine.Strategy.Events;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class DistributionStrategy : GenericStrategy, IExchangeStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private static ILog logger = LogManager.GetLogger(typeof(DistributionStrategy));

        /// <summary>
        /// 
        /// </summary>
        /// 
        public DistributionStrategy()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="name"></param>
        /// 
        public DistributionStrategy(string name) 
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

            List<long> prospects = runtime.GetData(ExchangeRuntime.DISTRIBUTIONS) as List<long>;

            DistributionEvent @event = new DistributionEvent(this.GetType().Name);

            if (prospects != null)
            {
                foreach (long prospect in prospects)
                {
                    DeliveryQueue queue = new DeliveryQueue();
                    queue.AdvertiserId = prospect;
                    queue.Disposition = "PENDING";
                    queue.LeadId = runtime.GetLead().Id;

                    ExchangeService.SaveDeliveryQueue(queue);
                    @event.Advertisers.Add(prospect);
                }
            }

            timer.Stop();
            @event.ElapsedTime = timer.ElapsedMilliseconds;
            runtime.AddStrategyEvent(@event);
            runtime.Continue();
        }
    }
}
