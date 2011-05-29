
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

using Exchange.Debt;
using Exchange.Engine;

#endregion

namespace Exchange.Acquisition
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public static class ExchangeTranslator
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="source"></param>
        /// 
        /// <returns></returns>
        /// 
        public static DebtLead Assign(DebtLeadType source)
        {
            DebtLead result = new DebtLead();

            result.Id = source.Id;
            result.Guid = source.Guid;
            result.Aid = source.Aid;
            result.Cid = source.Cid;
            result.Tid = source.Tid;
            result.Sid = source.Sid;

            result.FirstName = source.FirstName;
            result.LastName = source.LastName;
            result.Email = source.Email;
            result.Phone = source.Phone;
            result.SecondaryPhone = source.SecondaryPhone;

            result.Street = source.Street;
            result.City = source.City;
            result.Zip = source.Zip;
            result.State = source.State;
            result.Created = source.Created;

            result.CreditCardDebtAmount = source.CreditCardDebtAmount;
            result.UnsecuredDebtAmount = source.UnsecuredDebtAmount;
            result.PaymentStatus = source.PaymentStatus;
            result.MonthlyPayments = source.MonthlyPayments;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="source"></param>
        /// 
        /// <returns></returns>
        /// 
        public static ExchangeResponse Assign(ExchangeDisposition source)
        {
            ExchangeResponse result = new ExchangeResponse();

            result.Status = source.Status;

            result.Guid = source.Guid;
            result.Aid = source.Aid;
            result.Cid = source.Cid;
            result.Tid = source.Tid;

            result.MatchCount = source.MatchCount;
            result.Amount = source.Amount;
            result.Errors = source.Errors.ToArray();

            return result;
        }
    }
}
