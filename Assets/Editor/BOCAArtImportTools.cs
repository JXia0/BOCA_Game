using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BOCA.Editor
{
    public static class BOCAArtImportTools
    {
        private const string ProductionArtRoot = "Assets/Art/Chapters";

        [MenuItem("BOCA/Art/Reimport Production Art")]
        public static void ReimportProductionArt()
        {
            var texturePaths = AssetDatabase
                .FindAssets("t:Texture2D", new[] { ProductionArtRoot })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(IsSupportedTexture)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            try
            {
                AssetDatabase.StartAssetEditing();

                foreach (var path in texturePaths)
                {
                    AssetDatabase.ImportAsset(
                        path,
                        ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            BOCAArtCatalogBuilder.RebuildAll();
            Debug.Log($"[BOCA] Reimported {texturePaths.Length} production art textures and rebuilt art catalogs.");
        }

        private static bool IsSupportedTexture(string path)
        {
            var extension = System.IO.Path.GetExtension(path);
            return extension.Equals(".png", StringComparison.OrdinalIgnoreCase)
                   || extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
                   || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase);
        }
    }
}
