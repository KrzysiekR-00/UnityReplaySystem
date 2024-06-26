using System.Runtime.Serialization;

namespace ReplaySystem
{
    [DataContract]
    internal class DestroyObject : ReplayableObjectCommand
    {
        internal DestroyObject(string objectId) : base(objectId)
        {

        }

        public override void Do(ReplayPlayer replayPlayer)
        {
            var replayableObject = replayPlayer.ReplayableObjectCollection.GetReplayableObjectById(ReplayableObjectId);
            if (replayableObject == null) return;
            replayableObject.GetRootGameObject().SetActive(false);
        }

        public override void Undo(ReplayPlayer replayPlayer)
        {
            var replayableObject = replayPlayer.ReplayableObjectCollection.GetReplayableObjectById(ReplayableObjectId);
            if (replayableObject == null) return;
            replayableObject.GetRootGameObject().SetActive(true);
        }
    }
}
