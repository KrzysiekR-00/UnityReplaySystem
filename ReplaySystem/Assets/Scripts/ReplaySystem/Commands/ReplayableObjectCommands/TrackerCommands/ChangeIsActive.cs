using System.Runtime.Serialization;

namespace ReplaySystem
{
    [DataContract]
    internal class ChangeIsActive : TrackerCommand
    {
        [DataMember]
        internal bool IsActive { get; private set; } = false;

        internal ChangeIsActive(string objectId, uint trackerId, bool isActive) : base(objectId, trackerId)
        {
            IsActive = isActive;
        }

        public override void Do(ReplayPlayer replayPlayer)
        {
            var replayableObject = replayPlayer.ReplayableObjectCollection.GetReplayableObjectById(ReplayableObjectId);
            if (replayableObject == null) return;

            replayableObject.GetTrackerById(TrackerId).DoCommand(this, replayPlayer);
        }

        public override void Undo(ReplayPlayer replayPlayer)
        {
            var replayableObject = replayPlayer.ReplayableObjectCollection.GetReplayableObjectById(ReplayableObjectId);
            if (replayableObject == null) return;

            replayableObject.GetTrackerById(TrackerId).UndoCommand(this, replayPlayer);
        }
    }
}
