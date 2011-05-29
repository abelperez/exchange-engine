
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

using Exchange.Duplicate;
using Exchange.Engine.Strategy.Events;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class InboundDuplicateStrategy : GenericStrategy, IExchangeStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private static ILog logger = LogManager.GetLogger(typeof(InboundDuplicateStrategy));

        /// <summary>
        /// 
        /// </summary>
        /// 
        public InboundDuplicateStrategy()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="name"></param>
        /// 
        public InboundDuplicateStrategy(string name) 
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

            InboundDuplicateEvent @event = new InboundDuplicateEvent(this.GetType().Name);
            @event.Duplicate = false;
            runtime.AddStrategyEvent(@event);

            if (ExchangeService.IsInboundDuplicate(runtime.GetLead().Email))
            {
                runtime.IsInboundDuplicate = true;
                @event.Duplicate = true;
                @event.Aid = Convert.ToInt64(runtime.GetLead().Aid);

                runtime.Status = ExchangeRuntime.DUPLICATE;
                runtime.Abort = true;
            }
            else
            {
                InboundDuplicate duplicate = new InboundDuplicate();
                duplicate.Email = runtime.GetLead().Email;
                duplicate.PublisherId = runtime.GetLead().Aid;
                duplicate.Created = runtime.GetLead().Created;

                ExchangeService.SaveInboundDuplicate(duplicate);
            }

            timer.Stop();
            @event.ElapsedTime = timer.ElapsedMilliseconds;
            runtime.Continue();
        }
    }
}
