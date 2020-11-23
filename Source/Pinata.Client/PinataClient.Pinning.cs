namespace Pinata.Client
{
   public interface IPinningEndpoint
   {
   }

   public partial class PinataClient : IPinningEndpoint
   {
      public IPinningEndpoint Pinning => this;

   }
}
