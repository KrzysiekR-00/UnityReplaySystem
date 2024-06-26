using System.Runtime.Serialization;
using UnityEngine;

namespace ReplaySystem
{
    [DataContract]
    internal class SpawnObject : ReplayableObjectCommand
    {
        [DataMember]
        internal string PrefabName { get; private set; } = string.Empty;
        [DataMember]
        internal Vector3 StartPosition { get; private set; } = Vector3.zero;
        [DataMember]
        internal Quaternion StartRotation { get; private set; } = Quaternion.identity;

        internal SpawnObject(ReplayableObject replayableObject) : base(replayableObject.Id)
        {
            PrefabName = replayableObject.PrefabName;
            StartPosition = replayableObject.GetRootGameObject().transform.position;
            StartRotation = replayableObject.GetRootGameObject().transform.rotation;
        }

        public override void Do(ReplayPlayer replayPlayer)
        {
            var replayableObject = replayPlayer.ReplayableObjectCollection.GetReplayableObjectById(ReplayableObjectId);
            if (replayableObject != null)
            {
                replayableObject.GetRootGameObject().SetActive(true);
                return;
            }

            if (string.IsNullOrEmpty(PrefabName)) return;

            var prefabToSpawn = replayPlayer.PrefabsCollection.GetPrefabByName(PrefabName);
            if (prefabToSpawn == null) return;

            var parent = new GameObject("ReplaySystemSpawnedObjectParent");
            var spawnedObject = Object.Instantiate(prefabToSpawn, parent.transform);
            spawnedObject.transform.SetPositionAndRotation(StartPosition, StartRotation);
            spawnedObject.GetComponentInChildren<ReplayableObject>().Id = ReplayableObjectId;
            spawnedObject.GetComponentInChildren<ReplayableObject>().OnReplayStart?.Invoke();
        }

        public override void Undo(ReplayPlayer replayPlayer)
        {
            var replayableObject = replayPlayer.ReplayableObjectCollection.GetReplayableObjectById(ReplayableObjectId);
            if (replayableObject == null) return;
            replayableObject.GetRootGameObject().SetActive(false);
        }
    }
}
