
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
using System.Xml.Serialization;

using Exchange.Acquisition;
using Exchange.Validation;

#endregion

namespace Exchange.Engine
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [XmlRoot("ExchangeResponse")]
    public class ExchangeResponse
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly string ACCEPTED = "ACCEPTED";

        /// <summary>
        /// 
        /// </summary>
        ///
        public static readonly string PENDING = "PENDING";

        /// <summary>
        /// 
        /// </summary>
        ///
        public static readonly string DUPLICATE = "DUPLICATE";

        /// <summary>
        /// 
        /// </summary>
        ///
        public static readonly string INVALID = "INVALID";

        /// <summary>
        /// 
        /// </summary>
        ///
        public static readonly string ERROR = "ERROR";

        /// <summary>
        /// 
        /// </summary>
        ///
        public static readonly string SUSPECT = "SUSPECT";

        /// <summary>
        /// 
        /// </summary>
        ///
        public static readonly string QUEUED = "QUEUED";

        /// <summary>
        /// 
        /// </summary>
        ///
        public static readonly string SECURITY = "SECURITY";

        /// <summary>
        /// 
        /// </summary>
        ///
        public static readonly string EPHEMERAL = "EPHEMERAL";
 
        /// <summary>
        ///
        /// </summary>
        ///
        private string status;

        /// <summary>
        ///
        /// </summary>
        ///
        private string guid;

        /// <summary>
        ///
        /// </summary>
        ///
        private string aid;

        /// <summary>
        ///
        /// </summary>
        ///
        private string cid;

        /// <summary>
        ///
        /// </summary>
        ///
        private string tid;

        /// <summary>
        ///
        /// </summary>
        ///
        private int matchcount;

        /// <summary>
        ///
        /// </summary>
        ///
        private double amount;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private List<Error> errors = new List<Error>();

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="lead"></param>
        /// 
        /// <returns></returns>
        /// 
        public static ExchangeResponse SecurityViolation(LeadType lead)
        {
            ExchangeResponse response = new ExchangeResponse();
            response.aid = lead.Aid;
            response.amount = 0;
            response.cid = lead.Cid;
            response.guid = lead.Guid;
            response.matchcount = 0;
            response.status = SECURITY;
            response.tid = lead.Tid;

            List<Error> errors = new List<Error>();
            errors.Add(new Error(ExchangeValidator.Credentials, ExchangeValidator.credentialsErrorDesc));
            response.errors = errors;
            return response;
        }

        /// <summary>
        ///
        /// </summary>
        ///
        [XmlAttribute("status")]
        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        [XmlAttribute("guid")]
        public string Guid
        {
            get { return guid; }
            set { guid = value; }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        [XmlAttribute("aid")]
        public string Aid
        {
            get { return aid; }
            set { aid = value; }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        [XmlAttribute("cid")]
        public string Cid
        {
            get { return cid; }
            set { cid = value; }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        [XmlAttribute("tid")]
        public string Tid
        {
            get { return tid; }
            set { tid = value; }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        [XmlAttribute("matchcount")]
        public int MatchCount
        {
            get { return matchcount; }
            set { matchcount = value; }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        [XmlAttribute("amount")]
        public double Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        [XmlArray("errors")]
        [XmlArrayItem("error")]
        public Error[] Errors
        {
            get { return errors.ToArray(); }
            set { errors.AddRange(value); }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        /// <returns></returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("Status = {0}, ", Status);
            result.AppendFormat("Guid = {0}, ", Guid);
            result.AppendFormat("Aid = {0}, ", Aid);
            result.AppendFormat("Cid = {0}, ", Cid);
            result.AppendFormat("Tid = {0}, ", Tid);
            result.AppendFormat("MatchCount = {0}, ", MatchCount);
            result.AppendFormat("Amount = {0}, ", Amount);

            foreach (Error error in errors)
            {
                result.AppendFormat("Error={0},", error.ErrorDescription);
            }

            return result.ToString();
        }
    }
}
