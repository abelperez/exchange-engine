
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
    [Serializable]
    public class ImpoundEvent : GenericStrategyEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private long aid;

        /// <summary>
	    ///
	    /// </summary>
	    ///
        private bool impounded;

        /// <summary>
        ///
        /// </summary>
        ///
        public ImpoundEvent(string actor)
            : base(actor, typeof(ImpoundEvent).ToString(), "Impound strategy impounds leads that come from suspect publishers.")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public long Aid
        {
            get { return aid; }
            set { aid = value; }
        }

        /// <summary>
	    ///
	    /// </summary>
	    ///
        public bool Impounded
        {
            get { return impounded; }
            set { impounded = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        public override string ToXml()
        {
            StringBuilder xml = new StringBuilder();
            xml.AppendFormat("<impound email=\"{0}\" created=\"{1}\" guid=\"{2}\" ><suspect>", Email, Created, Guid);
            xml.AppendFormat("<aid>{0}</aid>", Aid);
            xml.AppendFormat("<impounded>{0}</impounded>", Impounded);
            xml.Append("</suspect>");
            xml.AppendFormat("<time unit=\"ms\">{0}</time>", ElapsedTime);
            xml.Append("</impound>");
            return xml.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        ///
        /// <returns></returns>
        ///
        public override string ToString()
        {
            return new StringBuilder(base.ToString())
                    .AppendFormat("Impounded = {0}, ", Impounded).ToString();
        }
    }
}
