using AntDesign;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Recipes.Data.Entities;
using Recipes.Features.Materials.GetById;
using Recipes.Features.Materials.Update;
using Recipes.Web.Services;
using static Recipes.Shared.Constants.Responses;

namespace Recipes.Web.Pages.Materials;

public partial class MaterialsUpdatePage
{
    [Inject]
    public IMaterialsService _materialsService { get; set; }
    [Inject]
    public IMessageService _message { get; set; }
    [Inject]
    public NavigationManager _navManager { get; set; }
    [Parameter]
    public Guid Id { get; set; }
    private Form<MaterialUpdateRequest> _form;

    private MaterialUpdateRequest materialUpdate = new MaterialUpdateRequest();

    private IEnumerable<MaterialGetResponse> materials { get; set; } = new List<MaterialGetResponse>();
    private MaterialGetResponse material { get; set; } = new();

    private bool submitDisabled;


    protected override async Task OnInitializedAsync() => materials = await _materialsService.Get();

    protected override async Task OnParametersSetAsync()
    {
        material = await _materialsService.Get(Id);
        if (material == null)
        {
            await _message.Error(NotFound(nameof(Material), Id));
            _navManager.NavigateTo("/Materials");
        }
        materialUpdate = new MaterialUpdateRequest
        {
            Id = material.Id,
            Name = material.Name,
            Description = material.Description,
            Image = material.Image
        };
    }

    private async Task OnFinish(EditContext editContext)
    {
        submitDisabled = true;
        var result = await _materialsService.Update(materialUpdate);
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
