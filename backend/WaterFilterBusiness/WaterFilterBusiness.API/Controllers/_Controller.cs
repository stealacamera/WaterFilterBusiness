using FluentResults;
using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;

namespace WaterFilterBusiness.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class Controller : ControllerBase
    {
        protected readonly IServicesManager _servicesManager;

        public Controller(IServicesManager servicesManager) 
            => _servicesManager = servicesManager;

        protected IActionResult GetResult<T>(Result<T> result, IActionResult onSuccess)
        {
            return result.IsFailed
                   ? BadRequest(result)
                   : onSuccess;
        }
    }
}
