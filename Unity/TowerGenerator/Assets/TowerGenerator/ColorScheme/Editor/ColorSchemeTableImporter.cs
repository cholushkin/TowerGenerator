using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TowerGenerator
{
    public class ColorSchemeTableImporter : AssetPostprocessor
    {
        private static readonly Vector2Int CellSize = new Vector2Int(8, 8);
        private static readonly Vector2Int TextureSizeInCells = new Vector2Int(32, 32);
        private static readonly Vector2Int TextureSize = new Vector2Int(256, 256);
        private static readonly int ExtendedColorSlots = 2;
        private static readonly string OutputFile = "Assets/Prefabs/ColorSchemes/{0}.png";

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (string assetPath in importedAssets)
            {
                var root = Path.GetPathRoot(assetPath);
                var parentDir = Path.GetDirectoryName(assetPath).Split(Path.DirectorySeparatorChar).Last();

                if (IsColorSchemeTableCsv(assetPath))
                {
                    GenerateTextureAtlas(assetPath);
                }
            }
        }

        private static void DrawCell(Texture2D tex, Vector2Int pointer, Color clr)
        {
            for (int x = 0; x < CellSize.x; x++)
                for (int y = 0; y < CellSize.y; y++)
                {
                    tex.SetPixel(pointer.x * CellSize.x + x, tex.height - pointer.y * CellSize.y - y - 1, clr);
                }
        }

        private static void GenerateTextureAtlas(string assetPath)
        {
            var colorSchemeTableName = Path.GetFileNameWithoutExtension(assetPath);
            using (var reader = new StreamReader(assetPath))
            {
                var texture = new Texture2D(TextureSize.x, TextureSize.y, TextureFormat.ARGB32, false);
                var pointer = new Vector2Int(0, 0);
                var clr = new Color();
                var curCol = 0;

                while (!reader.EndOfStream) 
                {
                    var line = reader.ReadLine();
                    var hexColors = line.Split(';');

                    for (int i = 0; i < hexColors.Length;)
                    {
                        pointer.x = curCol*(1+ExtendedColorSlots);
                        if (!ColorUtility.TryParseHtmlString(hexColors[i++], out clr))
                        {
                            Debug.LogError("Unable to parse color from csv");
                            return;
                        }

                        DrawCell(texture,pointer,clr);
                        pointer.x++;


                        for (int j = 0; j < ExtendedColorSlots; ++j)
                        {
                            if (ColorUtility.TryParseHtmlString(hexColors[i++], out clr))
                            {
                                DrawCell(texture, pointer, clr);
                                pointer.x++;
                            }
                            else
                            {
                                Debug.LogError("Unable to parse color from csv");
                                return;
                            }
                        }
                        pointer.y++;
                    }                                       
                    curCol++;
                    pointer.y = 0;
                }

                texture.Apply();
                byte[] pngData = texture.EncodeToPNG();
                if (pngData != null)
                    File.WriteAllBytes(string.Format(OutputFile, colorSchemeTableName), pngData);
                Object.DestroyImmediate(texture);

            }
            AssetDatabase.ImportAsset(string.Format(OutputFile, colorSchemeTableName));
        }

        private static bool IsColorSchemeTableCsv(string assetPath)
        {
            return assetPath.EndsWith(".CST.csv");
        }

    }

}
