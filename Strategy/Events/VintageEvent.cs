
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
    public class VintageEvent : GenericStrategyEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private bool vintage;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private int buyRate;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public VintageEvent()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="actor"></param>
        /// 
        public VintageEvent(string actor)
            : base(actor, typeof(VintageEvent).ToString(), "Vintage strategy adds leads that dont get allocated the max buy rate into the vintage market place.")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public bool Vintage
        {
            get { return vintage; }
            set { vintage = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public int BuyRate
        {
            get { return buyRate; }
            set { buyRate = value; }
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
            xml.AppendFormat("<vintage email=\"{0}\" created=\"{1}\" guid=\"{2}\" >", Email, Created, Guid);
            xml.AppendFormat("<isvintage>{0}</isvintage>", Vintage);
            xml.AppendFormat("<buyrate>{0}</buyrate>", BuyRate);
            xml.AppendFormat("<time unit=\"ms\">{0}</time>", ElapsedTime);
            xml.Append("</vintage>");
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
                    .AppendFormat("Vintage = {0}, ", Vintage)
                    .AppendFormat("BuyRate = {0}, ", BuyRate).ToString();
        }
    }
}
