
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
    public class NotificationStrategy : GenericStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private string dataKey;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string host;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string from;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string recipient;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string subject;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public NotificationStrategy()
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
        public string Host
        {
            get { return host; }
            set { host = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string From
        {
            get { return from; }
            set { from = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Recipient
        {
            get { return recipient; }
            set { recipient = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="runtime"></param>
        /// 
        public override void Invoke(ExchangeRuntime runtime)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            NotificationEvent @event = new NotificationEvent(this.GetType().Name);
            @event.From = From;
            @event.To = Recipient;
            @event.Subject = Subject;
            
            string body = runtime.GetData(DataKey) as string;
            EmailGateway.SendHtml(Host, From, Recipient, Subject, body);

            timer.Stop();
            @event.ElapsedTime = timer.ElapsedMilliseconds;
            runtime.AddStrategyEvent(@event);
            runtime.Continue();
        }
    }
}
