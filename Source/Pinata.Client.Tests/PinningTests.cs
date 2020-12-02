using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http.Content;
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
      public async Task pin_html_file()
      {
         this.server.RespondWithJsonTestFile();

         var html = @"
<html>
   <head>
      <title>Hello IPFS!</title>
   </head>
   <body>
      <h1>Hello World</h1>
   </body>
</html>
";
         var r = await this.client.Pinning.PinFileToIPFSAsync(payload =>
         {
            var file = new StringContent(html, Encoding.UTF8, MediaTypeNames.Text.Html)
               .AsPinataFile("index.html");

            payload.Add(file);
         });


         this.server.ShouldHaveCalledPath("/pinning/pinFileToIPFS")
            .With(fc =>
            {
               var content = fc.HttpRequestMessage.Content as CapturedMultipartContent;

               var cds = content.Headers.ContentDisposition;
               cds.Name.Should().Be("file");
               cds.FileName.Should().Be("index.html");
               return true;
            });


      }
   }
}
