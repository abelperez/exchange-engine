
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

using log4net;
using log4net.Config;

using Mindplex.Commons;
using Mindplex.Commons.Model;
using Mindplex.Commons.DAO;
using Mindplex.Commons.Exceptions;
using Mindplex.Commons.Service;
using Mindplex.Commons.Threading;


using Exchange.Debt;
using Exchange.Engine;
using Exchange.Service;
using Exchange.Validation;

using Exchange.Distance;
using Exchange.Duplicate;
using Exchange.Vintage;
using Exchange.Delivery;
using Exchange.Vintage.DataAccess;
using Exchange.Distance.DataAccess;
using Exchange.Duplicate.DataAccess;
using Exchange.Delivery.DataAccess;


//using Exchange.Distribution.Gateway;

using Exchange;
using Exchange.Acquisition;

#endregion

namespace Exchange.Engine
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class ExchangeEngine : ContextAware, IExchangeEngine
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private static ILog logger = LogManager.GetLogger(typeof(ExchangeEngine));

        /// <summary>
        /// 
        /// </summary>
        /// 
        public event EventHandler<ExchangeEvent<IExchangeEngine>> OnExchangeServiceStart;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public event EventHandler<ExchangeEvent<IExchangeEngine>> OnExchangeServiceEnd;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public ExchangeEngine()
        {
            OnExchangeServiceStart += GenericEvent;
            OnExchangeServiceEnd += GenericEvent;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        public ExchangeDisposition Allocate(Lead lead)
        {
            return Execute(lead);
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        public ExchangeDisposition Allocate(DebtLead lead)
        {
            return Execute(lead);
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="lead"></param>
        /// 
        /// <returns></returns>
        /// 
        public ExchangeDisposition Execute(Lead lead)
        {
            OnExchangeServiceStart(this, new ExchangeEvent<IExchangeEngine>(this));

            Stopwatch timer = new Stopwatch();
            timer.Start();

            ExchangeRuntime runtime = CreateRuntime();
            runtime.Active = true;
            runtime.StoreData(ExchangeRuntime.LEAD, lead);
            runtime.Vertical = "DEBT";
            runtime.VerticalType = "DATA";
            runtime.LeadSource = LeadSource.WS;

            try
            {
                runtime.Invoke();
            }
            catch (Exception exception)
            {
                ExceptionHandler<ServiceExceptionFactory>.Process(exception, true, "Failed to save outbound duplicate entity: {0}.", typeof(OutboundDuplicate));

                ExchangeDisposition error = new ExchangeDisposition();
                error.Aid = lead.Aid;
                error.Cid = lead.Cid;
                error.Tid = lead.Tid;
                error.Status = ExchangeRuntime.ERROR;
                error.Time = DateTime.Now;
                error.Guid = lead.Guid;
                error.Amount = 0;
                error.MatchCount = 0;
                return error;
            }

            timer.Stop();

            // Audit Trail
            // runtime.AllocationAuditTrail.TotalElapsedTime = timer.ElapsedMilliseconds;
            runtime.FlagComplete();

            OnExchangeServiceEnd(this, new ExchangeEvent<IExchangeEngine>(this));

            if (runtime.ValidationErrors.Count > 0)
            {
                ExchangeDisposition error = new ExchangeDisposition();
                error.Aid = lead.Aid;
                error.Cid = lead.Cid;
                error.Tid = lead.Tid;
                error.Status = ExchangeRuntime.INVALID;
                error.Time = DateTime.Now;
                error.Guid = lead.Guid;
                error.Amount = 0;
                error.MatchCount = 0;
                error.Errors = runtime.ValidationErrors;

                return error;
            }

            if (runtime.Errors.Count > 0)
            {
                ExchangeDisposition error = new ExchangeDisposition();
                error.Aid = lead.Aid;
                error.Cid = lead.Cid;
                error.Tid = lead.Tid;
                error.Status = ExchangeRuntime.ERROR;
                error.Time = DateTime.Now;
                error.Guid = lead.Guid;
                error.Amount = 0;
                error.MatchCount = 0;
                return error;
            }

            if (runtime.IsInboundDuplicate)
            {
                ExchangeDisposition duplicate = new ExchangeDisposition();
                duplicate.Aid = lead.Aid;
                duplicate.Cid = lead.Cid;
                duplicate.Tid = lead.Tid;
                duplicate.Status = ExchangeRuntime.DUPLICATE;
                duplicate.Time = DateTime.Now;
                duplicate.Guid = lead.Guid;
                duplicate.Amount = 0;
                duplicate.MatchCount = 0;
                return duplicate;
            }

            if (runtime.IsSuspect)
            {
                ExchangeDisposition suspect = new ExchangeDisposition();
                suspect.Aid = lead.Aid;
                suspect.Cid = lead.Cid;
                suspect.Tid = lead.Tid;
                suspect.Status = ExchangeRuntime.PENDING_VERIFICATION;
                suspect.Time = DateTime.Now;
                suspect.Guid = lead.Guid;
                suspect.Amount = 0;
                suspect.MatchCount = 0;
                return suspect;
            }

            if (runtime.PendingMatch)
            {
                ExchangeDisposition pendingMatch = new ExchangeDisposition();
                pendingMatch.Aid = lead.Aid;
                pendingMatch.Cid = lead.Cid;
                pendingMatch.Tid = lead.Tid;
                pendingMatch.Status = ExchangeRuntime.PENDING_MATCH;
                pendingMatch.Time = DateTime.Now;
                pendingMatch.Guid = lead.Guid;
                pendingMatch.Amount = 0;
                pendingMatch.MatchCount = 0;
                return pendingMatch;
            }

            ExchangeDisposition disposition = new ExchangeDisposition();
            disposition.Aid = lead.Aid;
            disposition.Amount = 0.00;
            disposition.Cid = lead.Cid;
            disposition.Tid = lead.Tid;
            disposition.Status = ExchangeRuntime.ACCEPTED;
            disposition.Time = DateTime.Now;
            disposition.Guid = lead.Guid;
            disposition.MatchCount = runtime.AllocationCount;

            return disposition;
        }

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
        public IAsyncResult BeginAllocate(Lead lead, AsyncCallback callback, object asyncState)
        {
            return BeginOperation(lead, callback, asyncState, DoBeginAllocate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="asyncResult"></param>
        /// 
        public ExchangeDisposition EndAllocate(IAsyncResult asyncResult)
        {
            return EndOperation(asyncResult);
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="lead"></param>
        /// <param name="callback"></param>
        /// <param name="asyncState"></param>
        /// <param name="waitCallback"></param>
        /// 
        /// <returns></returns>
        /// 
        private IAsyncResult BeginOperation(Lead lead, AsyncCallback callback, object asyncState, WaitCallback waitCallback)
        {
            AsyncResult<ExchangeDisposition> result = new AsyncResult<ExchangeDisposition>(callback, asyncState);
            AsyncOperation<Lead, AsyncResult<ExchangeDisposition>> operation = new AsyncOperation<Lead, AsyncResult<ExchangeDisposition>>(lead, result);

            ThreadPool.QueueUserWorkItem(waitCallback, operation);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="asyncResult"></param>
        /// 
        /// <returns></returns>
        /// 
        private ExchangeDisposition EndOperation(IAsyncResult asyncResult)
        {
            AsyncResult<ExchangeDisposition> result = asyncResult as AsyncResult<ExchangeDisposition>;
            return result.EndInvoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="asyncOperation"></param>
        /// 
        private void DoBeginAllocate(object asyncOperation)
        {
            DoBeginExecute(asyncOperation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="asyncOperation"></param>
        /// <param name="action"></param>
        /// 
        private void DoBeginExecute(object asyncOperation)
        {
            AsyncOperation<Lead, AsyncResult<ExchangeDisposition>> operation = asyncOperation as AsyncOperation<Lead, AsyncResult<ExchangeDisposition>>;
            ExchangeDisposition disposition = null;

            try
            {
                disposition = Execute(operation.State);
                operation.AsyncResult.Signal(disposition, null, false);
            }
            catch (Exception exception)
            {
                operation.AsyncResult.Signal(disposition, exception, false);
                throw exception;
            }
        }

        /// <summary>
        /// Creates default Exchange runtime.
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        private ExchangeRuntime CreateRuntime()
        {
            return GetObject("ExchangeRuntime") as ExchangeRuntime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="id"></param>
        /// 
        /// <returns></returns>
        /// 
        private ExchangeRuntime CreateRuntime(string id)
        {
            Guard.CheckForNull(id);
            return GetObject(id) as ExchangeRuntime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// 
        private void GenericEvent(object sender, EventArgs args)
        {
        }
    }
}