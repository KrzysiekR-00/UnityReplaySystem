using ReplaySystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReplaySystemFileDataManager
{
    public class FileReplayDataLoader : IReplayDataLoader
    {
        private readonly string _filePath;

        private ReplayCommand[] _loadedCommands;

        public FileReplayDataLoader(string filePath)
        {
            _filePath = filePath;
        }

        public void Open()
        {
            static IEnumerable<ReplayCommand> GetCommands(string filePath)
            {
                var lines = File.ReadLines(filePath);
                foreach(var line in lines) yield return line.DeserializeFromString<ReplayCommand>();
            }

            _loadedCommands = GetCommands(_filePath).ToArray();
        }

        public TimeSpan GetExerciseEndTimeStamp()
        {
            return _loadedCommands[^1].TimeStamp;
        }

        public ReplayCommand[] GetCommandsBetweenTimeStamps(TimeSpan from, TimeSpan to)
        {
            return _loadedCommands.Where(c => c.TimeStamp >= from && c.TimeStamp <= to).ToArray();
        }
    }
}
