
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
using Mindplex.Commons.DAO;

#endregion

namespace Exchange.Engine
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class ExchangeDataAccess : GenericDataAccess
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        public ExchangeDataAccess()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="connectionString"></param>
        /// 
        public ExchangeDataAccess(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Get's prospects that have expressed interest in the specified zipcode.  
        /// </summary>
        /// 
        /// <param name="zipcode"></param>
        /// 
        /// <returns></returns>
        /// 
        public EntityCollection<Prospect> GetProspects(string zipcode)
        {
        }
    }
}
