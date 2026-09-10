using System;
using System.Collections.Generic;
using System.IO;
using BOCA.Art;
using UnityEditor;
using UnityEngine;

namespace BOCA.Editor
{
    public static class BOCAArtCatalogBuilder
    {
        public const string SharedCatalogPath = "Assets/Content/ArtCatalogs/SharedArtCatalog.asset";

        private static readonly CatalogSpec[] ChapterCatalogs =
        {
            new CatalogSpec("Assets/Content/ArtCatalogs/Chapter01ArtCatalog.asset", "Assets/Art/Chapters/Chapter01_OilTide"),
            new CatalogSpec("Assets/Content/ArtCatalogs/Chapter02ArtCatalog.asset", "Assets/Art/Chapters/Chapter02_SpermWhaleRescue"),
            new CatalogSpec("Assets/Content/ArtCatalogs/Chapter03ArtCatalog.asset", "Assets/Art/Chapters/Chapter03_ReefWarning"),
            new CatalogSpec("Assets/Content/ArtCatalogs/Chapter04ArtCatalog.asset", "Assets/Art/Chapters/Chapter04_HawksbillGuzheng")
        };

        [MenuItem("BOCA/Art/Rebuild Art Catalogs")]
        public static void RebuildAll()
        {
            EnsureFolder("Assets/Content");
            EnsureFolder("Assets/Content/ArtCatalogs");

            RebuildCatalog(SharedCatalogPath, new[] { "Assets/Art/Shared" });
            foreach (var spec in ChapterCatalogs)
            {
                RebuildCatalog(spec.catalogPath, new[] { spec.searchFolder });
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("BOCA art catalogs rebuilt. Sprite names are used as stable asset keys.");
        }

        public static ArtCatalog LoadChapterCatalog(int chapterNumber)
        {
            if (chapterNumber < 1 || chapterNumber > ChapterCatalogs.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(chapterNumber));
            }

            return AssetDatabase.LoadAssetAtPath<ArtCatalog>(ChapterCatalogs[chapterNumber - 1].catalogPath);
        }

        private static void RebuildCatalog(string catalogPath, string[] searchFolders)
        {
            var catalog = AssetDatabase.LoadAssetAtPath<ArtCatalog>(catalogPath);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<ArtCatalog>();
                AssetDatabase.CreateAsset(catalog, catalogPath);
            }

            var entries = new List<ArtCatalog.Entry>();
            var keys = new HashSet<string>(StringComparer.Ordinal);
            foreach (var guid in AssetDatabase.FindAssets("t:Sprite", searchFolders))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(path))
                {
                    if (asset is not Sprite sprite)
                    {
                        continue;
                    }

                    var key = sprite.name;
                    if (!keys.Add(key))
                    {
                        Debug.LogError($"Duplicate BOCA art key '{key}' in catalog '{catalogPath}'. Rename one sprite before rebuilding.");
                        continue;
                    }

                    entries.Add(new ArtCatalog.Entry { key = key, sprite = sprite });
                }
            }

            entries.Sort((left, right) => string.CompareOrdinal(left.key, right.key));
            catalog.ReplaceEntries(entries);
            EditorUtility.SetDirty(catalog);
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            var name = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent))
            {
                EnsureFolder(parent);
                AssetDatabase.CreateFolder(parent, name);
            }
        }

        private readonly struct CatalogSpec
        {
            public readonly string catalogPath;
            public readonly string searchFolder;

            public CatalogSpec(string catalogPath, string searchFolder)
            {
                this.catalogPath = catalogPath;
                this.searchFolder = searchFolder;
            }
        }
    }
}
