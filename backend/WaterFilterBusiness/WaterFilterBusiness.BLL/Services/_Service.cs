using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services;

public abstract class _Service
{
    protected readonly IWorkUnit _workUnit;

    public _Service(IWorkUnit workUnit)
    {
        _workUnit = workUnit;
    }
}
