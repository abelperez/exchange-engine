
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

#if UNIT_TEST

#region Imports

using System;
using System.Collections.Generic;
using System.Text;

using Exchange.Acquisition.Tracking;

using Mindplex.Commons;

using NUnit.Framework;

using Exchange;
using Exchange.Acquisition.Debt;

#endregion

namespace Exchange.Acquisition.Tracking.Test
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [TestFixture]
    public class AcquisitionServiceTrackerTest
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        protected static ExchangeAcquisitionEvent CreateAllocationEvent()
        {
            DebtAcquisition allocation = new DebtAcquisition();
            allocation.Aid = new Random().Next(99999).ToString();
            allocation.Cid = new Random().Next(999).ToString();
            allocation.City = "Seattle";
            allocation.Email = "test@alx.com";
            allocation.FirstName = "John";
            allocation.LastName = "Cusak";
            allocation.Password = "abcdef";
            allocation.Phone = "2323234543";
            allocation.State = "WA";
            allocation.Street = "234 WanderLand Ave.";
            allocation.Username = "affiliate1";
            allocation.Zip = "98102";
            allocation.Variant = DebtAcquisition.AllocationEvent;

            return new AcquisitionEvent(allocation);
        }

        protected static ExchangeAcquisitionEvent CreateLeadTypeEvent()
        {
            LeadType lead = new LeadType();
            lead.Guid = GuidGenerator.Generate();
            lead.Aid = new Random().Next(99999).ToString();
            lead.Cid = new Random().Next(999).ToString();
            lead.City = "Seattle";
            lead.Email = "test@alx.com";
            lead.FirstName = "John";
            lead.LastName = "Cusak";
            lead.Password = "abcdef";
            lead.Phone = "2323234543";
            lead.State = "WA";
            lead.Street = "234 WanderLand Ave.";
            lead.Username = "affiliate1";
            lead.Zip = "98102";

            List<String> errors = new List<string>();
            errors.Add("Test Error 1");
            errors.Add("Test Error 2");

            return new ExchangeAcquisitionEvent(lead, DebtAcquisition.AllocationEvent, errors);
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        [Test]
        public void TestAsyncTrackAllocationEvent()
        {
            ExchangeAcquisitionEvent response = CreateAllocationEvent();// CreateAllocationEvent();
            //response.ExchangeResponse.Prospects .Add(new Random().Next(99999));
            //response.ExchangeResponse.Time = DateTime.Now;
            //response.ExchangeResponse.Distance = 5;
            //response.ExchangeResponse.Guid = Guid.NewGuid().ToString();
            //response.ExchangeResponse.Demand = true;
            //response.ExchangeResponse.Exclusive = true;
            //response.ExchangeResponse.Price = 25;
            //response.ExchangeResponse.Rating = 4;
            //response.ExchangeResponse.Elapsed = 10;

            new AcquisitionAsyncTracker().Track(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        [Test]
        public void TestAsyncTrackValidationEvent()
        {
            ExchangeAcquisitionEvent response = CreateLeadTypeEvent();
            //response.ExchangeResponse.Prospects.Add(new Random().Next(99999));
            //response.ExchangeResponse.Time = DateTime.Now;
            //response.ExchangeResponse.Distance = 5;
            //response.ExchangeResponse.Guid = GuidGenerator.Generate();
            //response.ExchangeResponse.Demand = true;
            //response.ExchangeResponse.Exclusive = true;
            //response.ExchangeResponse.Price = 25;
            //response.ExchangeResponse.Rating = 4;
            //response.ExchangeResponse.Elapsed = 10;

            //new AcquisitionAsyncTracker().TrackValidation(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        [Test]
        public void TestAsyncTrackSecurityEvent()
        {
            ExchangeAcquisitionEvent response = CreateLeadTypeEvent();
            //response.ExchangeResponse.Prospects.Add(new Random().Next(99999));
            //response.ExchangeResponse.Time = DateTime.Now;
            //response.ExchangeResponse.Distance = 5;
            //response.ExchangeResponse.Guid = GuidGenerator.Generate();
            //response.ExchangeResponse.Demand = true;
            //response.ExchangeResponse.Exclusive = true;
            //response.ExchangeResponse.Price = 25;
            //response.ExchangeResponse.Rating = 4;
            //response.ExchangeResponse.Elapsed = 10;

            new AcquisitionAsyncTracker().TrackSecurity(response);
        }
    }
}

#endif