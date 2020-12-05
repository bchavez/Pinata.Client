using System.Net.Http;
using System.Net.Http.Headers;
using Flurl.Http.Content;

namespace Pinata.Client
{
   public static class ExtensionsForHttpContent
   {
      //public static T AsPinataFile<T>(this T content, string remoteFilePath) where T : HttpContent
      //{
      //   var header = new ContentDispositionHeaderValue("form-data")
      //      {
      //         Name = "file",
      //         FileName = remoteFilePath
      //      };
      //   content.Headers.ContentDisposition = header;
      //   return content;
      //}

      public static CapturedMultipartContent AddPinataFile(this CapturedMultipartContent multipart, HttpContent httpContent, string remoteFilePath)
      {
         var header = new ContentDispositionHeaderValue("form-data")
            {
               Name = "file",
               FileName = remoteFilePath
            };
         httpContent.Headers.ContentDisposition = header;

         multipart.Add(httpContent);

         return multipart;
      }
   }
}
