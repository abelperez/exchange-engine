
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

using Mindplex.Commons;
using Mindplex.Commons.DAO;
using Mindplex.Commons.Exceptions;
using Mindplex.Commons.Messaging;
using Mindplex.Commons.Model;
using Mindplex.Commons.Service;

using Exchange.Acquisition.Tracking.DataAccess;

using log4net;

#endregion

namespace Exchange.Acquisition.Tracking
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class AcquisitionAsyncTracker : IAcquisitionTracker
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private ILog logger = LogManager.GetLogger(typeof(AcquisitionAsyncTracker));

        /// <summary>
        /// 
        /// </summary>
        /// 
        private MessageGateway<ExchangeAcquisitionEvent> gateway = new MessageGateway<ExchangeAcquisitionEvent>(TrackingChannel.AcquisitionEventDestination);

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="event"></param>
        /// 
        public void Track(ExchangeAcquisitionEvent @event)
        {
            try
            {
                gateway.Send(@event);
            }
            catch (MessageException exception)
            {
                ExceptionHandler<ServiceExceptionFactory>.Process(exception, false, "Failed to track acquisition event.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="event"></param>
        /// 
        public void TrackSecurity(ExchangeAcquisitionEvent @event)
        {
            try
            {
                gateway.Send(@event, TrackingChannel.SecurityAcquisitionEventDestination);
            }
            catch (MessageException exception)
            {
                ExceptionHandler<ServiceExceptionFactory>.Process(exception, false, "Failed to track security acquisition event.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="event"></param>
        /// 
        public void TrackValidation(ExchangeAcquisitionEvent @event)
        {
            try
            {
                gateway.Send(@event, TrackingChannel.ValidationAcquisitionEventDestination);
            }
            catch (MessageException exception)
            {
                ExceptionHandler<ServiceExceptionFactory>.Process(exception, false, "Failed to track validation acquisition event.");
            }
        }
    }
}
