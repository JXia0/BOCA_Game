using System;
using System.Collections.Generic;
using UnityEngine;

namespace BOCA.Art
{
    [CreateAssetMenu(fileName = "ArtCatalog", menuName = "BOCA/Art Catalog")]
    public sealed class ArtCatalog : ScriptableObject
    {
        [Serializable]
        public sealed class Entry
        {
            public string key;
            public Sprite sprite;
        }

        [SerializeField] private List<Entry> entries = new List<Entry>();

        private Dictionary<string, Sprite> lookup;

        public IReadOnlyList<Entry> Entries => entries;

        public Sprite Resolve(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            EnsureLookup();
            return lookup.TryGetValue(key, out var sprite) ? sprite : null;
        }

        public void ReplaceEntries(IEnumerable<Entry> replacement)
        {
            entries = replacement == null ? new List<Entry>() : new List<Entry>(replacement);
            lookup = null;
        }

        private void OnEnable()
        {
            lookup = null;
        }

        private void OnValidate()
        {
            lookup = null;
        }

        private void EnsureLookup()
        {
            if (lookup != null)
            {
                return;
            }

            lookup = new Dictionary<string, Sprite>(StringComparer.Ordinal);
            foreach (var entry in entries)
            {
                if (entry != null && !string.IsNullOrWhiteSpace(entry.key) && entry.sprite != null)
                {
                    lookup[entry.key] = entry.sprite;
                }
            }
        }
    }
}
