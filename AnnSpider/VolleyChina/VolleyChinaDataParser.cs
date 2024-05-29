using DotnetSpider.DataFlow;
using DotnetSpider.DataFlow.Parser;
using DotnetSpider.Selector;

namespace AnnSpider.VolleyChina
{
    internal class VolleyChinaDataParser : DataParser
    {
        public override Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        protected override Task ParseAsync(DataFlowContext context)
        {
            var newsList = context.Selectable.SelectList(Selectors.XPath(".//li[@class='new-detail']"));
            foreach (var item in newsList)
            {
                var title = item.Select(Selectors.XPath(".//a[@class='new-title']")).Value;
                var url = item.Select(Selectors.XPath(".//a[@class='new-title']/@href")).Value;
            }

            return Task.CompletedTask;
        }
    }
}
