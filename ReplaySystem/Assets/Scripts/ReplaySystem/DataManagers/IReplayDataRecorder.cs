namespace ReplaySystem
{
    public interface IReplayDataRecorder
    {
        void Open();
        void RecordCommand(ReplayCommand commandToRecord);
        void Close();
    }
}
