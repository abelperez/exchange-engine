
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
using System.IO;
using System.Reflection;

using log4net;

using Mindplex.Commons;

#endregion

namespace Exchange.Engine.Monitor
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class Resource
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private static ILog logger = LogManager.GetLogger(typeof(Resource));

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="name"></param>
        /// 
        /// <returns></returns>
        /// 
        public static string GetResource(string name)
        {
            Guard.CheckIsNullOrEmpty(name, "name");

            if (ResourceCache.Contains(name))
            {
                return ResourceCache.Fetch(name);
            }

            string resource = DoGetResource(name);
            ResourceCache.Add(name, resource);

            return resource;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        private static string DoGetResource(string name)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
            string template = new StreamReader(stream).ReadToEnd();

            if (logger.IsDebugEnabled)
            {
                logger.DebugFormat("Loaded template: {0}.", template);
            }

            return template;
        }
    }
}
