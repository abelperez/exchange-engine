
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

namespace Exchange.Engine.Strategy.Events
{
    /// <summary>
    ///
    /// </summary>
    /// 
    public abstract class GenericStrategyEvent
    {
        /// <summary>
        ///
        /// </summary>
        ///
        private string guid;

        /// <summary>
        ///
        /// </summary>
        ///
        private string email;

        /// <summary>
        ///
        /// </summary>
        ///
        private DateTime created;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string actor;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string source;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string description;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private long elapsedTime;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public GenericStrategyEvent()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="source"></param>
        /// <param name="description"></param>
        /// 
        public GenericStrategyEvent(string actor, string source, string description)
        {
            this.actor = actor;
            this.source = source;
            this.description = description;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        public abstract string ToXml();

        /// <summary>
        ///
        /// </summary>
        ///
        public string Guid
        {
            get { return guid; }
            set { guid = value; }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        public string Actor
        {
            get { return actor; }
            set { actor = value; }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        public string Source
        {
            get { return source; }
            set { source = value; }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        public DateTime Created
        {
            get { return created; }
            set { created = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public long ElapsedTime
        {
            get { return elapsedTime; }
            set { elapsedTime = value; }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        /// <returns></returns>
        ///
        public override string ToString()
        {
            return new StringBuilder()
                    .AppendFormat("Guid = {0}, ", Guid)
                    .AppendFormat("Email = {0}, ", Email)
                    .AppendFormat("Created = {0}, ", Created)
                    .AppendFormat("ElapsedTime = {0}, ", ElapsedTime).ToString();
        }
    }
}
