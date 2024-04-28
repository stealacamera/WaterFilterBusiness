using System.ComponentModel;
using System.Globalization;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.Common.Converters.TypeConverters;

class CallOutcomeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        if (sourceType == typeof(string))
            return true;

        return base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string)
        {
            CallOutcome outcome;

            if (CallOutcome.TryFromName((string)value, out outcome))
                return outcome;
        }

        return base.ConvertFrom(context, culture, value);
    }
}