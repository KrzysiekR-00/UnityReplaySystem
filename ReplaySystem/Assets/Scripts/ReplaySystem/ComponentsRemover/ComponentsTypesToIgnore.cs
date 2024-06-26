using System;
using UnityEngine;

namespace ComponentsRemover
{
    public abstract class ComponentsTypesToIgnore : MonoBehaviour
    {
        public abstract Type[] Types { get; }
    }
}