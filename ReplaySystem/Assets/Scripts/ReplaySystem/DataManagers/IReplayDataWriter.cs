namespace ReplaySystem
{
    public interface IReplayDataWriter
    {
        void WriteCommand(ReplayCommand commandToWrite);
    }
}
