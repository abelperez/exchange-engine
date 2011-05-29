

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

using Exchange.Acquisition;

#endregion

namespace Exchange.Acquisition.Test
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [TestFixture]
    public class AcquisitionValidatorTest
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        [Test]
        public void TestValidateZipCode()
        {
            Assert.IsTrue(ExchangeValidator.ValidateZip("93454"));
            Assert.IsFalse(ExchangeValidator.ValidateZip("93454-"));
            Assert.IsFalse(ExchangeValidator.ValidateZip("93454-34"));
            Assert.IsTrue(ExchangeValidator.ValidateZip("93454-4543"));
            Assert.IsFalse(ExchangeValidator.ValidateZip("kanda"));
            Assert.IsFalse(ExchangeValidator.ValidateZip(""));
            Assert.IsFalse(ExchangeValidator.ValidateZip(null));
            Assert.IsFalse(ExchangeValidator.ValidateZip("9"));
            Assert.IsFalse(ExchangeValidator.ValidateZip("93"));
            Assert.IsFalse(ExchangeValidator.ValidateZip("934"));
            Assert.IsFalse(ExchangeValidator.ValidateZip("9345"));
            Assert.IsTrue(ExchangeValidator.ValidateZip("93454"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        [Test]
        public void TestValidateEmailAddress()
        {
            Assert.IsTrue(ExchangeValidator.ValidateEmail("aminbandeali@sbcglobal.net"));
            Assert.IsFalse(ExchangeValidator.ValidateEmail("@sbcgobal.net"));
            Assert.IsFalse(ExchangeValidator.ValidateEmail("amibandeali@sbcglobal.net"));
            Assert.IsFalse(ExchangeValidator.ValidateEmail(""));
            Assert.IsFalse(ExchangeValidator.ValidateEmail(null));
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        [Test]
        public void TestValidateAid()
        {
            Assert.IsTrue(ExchangeValidator.ValidateAid("1000"));
            Assert.IsFalse(ExchangeValidator.ValidateAid("2"));
            Assert.IsFalse(ExchangeValidator.ValidateAid("2232323"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        [Test]
        public void TestValidateCid()
        {
            Assert.IsTrue(ExchangeValidator.ValidateCid("1000", "202"));
            Assert.IsTrue(ExchangeValidator.ValidateCid("1002", "216"));
            Assert.IsFalse(ExchangeValidator.ValidateCid("1000", "2232323"));
            Assert.IsFalse(ExchangeValidator.ValidateCid(null, null));
            Assert.IsFalse(ExchangeValidator.ValidateCid(string.Empty, string.Empty));
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        [Test]
        public void TestValidateState()
        {
            Assert.IsFalse(ExchangeValidator.ValidateState("1000"));
            Assert.IsTrue(ExchangeValidator.ValidateState("CA"));
            Assert.IsTrue(ExchangeValidator.ValidateState("NY"));
            Assert.IsFalse(ExchangeValidator.ValidateState("ca"));
            Assert.IsFalse(ExchangeValidator.ValidateState("CAs"));
            Assert.IsFalse(ExchangeValidator.ValidateState(null));
            Assert.IsFalse(ExchangeValidator.ValidateState(string.Empty));
        }
    }
}

#endif