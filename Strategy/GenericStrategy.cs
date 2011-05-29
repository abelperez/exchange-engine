
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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

using log4net;
using log4net.Config;

using Mindplex.Commons;

using Exchange.Service;

#endregion

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public abstract class GenericStrategy : ContextAware, IExchangeStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private int retryCount = 1;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string name;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private IExchangeService exchangeService;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public GenericStrategy()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="name"></param>
        /// 
        public GenericStrategy(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                return;
            }
            this.name = name;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// 
        protected string Name
        {
            get 
            {
                if (String.IsNullOrEmpty(name))
                {
                    return GetType().FullName; 
                }
                return name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public int RetryCount
        {
            get { return retryCount; }
            set { retryCount = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public IExchangeService ExchangeService
        {
            get { return exchangeService; }
            set { exchangeService = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="runtime"></param>
        /// 
        public abstract void Invoke(ExchangeRuntime runtime);
    }
}