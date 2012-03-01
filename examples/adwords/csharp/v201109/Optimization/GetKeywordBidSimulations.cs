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
using Google.Api.Ads.AdWords.v201109;

using System;
using System.Collections.Generic;
using System.IO;

namespace Google.Api.Ads.AdWords.Examples.CSharp.v201109 {
  /// <summary>
  /// This code example gets a bid landscape for an ad group and a keyword.
  /// To get ad groups, run GetAdGroups.cs. To get keywords, run
  /// GetKeywords.cs.
  ///
  /// Tags: DataService.getCriterionBidLandscape
  /// </summary>
  class GetKeywordBidSimulations : ExampleBase {
    /// <summary>
    /// Main method, to run this code example as a standalone application.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    public static void Main(string[] args) {
      ExampleBase codeExample = new GetKeywordBidSimulations();
      Console.WriteLine(codeExample.Description);
      codeExample.Run(new AdWordsUser(), codeExample.GetParameters(), Console.Out);
    }

    /// <summary>
    /// Returns a description about the code example.
    /// </summary>
    public override string Description {
      get {
        return "This code example gets a bid landscape for an ad group and a keyword. " +
            "To get ad groups, run GetAdGroups.cs. To get keywords, run GetKeywords.cs.";
      }
    }

    /// <summary>
    /// Gets the list of parameter names required to run this code example.
    /// </summary>
    /// <returns>
    /// A list of parameter names for this code example.
    /// </returns>
    public override string[] GetParameterNames() {
      return new string[] {"ADGROUP_ID", "KEYWORD_ID"};
    }

    /// <summary>
    /// Runs the code example.
    /// </summary>
    /// <param name="user">The AdWords user.</param>
    /// <param name="parameters">The parameters for running the code
    /// example.</param>
    /// <param name="writer">The stream writer to which script output should be
    /// written.</param>
    public override void Run(AdWordsUser user, Dictionary<string, string> parameters,
        TextWriter writer) {
      // Get the DataService.
      DataService dataService = (DataService) user.GetService(AdWordsService.v201109.DataService);

      long adGroupId = long.Parse(parameters["ADGROUP_ID"]);
      long keywordId = long.Parse(parameters["KEYWORD_ID"]);

      // Create the selector.
      Selector selector = new Selector();
      selector.fields = new string[] {"AdGroupId", "CriterionId", "StartDate", "EndDate", "Bid",
          "LocalClicks", "LocalCost", "MarginalCpc", "LocalImpressions"};

      // Create the filters.
      Predicate adGroupPredicate = new Predicate();
      adGroupPredicate.field = "AdGroupId";
      adGroupPredicate.@operator = PredicateOperator.IN;
      adGroupPredicate.values = new string[] {adGroupId.ToString()};

      Predicate keywordPredicate = new Predicate();
      keywordPredicate.field = "CriterionId";
      keywordPredicate.@operator = PredicateOperator.IN;
      keywordPredicate.values = new string[] {keywordId.ToString()};

      selector.predicates = new Predicate[] {adGroupPredicate, keywordPredicate};

      // Set selector paging.
      selector.paging = new Paging();

      int offset = 0;
      int pageSize = 500;

      CriterionBidLandscapePage page = new CriterionBidLandscapePage();

      try {
        do {
          selector.paging.startIndex = offset;
          selector.paging.numberResults = pageSize;

          // Get bid landscape for keywords.
          page = dataService.getCriterionBidLandscape(selector);

          // Display bid landscapes.
          if (page != null && page.entries != null) {
            int i = offset;

            foreach (CriterionBidLandscape bidLandscape in page.entries) {
              writer.WriteLine("{0}) Found criterion bid landscape with ad group id '{1}', " +
                  "keyword id '{2}', start date '{3}', end date '{4}', and landscape points:",
                  i, bidLandscape.adGroupId, bidLandscape.criterionId, bidLandscape.startDate,
                  bidLandscape.endDate);
              foreach (BidLandscapeLandscapePoint bidLandscapePoint in
                  bidLandscape.landscapePoints) {
                writer.WriteLine("- bid: {0} => clicks: {1}, cost: {2}, marginalCpc: {3}, " +
                    "impressions: {4}\n", bidLandscapePoint.bid.microAmount,
                    bidLandscapePoint.clicks, bidLandscapePoint.cost.microAmount,
                    bidLandscapePoint.marginalCpc.microAmount, bidLandscapePoint.impressions);
              }
              i++;
            }
          }
          offset += pageSize;
        } while (offset < page.totalNumEntries);
        writer.WriteLine("Number of keyword bid landscapes found: {0}", page.totalNumEntries);
      } catch (Exception ex) {
        writer.WriteLine("Failed to retrieve keyword bid landscapes. Exception says \"{0}\"",
            ex.Message);
      }
    }
  }
}