using AntDesign;
using Microsoft.AspNetCore.Components;
using Recipes.Features.Materials.GetById;
using Recipes.Web.Services;

namespace Recipes.Web.Pages.Materials;

public partial class MaterialsIndexPage
{
    [Inject]
    public IMaterialsService _materialsService { get; set; }
    [Inject]
    public NavigationManager _navManager { get; set; }

    private IEnumerable<MaterialGetResponse> materials { get; set; } = new List<MaterialGetResponse>();

    private string searchTerm = string.Empty;

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

    protected override async Task OnInitializedAsync() => materials = await _materialsService.Get();

    private IEnumerable<string> materialNames => materials.Select(x => x.Name);

    private IEnumerable<MaterialGetResponse> filteredMaterials =>
        string.IsNullOrEmpty(searchTerm) ?
            materials :
            materials.Where(x => x.Name.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase));
}
