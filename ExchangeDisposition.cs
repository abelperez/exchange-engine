
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

using Mindplex.Commons;

using Exchange;
using Exchange.Validation;
//using Exchange.Distribution.Gateway;

#endregion

namespace Exchange.Engine
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [Serializable]
    public class ExchangeDisposition
    {
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
        public string Status
        {
            get { return status; }
            set { status = value; }
        }

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
        public string Aid
        {
            get { return aid; }
            set { aid = value; }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        public string Cid
        {
            get { return cid; }
            set { cid = value; }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        public string Tid
        {
            get { return tid; }
            set { tid = value; }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        public int MatchCount
        {
            get { return matchcount; }
            set { matchcount = value; }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        public double Amount
        {
            get { return amount; }
            set { amount = value; }
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

        /// <summary>
        /// 
        /// </summary>
        /// 
        private bool coverage;

        /// <summary>
        ///
        /// </summary>
        ///
        //private double distance;

        /// <summary>
        ///
        /// </summary>
        ///
        //private bool exclusive;

        /// <summary>
        ///
        /// </summary>
        ///
        //private bool demand;

        /// <summary>
        ///
        /// </summary>
        ///
        private List<long> prospects = new List<long>();

        /// <summary>
        ///
        /// </summary>
        ///
        //private long price;

        /// <summary>
        ///
        /// </summary>
        ///
        //private long rating;

        /// <summary>
        ///
        /// </summary>
        ///
        private DateTime time;

        /// <summary>
        /// 
        /// </summary>
        /// 
        //private decimal threshold;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public ExchangeDisposition()
        {
            //this.guid = GuidGenerator.Generate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public bool Coverage
        {
            get { return coverage; }
            set { coverage = value; }
        }

        /// <summary>
        ///
        /// </summary>
        ///
        //public bool Demand
        //{
        //    get { return demand; }
        //    set { demand = value; }
        //}

        /// <summary>
        ///
        /// </summary>
        ///
        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }
    }
}
