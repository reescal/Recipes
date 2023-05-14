using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Recipes.Api;
public interface IHttpFunctionExecutor
{
    Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> func);
}
