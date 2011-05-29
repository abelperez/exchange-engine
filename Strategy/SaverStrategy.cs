
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

using Mindplex.Commons;

using Exchange.Engine.Strategy.Events;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class SaverStrategy : GenericStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private string context;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string status;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public SaverStrategy()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Context
        {
            get { return context; }
            set { context = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// TODO: need to figure out how to add current strategy to soap message. 
        /// </summary>
        /// 
        /// <param name="runtime"></param>
        public override void Invoke(ExchangeRuntime runtime)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            SaverEvent @event = new SaverEvent(this.GetType().Name);
            
            string xml = ExchangeTracker.CreateSoapMessage(runtime);
            string filename = Utility.CreateFileName(DateTime.Now, Context, Status, runtime.GetLead().Guid);
            FileInfo file = new FileInfo(filename);
            StreamWriter writer = file.CreateText();
            writer.Write(xml);
            writer.Close();

            @event.Filename = filename;
            @event.Context = Context;
            @event.Status = Status;
            @event.Content = xml;

            timer.Stop();
            @event.ElapsedTime = timer.ElapsedMilliseconds;
            runtime.AddStrategyEvent(@event);
            runtime.Continue();
        }
    }
}
