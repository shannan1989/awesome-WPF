using DotnetSpider;
using DotnetSpider.DataFlow;
using DotnetSpider.Downloader;
using DotnetSpider.Http;
using DotnetSpider.Scheduler;
using DotnetSpider.Scheduler.Component;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace AnnSpider.VolleyChina
{
    internal class VolleyChinaSpider : Spider
    {
        public VolleyChinaSpider(IOptions<SpiderOptions> options, DependenceServices services, ILogger<Spider> logger) : base(options, services, logger)
        {
        }

        protected override async Task InitializeAsync(CancellationToken stoppingToken = default)
        {
            //添加自定义解析
            AddDataFlow(new VolleyChinaDataParser());

            //使用控制台存储器
            AddDataFlow(new ConsoleStorage());

            //添加采集请求:博客园10天推荐排行榜
            await AddRequestsAsync(new Request("http://www.volleychina.org/allnews/index.html")
            {
                Timeout = 10000 //请求超时10秒
            });
        }

        internal static async Task RunAsync()
        {
            var builder = Builder.CreateDefaultBuilder<VolleyChinaSpider>();
            builder.UseSerilog();
            builder.UseDownloader<HttpClientDownloader>();
            builder.UseQueueDistinctBfsScheduler<HashSetDuplicateRemover>();

            await builder.Build().RunAsync();
        }
    }
}
