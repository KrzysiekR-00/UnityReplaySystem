using System;
using UnityEngine;

namespace ComponentsRemover.TypesToIgnore
{
    internal class IgnoreAudiovisualTypes : ComponentsTypesToIgnore
    {
        public override Type[] Types => new[]
        {
            typeof(Renderer),
            typeof(MeshFilter),
            typeof(Transform),
            typeof(ParticleSystem),
            typeof(Light),
            typeof(LODGroup),
            typeof(AudioSource)
        };
    }
}