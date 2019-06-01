using System;

namespace Com.Ctrip.Framework.Apollo.Core.Schedule
{
    public class ExponentialSchedulePolicy : ISchedulePolicy
    {
        private readonly int _delayTimeLowerBound;
        private readonly int _delayTimeUpperBound;
        private int _lastDelayTime;

        public ExponentialSchedulePolicy(int delayTimeLowerBound, int delayTimeUpperBound)
        {
            _delayTimeLowerBound = delayTimeLowerBound;
            _delayTimeUpperBound = delayTimeUpperBound;
        }

        public int Fail()
        {
            var delayTime = _lastDelayTime;

            if (delayTime == 0)
            {
                delayTime = _delayTimeLowerBound;
            }
            else
            {
                delayTime = Math.Min(_lastDelayTime << 1, _delayTimeUpperBound);
            }

            _lastDelayTime = delayTime;

            return delayTime;
        }

        public void Success() => _lastDelayTime = 0;
    }
}
