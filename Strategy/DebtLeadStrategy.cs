
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

using Exchange.Debt;
using Exchange.Engine.Strategy.Events;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class DebtLeadStrategy : GenericStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        public DebtLeadStrategy()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="runtimme"></param>
        /// 
        public override void Invoke(ExchangeRuntime runtime)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            // NOTE: this could throw an invalid cast exception if bad developer makes a mistake from ExchangeEngine.
            DebtLeadEvent @event = new DebtLeadEvent("");
            @event.Lead = runtime.GetLead() as DebtLead;

            timer.Stop();
            @event.ElapsedTime = timer.ElapsedMilliseconds;
            runtime.AddStrategyEvent(@event);
            runtime.Continue();
        }
    }
}
