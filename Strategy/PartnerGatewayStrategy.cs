
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
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

using log4net;

using Exchange.Distribution.Gateway;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class PartnerGatewayStrategy : GenericAllocationStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private ILog logger = LogManager.GetLogger(typeof(PartnerGatewayStrategy));

        /// <summary>
        /// 
        /// </summary>
        /// 
        // private static readonly string ALLOCATION_TIMEOUT = "AllocationTimeout";

        /// <summary>
        /// 
        /// </summary>
        /// 
        public PartnerGatewayStrategy()
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
                        
            Reduce(runtime);

            timer.Stop();
            if (logger.IsInfoEnabled)
            {
                logger.InfoFormat("gateway elapsed time: {0}.", timer.ElapsedMilliseconds);
            }
            
            runtime.Continue();
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="runtime"></param>
        /// 
        /// <returns></returns>
        /// 
        private ExchangeDisposition Reduce(ExchangeRuntime runtime)
        {
            //string timeout = ConfigurationManager.AppSettings[ALLOCATION_TIMEOUT];
            //if (String.IsNullOrEmpty(timeout))
            //{
            //    timeout = "3000";
            //    if (logger.IsWarnEnabled)
            //    {
            //        logger.WarnFormat("Failed to get timeout from configuration.  Defaulting to timeout: {0}.", timeout);
            //    }
            //}

            //ParallelAllocation allocation = new ParallelAllocation(Convert.ToInt32(timeout));
            
            //foreach (IntegrationPartner partner in runtime.GetExecutionPlan().Partners)
            //{
            //    try
            //    {
            //        IPartnerGateway gateway = GetPartnerGateway(partner.PartnerName) as IPartnerGateway;
            //        allocation.AddPartnerGateway(gateway);
            //    }
            //    catch (Exception exception)
            //    {
            //        // This exception will be raised whenever the specified PartnerGateway name
            //        // is not configured in the dependancy injection container.  No need to take
            //        // action besides logging the missing gateway name.
            //        if (logger.IsWarnEnabled)
            //        {
            //            logger.Warn(string.Format("missing gateway: {0}?", partner.PartnerName));
            //        }
            //    }
            //}
            
            //Allocation result = allocation.Allocate(runtime.GetLead());
            //runtime.Allocation = result;
            //return result;
            return null;
        }
    }
}