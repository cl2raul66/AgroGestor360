using AgroGestor360Client.Models;

namespace AgroGestor360App.Tools.DataTemplateSelectors;

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
        return ((DTO3_1)item).Price == 0 ? ArticlePriceZeroTemplate : ArticlePriceNonZeroTemplate;
    }
}
