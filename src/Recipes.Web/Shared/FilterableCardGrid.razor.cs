using AntDesign;
using Microsoft.AspNetCore.Components;
using Recipes.Shared.Enums;
using Recipes.Shared.Models;

namespace Recipes.Web.Shared;

public partial class FilterableCardGrid<TItem> where TItem : ComplexEntity
{
    [CascadingParameter]
    protected Lang Lang { get; set; }

    [Parameter]
    public IEnumerable<TItem> Entities { get; set; }
    [Parameter]
    public EventCallback CreateAction { get; set; }
    [Parameter]
    public string Route { get; set; }

    private IOrderedEnumerable<SimpleEntity> entities => Entities
                                                                .Select(e => e.ToSimpleEntity((int)Lang))
                                                                .OrderBy(x => x.Properties.Type)
                                                                .ThenBy(x => x.Properties.Name);

    private IEnumerable<IGrouping<string, SimpleEntity>> filteredEntities =>
        string.IsNullOrEmpty(input) ?
        entities.GroupBy(x => x.Properties.Type) :
        entities
            .Where(x => (x.Properties.Type + x.Properties.Name).Contains(input, StringComparison.InvariantCultureIgnoreCase))
            .GroupBy(x => x.Properties.Type);

    private string input = string.Empty;

    private ListGridType grid = new()
    {
        Gutter = 16,
        Xs = 1,
        Sm = 2,
        Md = 4,
        Lg = 4,
        Xl = 6,
        Xxl = 3,
    };

    void OnInput(ChangeEventArgs e) => input = e.Value.ToString();

    async Task OnSelectionChange(AutoCompleteOption item)
    {
        var selectedEntity = item.Value is SimpleEntity o ? o : null;
        if (selectedEntity != null)
        {
            input = selectedEntity.Properties.Name;
            NavigateTo(selectedEntity.Id);
        }
    }

    private void NavigateTo(Guid id) => NavigationManager.NavigateTo(Route + "/" + id);
}
