
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

using Exchange.Acquisition;
using Exchange.Validation;
using Exchange.Debt;
using Exchange.Engine.Strategy.Events;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class LeadValidationStrategy : GenericStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        public LeadValidationStrategy()
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
            ValidationEvent @event = new ValidationEvent(this.GetType().Name);
            @event.Valid = true;

            if ("DEBT".Equals(runtime.Vertical))
            {
                DebtLead lead = runtime.GetLead() as DebtLead;
                List<Error> errors = DebtValidator.Validate(lead, runtime.LeadSource);
               
                if (errors.Count > 0)
                {
                    @event.Valid = false;
                    @event.Errors = errors;

                    runtime.Status = ExchangeRuntime.INVALID;
                    runtime.ValidationErrors = errors;
                    runtime.Abort = true;
                }
            }

            runtime.AddStrategyEvent(@event);
            runtime.Continue();
        }
    }
}
