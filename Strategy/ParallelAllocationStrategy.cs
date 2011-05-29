
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
using System.Threading;

using Mindplex.Commons.Threading;
using Exchange.Distribution.Gateway;

using log4net;

#endregion

namespace Exchange.Engine
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    internal sealed class ParallelAllocationStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private static ILog logger = LogManager.GetLogger(typeof(ParallelAllocationStrategy));
        
        /// <summary>
        /// 
        /// </summary>
        /// 
        private readonly List<PartnerGatewayResponse> results = new List<PartnerGatewayResponse>();

        /// <summary>
        /// 
        /// </summary>
        /// 
        private readonly List<IPartnerGateway> gateways = new List<IPartnerGateway>();

        /// <summary>
        /// 
        /// </summary>
        /// 
        private readonly int timout;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private ManualResetEvent signal;
                
        /// <summary>
        /// 
        /// </summary>
        /// 
        public ParallelAllocationStrategy(int timout)
        {
            this.timout = timout;
            signal = new ManualResetEvent(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="partnerGateway"></param>
        /// 
        public void AddPartnerGateway(IPartnerGateway partnerGateway)
        {
            if (partnerGateway != null)
            {
                gateways.Add(partnerGateway);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public int Timeout
        {
            get { return timout; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public int PartnerGatewayCount
        {
            get { return gateways.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="lead"></param>
        /// 
        /// <returns></returns>
        /// 
        public ExchangeDisposition Allocate(Lead lead)
        {
            lock (gateways)
            {
                foreach (IPartnerGateway gateway in gateways)
                {
                    gateway.BeginPost(lead, ReduceCallback, this);
                }
            }
                        
            signal.WaitOne(timout, false);

            ExchangeDisposition allocation = new ExchangeDisposition();
            allocation.Coverage = false;
            allocation.Responses = results;

            foreach (PartnerGatewayResponse response in results)
            {
                if (response.Coverage)
                {
                    allocation.Coverage = true;
                    break;
                }
            }

            return allocation;
        }
                                
        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="asyncResult"></param>
        /// 
        private void ReduceCallback(IAsyncResult asyncResult)
        {
            ParallelAllocationStrategy state = asyncResult.AsyncState as ParallelAllocationStrategy;
            GatewayAsyncResult result = asyncResult as GatewayAsyncResult;
            try
            {
                PartnerGatewayResponse response = result.EndInvoke();
                results.Add(response);

                if (response.Coverage && "CarsDirectDynamic".Equals(response.GatewayName))
                {
                    if (logger.IsDebugEnabled)
                    {
                        logger.DebugFormat("Thread: {0}, found coverage, gateway: {1}.", Thread.CurrentThread.GetHashCode(), response.GatewayName);
                    }
                    state.signal.Set();
                }
                else
                {
                    if (logger.IsDebugEnabled)
                    {
                        logger.DebugFormat("Thread: {0}, found no coverage, gateway: {1}.", Thread.CurrentThread.GetHashCode(), response.GatewayName);
                    }
                }
            }
            catch (Exception exception)
            {
                // most likely a timeout operation on the current gateway.
                if (logger.IsWarnEnabled)
                {
                    logger.Warn("Failed to reduce result.", exception);
                }
            }
        }
    }    
}