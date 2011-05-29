
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
using System.Linq;
using System.Text;

using Mindplex.Commons.Messaging;

using Exchange.Engine.Strategy.Events;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class MessageGatewayStrategy : GenericStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private string destination;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public MessageGatewayStrategy()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Destination
        {
            get { return destination; }
            set { destination = value; }
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

            MessageGatewayEvent @event = new MessageGatewayEvent(this.GetType().Name);

            string xml = ExchangeTracker.CreateSoapMessage(runtime);
            MessageGateway<string> gateway = new MessageGateway<string>(Destination);
            gateway.Send(xml);

            @event.Destination = Destination;
            @event.Content = xml;

            timer.Stop();
            @event.ElapsedTime = timer.ElapsedMilliseconds;
            runtime.AddStrategyEvent(@event);
            runtime.Continue();
        }
    }
}
