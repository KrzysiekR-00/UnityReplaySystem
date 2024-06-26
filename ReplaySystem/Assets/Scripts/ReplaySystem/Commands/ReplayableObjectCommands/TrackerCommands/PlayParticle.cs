using System.Runtime.Serialization;

namespace ReplaySystem
{
    [DataContract]
    internal class PlayParticle : TrackerCommand
    {
        internal PlayParticle(string objectId, uint trackerId) : base(objectId, trackerId)
        {

        }

        public override void Do(ReplayPlayer replayPlayer)
        {
            var replayableObject = replayPlayer.ReplayableObjectCollection.GetReplayableObjectById(ReplayableObjectId);
            if (replayableObject == null) return;

            replayableObject.GetTrackerById(TrackerId).DoCommand(this, replayPlayer);
        }

        public override void Undo(ReplayPlayer replayPlayer)
        {

        }
    }
}
