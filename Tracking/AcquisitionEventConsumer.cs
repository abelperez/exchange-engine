
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

using log4net;

using Mindplex.Commons.Messaging;

#endregion

namespace Exchange.Acquisition.Tracking
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class AcquisitionEventConsumer
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private static ILog logger = LogManager.GetLogger(typeof(AcquisitionEventConsumer));

        /// <summary>
        /// 
        /// </summary>
        /// 
        private MessageGateway<ExchangeAcquisitionEvent> acquisitionConsumer = new MessageGateway<ExchangeAcquisitionEvent>(TrackingChannel.AcquisitionEventDestination);

        /// <summary>
        /// 
        /// </summary>
        /// 
        private MessageGateway<ExchangeAcquisitionEvent> securityAcquisitionConsumer = new MessageGateway<ExchangeAcquisitionEvent>(TrackingChannel.SecurityAcquisitionEventDestination);

        /// <summary>
        /// 
        /// </summary>
        /// 
        private MessageGateway<ExchangeAcquisitionEvent> validationAcquisitionConsumer = new MessageGateway<ExchangeAcquisitionEvent>(TrackingChannel.ValidationAcquisitionEventDestination);

        /// <summary>
        /// 
        /// </summary>
        /// 
        private IAcquisitionTracker tracker;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public AcquisitionEventConsumer()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public IAcquisitionTracker Tracker
        {
            get { return tracker; }
            set { tracker = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public void Start()
        {
            acquisitionConsumer.OnMessageReceived += new OnMessageEvent<ExchangeAcquisitionEvent>(OnAcquisitionEventReceived);
            securityAcquisitionConsumer.OnMessageReceived += new OnMessageEvent<ExchangeAcquisitionEvent>(OnSecurityAcquisitionEventReceived);
            validationAcquisitionConsumer.OnMessageReceived += new OnMessageEvent<ExchangeAcquisitionEvent>(OnValidationAcquisitionEventReceived);
            
            acquisitionConsumer.StartAsync();
            securityAcquisitionConsumer.StartAsync();
            validationAcquisitionConsumer.StartAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public void Stop()
        {
            acquisitionConsumer.StopAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="event"></param>
        /// 
        public void OnAcquisitionEventReceived(ExchangeAcquisitionEvent @event)
        {
            Console.WriteLine(@event);
            try
            {
                tracker.Track(@event);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error: " + exception.Message);
                logger.Warn("Error OnAcquisitionEventReceived", exception);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="message"></param>
        /// 
        void OnValidationAcquisitionEventReceived(ExchangeAcquisitionEvent @event)
        {
            Console.WriteLine(@event);
            try
            {
                logger.DebugFormat("Validation Event Received: {0}", @event.ExchangeResponse.ToString());

                tracker.TrackValidation(@event);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error: " + exception.Message);
                logger.Warn("Error OnValidationAcquisitionEventReceived", exception);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="message"></param>
        /// 
        void OnSecurityAcquisitionEventReceived(ExchangeAcquisitionEvent @event)
        {
            Console.WriteLine(@event);
            try
            {
                logger.DebugFormat("Security Event Received: {0}", @event.ExchangeResponse.ToString());

                tracker.TrackSecurity(@event);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error: " + exception.Message);
                logger.Warn("Error OnSecurityAcquisitionEventReceived", exception);
            }
        }
    }
}
