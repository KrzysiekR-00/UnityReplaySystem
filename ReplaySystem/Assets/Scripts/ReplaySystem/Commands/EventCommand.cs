using System.Runtime.Serialization;

namespace ReplaySystem
{
    [DataContract]
    public abstract class EventCommand : ReplayCommand
    {
        public string Type => GetType().Name;

        public override void Do(ReplayPlayer replayPlayer)
        {

        }

        public override void Undo(ReplayPlayer replayPlayer)
        {

        }
    }
}
