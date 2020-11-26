using System.Threading.Tasks;
using Flurl.Http.Testing;
using NUnit.Framework;
using VerifyTests;

namespace Pinata.Client.Tests
{
   public class Test
   {
      protected HttpTest server;
      protected VerifySettings settings;

      [SetUp]
      public virtual void BeforeEachTest()
      {
         this.server = new HttpTest();

#if NET45
         Directory.SetCurrentDirectory(Path.GetDirectoryName(this.GetType().Assembly.Location));
#endif
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
            .WithHeader("User-Agent", PinataClient.UserAgent);
      }
   }
}
