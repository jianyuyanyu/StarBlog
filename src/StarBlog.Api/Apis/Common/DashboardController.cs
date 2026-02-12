using CodeLab.Share.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarBlog.Infrastructure.CLRStats;
using StarBlog.Api.Extensions;

namespace StarBlog.Api.Apis.Common;

[Authorize]
[ApiController]
[Route("Api/[controller]")]
[ApiExplorerSettings(GroupName = ApiGroups.Common)]
public class DashboardController : ControllerBase {
    [HttpGet("[action]")]
    public ApiResponse ClrStats() {
        return ApiResponse.Ok(CLRStatsUtils.GetCurrentClrStats());
    }
}
