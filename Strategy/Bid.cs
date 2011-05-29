
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
using System.Linq;
using System.Text;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [Serializable]
    public class Bid
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private long aid;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private long oid;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public Bid()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="aid"></param>
        /// <param name="oid"></param>
        /// 
        public Bid(long aid, long oid)
        {
            this.aid = aid;
            this.oid = oid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public long Aid
        {
            get { return aid; }
            set { aid = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public long Oid
        {
            get { return oid; }
            set { oid = value; }
        }
    }
}
