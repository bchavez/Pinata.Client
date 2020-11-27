using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Pinata.Client.Tests
{
   public class DataTests : MockServerTest
   {
      private PinataClient client;

      [SetUp]
      public virtual void BeforeEachTest()
      {
         var config = new Config()
         {
            ApiKey = "key",
            ApiSecret = "secret"
         };

         this.client = new PinataClient(config);
      }

      [Test]
      public async Task UserPinnedDataTotal_with_no_values()
      {
         this.server.RespondWithJsonTestFile();

         var r = await client.Data.UserPinnedDataTotalAsync();

         this.server
            .ShouldHaveCalledPath("/data/userPinnedDataTotal")
            .WithVerb(HttpMethod.Get);

         await Verify(r);
      }

      [Test]
      public async Task UserPinnedDataTotal_with_values()
      {
         this.server.RespondWithJsonTestFile();

         var r = await client.Data.UserPinnedDataTotalAsync();

         this.server
            .ShouldHaveCalledPath("/data/userPinnedDataTotal")
            .WithVerb(HttpMethod.Get);

         await Verify(r);
      }

   }
}
