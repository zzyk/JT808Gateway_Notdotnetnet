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

            //ע������
            var currentDirectory = Directory.GetCurrentDirectory();
            var configuration = new ConfigurationBuilder()
               .SetBasePath(currentDirectory)
               .AddJsonFile("appsettings.json")
               .Build();

            //ʹ�� AbpApplicationFactory ����һ��Ӧ��
            var app = AbpApplicationFactory.Create<JT808ServerAppModule>();
            
            app.Services.ReplaceConfiguration(configuration);
            app.Services.AddMediatR(config => config.Using<DefaultMediator>(), typeof(Program));
            GlobalContext.Configuration = configuration;
            //ע��NLog��־��¼
            app.Services.AddLogging(builder =>
            {
                builder.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
            });
            
            // ��ʼ��Ӧ��
            app.Initialize();

            GlobalContext.ServiceProvider = app.ServiceProvider;
            var form = app.ServiceProvider.GetService<MainForm>();
            System.Windows.Forms.Application.Run(form);
        }
    }
}