namespace Com.Ctrip.Framework.Apollo.Core.Schedule
{
    public interface ISchedulePolicy
    {
        int Fail();

        void Success();
    }
}
