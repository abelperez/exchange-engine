
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
    public class AggregatedContribution
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <typeparam name="T"></typeparam>
        /// 
        /// <param name="cost"></param>
        /// <param name="click"></param>
        /// 
        /// <returns></returns>
        /// 
        public delegate bool Func<T>(T cost, T click);

        /// <summary>
        /// 
        /// </summary>
        /// 
        private int cost;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private int click;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public AggregatedContribution()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public int Cost
        {
            get { return cost; }
            set { cost = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public int Click
        {
            get { return click; }
            set { click = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="expression"></param>
        /// 
        /// <returns></returns>
        /// 
        public bool Compare(Func<int> expression)
        {
            return expression.Invoke(cost, click);
        }
    }
}
