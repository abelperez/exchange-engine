
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
using System.Collections;

using Mindplex.Commons;

#endregion

namespace Exchange.Engine.Monitor
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    internal sealed class ResourceCache
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private static readonly Hashtable resourceCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// 
        public static void Add(string key, string value)
        {
            Guard.CheckIsNullOrEmpty(key, value);
            resourceCache[key] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="key"></param>
        /// 
        /// <returns></returns>
        /// 
        public static string Fetch(string key)
        {
            return resourceCache[key] as string;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static void Flush()
        {
            resourceCache.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="key"></param>
        /// 
        public static bool Contains(string key)
        {
            return resourceCache.Contains(key);
        }
    }
}
