using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using static System.Net.Http.HttpMethod;

namespace Pinata.Client.Tests
{
   public class PinningTests : MockServerTest
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
      public async Task pin_json_content()
      {
         this.server.RespondWithJsonTestFile();

         var content = "{\"hello\":\"world\"}";

         var r = await this.client.Pinning.PinJsonToIpfsAsync(content);

         this.server.ShouldHaveCalledPath("/pinning/pinJSONToIPFS")
            .WithVerb(Post)
            .WithRequestBody(content);

         await Verify(r);
      }

      [Test]
      public async Task pin_json_content_with_options()
      {
         this.server.RespondWithJsonTestFile();

         var content = "{\"hello\":\"world\"}";

         var opts = new PinataOptions
            {
               CidVersion = 1,
            };
         opts.CustomPinPolicy.AddOrUpdateRegion("FRA1", 1);

         var meta = new PinataMetadata
            {
               Name = "hello",
               KeyValues =
                  {
                     { "someKey", "someValue" }
                  }
            };

         var r = await this.client.Pinning.PinJsonToIpfsAsync(content, meta, opts);

         var expectedBody = "{\"pinataContent\":{\"hello\":\"world\"},\"pinataOptions\":{\"cidVersion\":1,\"customPinPolicy\":{\"regions\":[{\"id\":\"FRA1\",\"desiredReplicationCount\":1}]}},\"pinataMetadata\":{\"name\":\"hello\",\"keyvalues\":{\"someKey\":\"someValue\"}}}";

         this.server.ShouldHaveCalledPath("/pinning/pinJSONToIPFS")
            .WithVerb(Post)
            .WithExactBody(expectedBody);

         await Verify(r);
      }

      [Test]
      public async Task pin_json_object()
      {
         this.server.RespondWithJsonTestFile();

         var body = new { hello = "world" };

         var r = await this.client.Pinning.PinJsonToIpfsAsync(body);

         var expectedBody = "{\"pinataOptions\":null,\"pinataMetadata\":null,\"pinataContent\":{\"hello\":\"world\"}}";

         this.server.ShouldHaveCalledPath("/pinning/pinJSONToIPFS")
            .WithVerb(Post)
            .WithExactBody(expectedBody);

         await Verify(r);
      }

      [Test]
      public async Task pin_json_object_with_options()
      {
         this.server.RespondWithJsonTestFile();

         var body = new { hello = "world" };
         var opts = new PinataOptions
            {
               CidVersion = 1,
            };
         opts.CustomPinPolicy.AddOrUpdateRegion("FRA1", 1);

         var meta = new PinataMetadata
            {
               Name = "hello",
               KeyValues =
                  {
                     { "someKey", "someValue" }
                  }
            };
         var r = await this.client.Pinning.PinJsonToIpfsAsync(body, meta, opts);

         var expectedBody = "{\"pinataOptions\":{\"cidVersion\":1,\"customPinPolicy\":{\"regions\":[{\"id\":\"FRA1\",\"desiredReplicationCount\":1}]}},\"pinataMetadata\":{\"name\":\"hello\",\"keyvalues\":{\"someKey\":\"someValue\"}},\"pinataContent\":{\"hello\":\"world\"}}";
         this.server.ShouldHaveCalledPath("/pinning/pinJSONToIPFS")
            .WithVerb(Post)
            .WithExactBody(expectedBody);

         await Verify(r);
      }
   }
}
