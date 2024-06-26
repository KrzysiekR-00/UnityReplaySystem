using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReplaySystem
{
    [RequireComponent(typeof(ReplayableObject))]
    public abstract class Tracker : MonoBehaviour
    {
        internal uint TrackerId { get; private set; }

        internal int InitializationOffsetInFrames => _initializationOffsetInFrames;

        protected string _parentReplayableId = string.Empty;

        [SerializeField]
        private int _initializationOffsetInFrames = 0;

        internal void SetParentReplayableId(uint trackerId, string parentReplayableId)
        {
            TrackerId = trackerId;
            _parentReplayableId = parentReplayableId;
        }

        internal abstract void PrepareForReplayRecording();
        internal abstract IEnumerable<ReplayCommand> GetCommandsToRecord();
        internal abstract void DoCommand(ReplayCommand command, ReplayPlayer replayPlayer);
        internal abstract void UndoCommand(ReplayCommand command, ReplayPlayer replayPlayer);

        protected abstract void Initialize();

        private bool _isInitialized = false;

        private void OnEnable()
        {
            if (_isInitialized) return;

            StartCoroutine(Initialization());
        }

        private IEnumerator Initialization()
        {
            for (int i = 0; i < InitializationOffsetInFrames; i++)
            {
                yield return new WaitForEndOfFrame();
            }

            Initialize();

            _isInitialized = true;
        }
    }
}
