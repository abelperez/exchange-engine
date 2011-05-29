
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
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

using log4net;

using Mindplex.Commons;
using Mindplex.Commons.Model;
using Mindplex.Commons.DAO;
using Mindplex.Commons.Exceptions;

using Exchange.Acquisition.Tracking;

#endregion

namespace Exchange.Acquisition.Tracking.DataAccess
{
    /// <summary>
    ///
    /// </summary>
    ///
    public partial class AcquisitionTrackerDataAccess : GenericDataAccess
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private static ILog logger = LogManager.GetLogger(typeof(AcquisitionTrackerDataAccess));

        /// <summary>
        ///
        /// </summary>
        ///
        //private static RowMapper<DebtAcquisition> rowMapper;

        ///// <summary>
        /////
        ///// </summary>
        /////
        //private static RowMapper<DebtAcquisition> securityRowMapper;

        ///// <summary>
        /////
        ///// </summary>
        /////
        //private static RowMapper<DebtAcquisition> validationRowMapper;

        /// <summary>
        ///
        /// </summary>
        ///
        public AcquisitionTrackerDataAccess()
        {
            //rowMapper = AcquisitionEntityMapper.GetAcquisitionEntity;
            //securityRowMapper = AcquisitionEntityMapper.GetSecurityAcquisitionEntity;
            //validationRowMapper = AcquisitionEntityMapper.GetValidationAcquisitionEntity;
        }

        ///// <summary>
        /////
        ///// </summary>
        /////
        //public AcquisitionTrackerDataAccess(string databaseInstance)
        //    : base(databaseInstance)
        //{
        //    rowMapper = AcquisitionEntityMapper.GetAcquisitionEntity;
        //}

        ///// <summary>
        /////
        ///// </summary>
        /////
        ///// <param name="entity"></param>
        /////
        //public DebtAcquisition SaveAcquisitionEvent(ExchangeAcquisitionEvent @event)
        //{
        //    if (Guard.IsNull(@event) && Guard.IsNull(@event.ExchangeResponse))
        //    {
        //        ExceptionHandler<DataAccessExceptionFactory>.Process(new DataAccessException(), true, true, "specified acquisition event is null.");
        //    }

        //    DebtAcquisition result = null;

        //    try
        //    {
        //        DebtAcquisition entity = @event.ExchangeResponse;

        //        SqlParameter[] sqlParams = {
        //                new SqlParameter("@Guid", entity.Guid),
        //                new SqlParameter("@Username", entity.Username),
        //                new SqlParameter("@Password", entity.Password),
        //                new SqlParameter("@Aid", entity.Aid),
        //                new SqlParameter("@Cid", entity.Cid),
        //                new SqlParameter("@Tid", entity.Tid),
        //                new SqlParameter("@Sid", entity.Sid),
        //                new SqlParameter("@FirstName", entity.FirstName),
        //                new SqlParameter("@LastName", entity.LastName),
        //                new SqlParameter("@Email", entity.Email),
        //                new SqlParameter("@Phone", entity.Phone),
        //                new SqlParameter("@SecondaryPhone", entity.SecondaryPhone),
        //                new SqlParameter("@Street", entity.Street),
        //                new SqlParameter("@City", entity.City),
        //                new SqlParameter("@Zip", entity.Zip),
        //                new SqlParameter("@State", entity.State),
                        
        //                // new SqlParameter("@Distance", entity.Distance),
        //                // new SqlParameter("@Exclusive", entity.Exclusive),
        //                // new SqlParameter("@Demand", entity.Demand),
        //                // new SqlParameter("@Prospects", entity.Prospects.ToString()),
                        
        //                new SqlParameter("@Price", entity.Price),
        //                // new SqlParameter("@Rating", entity.Rating),
        //                new SqlParameter("@Time", entity.Time),
                        
        //                new SqlParameter("@Elapsed", entity.Elapsed),
        //                // new SqlParameter("@Variant", entity.Variant),
        //        };

        //        result = Save<DebtAcquisition>("SaveExchangeAcquisitionEvent", sqlParams, rowMapper);
        //    }
        //    catch (Exception exception)
        //    {
        //        if (logger.IsErrorEnabled)
        //        {
        //            logger.Error("Failed to save acquisition event.", exception);
        //        }

        //        ExceptionHandler<DataAccessExceptionFactory>.Process(exception, "Failed to save acquisition event.");
        //    }

        //    return result;
        //}

        ///// <summary>
        /////
        ///// </summary>
        /////
        ///// <param name="entity"></param>
        /////
        //public DebtAcquisition SaveAcquisitionEvent(AcquisitionEvent @event, string eventType)
        //{
        //    if (Guard.IsNull(@event) && Guard.IsNull(@event.ExchangeResponse))
        //    {
        //        ExceptionHandler<DataAccessExceptionFactory>.Process(new DataAccessException(), true, true, "specified acquisition event is null.");
        //    }

        //    DebtAcquisition result = null;

        //    try
        //    {
        //        DebtAcquisition entity = @event.ExchangeResponse;

        //        if (eventType.Equals(DebtAcquisition.SecurityEvent))
        //        {
        //            SqlParameter[] sqlParams = {
        //                new SqlParameter("@Username", entity.Username),
        //                new SqlParameter("@Password", entity.Password),
        //                new SqlParameter("@Aid", entity.Aid),
        //                new SqlParameter("@Cid", entity.Cid),
        //                new SqlParameter("@Time", entity.Time),
        //                new SqlParameter("@Guid", entity.Guid),
        //                new SqlParameter("@Errors", entity.FormattedErrors),
        //                new SqlParameter("@Variant", entity.Variant),
        //        };

        //            result = Save<DebtAcquisition>("SaveSecurityAcquisitionEvent", sqlParams, securityRowMapper);
        //        }
        //        else if (eventType.Equals(DebtAcquisition.ValidationEvent))
        //        {
        //            SqlParameter[] sqlParams = {
        //                new SqlParameter("@Aid", entity.Aid),
        //                new SqlParameter("@Cid", entity.Cid),
        //                new SqlParameter("@Time", entity.Time),
        //                new SqlParameter("@Guid", entity.Guid),
        //                new SqlParameter("@Errors", entity.FormattedErrors),
        //                new SqlParameter("@Variant", entity.Variant),
        //        };

        //            result = Save<DebtAcquisition>("SaveValidationAcquisitionEvent", sqlParams, validationRowMapper);
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        if (logger.IsErrorEnabled)
        //        {
        //            logger.Error("Failed to save acquisition event.", exception);
        //        }

        //        ExceptionHandler<DataAccessExceptionFactory>.Process(exception, "Failed to save acquisition event.");
        //    }

        //    return result;
        //}
    }
}