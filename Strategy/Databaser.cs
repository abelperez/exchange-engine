
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
using System.Linq;
using System.Text;

using Mindplex.Commons.Model;
using Mindplex.Commons.DAO;
using Mindplex.Commons.Exceptions;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class Databaser : GenericDataAccess
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        public Databaser()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="database"></param>
        /// 
        public Databaser(string database)
            : base(database)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// 
        /// <param name="procedure"></param>
        /// <param name="parameters"></param>
        /// <param name="rowMapper"></param>
        /// 
        /// <returns></returns>
        /// 
        public EntityCollection<T> FetchAll<T>(string procedure, SqlParameter[] parameters, RowMapper<T> rowMapper)
        {
            EntityCollection<T> result = default(EntityCollection<T>);

            try
            {
                result = FindAll<T>(procedure, parameters, rowMapper);
            }
            catch (Exception exception)
            {
                ExceptionHandler<DataAccessExceptionFactory>.Process(exception, "Failed to execute procedure: {0}.", typeof(T).Name);
            }

            return result;
        }
    }
}
