//using GameLib;
//using System;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;
//using static UnityEngine.Rendering.DebugUI.Table;

//public class RenderColorTexture : EditorWindow
//{
//    private int textureWidth = 256;
//    private int textureHeight = 256;
//    private int squareSize = 8;
//    private Color[] colors;
//    private string[] colorNames;
//    private string textureName;
//    private bool overrideAlpha;


//    [MenuItem("Tools/Gamelib/Color/Color Square Texture Generator")]
//    public static void ShowWindow()
//    {
//        GetWindow<RenderColorTexture>("Color Square Texture Generator");
//    }

//    private void OnGUI()
//    {
//        textureName = EditorGUILayout.TextField("Texture Name", textureName);
//        textureWidth = EditorGUILayout.IntField("Texture Width", textureWidth);
//        textureHeight = EditorGUILayout.IntField("Texture Height", textureHeight);
//        squareSize = EditorGUILayout.IntField("Square Size", squareSize);

//        int colorsCount = EditorGUILayout.IntField("Number of Colors", colors != null ? colors.Length : 0);
//        if (colors == null || colors.Length != colorsCount)
//        {
//            colors = new Color[colorsCount];
//            colorNames = new string[colorsCount];
//        }

//        EditorGUI.BeginChangeCheck();

//        for (int i = 0; i < colors.Length; i++)
//        {
//            colors[i] = EditorGUILayout.ColorField("Color " + i, colors[i]);
//            colorNames[i] = EditorGUILayout.TextField("Color " + i + " Name", colorNames[i]);
//        }

//        if (EditorGUI.EndChangeCheck())
//        {
//            Repaint();
//        }

//        overrideAlpha = EditorGUILayout.Toggle("Override alpha", overrideAlpha);

//        if (GUILayout.Button("Generate Texture"))
//        {
//            GenerateTexture();
//        }
//    }

//    private void GenerateTexture()
//    {
//        Texture2D texture = new Texture2D(textureWidth, textureHeight);
//        Color clearColor = Color.clear;
//        Color[] pixels = new Color[textureWidth * textureHeight];
//        for (int i = 0; i < pixels.Length; i++)
//        {
//            pixels[i] = clearColor;
//        }
//        texture.SetPixels(pixels);

//        int row;
//        for (row = 0; row <= colors.Length; row++)
//        {
//            Color baseColor = Color.black;
//            string colorName = "BLACK";
//            if (row < colors.Length)
//            {
//                baseColor = colors[row];
//                colorName = colorNames[row];
//            }

//            if (overrideAlpha)
//                baseColor.a = 1f;
//            var b0 = baseColor.Brighten(0.1f);
//            var b1 = baseColor.Brighten(0.3f);
//            var b2 = baseColor.Brighten(0.7f);


//            var d0 = baseColor.Shade(0.7f);
//            var d1 = baseColor.Shade(0.5f);
//            var d2 = baseColor.Shade(0.3f);


//            List<Color> colorRow = new List<Color>() { b2, b1, b0, baseColor, d0, d1, d2 };
//            if(row==colors.Length)
//            {
//                colorRow = new List<Color>() { Color.white, baseColor.Brighten(0.9f), baseColor.Brighten(0.7f), baseColor.Brighten(0.5f), baseColor.Brighten(0.3f), baseColor.Brighten(0.2f), Color.black };
//            }

//            for (int column = 0; column < colorRow.Count; column++)
//            {
//                int startX = column * squareSize;
//                int startY = row * squareSize;
//                DrawColorSquare(texture, startX, startY, colorRow[column], squareSize, squareSize);
//            }
//            RenderTextToTexture(texture, colorRow.Count * squareSize, row * squareSize, " " + colorName, Color.black);
//        }

//        texture.Apply();

//        string path = $"Assets/{textureName}.png"; // Change the path as needed
//        byte[] bytes = texture.EncodeToPNG();
//        System.IO.File.WriteAllBytes(path, bytes);
//        AssetDatabase.Refresh();
//        Debug.Log("Texture generated at: " + path);
//    }

//    private void DrawColorSquare(Texture2D texture, int x, int y, Color color, int squareWidth, int squareHeight)
//    {
//        for (int i = x; i < x + squareWidth; i++)
//        {
//            for (int j = y; j < y + squareHeight; j++)
//            {
//                texture.SetPixel(i, j, color);
//            }
//        }
//    }

//    Texture2D RenderTextToTexture(Texture2D texture, int x, int y, string text, Color textColor)
//    {
//        int charWidth = 5;
//        int charHeight = 5; // Height based on the pixel size and number of rows in the font

//        text = text.ToUpper();
//        int caret = 0;
//        int spacing = 1; // Additional space between characters

//        for (int i = 0; i < text.Length; i++)
//        {
//            int[,] pixels;
//            if (MiniPixelFontDictionary.Chars.TryGetValue(text[i], out pixels))
//            {
//                for (int cy = 0; cy < pixels.GetLength(0); ++cy)
//                {
//                    for (int cx = 0; cx < pixels.GetLength(1); ++cx)
//                    {
//                        int pixel = pixels[cy, cx];
//                        Color color = (pixel == 1) ? textColor : Color.clear;
//                        texture.SetPixel(caret + x + cx, y + 5 - cy, color);
//                    }
//                }
//                charWidth = pixels.GetLength(1);
//                // Add a column of empty pixels
//                for (int extraY = 0; extraY < charHeight; ++extraY)
//                {
//                    texture.SetPixel(caret + charWidth + x, y + 5 - extraY, Color.clear);
//                }

//                caret += charWidth + spacing;
//            }
//        }
//        return texture;
//    }

//}