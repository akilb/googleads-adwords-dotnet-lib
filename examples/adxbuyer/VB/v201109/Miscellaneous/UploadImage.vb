' Copyright 2011, Google Inc. All Rights Reserved.
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
'
'     http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.

' Author: api.anash@gmail.com (Anash P. Oommen)

Imports Google.Api.Ads.AdWords.Lib
Imports Google.Api.Ads.AdWords.v201109
Imports Google.Api.Ads.Common.Util

Imports System
Imports System.Collections.Generic
Imports System.IO

Namespace Google.Api.Ads.AdWords.Examples.VB.v201109
  ''' <summary>
  ''' This code example uploads an image. To get images, run GetAllVideosAndImages.vb.
  '''
  ''' Tags: MediaService.upload
  ''' </summary>
  Class UploadImage
    Inherits ExampleBase
    ''' <summary>
    ''' Main method, to run this code example as a standalone application.
    ''' </summary>
    ''' <param name="args">The command line arguments.</param>
    Public Shared Sub Main(ByVal args As String())
      Dim codeExample As ExampleBase = New UploadImage
      Console.WriteLine(codeExample.Description)
      codeExample.Run(New AdWordsUser(), codeExample.GetParameters(), Console.Out)
    End Sub

    ''' <summary>
    ''' Returns a description about the code example.
    ''' </summary>
    Public Overrides ReadOnly Property Description() As String
      Get
        Return "This code example uploads an image. To get images, run GetAllVideosAndImages.vb."
      End Get
    End Property

    ''' <summary>
    ''' Gets the list of parameter names required to run this code example.
    ''' </summary>
    ''' <returns>
    ''' A list of parameter names for this code example.
    ''' </returns>
    Public Overrides Function GetParameterNames() As String()
      Return New String() {}
    End Function

    ''' <summary>
    ''' Runs the code example.
    ''' </summary>
    ''' <param name="user">The AdWords user.</param>
    ''' <param name="parameters">The parameters for running the code
    ''' example.</param>
    ''' <param name="writer">The stream writer to which script output should be
    ''' written.</param>
    Public Overrides Sub Run(ByVal user As AdWordsUser, ByVal parameters As  _
        Dictionary(Of String, String), ByVal writer As TextWriter)
      ' Get the MediaService.
      Dim mediaService As MediaService = user.GetService( _
          AdWordsService.v201109.MediaService)

      ' Create the image.
      Dim image As New Image
      image.data = MediaUtilities.GetAssetDataFromUrl("http://goo.gl/HJM3L")
      image.type = MediaMediaType.IMAGE

      Try
        ' Upload the image.
        Dim result As Media() = mediaService.upload(New Media() {image})

        ' Display the results.
        If ((Not result Is Nothing) AndAlso (result.Length > 0)) Then
          Dim newImage As Media = result(0)
          Dim dimensions As Dictionary(Of MediaSize, Dimensions) = _
                CreateMediaDimensionMap(newImage.dimensions)
          writer.WriteLine("Image with id '{0}', dimensions '{1}x{2}', and MIME type '{3}'" & _
              " was uploaded.", newImage.mediaId, dimensions.Item(MediaSize.FULL).width, _
              dimensions.Item(MediaSize.FULL).height, newImage.mimeType)
        Else
          writer.WriteLine("No images were uploaded.")
        End If
      Catch ex As Exception
        writer.WriteLine("Failed to upload images. Exception says ""{0}""", ex.Message)
      End Try
    End Sub

    ''' <summary>
    ''' Converts an array of Media_Size_DimensionsMapEntry into a dictionary.
    ''' </summary>
    ''' <param name="dimensions">The array of Media_Size_DimensionsMapEntry to
    ''' be converted into a dictionary.</param>
    ''' <returns>A dictionary with key as MediaSize, and value as Dimensions.
    ''' </returns>
    Private Function CreateMediaDimensionMap(ByVal dimensions As Media_Size_DimensionsMapEntry()) _
        As Dictionary(Of MediaSize, Dimensions)
      Dim mediaMap As New Dictionary(Of MediaSize, Dimensions)
      For Each dimension As Media_Size_DimensionsMapEntry In dimensions
        mediaMap.Add(dimension.key, dimension.value)
      Next
      Return mediaMap
    End Function
  End Class
End Namespace