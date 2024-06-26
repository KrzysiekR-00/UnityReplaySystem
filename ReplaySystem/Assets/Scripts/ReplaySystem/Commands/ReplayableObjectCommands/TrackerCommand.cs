using System.Runtime.Serialization;

namespace ReplaySystem
{
    [DataContract]
    internal abstract class TrackerCommand : ReplayableObjectCommand
    {
        [DataMember]
        internal uint TrackerId { get; private set; } = 0;

        internal TrackerCommand(string objectId, uint trackerId) : base(objectId)
        {
            TrackerId = trackerId;
        }
    }
}
