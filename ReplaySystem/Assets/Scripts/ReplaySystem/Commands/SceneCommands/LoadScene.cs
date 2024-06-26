using System.Runtime.Serialization;

namespace ReplaySystem
{
    [DataContract]
    class LoadScene : ReplayCommand
    {
        [DataMember]
        internal string SceneName { get; private set; } = string.Empty;

        internal LoadScene(string sceneName)
        {
            SceneName = sceneName;
        }

        public override void Do(ReplayPlayer replayPlayer)
        {

        }

        public override void Undo(ReplayPlayer replayPlayer)
        {

        }
    }
}
