using System.Runtime.Serialization;

namespace ReplaySystem
{
    [DataContract]
    internal class ChangeAudioSourceState : TrackerCommand
    {
        [DataMember]
        internal bool IsPlaying { get; private set; } = false;
        [DataMember]
        internal string AudioClipName { get; private set; } = string.Empty;
        [DataMember]
        internal bool Loop { get; private set; } = false;
        [DataMember]
        internal float Volume { get; private set; } = 0;

        internal ChangeAudioSourceState(string objectId, uint trackerId, bool isPlaying, string audioClipName, bool loop, float volume)
            : base(objectId, trackerId)
        {
            IsPlaying = isPlaying;
            AudioClipName = audioClipName;
            Loop = loop;
            Volume = volume;
        }

        public override void Do(ReplayPlayer replayPlayer)
        {
            var replayableObject = replayPlayer.ReplayableObjectCollection.GetReplayableObjectById(ReplayableObjectId);
            if (replayableObject == null) return;

            replayableObject.GetTrackerById(TrackerId).DoCommand(this, replayPlayer);
        }

        public override void Undo(ReplayPlayer replayPlayer)
        {

        }
    }
}
