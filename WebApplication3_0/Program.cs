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
           // 最小的日志输出级别
           .MinimumLevel.Information()
           // 日志调用类命名空间如果以 Microsoft 开头，覆盖日志输出最小级别为 Information
           .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
           .Enrich.FromLogContext()
           // 配置日志输出到控制台
           .WriteTo.Console()
           // 配置日志输出到文件，文件输出到当前项目的 logs 目录下
           // 日记的生成周期为每天
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
           // 创建 logger
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
