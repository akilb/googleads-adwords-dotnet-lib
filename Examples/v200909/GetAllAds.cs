// Copyright 2009, Google Inc. All Rights Reserved.
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

using com.google.api.adwords.lib;
using com.google.api.adwords.v200909;

using System;

namespace com.google.api.adwords.samples.v200909 {
  /// <summary>
  /// This code sample retrieves all ads given an existing ad group. To add
  /// ads to an existing ad group, you can run AddAds.cs.
  /// </summary>
  class GetAllAds : SampleBase {
    /// <summary>
    /// Returns a description about the sample code.
    /// </summary>
    public override string Description {
      get {
        return "This code sample retrieves all ads given an existing ad group.";
      }
    }

    /// <summary>
    /// Run the sample code.
    /// </summary>
    /// <param name="user">The AdWords user object running the sample.
    /// </param>
    public override void Run(AdWordsUser user) {
      // Get the AdGroupAdService.
      AdGroupAdService service =
          (AdGroupAdService) user.GetService(AdWordsService.v200909.AdGroupAdService);

      long adGroupId = long.Parse(_T("INSERT_AD_GROUP_ID_HERE"));

      // Create a selector and set the filters.
      AdGroupAdSelector selector = new AdGroupAdSelector();
      selector.adGroupIds = new long[] {adGroupId};

      try {
        AdGroupAdPage page = service.get(selector);

        if (page != null && page.entries != null) {
          foreach (AdGroupAd tempAdGroupAd in page.entries) {
            Console.WriteLine("Ad id is {0} and status is {1}", tempAdGroupAd.ad.id,
                tempAdGroupAd.status);
          }
        }
      } catch (Exception ex) {
        Console.WriteLine("Failed to get Ad(s). Exception says \"{0}\"", ex.Message);
      }
    }
  }
}