
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
    public class SaverEvent : GenericStrategyEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        private string filename;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string content;
        
        /// <summary>
        /// 
        /// </summary>
        /// 
        private string context;

        /// <summary>
        /// 
        /// </summary>
        /// 
        private string status;

        /// <summary>
        /// 
        /// </summary>
        /// 
        public SaverEvent()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public string Context
        {
            get { return context; }
            set { context = value; }
        }

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
        /// <param name="name"></param>
        /// 
        public SaverEvent(string actor)
            : base(actor, typeof(SaverEvent).ToString(), "Saver stores specified data as an xml file on disk.")
        {
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
            xml.AppendFormat("<saver email=\"{0}\" created=\"{1}\" guid=\"{2}\" >", Email, Created, Guid);
            xml.AppendFormat("<filename>{0}</filename>", Filename);
            xml.AppendFormat("<context>{0}</context>", Context);
            xml.AppendFormat("<status>{0}</status>", Status);
            xml.AppendFormat("<result type=\"text\"><![CDATA[{0}]]></result>", Content);
            xml.AppendFormat("<time unit=\"ms\">{0}</time>", ElapsedTime);
            xml.Append("</saver>");
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
                    .AppendFormat("Filename = {0}, ", Filename)
                    .AppendFormat("Context = {0}, ", Context)
                    .AppendFormat("Status = {0}, ", Status).ToString();
        }
    }
}
