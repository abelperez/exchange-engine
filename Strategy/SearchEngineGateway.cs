
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

namespace Exchange.Engine.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class SearchEngineGateway
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        public static bool HasLostConversions()
        {
            return true;
        }

        #region Has Lost Conversions path 1

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        public static bool IsCurrentAndHistoricalPositionTheSame()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <returns></returns>
        /// 
        public static bool HasAdCopyChanged()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static void AlertAdCopyChangeRollback()
        {
            Console.WriteLine("alertin user to rollback adcopy changes.");
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// 
        public static void AlertPotentialLandingPageChanges()
        {
            Console.WriteLine("alerting user of landing page changes.");
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static void CalculateAggregatedCostContribution()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static void CalculateAggregatedClickContribution()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static AggregatedContribution CalculateAggregatedCostAndClickContribution()
        {
            return new AggregatedContribution();
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="costContributionPercent"></param>
        /// <param name="clickContributionPercent"></param>
        /// 
        /// <returns></returns>
        /// 
        public static bool ContributionCheck(int costContributionPercent, int clickContributionPercent)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static void PauseAccount()
        {
            Console.WriteLine("pausing account.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static void ChangeBidToMostRelevantCombinationForPositiveROI()
        {
            Console.WriteLine("changing bid to most relevant combination for positive roi.");
        }


    }
}
