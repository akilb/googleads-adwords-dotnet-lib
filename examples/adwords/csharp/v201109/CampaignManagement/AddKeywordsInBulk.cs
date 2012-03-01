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
using System.Text.RegularExpressions;
using System.Threading;

namespace Google.Api.Ads.AdWords.Examples.CSharp.v201109 {
  /// <summary>
  /// This code example shows how to add keywords in bulk using the
  /// MutateJobService.
  ///
  /// Tags: MutateJobService.mutate, MutateJobService.get
  /// Tags: MutateJobService.getResult
  /// </summary>
  class AddKeywordsInBulk : ExampleBase {
    /// <summary>
    /// Main method, to run this code example as a standalone application.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    public static void Main(string[] args) {
      ExampleBase codeExample = new AddKeywordsInBulk();
      Console.WriteLine(codeExample.Description);
      codeExample.Run(new AdWordsUser(), codeExample.GetParameters(), Console.Out);
    }

    /// <summary>
    /// Returns a description about the code example.
    /// </summary>
    public override string Description {
      get {
        return "This code example shows how to add keywords in bulk using the MutateJobService.";
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
      // Get the MutateJobService.
      MutateJobService mutateJobService = (MutateJobService) user.GetService(
          AdWordsService.v201109.MutateJobService);

      const int RETRY_INTERVAL = 30;
      const int RETRIES_COUNT = 30;
      const int KEYWORD_NUMBER = 100;
      const string INDEX_REGEX = "operations\\[(\\d+)\\].operand";

      long adGroupId = long.Parse(parameters["ADGROUP_ID"]);

      List<Operation> operations = new List<Operation>();

      // Create AdGroupCriterionOperation to add keywords.
      for (int i = 0; i < KEYWORD_NUMBER; i++) {
        Keyword keyword = new Keyword();
        keyword.text = string.Format("mars cruise {0}", i);
        keyword.matchType = KeywordMatchType.BROAD;

        BiddableAdGroupCriterion criterion = new BiddableAdGroupCriterion();
        criterion.adGroupId = adGroupId;
        criterion.criterion = keyword;

        AdGroupCriterionOperation adGroupCriterionOperation = new AdGroupCriterionOperation();
        adGroupCriterionOperation.@operator = Operator.ADD;
        adGroupCriterionOperation.operand = criterion;

        operations.Add(adGroupCriterionOperation);
      }

      BulkMutateJobPolicy policy = new BulkMutateJobPolicy();
      // You can specify up to 3 job IDs that must successfully complete before
      // this job can be processed.
      policy.prerequisiteJobIds = new long[] {};

      SimpleMutateJob job = mutateJobService.mutate(operations.ToArray(), policy);

      // Wait for the job to complete.
      bool completed = false;
      int retryCount = 0;
      writer.WriteLine("Retrieving job status...");

      while (completed == false && retryCount < RETRIES_COUNT) {
        BulkMutateJobSelector selector = new BulkMutateJobSelector();
        selector.jobIds = new long[] {job.id};

        try {
          Job[] allJobs = mutateJobService.get(selector);
          if (allJobs != null && allJobs.Length > 0) {
            job = (SimpleMutateJob) allJobs[0];
            if (job.status == BasicJobStatus.COMPLETED || job.status == BasicJobStatus.FAILED) {
              completed = true;
              break;
            } else {
              writer.WriteLine("{0}: Current status is {1}, waiting {2} seconds to retry...",
                  retryCount, job.status, RETRY_INTERVAL);
              Thread.Sleep(RETRY_INTERVAL * 1000);
              retryCount++;
            }
          }
        } catch (Exception ex) {
          writer.WriteLine("Failed to fetch simple mutate job with id = {0}. Exception says " +
              "\"{1}\"", job.id, ex.Message);
          return;
        }
      }

      if (job.status == BasicJobStatus.COMPLETED) {
        // Handle cases where the job completed.

        // Create the job selector.
        BulkMutateJobSelector selector = new BulkMutateJobSelector();
        selector.jobIds = new long[] {job.id};

        // Get the job results.
        JobResult jobResult = mutateJobService.getResult(selector);
        if (jobResult != null) {
          SimpleMutateResult results = (SimpleMutateResult) jobResult.Item;
          if (results != null) {
            // Display the results.
            if (results.results != null) {
              for (int i = 0; i < results.results.Length; i++) {
                Operand operand = results.results[i];
                writer.WriteLine("Operation {0} - {1}", i, (operand.Item is PlaceHolder) ?
                    "FAILED" : "SUCCEEDED");
              }
            }

            // Display the errors.
            if (results.errors != null) {
              foreach (ApiError apiError in results.errors) {
                Match match = Regex.Match(apiError.fieldPath, INDEX_REGEX, RegexOptions.IgnoreCase);
                string index = (match.Success)? match.Groups[1].Value : "???";
                writer.WriteLine("Operation index {0} failed due to reason: '{1}', " +
                    "trigger: '{2}'", index, apiError.errorString, apiError.trigger);
              }
            }
          }
        }
        writer.WriteLine("Job completed successfully!");
      } else if (job.status == BasicJobStatus.FAILED) {
        // Handle the cases where job failed.
        writer.WriteLine("Job failed with reason: " + job.failureReason);
      } else if (job.status == BasicJobStatus.PROCESSING || job.status == BasicJobStatus.PENDING) {
        // Handle the cases where job didn't complete after wait period.
        writer.WriteLine("Job did not complete in {0} secconds.", RETRY_INTERVAL * RETRIES_COUNT);
      }
    }
  }
}