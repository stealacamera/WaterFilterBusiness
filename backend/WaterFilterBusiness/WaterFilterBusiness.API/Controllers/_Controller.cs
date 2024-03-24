using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;

namespace WaterFilterBusiness.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class Controller : ControllerBase
    {
        protected readonly IServicesManager _servicesManager;

        public Controller(IServicesManager servicesManager) => _servicesManager = servicesManager;
    }
}
