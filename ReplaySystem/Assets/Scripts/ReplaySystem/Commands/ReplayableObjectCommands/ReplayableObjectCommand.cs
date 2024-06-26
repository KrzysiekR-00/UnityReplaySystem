using System.Runtime.Serialization;

namespace ReplaySystem
{
    [DataContract]
    internal abstract class ReplayableObjectCommand : ReplayCommand
    {
        [DataMember]
        internal string ReplayableObjectId { get; private set; }

        internal ReplayableObjectCommand(string replayableObjectId)
        {
            ReplayableObjectId = replayableObjectId;
        }
    }
}
