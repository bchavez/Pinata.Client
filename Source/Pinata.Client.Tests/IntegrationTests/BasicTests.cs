﻿using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Pinata.Client.Tests.IntegrationTests
{
   [Explicit]
   public class BasicTests : IntegrationTest
   {
      private PinataClient client;

      [SetUp]
      public void BeforeEachTest()
      {
         var config = new Config
            {
               ApiKey = this.secrets.ApiKey,
               ApiSecret = this.secrets.ApiSecret
            };
         client = new PinataClient(config);
         client.EnableFiddlerDebugProxy("http://localhost.:8888");
      }


      [Test]
      public async Task can_auth()
      {
         var resp = await this.client.Data.TestAuthenticationAsync();
         resp.Should().NotBeNull();
         resp.Message.Should().Be("Congratulations! You are communicating with the Pinata API!");
      }
   }
}