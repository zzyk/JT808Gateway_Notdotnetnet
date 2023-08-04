using JT808Server.Utility.Common;
using JT808Server.WinForm.MediatorExpand;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using Volo.Abp;

namespace JT808Server.WinForm
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.SystemAware);
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            //注册配置
            var currentDirectory = Directory.GetCurrentDirectory();
            var configuration = new ConfigurationBuilder()
               .SetBasePath(currentDirectory)
               .AddJsonFile("appsettings.json")
               .Build();

            //使用 AbpApplicationFactory 创建一个应用
            var app = AbpApplicationFactory.Create<JT808ServerAppModule>();
            
            app.Services.ReplaceConfiguration(configuration);
            app.Services.AddMediatR(config => config.Using<DefaultMediator>(), typeof(Program));
            GlobalContext.Configuration = configuration;
            //注册NLog日志记录
            app.Services.AddLogging(builder =>
            {
                builder.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
            });
            
            // 初始化应用
            app.Initialize();

            GlobalContext.ServiceProvider = app.ServiceProvider;
            var form = app.ServiceProvider.GetService<MainForm>();
            System.Windows.Forms.Application.Run(form);
        }
    }
}