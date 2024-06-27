using System;

namespace ReplaySystem
{
    public interface IReplayDataReader
    {
        TimeSpan GetReplayLength();
        ReplayCommand[] ReadCommandsBetweenTimeStamps(TimeSpan from, TimeSpan to);
    }
}
