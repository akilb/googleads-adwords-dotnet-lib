// Copyright 2010, Google Inc. All Rights Reserved.
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
using com.google.api.adwords.v201008;

using System;
using System.Collections.Generic;
using System.Text;

namespace com.google.api.adwords.examples.v201008 {
  /// <summary>
  /// This code example gets all images. To upload an image, run
  /// UploadImage.cs.
  ///
  /// Tags: MediaService.get
  /// </summary>
  class GetAllImages : SampleBase {
    /// <summary>
    /// Returns a description about the code example.
    /// </summary>
    public override string Description {
      get {
        return "This code example gets all images. To upload an image, run UploadImage.cs.";
      }
    }

    /// <summary>
    /// Run the code example.
    /// </summary>
    /// <param name="user">The AdWords user object running the code example.
    /// </param>
    public override void Run(AdWordsUser user) {
      // Get the MediaService.
      MediaService mediaService = (MediaService) user.GetService(
          AdWordsService.v201008.MediaService);

      // Create selector.
      MediaSelector selector = new MediaSelector();
      selector.mediaType = MediaMediaType.IMAGE;
      selector.mediaTypeSpecified = true;

      try {
        // Get all images.
        MediaPage page = mediaService.get(selector);

        // Display images.
        if (page != null && page.entries != null && page.entries.Length > 0) {
          foreach (Image image in page.entries) {
            Dictionary<MediaSize, Dimensions> dimensions = CreateMediaDimensionMap(
                image.dimensions);
            Console.WriteLine("Image with id '{0}', dimensions '{1}x{2}', and MIME type '{3}' " +
                "was found.", image.mediaId, dimensions[MediaSize.FULL].width,
                dimensions[MediaSize.FULL].height, image.mimeType);
          }
        } else {
          Console.WriteLine("No images were found.");
        }
      } catch (Exception ex) {
        Console.WriteLine("Failed to get all images. Exception says \"{0}\"", ex.Message);
      }
    }

    /// <summary>
    /// Converts an array of Media_Size_DimensionsMapEntry into a dictionary.
    /// </summary>
    /// <param name="dimensions">The array of Media_Size_DimensionsMapEntry to be
    /// converted into a dictionary.</param>
    /// <returns>A dictionary with key as MediaSize, and value as Dimensions.
    /// </returns>
    private Dictionary<MediaSize, Dimensions> CreateMediaDimensionMap(
        Media_Size_DimensionsMapEntry[] dimensions) {
      Dictionary<MediaSize, Dimensions> mediaMap = new Dictionary<MediaSize, Dimensions>();
      foreach (Media_Size_DimensionsMapEntry dimension in dimensions) {
        mediaMap.Add(dimension.key, dimension.value);
      }
      return mediaMap;
    }
  }
}