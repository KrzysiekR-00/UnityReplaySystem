using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ReplaySystem
{
    [DisallowMultipleComponent]
    public class ReplayableObject : MonoBehaviour
    {
        internal UnityEvent OnReplayStart => _onReplayStart;

        [SerializeField]
        private GameObject _rootGameObject;

        [SerializeField]
        private UnityEvent _onReplayStart;

        [SerializeField]
        private string _id = string.Empty;

        private Tracker[] _trackers;

        internal string Id
        {
            get => _id;
            set
            {
                _id = value;
            }
        }

        internal string PrefabName { get; set; } = string.Empty;

        internal SpawnObject GetSpawnObjectCommand()
        {
            return new SpawnObject(this);
        }

        internal DestroyObject GetDestroyObjectCommand()
        {
            return new DestroyObject(_id);
        }

        internal IEnumerable<ReplayCommand> GetCommandsToRecord()
        {
            IEnumerable<ReplayCommand> commands = Enumerable.Empty<ReplayCommand>();
            foreach (var tracker in _trackers)
            {
                commands = commands.Concat(tracker.GetCommandsToRecord());
            }
            return commands;
        }

        internal void PrepareForReplayRecording()
        {
            foreach (var tracker in _trackers)
            {
                tracker.PrepareForReplayRecording();
            }
        }

        internal void PrepareForReplayPlaying()
        {
            _onReplayStart?.Invoke();
        }

        internal Tracker GetTrackerById(uint trackerId)
        {
            return _trackers.Where(t => t.TrackerId == trackerId).FirstOrDefault();
        }

        internal GameObject GetRootGameObject()
        {
            return _rootGameObject;
        }

        private void Reset()
        {
            _id = GetId();
        }

        private string GetId()
        {
            return Guid.NewGuid().ToString();
        }

        private void Awake()
        {
            if (ReplayableObjectCollection.Instance == null)
            {
                ReplayableObjectCollection.CollectionInitialized += StartInitializationCoroutine;
            }
            else
            {
                StartInitializationCoroutine(ReplayableObjectCollection.Instance);
            }
        }

        private void StartInitializationCoroutine(ReplayableObjectCollection replayableObjectCollection)
        {
            replayableObjectCollection.StartCoroutine(Initialization());
        }

        private IEnumerator Initialization()
        {
            _trackers = GetComponents<Tracker>();
            yield return WaitTrackersMaxInitializationOffset();

            if (ReplayableObjectCollection.Instance.GetReplayableObjectById(_id) != null)
            {
                _id = GetId();
            }

            if (_rootGameObject != null) PrefabName = _rootGameObject.name;

            uint i = 0;
            foreach (var tracker in _trackers)
            {
                tracker.SetParentReplayableId(i++, _id);
            }

            AddToReplayableObjectCollection();
        }

        private IEnumerator WaitTrackersMaxInitializationOffset()
        {
            int initializationOffset = 0;

            if (_trackers.Length > 0) initializationOffset = _trackers.Max(t => t.InitializationOffsetInFrames);

            for (int j = 0; j < initializationOffset; j++)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        private void AddToReplayableObjectCollection()
        {
            if (this != null && transform != null) ReplayableObjectCollection.Instance.Register(this);
        }

        private void OnDestroy()
        {
            if (ReplayableObjectCollection.Instance != null)
            {
                ReplayableObjectCollection.Instance.Unregister(this);
            }

            try
            {
                ReplayableObjectCollection.CollectionInitialized -= StartInitializationCoroutine;
            }
            catch { }
        }
    }
}
