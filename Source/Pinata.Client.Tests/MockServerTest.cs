using System.IO;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using NUnit.Framework;
using VerifyTests;
using static Pinata.Client.HeaderNames;
namespace Pinata.Client.Tests
{
   public class MockServerTest : HasVerifyTest
   {
      protected HttpTest server;

      [SetUp]
      public virtual void BeforeEachTest()
      {
         this.server = new HttpTest();

         var startupFolder = Path.GetDirectoryName(this.GetType().Assembly.Location);
         Directory.SetCurrentDirectory(startupFolder);
      }

      [TearDown]
      public virtual void AfterEachTest()
      {
         EnsureEveryRequestHasCorrectHeaders();

         this.server.Dispose();
      }

      protected virtual void EnsureEveryRequestHasCorrectHeaders()
      {
         server.ShouldHaveMadeACall()
            .WithHeader("User-Agent", PinataClient.UserAgent)
            .WithHeader(PinataApiKey)
            .WithHeader(PinataSecretApiKey);
      }
   }
}
