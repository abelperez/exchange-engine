
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
using System.Threading;

using Spring.Context;
using Spring.Context.Support;

using Exchange.Acquisition;
//using Exchange.Acquisition.Tracking;

#endregion

namespace Acquisition.Demo
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        //protected static AcquisitionEvent CreateDemandEvent()
        //{
        //    AcquisitionDemand demand = new AcquisitionDemand();
        //    demand.Aid = new Random().Next(99999).ToString();
        //    demand.Cid = new Random().Next(999).ToString();
        //    demand.City = "Seattle";
        //    demand.Email = "test@alx.com";
        //    demand.FirstName = "John";
        //    demand.LastName = "Cusak";
        //    demand.Password = "abcdef";
        //    demand.Phone = "2323234543";
        //    demand.State = "WA";
        //    demand.Street = "234 WanderLand Ave.";
        //    demand.Username = "affiliate1";
        //    demand.Zip = "98102";
        //    demand.Variant = Exchange.Acquisition.DebtAcquisition.DemandEvent;

        //    return new AcquisitionEvent(demand);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        //protected static AcquisitionEvent CreateAllocationEvent()
        //{
        //    DebtAcquisition allocation = new DebtAcquisition();
        //    allocation.Aid = new Random().Next(99999).ToString();
        //    allocation.Cid = new Random().Next(999).ToString();
        //    allocation.City = "Seattle";
        //    allocation.Email = "test@alx.com";
        //    allocation.FirstName = "John";
        //    allocation.LastName = "Cusak";
        //    allocation.Password = "abcdef";
        //    allocation.Phone = "2323234543";
        //    allocation.State = "WA";
        //    allocation.Street = "234 WanderLand Ave.";
        //    allocation.Username = "affiliate1";
        //    allocation.Zip = "98102";
        //    allocation.Variant = Exchange.Acquisition.DebtAcquisition.AllocationEvent;

        //    return new AcquisitionEvent(allocation);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// 
        //public static void TestAsyncTrackDemandEvent()
        //{
        //    AcquisitionEvent result = CreateDemandEvent();
        //    result.ExchangeResponse.Prospects.Add(new Random().Next(99999));
        //    result.ExchangeResponse.Time = DateTime.Now;
        //    result.ExchangeResponse.Distance = 5;
        //    result.ExchangeResponse.Guid = Guid.NewGuid().ToString();
        //    result.ExchangeResponse.Demand = true;
        //    result.ExchangeResponse.Exclusive = true;
        //    result.ExchangeResponse.Price = 25;
        //    result.ExchangeResponse.Rating = 4;
        //    result.ExchangeResponse.Elapsed = 5;

        //    new AcquisitionAsyncTracker().Track(result);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// 
        //public static void TestAsyncTrackAllocationEvent()
        //{
        //    AcquisitionEvent response = CreateAllocationEvent();// CreateAllocationEvent();
        //    response.ExchangeResponse.Prospects.Add(new Random().Next(99999));
        //    response.ExchangeResponse.Time = DateTime.Now;
        //    response.ExchangeResponse.Distance = 5;
        //    response.ExchangeResponse.Guid = Guid.NewGuid().ToString();
        //    response.ExchangeResponse.Demand = true;
        //    response.ExchangeResponse.Exclusive = true;
        //    response.ExchangeResponse.Price = 25;
        //    response.ExchangeResponse.Rating = 4;
        //    response.ExchangeResponse.Elapsed = 10;

        //    new AcquisitionAsyncTracker().Track(response);
        //}

        static void Main(string[] args)
        {
            //AcquisitionEventConsumer consumer = ContextRegistry.GetContext().GetObject("AcquisitionEventConsumer") as AcquisitionEventConsumer;
            //consumer.Start();

            //Thread.Sleep(int.MaxValue); 

            //while (true)
            //{
            //    Program.TestAsyncTrackAllocationEvent();
            //    Program.TestAsyncTrackDemandEvent();
            //}
        }
    }
}
