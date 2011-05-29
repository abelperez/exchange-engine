
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

using log4net;

using Mindplex.Commons;
using Mindplex.Commons.Model;
using Mindplex.Commons.DAO;
using Mindplex.Commons.Exceptions;

using Exchange.Acquisition.Tracking.DataAccess;

#endregion

namespace Exchange.Acquisition.Tracking
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class AcquisitionTracker : IAcquisitionTracker   
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private static ILog logger = LogManager.GetLogger(typeof(AcquisitionTracker));

        /// <summary>
        /// 
        /// </summary>
        /// 
        private AcquisitionTrackerDataAccess acquisitionTrackerDataAccess;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public AcquisitionTracker()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        protected AcquisitionTrackerDataAccess AcquisitionTrackerDataAccess
        {
            get
            {
                return acquisitionTrackerDataAccess;
            }
            set
            {
                acquisitionTrackerDataAccess = value;
            }
        }

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
                //acquisitionTrackerDataAccess.SaveAcquisitionEvent(@event);
            }
            catch (Exception exception)
            {
                ExceptionHandler<DataAccessExceptionFactory>.Process(exception, "Failed to save acquisition event.");
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
                //acquisitionTrackerDataAccess.SaveAcquisitionEvent(@event, DebtAcquisition.SecurityEvent);
            }
            catch (Exception exception)
            {
                ExceptionHandler<DataAccessExceptionFactory>.Process(exception, "Failed to save security acquisition event.");
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
                //acquisitionTrackerDataAccess.SaveAcquisitionEvent(@event, DebtAcquisition.ValidationEvent);
            }
            catch (Exception exception)
            {
                ExceptionHandler<DataAccessExceptionFactory>.Process(exception, "Failed to save validation acquisition event.");
            }
        }
    }
}
