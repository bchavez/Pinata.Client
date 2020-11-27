using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using FluentAssertions;
using Flurl.Http.Testing;
using Newtonsoft.Json;

namespace Pinata.Client.Tests
{
   internal static class ExtensionsForTesting
   {
      public static void Dump(this object obj)
      {
         Console.WriteLine(obj.DumpString());
      }

      public static string DumpString(this object obj)
      {
         return JsonConvert.SerializeObject(obj, Formatting.Indented);
      }

      public static HttpCallAssertion ShouldHaveCalledPath(this HttpTest server, string path)
      {
         return server.ShouldHaveMadeACall()
            .With(fc => fc.Request.Url.Path == path);
      }

      public static HttpCallAssertion WithExactBody(this HttpCallAssertion assert, string body)
      {
         return assert.With(fc =>
            {
               fc.RequestBody.Should().Be(body);
               return true;
            });
      }

      public static HttpTest RespondWithJsonTestFile(this HttpTest server,
         object headers = null,
         [CallerMemberName] string methodName = "",
         [CallerFilePath] string filePath = "")
      {
         var responseFile = Path.ChangeExtension(filePath, $"{methodName}.server.json");

         if( !File.Exists(responseFile) )
         {
            var p = Process.Start("notepad.exe", responseFile);
            p.WaitForExit();

            if( !File.Exists(responseFile) )
            {
               throw new FileNotFoundException($"*.server.json test file not found '{responseFile}'", responseFile);
            }
         }

         var json = File.ReadAllText(responseFile);
         server.RespondWith(json, headers: headers);
         return server;
      }

   }

}
