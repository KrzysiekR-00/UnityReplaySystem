using System.Collections.Generic;
using UnityEngine;

namespace ReplaySystem
{
    internal class ActivationTracker : Tracker
    {
        [SerializeField]
        private GameObject _gameObjectToTrack;

        private bool _isStarted = false;
        private bool? _wasActiveLastTime = null;

        internal override void PrepareForReplayRecording()
        {

        }

        internal override IEnumerable<ReplayCommand> GetCommandsToRecord()
        {
            if (_isStarted)
            {
                if (_wasActiveLastTime == null || _wasActiveLastTime != _gameObjectToTrack.activeSelf)
                {
                    _wasActiveLastTime = _gameObjectToTrack.activeSelf;
                    yield return new ChangeIsActive(_parentReplayableId, TrackerId, _gameObjectToTrack.activeSelf);
                }
            }
        }

        internal override void DoCommand(ReplayCommand command, ReplayPlayer replayPlayer)
        {
            if (command is ChangeIsActive changeIsActive)
            {
                _gameObjectToTrack.SetActive(changeIsActive.IsActive);
            }
        }

        internal override void UndoCommand(ReplayCommand command, ReplayPlayer replayPlayer)
        {
            if (command is ChangeIsActive changeIsActive)
            {
                _gameObjectToTrack.SetActive(!changeIsActive.IsActive);
            }
        }

        protected override void Initialize()
        {
            _isStarted = true;
        }
    }
}
