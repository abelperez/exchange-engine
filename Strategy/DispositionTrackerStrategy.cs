
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using log4net;

using Mindplex.Commons;
using Mindplex.Commons.Mail;

using Exchange.Engine.Strategy.Events;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class DispositionTrackerStrategy : GenericStrategy, IExchangeStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private static ILog logger = LogManager.GetLogger(typeof(DispositionTrackerStrategy));

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string dataKey;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public DispositionTrackerStrategy()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="name"></param>
        /// 
        public DispositionTrackerStrategy(string name) 
            : base(name)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string DataKey
        {
            get { return dataKey; }
            set { dataKey = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="context"></param>
        /// 
        public override void Invoke(ExchangeRuntime runtime)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            DispositionTrackerEvent @event = new DispositionTrackerEvent();

            string body = runtime.GetData(DataKey) as string;
            ExchangeTracker.TrackComplete(string.Format("{0}-{1}", runtime.Vertical, runtime.VerticalType), runtime.Status, runtime, body);

            timer.Stop();
            @event.ElapsedTime = timer.ElapsedMilliseconds;
            runtime.Continue();
        }
    }
}
