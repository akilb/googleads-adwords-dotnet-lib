/*
* Copyright (C) 2006 Google Inc.
* 
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
* 
*      http://www.apache.org/licenses/LICENSE-2.0
* 
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
using System;
using System.Text;
using com.google.api.adwords.v9;
using com.google.api.adwords.lib;

namespace com.google.api.adwords.examples
{
	/**
	 * Adds campaign, ad group, creative, and websites. 
	 */
	class CampaignServiceWebsiteDemo
	{
		public static void run()
		{
			// Create a user (reads headers from app.config file)
			AdWordsUser user = new AdWordsUser();
			// Use sandbox
			user.useSandbox();
			// Get the services
			CampaignService campaignService = (CampaignService)user.getService("CampaignService");
			AdGroupService adgroupService = (AdGroupService)user.getService("AdGroupService");
			CriterionService criterionService = (CriterionService)user.getService("CriterionService");
			AdService adService = (AdService)user.getService("AdService");

			// Create a new campaign with some ad groups.
			// First create the campaign so we can get its id.
			Campaign newCampaign = new Campaign();
			newCampaign.dailyBudget = 10000000;
			newCampaign.dailyBudgetSpecified = true;

			// The campaign name is optional.
			// An error results if a campaign of the same name already exists.
			//newCampaign.name = "AdWords API Campaign";

			// Target the campaign at France and Spain.
			// Only one kind of geotargeting can be specified.
			GeoTarget g_target = new GeoTarget();
			String[] countries = {"FR", "ES"};
			g_target.countries = countries;
			newCampaign.geoTargeting = g_target;

			// Target the campaign at English, French and Spanish.
			String[] languages =  {"en", "fr", "es"};
			newCampaign.languageTargeting = languages;
			// Set the campaign status to paused, we don't want to start paying for this test
			newCampaign.status = CampaignStatus.Paused;

			// Add the new campaign.
			// The campaign object is returned with ids filled in.
			newCampaign = campaignService.addCampaign(newCampaign);
			int campaign_id = newCampaign.id;

			// Create an ad group
			AdGroup myAdGroup = new AdGroup();
			myAdGroup.name = "dev guide";
			myAdGroup.maxCpm = 10000000;
			myAdGroup.maxCpmSpecified = true;

			// Associate the ad group with the new campaign.
			// Send the request to add the new AdGroup.
			AdGroup newAdGroup = adgroupService.addAdGroup(campaign_id, myAdGroup);

			int adgroup_id=newAdGroup.id;

			// Create a text ad.
			// IMPORTANT: create the ad before adding keywords! Else the minCpc will 
			// have a higher value
			TextAd ad1 = new TextAd();
			ad1.headline = "AdWords API Dev Guide";
			ad1.description1 = "Access your AdWords";
			ad1.description2 = "accounts programmatically";
			ad1.displayUrl = "blog.chanezon.com";
			ad1.destinationUrl = "http://blog.chanezon.com/";
			ad1.adGroupId = adgroup_id;
			Ad[] ads = adService.addAds(new Ad[] {ad1});
    
			// Add keywords to the AdGroup.
			Website ws = new Website();
			ws.adGroupId = adgroup_id;
			ws.url = "artima.com";
			ws.maxCpm = 1000000;

			Criterion[] newWebsites= {ws};

			newWebsites = criterionService.addCriteria(newWebsites);

			// Update all criteria maxCpc
			((Website)newWebsites[0]).maxCpm = 3000000;			
			((Website)newWebsites[0]).maxCpmSpecified = true;			
			criterionService.updateCriteria(newWebsites);
    
			// Check criteria maxCpm
			Criterion[] updatedWs = criterionService.getAllCriteria(adgroup_id);
			for (int i = 0; i < updatedWs.Length; i++) 
			{
				ws = (Website)updatedWs[i];
				Console.WriteLine(ws.url + " maxCpm = " + ws.maxCpm);
			}
    
			Console.ReadLine();
		}
	}
}