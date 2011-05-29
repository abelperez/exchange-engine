
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

using Exchange.Debt;
using Exchange.Debt.DataAccess;
using Exchange.Duplicate;
using Exchange.Duplicate.DataAccess;
using Exchange.Engine.Strategy.Events;

using Mindplex.Commons.DAO;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class LeadPersistStrategy : GenericStrategy, IExchangeStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private static ILog logger = LogManager.GetLogger(typeof(LeadPersistStrategy));

        /// <summary>
        /// 
        /// </summary>
        /// 
        public LeadPersistStrategy()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="name"></param>
        /// 
        public LeadPersistStrategy(string name)
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
            Stopwatch timer = new Stopwatch();
            timer.Start();

            DebtLead result = ExchangeService.SaveDebtLead(runtime.GetLead() as DebtLead);
            runtime.GetLead().Id = result.Id;

            PersistenceEvent @event = new PersistenceEvent(this.GetType().Name);
            @event.Lead = runtime.GetLead();

            //if (logger.IsDebugEnabled)
            //{
            //    logger.DebugFormat("persisting inbound duplicate for publiser: {0}, lead: {1}.", runtime.GetLead().Aid, runtime.GetLead().Email);
            //}

            //InboundDuplicate dupe = new InboundDuplicate();
            //dupe.PublisherId = runtime.GetLead().Aid;
            //dupe.Email = runtime.GetLead().Email;
            //dupe.Created = runtime.GetLead().Created;
            //DuplicateDataAccess.SaveInboundDuplicate(dupe);

            timer.Stop();
            @event.ElapsedTime = timer.ElapsedMilliseconds;
            runtime.AddStrategyEvent(@event);
            runtime.Continue();
        }
    }
}
