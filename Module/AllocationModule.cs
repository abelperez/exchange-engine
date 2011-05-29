
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
using System.Text;

using log4net;

using Mindplex.Commons;

#endregion

namespace Exchange.Engine.Module
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class AllocationModule : ContextAware, IExchangeModule
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private static ILog logger = LogManager.GetLogger(typeof(AllocationModule));

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="runtime"></param>
        /// 
        public void Init(ExchangeRuntime runtime)
        {
            runtime.BeforeRequest += BeforeRequest;
            runtime.AfterRequest += AfterRequest;
            runtime.StrategyError += StrategyError;
            runtime.BeforeAllocation += BeforeAllocation;
            runtime.AfterAllocation += AfterAllocation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// 
        public void BeforeRequest(object sender, ExchangeEvent<ExchangeRuntime> args)
        {
            if (logger.IsDebugEnabled)
            {
                logger.Debug("captured: Before Request.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// 
        public void AfterRequest(object sender, ExchangeEvent<ExchangeRuntime> args)
        {
            if (logger.IsDebugEnabled)
            {
                logger.Debug("captured: After Request.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// 
        public void StrategyError(object sender, ExchangeEvent<Exception> args)
        {
            if (logger.IsDebugEnabled)
            {
                logger.Debug("captured: Strategy Error.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// 
        public void BeforeAllocation(object sender, ExchangeEvent<ExchangeRuntime> args)
        {
            if (logger.IsDebugEnabled)
            {
                logger.Debug("captured: Before Allocation.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// 
        public void AfterAllocation(object sender, ExchangeEvent<ExchangeRuntime> args)
        {
            try
            {
                //MessageSenderGateway gateway = GetObject("MessageSenderGateway") as MessageSenderGateway;
                //gateway.Send(args.AllocationAuditTrail);
            }
            catch (Exception exception)
            {
                // we capture exception because we dont want to interupt pipeline.
                if (logger.IsWarnEnabled)
                {
                    logger.Warn("Failed to send audit trail.", exception);
                }
            }

            if (logger.IsDebugEnabled)
            {
                logger.Debug("captured: After Allocation.");
            }
        }
    }
}
