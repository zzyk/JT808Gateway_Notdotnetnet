using JT808Server.Application;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace JT808Server.WinForm
{
    [DependsOn(
       typeof(AbpAutofacModule),
       typeof(JT808ServerApplicationModule))]
    public class JT808ServerAppModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddSingleton(typeof(MainForm));
        }
    }
}
