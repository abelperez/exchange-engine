
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
using System.Data.Sql;
using System.Text;

#endregion

namespace Exchange.Acquisition.Tracking.DataAccess
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public static class AcquisitionEntityMapper
    {
        /// <summary>
        ///
        /// </summary>
        ///
        /// <param name="reader"></param>
        ///
        /// <returns></returns>
        ///
        //public static DebtAcquisition GetAcquisitionEntity(IDataReader reader)
        //{
        //    DebtAcquisition entity = new DebtAcquisition();
        //    entity.Id = Convert.ToInt64(reader["Id"]);
        //    entity.Username = Convert.ToString(reader["Username"]);
        //    entity.Password = Convert.ToString(reader["Password"]);
        //    entity.Aid = Convert.ToString(reader["Aid"]);
        //    entity.Cid = Convert.ToString(reader["Cid"]);
        //    entity.FirstName = Convert.ToString(reader["FirstName"]);
        //    entity.LastName = Convert.ToString(reader["LastName"]);
        //    entity.Email = Convert.ToString(reader["Email"]);
        //    entity.Street = Convert.ToString(reader["Street"]);
        //    entity.City = Convert.ToString(reader["City"]);
        //    entity.State = Convert.ToString(reader["State"]);
        //    entity.Zip = Convert.ToString(reader["Zip"]);
        //    entity.Phone = Convert.ToString(reader["Phone"]);
        //    entity.Distance = Convert.ToDouble(reader["Distance"]);
        //    entity.Exclusive = Convert.ToBoolean(reader["Exclusive"]);
        //    entity.Demand = Convert.ToBoolean(reader["Demand"]);
        //    //entity.Prospects = Convert.ToString(reader["Prospects"]);
        //    entity.Price = Convert.ToInt64(reader["Price"]);
        //    entity.Rating = Convert.ToInt64(reader["Rating"]);
        //    entity.Time = Convert.ToDateTime(reader["Time"]);
        //    entity.Guid = Convert.ToString(reader["Guid"]);
        //    entity.Elapsed = Convert.ToInt64(reader["Elapsed"]);
        //    entity.Variant = Convert.ToString(reader["Variant"]);
        //    return entity;
        //}

        /// <summary>
        ///
        /// </summary>
        ///
        /// <param name="reader"></param>
        ///
        /// <returns></returns>
        ///
        //public static DebtAcquisition GetValidationAcquisitionEntity(IDataReader reader)
        //{
        //    DebtAcquisition entity = new DebtAcquisition();
        //    entity.Id = Convert.ToInt64(reader["Id"]);
        //    entity.Aid = Convert.ToString(reader["Aid"]);
        //    entity.Cid = Convert.ToString(reader["Cid"]);
        //    entity.Guid = Convert.ToString(reader["Guid"]);
        //    entity.Variant = Convert.ToString(reader["Variant"]);
        //    //entity.Variant = Convert.ToString(reader["Error"]);
        //    return entity;
        //}

        /// <summary>
        ///
        /// </summary>
        ///
        /// <param name="reader"></param>
        ///
        /// <returns></returns>
        ///
        //public static DebtAcquisition GetSecurityAcquisitionEntity(IDataReader reader)
        //{
        //    DebtAcquisition entity = new DebtAcquisition();
        //    entity.Id = Convert.ToInt64(reader["Id"]);
        //    entity.Username = Convert.ToString(reader["Username"]);
        //    entity.Password = Convert.ToString(reader["Password"]);
        //    entity.Aid = Convert.ToString(reader["Aid"]);
        //    entity.Cid = Convert.ToString(reader["Cid"]);
        //    entity.Guid = Convert.ToString(reader["Guid"]);
        //    entity.Variant = Convert.ToString(reader["Variant"]);
        //    //entity.Variant = Convert.ToString(reader["Error"]);
        //    return entity;
        //}
    }
}
