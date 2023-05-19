using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Recipes.Features.Materials.Create;
using Recipes.Features.Materials.GetById;
using Recipes.Web.Services;

namespace Recipes.Web.Pages.Materials;

public partial class MaterialsCreatePage
{
    [Inject]
    public IMaterialsService _materialsService { get; set; }
    [Inject]
    public IMessageService _message { get; set; }
    [Inject]
    public NavigationManager _navManager { get; set; }

    private Form<MaterialCreateRequest> _form;

    private MaterialCreateRequest material = new MaterialCreateRequest();

    private IEnumerable<MaterialGetResponse> materials { get; set; } = new List<MaterialGetResponse>();

    private bool submitDisabled;

    protected override async Task OnInitializedAsync() => materials = await _materialsService.Get();

    private async Task OnFinish(EditContext editContext)
    {
        submitDisabled = true;
        var result = await _materialsService.Add(material);
        if (result.Valid)
        {
            await _message.Success(result.Message);
            _navManager.NavigateTo("/Materials");
        }
        else
        {
            var errors = result.Message.Split('\n');
            var tasks = new List<Task>();
            foreach (var error in errors)
            {
                tasks.Add(_message.Error(error));
            }
            await Task.WhenAll(tasks);
        }
        submitDisabled = false;
    }
}
