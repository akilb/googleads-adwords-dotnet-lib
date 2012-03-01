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
using Google.Api.Ads.AdWords.Util;
using Google.Api.Ads.AdWords.v201109;

using System;
using System.Collections.Generic;
using System.IO;

namespace Google.Api.Ads.AdWords.Examples.CSharp.v201109 {
  /// <summary>
  /// This code example adds a text ad, and shows how to handle a policy
  /// violation.
  ///
  /// Tags: AdGroupAdService.mutate
  /// </summary>
  class HandlePolicyViolationError : ExampleBase {
    /// <summary>
    /// Main method, to run this code example as a standalone application.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    public static void Main(string[] args) {
      ExampleBase codeExample = new HandlePolicyViolationError();
      Console.WriteLine(codeExample.Description);
      codeExample.Run(new AdWordsUser(), codeExample.GetParameters(), Console.Out);
    }

    /// <summary>
    /// Returns a description about the code example.
    /// </summary>
    public override string Description {
      get {
        return "This code example adds a text ad, and shows how to handle a policy violation.";
      }
    }

    /// <summary>
    /// Gets the list of parameter names required to run this code example.
    /// </summary>
    /// <returns>
    /// A list of parameter names for this code example.
    /// </returns>
    public override string[] GetParameterNames() {
      return new string[] {"ADGROUP_ID"};
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
      // Get the AdGroupAdService.
      AdGroupAdService service =
          (AdGroupAdService) user.GetService(AdWordsService.v201109.AdGroupAdService);

      long adGroupId = long.Parse(parameters["ADGROUP_ID"]);

      // Create the text ad.
      TextAd textAd = new TextAd();
      textAd.headline = "Luxury Cruise to Mars";
      textAd.description1 = "Visit the Red Planet in style.";
      textAd.description2 = "Low-gravity fun for everyone!!";
      textAd.displayUrl = "www.example.com";
      textAd.url = "http://www.example.com";

      AdGroupAd textadGroupAd = new AdGroupAd();
      textadGroupAd.adGroupId = adGroupId;
      textadGroupAd.ad = textAd;

      // Create the operations.
      AdGroupAdOperation textAdOperation = new AdGroupAdOperation();
      textAdOperation.@operator = Operator.ADD;
      textAdOperation.operand = textadGroupAd;

      try {
        AdGroupAdReturnValue retVal = null;

        // Setup two arrays, one to hold the list of all operations to be
        // validated, and another to hold the list of operations that cannot be
        // fixed after validation.
        List<AdGroupAdOperation> allOperations = new List<AdGroupAdOperation>();
        List<AdGroupAdOperation> operationsToBeRemoved = new List<AdGroupAdOperation>();

        allOperations.Add(textAdOperation);

        try {
          // Validate the operations.
          service.RequestHeader.validateOnly = true;
          retVal = service.mutate(allOperations.ToArray());
        } catch (AdWordsApiException ex) {
          ApiException innerException = ex.ApiException as ApiException;
          if (innerException == null) {
            throw new Exception("Failed to retrieve ApiError. See inner exception for more " +
                "details.", ex);
          }

          // Examine each ApiError received from the server.
          foreach (ApiError apiError in innerException.errors) {
            int index = ErrorUtilities.GetOperationIndex(apiError.fieldPath);
            if (index == -1) {
              // This API error is not associated with an operand, so we cannot
              // recover from this error by removing one or more operations.
              // Rethrow the exception for manual inspection.
              throw;
            }

            // Handle policy violation errors.
            if (apiError is PolicyViolationError) {
              PolicyViolationError policyError = (PolicyViolationError) apiError;

              if (policyError.isExemptable) {
                // If the policy violation error is exemptable, add an exemption
                // request.
                List<ExemptionRequest> exemptionRequests = new List<ExemptionRequest>();
                if (allOperations[index].exemptionRequests != null) {
                  exemptionRequests.AddRange(allOperations[index].exemptionRequests);
                }

                ExemptionRequest exemptionRequest = new ExemptionRequest();
                exemptionRequest.key = policyError.key;
                exemptionRequests.Add(exemptionRequest);
                allOperations[index].exemptionRequests = exemptionRequests.ToArray();
              } else {
                // Policy violation error is not exemptable, remove this
                // operation from the list of operations.
                operationsToBeRemoved.Add(allOperations[index]);
              }
            } else {
              // This is not a policy violation error, remove this operation
              // from the list of operations.
              operationsToBeRemoved.Add(allOperations[index]);
            }
          }
          // Remove all operations that aren't exemptable.
          foreach (AdGroupAdOperation operation in operationsToBeRemoved) {
            allOperations.Remove(operation);
          }
        }

        if (allOperations.Count > 0) {
          // Perform the operations exemptible of a policy violation.
          service.RequestHeader.validateOnly = false;
          retVal = service.mutate(allOperations.ToArray());

          // Display the results.
          if (retVal != null && retVal.value != null && retVal.value.Length > 0) {
            foreach (AdGroupAd newAdGroupAd in retVal.value) {
              writer.WriteLine("New ad with id = \"{0}\" and displayUrl = \"{1}\" was created.",
                  newAdGroupAd.ad.id, newAdGroupAd.ad.displayUrl);
            }
          } else {
            writer.WriteLine("No ads were created.");
          }
        } else {
          writer.WriteLine("There are no ads to create after policy violation checks.");
        }
      } catch (Exception ex) {
        writer.WriteLine("Failed to create ad(s). Exception says \"{0}\"", ex.Message);
      }
    }
  }
}