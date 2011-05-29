
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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

using log4net;
using log4net.Config;

using Mindplex.Commons.Service;

using Exchange;
using Exchange.Acquisition;
using Exchange.Engine.Strategy.Events;
using Exchange.Validation;
using Exchange.Debt;

#endregion

namespace Exchange.Engine
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class ExchangeRuntime
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private static ILog logger = LogManager.GetLogger(typeof(ExchangeRuntime));

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly string ACCEPTED = "ACCEPTED";

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly string PENDING = "PENDING";

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly string DUPLICATE = "DUPLICATE";

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly string INVALID = "INVALID";

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly string ERROR = "ERROR";

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly string SUSPECT = "SUSPECT";

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly string QUEUED = "QUEUED";

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly string SECURITY = "SECURITY";

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly string EPHEMERAL = "EPHEMERAL";

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly string PENDING_MATCH = "PENDING_MATCH";

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly string PENDING_VERIFICATION = "PENDING_VERIFICATION";

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly string LEAD = "lead";

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly string LEAD_TYPE = "leadType";

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly string ORDER_STATES = "OrderStates";

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly string PROSPECTS = "Prospects";

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly string DISTRIBUTIONS = "Distributions";

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly int DemandAction = 1;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly int AllocationAction = 2;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string vertical;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string verticalType;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private bool abort = false;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string status;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private List<GenericStrategyEvent> events = new List<GenericStrategyEvent>();

        /// <summary>
        /// 
        /// </summary>
        /// 
        private int allocationCount = 0;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private bool suspect;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private bool inboundDuplicate;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private bool pendingMatch;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public event EventHandler<ExchangeEvent<ExchangeRuntime>> BeforeRequest;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public event EventHandler<ExchangeEvent<ExchangeRuntime>> AfterRequest;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public event EventHandler<ExchangeEvent<Exception>> StrategyError;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public event EventHandler<ExchangeEvent<ExchangeRuntime>> BeforeAllocation;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public event EventHandler<ExchangeEvent<ExchangeRuntime>> AfterAllocation;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private IList strategies;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private IList modules;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private int index = 0;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private int waitTime = -1;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private LeadSource leadSource;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private List<Error> validationErrors = new List<Error>();

        /// <summary>
        /// 
        /// </summary>
        /// 
        private Dictionary<string, object> registry = new Dictionary<string, object>();

        /// <summary>
        /// 
        /// </summary>
        /// 
        private bool active = false;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private bool paused = false;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private ExchangeDisposition allocation;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private List<Exception> errors = new List<Exception>();

        /// <summary>
        /// 
        /// </summary>
        /// 
        public ExchangeRuntime()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="lead"></param>
        public ExchangeRuntime(Lead lead) 
            : this()
        {
            if (lead != null)
            {
                StoreData(LEAD, lead);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public void Init()
        {
            BeforeRequest += EmptyEvent;
            AfterRequest += EmptyEvent;
            StrategyError += EmptyEvent;
            BeforeAllocation += EmptyEvent;
            AfterAllocation += EmptyEvent;

            foreach (IExchangeModule module in modules)
            {
                module.Init(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public LeadSource LeadSource
        {
            get { return leadSource; }
            set { leadSource = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public List<Exception> Errors
        {
            get { return errors; }
            set { errors = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public List<Error> ValidationErrors
        {
            get { return validationErrors; }
            set { validationErrors = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="strategy"></param>
        /// 
        public void AddStrategy(IExchangeStrategy strategy)
        {
            if (strategy != null)
            {
                strategies.Add(strategy); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public void Invoke()
        {
            BeforeRequest(this, new ExchangeEvent<ExchangeRuntime>(this));
            Continue();
            AfterRequest(this, new ExchangeEvent<ExchangeRuntime>(this));
            registry.Remove(LEAD);
        }

        /// <summary>
        /// Continues exchange runtime execution.
        /// </summary>
        /// 
        public void Continue()
        {
            if (strategies != null)
            {
                int current = index;
                Interlocked.Increment(ref index);
                if (current < strategies.Count)
                {
                    if (!active)
                    {
                        return;
                    }

                    if (paused)
                    {
                        Thread.Sleep(waitTime);
                        waitTime = -1;
                        paused = false;
                    }
                    if (logger.IsDebugEnabled)
                    {
                        logger.DebugFormat("Invoking strategy no. {0}", index);
                    }

                    try
                    {
                        IExchangeStrategy strategy = (strategies[current] as IExchangeStrategy);
                        for (int x = 0; x < strategy.RetryCount; x++)
                        {
                            try
                            {
                                strategy.Invoke(this);
                                break;
                            }
                            catch (Exception exception)
                            {
                                logger.Warn("Failed to invoke strategy.", exception);
                                errors.Add(exception);
                                
                                if (x == (strategy.RetryCount - 1))
                                {
                                    Abort = true;
                                    Continue();
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        logger.Warn("Failed to invoke strategy.", exception);
                        errors.Add(exception);
                        Abort = true;
                        Continue();
                    }
                }
                else
                {
                    if (logger.IsDebugEnabled)
                    {
                        logger.Debug("All strategies invoked");
                    }
                }
            }
            else
            {
                throw new ServiceException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public void Stop()
        {
            active = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="milliseconds"></param>
        /// 
        public void PauseAndContinue(int milliseconds)
        {
            this.waitTime = milliseconds;
            this.paused = true;
            Continue();
        }
                
        /// <summary>
        /// 
        /// </summary>
        /// 
        public IList Strategies
        {
            get { return strategies; }
            set { strategies = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public bool Abort
        {
            get { return abort; }
            set { abort = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Vertical
        {
            get { return vertical; }
            set { vertical = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string VerticalType
        {
            get { return verticalType; }
            set { verticalType = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public IList Modules
        {
            get { return modules; }
            set { modules = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public ExchangeDisposition Allocation
        {
            get { return allocation; }
            set { allocation = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public int AllocationCount
        {
            get { return allocationCount; }
            set { allocationCount = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="key"></param>
        /// 
        /// <returns></returns>
        /// 
        public object GetData(string key)
        {
            object result;
            registry.TryGetValue(key, out result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// 
        public void StoreData(string key, object value)
        {
            if (String.IsNullOrEmpty(key) || value == null)
            {
                return;
            }
            registry.Add(key, value);
        }

        /// <summary>
        /// Add exception logic around casting errors.
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        public Lead GetLead()
        {
            return GetData(LEAD) as Lead;
        }

        /// <summary>
        /// Add exception logic around casting errors.
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        public DebtLead GetDebtLead()
        {
            return GetData(LEAD) as DebtLead;
        }

        /// <summary>
        /// Add exception logic around casting errors.
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        public LeadType GetLeadType()
        {
            return GetData(LEAD_TYPE) as LeadType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="event"></param>
        /// 
        public ExchangeRuntime AddStrategyEvent(GenericStrategyEvent @event)
        {
            @event.Guid = GetLead().Guid;
            @event.Email = GetLead().Email;
            @event.Created = DateTime.Now;
            events.Add(@event);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public List<GenericStrategyEvent> StrategyEvents
        {
            get { return events; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public bool IsSuspect
        {
            get { return suspect; }
            set { suspect = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public bool IsInboundDuplicate
        {
            get { return inboundDuplicate; }
            set { inboundDuplicate = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public bool PendingMatch
        {
            get { return pendingMatch; }
            set { pendingMatch = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public void FlagComplete() 
        {
            //allocationAuditTrail.Responses = Allocation.Responses;
            //allocationAuditTrail.Coverage = "false"; // Convert.ToString(Allocation.Coverage);
            //allocationAuditTrail.ExecutionPlanID = 0; // GetExecutionPlan().Id;
            //allocationAuditTrail.Threshold = 0; // GetExecutionPlan().Threshold;
            //allocationAuditTrail.Mode = 2; // 2 will mean Ping Only Retail+Daytrade.

            //AfterAllocation(this, new AllocationRuntimeEventArgs(allocationAuditTrail));
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// 
        protected void EmptyEvent(object sender, EventArgs args)
        {
        }
    }
}