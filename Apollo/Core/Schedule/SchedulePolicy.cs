namespace Com.Ctrip.Framework.Apollo.Core.Schedule
{
    public interface SchedulePolicy
    {
        int Fail();

        void Success();
    }
}
