using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ReplaySystem
{
    public class ReplayableObjectCollection : MonoBehaviour
    {
        internal static ReplayableObjectCollection Instance { get; private set; }

        internal static UnityAction<ReplayableObjectCollection> CollectionInitialized;

        internal UnityAction<ReplayableObject> ReplayableObjectRegistered;
        internal UnityAction<ReplayableObject> ReplayableObjectUnregistered;

        private readonly List<ReplayableObject> _replayableObjects = new();

        internal void Register(ReplayableObject replayableObject)
        {
            if (_replayableObjects.Contains(replayableObject)) return;

            ReplayableObjectRegistered?.Invoke(replayableObject);

            _replayableObjects.Add(replayableObject);
        }

        internal void Unregister(ReplayableObject replayableObject)
        {
            if (!_replayableObjects.Contains(replayableObject)) return;

            ReplayableObjectUnregistered?.Invoke(replayableObject);

            _replayableObjects.Remove(replayableObject);
        }

        internal ReplayableObject[] GetReplayableObjects()
        {
            return _replayableObjects.ToArray();
        }

        internal ReplayableObject GetReplayableObjectById(string id)
        {
            return _replayableObjects.Where(r => r.Id == id).FirstOrDefault();
        }

        private void Awake()
        {
            if (Instance != null) Destroy(this);
            else
            {
                Instance = this;

                CollectionInitialized?.Invoke(this);
            }
        }
    }
}