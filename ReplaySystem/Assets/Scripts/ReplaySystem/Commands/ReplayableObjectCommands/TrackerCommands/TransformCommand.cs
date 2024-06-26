using System.Runtime.Serialization;

namespace ReplaySystem
{
    [DataContract]
    internal abstract class TransformCommand : TrackerCommand
    {
        [DataMember]
        internal uint TransformId { get; private set; } = 0;

        internal TransformCommand(string objectId, uint trackerId, uint transformId) : base(objectId, trackerId)
        {
            TransformId = transformId;
        }
    }
}
