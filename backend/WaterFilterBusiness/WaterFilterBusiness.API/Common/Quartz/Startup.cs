using Quartz;
using WaterFilterBusiness.API.Common.Quartz.Jobs;

namespace WaterFilterBusiness.API.Common.Quartz;

public static class Startup
{
    public static void RegisterQuartzServices(this IServiceCollection services)
    {
        services.AddQuartz(options =>
        {
            var redlistedCustomersJobKey = JobKey.Create(nameof(RemoveCustomersFromRedlistedJob));
            options.AddJob<RemoveCustomersFromRedlistedJob>(redlistedCustomersJobKey)
                   .AddTrigger(trigger =>
                        trigger.ForJob(redlistedCustomersJobKey)
                               .WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(DayOfWeek.Sunday, 1, 0)));
        });

        services.AddQuartzHostedService();
    }
}
