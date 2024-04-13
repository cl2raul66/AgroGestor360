using AgroGestor360.Client.Models;

namespace AgroGestor360.App.Tools.DataTemplateSelectors;

public class ArticlesTemplateSelector : DataTemplateSelector
{
    public DataTemplate ArticlePriceZeroTemplate { get; set; }
    public DataTemplate ArticlePriceNonZeroTemplate { get; set; }

    public ArticlesTemplateSelector()
    {
        ArticlePriceZeroTemplate = new();
        ArticlePriceNonZeroTemplate = new();
    }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return ((Article)item).Price == 0 ? ArticlePriceZeroTemplate : ArticlePriceNonZeroTemplate;
    }
}
