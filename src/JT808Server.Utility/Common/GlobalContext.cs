using Microsoft.Extensions.Configuration;

namespace JT808Server.Utility.Common
{
    public class GlobalContext
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static IConfiguration Configuration { get; set; }
    }
}
