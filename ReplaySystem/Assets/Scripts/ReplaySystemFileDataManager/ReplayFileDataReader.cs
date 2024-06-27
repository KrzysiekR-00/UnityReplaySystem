using ReplaySystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReplaySystemFileDataManager
{
    public class ReplayFileDataReader : IReplayDataReader
    {
        private readonly ReplayCommand[] _cachedCommands;

        public ReplayFileDataReader(string filePath)
        {
            _cachedCommands = ReadCommandsFromFile(filePath).ToArray();
        }

        public TimeSpan GetReplayLength()
        {
            return _cachedCommands[^1].TimeStamp;
        }

        public ReplayCommand[] ReadCommandsBetweenTimeStamps(TimeSpan from, TimeSpan to)
        {
            return _cachedCommands.Where(c => c.TimeStamp >= from && c.TimeStamp <= to).ToArray();
        }

        private IEnumerable<ReplayCommand> ReadCommandsFromFile(string filePath)
        {
            var lines = File.ReadLines(filePath);
            foreach (var line in lines) yield return line.DeserializeFromString<ReplayCommand>();
        }
    }
}
