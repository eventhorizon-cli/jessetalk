using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PwdClient
{

    class Program
    {
        static void Main(string[] args)
        {
          var t = Test();
          Console.ReadLine();
        }

        static async Task Test()
        {
            var diso = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (diso.IsError)
            {
                Console.WriteLine(diso.Error);
            }

            var tokenClient = new TokenClient(diso.TokenEndpoint, "pwdClient", "secret");

            var tokenRespoense = await tokenClient.RequestClientCredentialsAsync("api");

            if (tokenRespoense.IsError)
            {
                Console.WriteLine(tokenRespoense.Error);
            }
            else
            {
                Console.WriteLine(tokenRespoense.AccessToken);
            }

            var httpClient = new HttpClient();

            httpClient.SetBearerToken(tokenRespoense.AccessToken);
            var res = await httpClient.GetAsync("http://localhost:5001/api/values");

            Console.WriteLine(await res.Content.ReadAsStringAsync());

        }
    }
}
