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

         var content = @"{""hello"":""world""}";

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

         var content = @"{""hello"":""world""}";

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

         var expectedBody = @"{""pinataContent"":{""hello"":""world""},""pinataOptions"":{""cidVersion"":1,""customPinPolicy"":{""regions"":[{""id"":""FRA1"",""desiredReplicationCount"":1}]},""wrapWithDirectory"":null},""pinataMetadata"":{""name"":""hello"",""keyvalues"":{""someKey"":""someValue""}}}";

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

         var expectedBody = @"{""pinataOptions"":null,""pinataMetadata"":null,""pinataContent"":{""hello"":""world""}}";

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

         var expectedBody = @"{""pinataOptions"":{""cidVersion"":1,""customPinPolicy"":{""regions"":[{""id"":""FRA1"",""desiredReplicationCount"":1}]},""wrapWithDirectory"":null},""pinataMetadata"":{""name"":""hello"",""keyvalues"":{""someKey"":""someValue""}},""pinataContent"":{""hello"":""world""}}";
         this.server.ShouldHaveCalledPath("/pinning/pinJSONToIPFS")
            .WithVerb(Post)
            .WithExactBody(expectedBody);

         await Verify(r);
      }

      [Test]
      public async Task new_user_pin_policy()
      {
         this.server.RespondWithJsonTestFile();

         var policy = new PinPolicy();
         policy.AddOrUpdateRegion("FRA1", 1);

         var r = await this.client.Pinning.UserPinPolicyAsync(policy);

         var expectedBody = @"{""newPinPolicy"":{""regions"":[{""id"":""FRA1"",""desiredReplicationCount"":1}]},""migratePreviousPins"":false}";

         this.server.ShouldHaveCalledPath("/pinning/userPinPolicy")
            .WithVerb(Put)
            .WithExactBody(expectedBody);

         await Verify(r);
      }

      [Test]
      public async Task upload()
      {
         var svg = @"
<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 100 100"">
  <circle cx=""50"" cy=""50"" r=""48"" fill=""none"" stroke=""#000""/>
  <path d=""M50,2a48,48 0 1 1 0,96a24 24 0 1 1 0-48a24 24 0 1 0 0-48""/>
  <circle cx=""50"" cy=""26"" r=""6""/>
  <circle cx=""50"" cy=""74"" r=""6"" fill=""#FFF""/>
</svg>";


      }
   }
}
