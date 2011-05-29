
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
using System.IO;
using System.Text;
using System.Reflection;

using NUnit.Framework;

using Mindplex.Commons.Model;
using Mindplex.Commons.Test;

using Exchange.Service;
using Exchange.Distance;
using Exchange.Duplicate;
using Exchange.Duplicate.DataAccess;
using Exchange.Vintage;
using Exchange.Engine.Monitor;
using Exchange.Acquisition;

#endregion

namespace Exchange.Engine.Test
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [TestFixture]
    public class AllocationServiceTest : GenericExchangeEngineTest
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        [Test]
        public void TestExchangeAllocation()
        {
            ExchangeDisposition disposition = ExchangeEngine.Allocate(CreateValidDebtLead());
            Console.WriteLine(disposition);
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        [Test]
        public void TestAllocation()
        {
            IExchangeEngine service = GetApplicationContext().GetObject("ExchangeEngine") as IExchangeEngine;
            ExchangeDisposition allocation = service.Allocate(CreateValidDebtLead());
            Console.WriteLine(allocation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        [Test]
        public void TestAdapterAllocation()
        {
            ExchangeAdapter adapter = GetApplicationContext().GetObject("ExchangeAdapter") as ExchangeAdapter;
            ExchangeResponse allocation = adapter.Post(CreateValidDebtLeadType(), LeadSource.WS);
            Console.WriteLine(allocation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        [Test]
        public void TestInvalidAdapterAllocation()
        {
            ExchangeAdapter adapter = GetApplicationContext().GetObject("ExchangeAdapter") as ExchangeAdapter;
            ExchangeResponse allocation = adapter.Post(CreateInvalidDebtLeadType(), LeadSource.WS);
            Console.WriteLine(allocation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        [Test]
        public void TestSystemAdapterAllocation()
        {
            ExchangeEngine adapter = GetApplicationContext().GetObject("SystemExchangeEngine") as ExchangeEngine;
            ExchangeDisposition allocation = adapter.Allocate(CreateValidDebtLead());
            Console.WriteLine(allocation);
        }

        [Test]
        public void TestResource()
        {
            
            ////string[] res = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            //Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("Exchange.Engine.EmailTemplate.txt");
            //Assert.IsNotNull(s);
            ////foreach (string s in res)
            ////{
            ////    Console.WriteLine(s);
            ////}

            Resource.GetResource("Exchange.Engine.EmailTemplate.txt");
        }
    }
}

#endif
