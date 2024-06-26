using System.Runtime.Serialization;
using UnityEngine;

namespace ReplaySystem
{
    [DataContract]
    internal class ChangePosition : TransformCommand
    {
        [DataMember]
        internal Vector3 PreviousPosition { get; private set; } = Vector3.zero;
        [DataMember]
        internal Vector3 CurrentPosition { get; private set; } = Vector3.zero;

        internal bool Changed
        {
            get { return PreviousPosition != CurrentPosition; }
        }

        internal ChangePosition(string objectId, uint trackerId, uint transformId, Vector3 previousPosition, Vector3 currentPosition)
            : base(objectId, trackerId, transformId)
        {
            PreviousPosition = previousPosition;
            CurrentPosition = currentPosition;
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
