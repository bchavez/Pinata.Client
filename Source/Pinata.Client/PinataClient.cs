using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;
using static Pinata.Client.HeaderNames;

namespace Pinata.Client
{
   public class Config
   {
      public string ApiKey { get; set; }
      public string ApiSecret { get; set; }
      public string ApiUrl { get; set; } = PinataClient.Endpoint;

      public void EnsureValid()
      {
      }
   }

   public interface IPinataClient
   {
      IDataEndpoint Data { get; }
      IPinningEndpoint Pinning { get; }
   }

   public partial class PinataClient : FlurlClient, IPinataClient
   {
      public const string Endpoint = "https://api.pinata.cloud";

      public Config Config { get; }

      internal static readonly string UserAgent =
         $"{AssemblyVersionInformation.AssemblyProduct}/{AssemblyVersionInformation.AssemblyVersion} ({AssemblyVersionInformation.AssemblyTitle}; {AssemblyVersionInformation.AssemblyDescription})";

      public PinataClient(Config config = null)
      {
         this.Config = config ?? new Config();
         this.Config.EnsureValid();
         this.ConfigureClient();
      }

      protected internal virtual void ConfigureClient()
      {
         this.WithHeader("User-Agent", UserAgent);

         if (!string.IsNullOrWhiteSpace(this.Config.ApiKey))
         {
            this.Configure(ApiKeyAuth);
         }
      }

      private void ApiKeyAuth(ClientFlurlHttpSettings settings)
      {
         async Task SetHeaders(HttpCall http)
         {
            http.FlurlRequest
               .WithHeader(PinataApiKey, this.Config.ApiKey)
               .WithHeader(PinataSecretApiKey, this.Config.ApiSecret);
         }

         settings.BeforeCallAsync = SetHeaders;
      }

      /// <summary>
      /// Enable HTTP debugging via Fiddler. Ensure Tools > Fiddler Options... > Connections is enabled and has a port configured.
      /// Then, call this method with the following URL format: http://localhost.:PORT where PORT is the port number Fiddler proxy
      /// is listening on. (Be sure to include the period after the localhost).
      /// </summary>
      /// <param name="proxyUrl">The full proxy URL Fiddler proxy is listening on. IE: http://localhost.:8888 - The period after localhost is important to include.</param>
      public PinataClient EnableFiddlerDebugProxy(string proxyUrl)
      {
         var webProxy = new WebProxy(proxyUrl, BypassOnLocal: false);

         this.Configure(cf =>
            {
               cf.HttpClientFactory = new DebugProxyFactory(webProxy);
            });

         return this;
      }

      private class DebugProxyFactory : DefaultHttpClientFactory
      {
         private readonly WebProxy proxy;

         public DebugProxyFactory(WebProxy proxy)
         {
            this.proxy = proxy;
         }

         public override HttpMessageHandler CreateMessageHandler()
         {
            return new HttpClientHandler
               {
                  Proxy = this.proxy,
                  UseProxy = true
               };
         }
      }
   }






}
