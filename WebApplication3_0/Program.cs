using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace WebApplication3_0
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
           // ��С����־�������
           .MinimumLevel.Information()
           // ��־�����������ռ������ Microsoft ��ͷ��������־�����С����Ϊ Information
           .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
           .Enrich.FromLogContext()
           // ������־���������̨
           .WriteTo.Console()
           // ������־������ļ����ļ��������ǰ��Ŀ�� logs Ŀ¼��
           // �ռǵ���������Ϊÿ��
           //.Filter.ByIncludingOnly((e) =>e.Level == LogEventLevel.Error)
           //.WriteTo.File(Path.Combine("logs", @"logError.txt"), rollingInterval: RollingInterval.Day)
          .WriteTo.Logger(lc=> {
              lc.Filter.ByExcluding(e => e.Level == LogEventLevel.Error || e.Level == LogEventLevel.Debug)
              .WriteTo.File("logs/normal/log.txt",
                  rollingInterval: RollingInterval.Day,
                  rollOnFileSizeLimit: true);
          })
           .WriteTo.Logger(lc => {
               lc.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
               .WriteTo.File("logs/errors/log.txt",
               outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {SourceContext} {NewLine}{Message}{NewLine}{Exception}",
                   rollingInterval: RollingInterval.Day,
                   rollOnFileSizeLimit: true);
           })
            .WriteTo.Logger(lc => {
                lc.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug)
                .WriteTo.File("logs/debugs/log.txt",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true);
            })
           // ���� logger
           .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
           .UseSerilog()
            .UseServiceProviderFactory(new JackAspNetCoreServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
}
