using ReplaySystem;
using System;

namespace ComponentsRemover.TypesToIgnore
{
    internal class IgnoreReplaySystemTypes : ComponentsTypesToIgnore
    {
        public override Type[] Types => new[]
        {
            typeof(ReplayableObject),
            typeof(Tracker)
        };
    }
}