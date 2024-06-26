using ReplaySystem;
using System;
using System.IO;

namespace ReplaySystemFileDataManager
{
    public class FileReplayDataRecorder : IReplayDataRecorder
    {
        private readonly string _filePath;

        public FileReplayDataRecorder(string filePath)
        {
            _filePath = filePath;
        }

        public void Open()
        {
            File.WriteAllText(_filePath, string.Empty);
        }

        public void RecordCommand(ReplayCommand commandToRecord)
        {
            File.AppendAllText(_filePath, commandToRecord.SerializeToString() + Environment.NewLine);
        }

        public void Close()
        {
            
        }
    }
}
