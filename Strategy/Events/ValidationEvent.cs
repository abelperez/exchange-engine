
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

using Exchange.Validation;

#endregion

namespace Exchange.Engine.Strategy.Events
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class ValidationEvent : GenericStrategyEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private bool valid;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private List<Error> errors = new List<Error>();

        /// <summary>
        /// 
        /// </summary>
        /// 
        public ValidationEvent()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="actor"></param>
        /// 
        public ValidationEvent(string actor)
            : base(actor, typeof(ValidationEvent).ToString(), "Validates required lead values are present and correct.") 
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public bool Valid
        {
            get { return valid; }
            set { valid = value; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// 
        public List<Error> Errors
        {
            get { return errors; }
            set { errors = value; }
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
            xml.Append("<validation>");
            xml.AppendFormat("<valid>{0}</valid>", Valid);
            xml.Append("<errors>");

            foreach (Error error in errors)
            {
                xml.AppendFormat("<error><type>{0}</type><description>{1}</description></error>", error.ErrorType, error.ErrorDescription);
            }

            xml.Append("</errors>");
            xml.AppendFormat("<time unit=\"ms\">{0}</time>", ElapsedTime);
            xml.Append("</validation>");
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
                    .AppendFormat("Valid = {0}, ", Valid)
                    .AppendFormat("Errors = {0}, ", Errors).ToString();
        }
    }
}
