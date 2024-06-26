using System.Runtime.Serialization;

namespace ReplaySystem
{
    [DataContract]
    class UnloadScene : ReplayCommand
    {
        [DataMember]
        internal string SceneName { get; private set; } = string.Empty;

        internal UnloadScene(string sceneName)
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
