
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

using NUnit.Framework;

using Mindplex.Commons.Test;
using Mindplex.Commons;

using Exchange.Engine;
using Exchange.Debt;

#endregion

namespace Exchange.Acquisition.Test
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [TestFixture]
    public class AcquisitionTest : GenericTest
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        [Test]
        public void TestSpringConfiguration()
        {
            IExchangeEngine engine = GetApplicationContext().GetObject("ExchangeEngine") as IExchangeEngine;
        }

        [Test]
        public void TestAllocate()
        {
            IExchangeEngine engine = GetApplicationContext().GetObject("ExchangeEngine") as IExchangeEngine;
            engine.Allocate(CreateValidDebtLead());
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        public DebtLead CreateValidDebtLead()
        {
            DebtLead lead = new DebtLead();
            lead.Guid = GuidGenerator.Generate();
            lead.Aid = "1000";
            lead.Cid = "202";
            lead.Sid = "voice";
            lead.Tid = "100";
            lead.FirstName = "Test Firstname";
            lead.LastName = "Test Lastname";
            lead.Email = "testdebtlead@yahoo.com";
            lead.Phone = "3105402121";
            lead.SecondaryPhone = "3105405050";
            lead.Street = "123 Main St.";
            lead.City = "Los Angeles";
            lead.Zip = "90042";
            lead.State = "CA";
            lead.Created = DateTime.Now;
            lead.CreditCardDebtAmount = "30000";
            lead.UnsecuredDebtAmount = "50000";
            lead.PaymentStatus = "30+";
            lead.MonthlyPayments = "400";
            return lead;
        }
    }
}

#endif
