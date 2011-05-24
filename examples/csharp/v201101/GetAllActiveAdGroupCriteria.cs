// Copyright 2011, Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// Author: api.anash@gmail.com (Anash P. Oommen)

using Google.Api.Ads.AdWords.Lib;
using Google.Api.Ads.AdWords.v201101;

using System;
using System.IO;
using System.Net;

namespace Google.Api.Ads.AdWords.Examples.CSharp.v201101 {
  /// <summary>
  /// This code example gets all active ad group criteria in an ad group. To
  /// add ad group criteria, run AddAdGroupCriteria.cs. To get ad groups in an
  /// account, run GetAllAdGroups.cs.
  ///
  /// Tags: AdGroupCriterionService.get
  /// </summary>
  class GetAllActiveAdGroupCriteria : SampleBase {
    /// <summary>
    /// Returns a description about the code example.
    /// </summary>
    public override string Description {
      get {
        return "This code example gets all active ad group criteria in an ad group. To add ad " +
            "group criteria, run AddAdGroupCriteria.cs. To get ad groups in an account, run " +
            "GetAllAdGroups.cs.";
      }
    }

    /// <summary>
    /// Main method, to run this code example as a standalone application.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    public static void Main(string[] args) {
      SampleBase codeExample = new GetAllActiveAdGroupCriteria();
      Console.WriteLine(codeExample.Description);
      codeExample.Run(new AdWordsUser());
    }

    /// <summary>
    /// Run the code example.
    /// </summary>
    /// <param name="user">The AdWords user object running the code example.
    /// </param>
    public override void Run(AdWordsUser user) {
      // Get the AdGroupCriterionService.
      AdGroupCriterionService adGroupCriterionService =
          (AdGroupCriterionService) user.GetService(AdWordsService.v201101.AdGroupCriterionService);

      long adGroupId = long.Parse(_T("INSERT_ADGROUP_ID_HERE"));

      // Create a selector.
      Selector selector = new Selector();
      selector.fields = new string[] {"Id", "AdGroupId", "KeywordText", "Status",
          "KeywordMatchType", "PlacementUrl"};

      // Set filter conditions.
      Predicate adGroupPredicate = new Predicate();
      adGroupPredicate.field = "AdGroupId";
      adGroupPredicate.@operator = PredicateOperator.IN;
      adGroupPredicate.values = new string[] {adGroupId.ToString()};

      Predicate statusPredicate = new Predicate();
      statusPredicate.field = "Status";
      statusPredicate.@operator = PredicateOperator.EQUALS;
      statusPredicate.values = new string[] {UserStatus.ACTIVE.ToString()};

      selector.predicates = new Predicate[] {adGroupPredicate, statusPredicate};

      try {
        // Get all ad group criteria.
        AdGroupCriterionPage adGroupCriterionPage = adGroupCriterionService.get(selector);

        if (adGroupCriterionPage != null && adGroupCriterionPage.entries != null) {
          // Display ad group criteria.
          foreach (AdGroupCriterion adGroupCriterion in adGroupCriterionPage.entries) {
            if (adGroupCriterion.criterion is Keyword) {
              Keyword keyword = (Keyword) adGroupCriterion.criterion;
              Console.WriteLine("Keyword ad group criterion with criterion ID = '{0}', text =" +
                  " '{1}' and matchType = '{2} was found.", keyword.id, keyword.text,
                  keyword.matchType);
            } else if (adGroupCriterion.criterion is Placement) {
              Placement placement = (Placement) adGroupCriterion.criterion;
              Console.WriteLine("Placement ad group criterion criterion ID = '{0}' and url =" +
                  " '{1}' was found.", placement.id, placement.url);
            }
          }
        } else {
          Console.WriteLine("No ad group criteria found.");
        }
      } catch (Exception ex) {
        Console.WriteLine("Failed to retrieve criteria. Exception says \"{0}\"", ex.Message);
      }
    }
  }
}