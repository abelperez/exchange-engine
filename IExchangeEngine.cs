
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

using Mindplex.Commons.Model;

using Exchange;
using Exchange.Debt;
using Exchange.Distance;
using Exchange.Duplicate;
using Exchange.Vintage;
using Exchange.Delivery;
using Exchange.Delivery.DataAccess;

#endregion

namespace Exchange.Engine
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public interface IExchangeEngine
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        event EventHandler<ExchangeEvent<IExchangeEngine>> OnExchangeServiceStart;

        /// <summary>
        /// 
        /// </summary>
        /// 
        event EventHandler<ExchangeEvent<IExchangeEngine>> OnExchangeServiceEnd;

        #region Allocation

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        ExchangeDisposition Allocate(Lead lead);

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="lead"></param>
        /// 
        /// <returns></returns>
        /// 
        ExchangeDisposition Allocate(DebtLead lead);

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="lead"></param>
        /// <param name="callback"></param>
        /// <param name="asyncState"></param>
        /// 
        /// <returns></returns>
        /// 
        IAsyncResult BeginAllocate(Lead lead, AsyncCallback callback, object asyncState);

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="asyncResult"></param>
        /// 
        /// <returns></returns>
        /// 
        ExchangeDisposition EndAllocate(IAsyncResult asyncResult);

        #endregion
    }
}