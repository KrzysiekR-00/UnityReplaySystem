using System.Collections.Generic;
using UnityEngine;

namespace ReplaySystem
{
    internal class ParticleTracker : Tracker
    {
        [SerializeField]
        private ParticleSystem _particleSystemToTrack;

        private bool _isPlaying = false;

        internal override void PrepareForReplayRecording()
        {

        }

        internal override IEnumerable<ReplayCommand> GetCommandsToRecord()
        {
            if (PlayingParticlesystemChangeDetected()) yield return GetPlayParticleCommand(_parentReplayableId);
        }

        internal override void DoCommand(ReplayCommand command, ReplayPlayer replayPlayer)
        {
            if (command is PlayParticle)
            {
                _particleSystemToTrack.Play();
            }
        }

        internal override void UndoCommand(ReplayCommand command, ReplayPlayer replayPlayer)
        {

        }

        protected override void Initialize()
        {

        }

        private bool PlayingParticlesystemChangeDetected()
        {
            var previousIsPlaying = _isPlaying;

            _isPlaying = _particleSystemToTrack.isPlaying;

            if (!_isPlaying) return false;
            if (_isPlaying == previousIsPlaying) return false;

            return true;
        }

        private PlayParticle GetPlayParticleCommand(string objectId)
        {
            return new PlayParticle(objectId, TrackerId);
        }
    }
}
