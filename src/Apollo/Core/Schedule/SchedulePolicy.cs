namespace Com.Ctrip.Framework.Apollo.Core.Schedule;

internal interface ISchedulePolicy
{
    int Fail();

    void Success();
}
