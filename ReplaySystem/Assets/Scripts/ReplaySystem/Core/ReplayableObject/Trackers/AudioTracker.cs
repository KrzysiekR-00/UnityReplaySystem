using System.Collections.Generic;
using UnityEngine;

namespace ReplaySystem
{
    internal class AudioTracker : Tracker
    {
        [SerializeField]
        private AudioSource _audioSourceToTrack;

        private bool _isPlaying = false;
        private string _clip = string.Empty;

        internal override void PrepareForReplayRecording()
        {

        }

        internal override IEnumerable<ReplayCommand> GetCommandsToRecord()
        {
            if (PlayingAudioClipChangeDetected()) yield return GetPlayAudioCommand(_parentReplayableId);
        }

        internal override void DoCommand(ReplayCommand command, ReplayPlayer replayPlayer)
        {
            if (command is ChangeAudioSourceState playAudioClipCommand)
            {
                var audioClipToPlay = replayPlayer.PrefabsCollection.GetAudioClipByName(playAudioClipCommand.AudioClipName);
                _audioSourceToTrack.clip = audioClipToPlay;
                _audioSourceToTrack.pitch = replayPlayer.GetReplaySpeed();
                _audioSourceToTrack.volume = replayPlayer.GetAudioVolume() * playAudioClipCommand.Volume;
                _audioSourceToTrack.loop = playAudioClipCommand.Loop;
                if (playAudioClipCommand.IsPlaying) _audioSourceToTrack.Play();
                else _audioSourceToTrack.Stop();

                replayPlayer.OnReplaySpeedChanged += (float replaySpeed) =>
                {
                    _audioSourceToTrack.pitch = replaySpeed;
                };

                replayPlayer.OnAudioVolumeChanged += (float volume) =>
                {
                    _audioSourceToTrack.volume = playAudioClipCommand.Volume * volume;
                };
            }
        }

        internal override void UndoCommand(ReplayCommand command, ReplayPlayer replayPlayer)
        {

        }

        protected override void Initialize()
        {

        }

        private bool PlayingAudioClipChangeDetected()
        {
            var previousIsPlaying = _isPlaying;
            var previousClip = _clip;

            _isPlaying = _audioSourceToTrack.isPlaying;
            _clip = _audioSourceToTrack.clip?.name;

            if (_isPlaying == previousIsPlaying && _clip == previousClip) return false;

            return true;
        }

        private ChangeAudioSourceState GetPlayAudioCommand(string objectId)
        {
            return new ChangeAudioSourceState(objectId, TrackerId, _audioSourceToTrack.isPlaying, _clip, _audioSourceToTrack.loop, _audioSourceToTrack.volume);
        }
    }
}
