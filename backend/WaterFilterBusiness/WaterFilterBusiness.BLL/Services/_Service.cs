using Microsoft.IdentityModel.Tokens;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services;

internal abstract class Service
{
    protected readonly IWorkUnit _workUnit;
    protected readonly IUtilityService _utilityService;

    public Service(IWorkUnit workUnit, IUtilityService utilityService)
    {
        _workUnit = workUnit;
        _utilityService = utilityService;
    }

    protected bool IsUpdateEmpty<T>(T model) where T : class
    {
        return model.GetType()
                    .GetProperties()
                    .Select(e => e.GetValue(model))
                    .All(value =>
                    {
                        if (value is string)
                            return ((string)value).IsNullOrEmpty();

                        return value == null;
                    });
    }

    protected bool HasAttributeChanged<T>(T currentAttribute, T newAttribute)
    {
        if(currentAttribute is string)
        {
            var strAttribute = (string)((object)currentAttribute);

            return strAttribute.IsNullOrEmpty() ? false : !strAttribute.Equals(newAttribute);
        }
        
        return currentAttribute == null ? false : currentAttribute.Equals(newAttribute);
    }
}
