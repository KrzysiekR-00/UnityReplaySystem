using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace ReplaySystem
{
    [DataContract]
    [KnownType(nameof(DerivedTypes))]
    public abstract class ReplayCommand
    {
        [DataMember]
        public TimeSpan TimeStamp { get; private set; } = TimeSpan.Zero;

        public abstract void Do(ReplayPlayer replayPlayer);
        public abstract void Undo(ReplayPlayer replayPlayer);

        internal void SetTimeStamp(TimeSpan timeSpan)
        {
            TimeStamp = timeSpan;
        }

        private static Type[] DerivedTypes()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(_ => _.IsSubclassOf(typeof(ReplayCommand))).ToArray();
        }
    }
}
