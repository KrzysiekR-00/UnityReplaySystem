using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReplaySystem
{
    internal class PrefabsCollection : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> Prefabs;
        [SerializeField]
        private List<AudioClip> AudioClips;

        internal GameObject GetPrefabByName(string name)
        {
            name = name.Replace("(Clone)", "");

            var prefab = Prefabs.Where(p => p.name == name).FirstOrDefault();

            if (prefab == null) Debug.LogWarning("Prefab not found: " + name);

            return prefab;
        }

        internal AudioClip GetAudioClipByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            var audio = AudioClips.Where(p => p.name == name).FirstOrDefault();

            if (audio == null) Debug.LogWarning("Audio not found: " + name);

            return audio;
        }
    }
}