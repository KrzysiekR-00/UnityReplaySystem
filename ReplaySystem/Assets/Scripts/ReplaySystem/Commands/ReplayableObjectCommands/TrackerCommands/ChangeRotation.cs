using System.Runtime.Serialization;
using UnityEngine;

namespace ReplaySystem
{
    [DataContract]
    internal class ChangeRotation : TransformCommand
    {
        [DataMember]
        internal Quaternion PreviousRotation { get; private set; } = Quaternion.identity;
        [DataMember]
        internal Quaternion CurrentRotation { get; private set; } = Quaternion.identity;

        internal bool Changed
        {
            get { return !PreviousRotation.Equals(CurrentRotation); }
        }

        internal ChangeRotation(string objectId, uint trackerId, uint transformId, Quaternion previousRotation, Quaternion currentRotation)
            : base(objectId, trackerId, transformId)
        {
            PreviousRotation = previousRotation;
            CurrentRotation = currentRotation;
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
