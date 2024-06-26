using System;

namespace ReplaySystem
{
    public interface IReplayDataLoader
    {
        void Open();
        TimeSpan GetExerciseEndTimeStamp();
        ReplayCommand[] GetCommandsBetweenTimeStamps(TimeSpan from, TimeSpan to);
    }
}
