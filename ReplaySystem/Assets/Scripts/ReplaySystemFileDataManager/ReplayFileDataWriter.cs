using ReplaySystem;
using System;
using System.IO;

namespace ReplaySystemFileDataManager
{
    public class ReplayFileDataWriter : IReplayDataWriter
    {
        private readonly string _filePath;

        public ReplayFileDataWriter(string filePath)
        {
            _filePath = filePath;

            CreateEmptyFile(filePath);
        }

        public void WriteCommand(ReplayCommand commandToWrite)
        {
            File.AppendAllText(_filePath, commandToWrite.SerializeToString() + Environment.NewLine);
        }

        private void CreateEmptyFile(string filePath)
        {
            File.WriteAllText(filePath, string.Empty);
        }
    }
}
