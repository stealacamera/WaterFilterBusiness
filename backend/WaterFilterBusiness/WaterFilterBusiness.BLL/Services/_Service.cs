using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services;

public abstract class Service
{
    protected readonly IWorkUnit _workUnit;

    public Service(IWorkUnit workUnit)
    {
        _workUnit = workUnit;
    }
}
