using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyNUnit;
using VerifyTests;

namespace Pinata.Client.Tests
{
   public class HasVerifyTest
   {
      private VerifySettings jsonFileExtension;

      public HasVerifyTest()
      {
         this.jsonFileExtension = new VerifySettings();
         this.jsonFileExtension.UseExtension("json");
      }
      public Task Verify<T>(T t, [CallerFilePath] string sourceFile = "")
      {
         return Verifier.Verify(t, this.jsonFileExtension, sourceFile);
      }
   }
}
