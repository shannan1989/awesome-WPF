using DotnetSpider;
using DotnetSpider.DataFlow;
using DotnetSpider.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace AnnSpider.VolleyChina
{
    internal class VolleyChinaSpider : Spider
    {
        public VolleyChinaSpider(IOptions<SpiderOptions> options, DependenceServices services, ILogger<VolleyChinaSpider> logger) : base(options, services, logger)
        {
        }

        protected override async Task InitializeAsync(CancellationToken stoppingToken = default)
        {
            //添加自定义解析
            AddDataFlow<VolleyChinaDataParser>();

            //使用控制台存储器
            AddDataFlow<ConsoleStorage>();

            //添加采集请求:博客园10天推荐排行榜
            await AddRequestsAsync(new Request("http://www.volleychina.org/allnews/index.html")
            {
                Timeout = 10000 //请求超时10秒
            });
        }

        internal static async Task RunAsync()
        {
            var builder = Builder.CreateDefaultBuilder<VolleyChinaSpider>(x =>
            {
                x.Speed = 5;
            });
            builder.UseSerilog();

            await builder.Build().RunAsync();
        }
    }
}
