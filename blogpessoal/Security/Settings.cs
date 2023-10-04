using Microsoft.AspNetCore.DataProtection;

namespace blogpessoal.Security
{
    public class Settings
    {
        private static string secret = "cefc7e69520261034d713e8f3fac5a556f6dae0c6e0bc21f49b52f2237dd980a";

        public static string Secret { get => secret; set => secret = value;}
    }
}
