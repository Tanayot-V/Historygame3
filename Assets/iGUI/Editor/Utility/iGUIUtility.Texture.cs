using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using InnovaFramework.iGUI.Icons;
using UnityEditor;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public partial class iGUIUtility
    {
        public static Texture2D GetSolidTextureColor(Color color)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }


        public static Texture2D CreateCircleTexture(int radius, Color color)
        {
            int size = radius * 2;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);

            int centerX = size / 2;
            int centerY = size / 2;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    int dx = x - centerX;
                    int dy = y - centerY;
                    float distance = Mathf.Sqrt(dx * dx + dy * dy);

                    if (distance <= radius)
                    {
                        texture.SetPixel(x, y, color);
                    }
                }
            }
            texture.Apply();
            return texture;
        }


        public static Texture2D DuplicateTexture(Texture2D source)
        {
            Texture2D newTex = new Texture2D(source.width, source.height, source.format, source.mipmapCount, false);
            newTex.LoadRawTextureData(source.GetRawTextureData());
            newTex.Apply();
            return newTex;
        }


        public static Texture2D LoadBuiltInIcon(string textureName, bool isReadable = false)
        {
            // Materials Icon
            string prefix    = @"^mat-(\d)-icon.";
            Match match = Regex.Match(textureName, prefix);
            if (match.Success)
            {
                textureName = textureName.Remove(0, 11);
                bool x2 = match.Groups[1].Value == "2";
                return iGUIMaterialIcons.LoadImage(textureName, x2);
            }

            // Built-in Icon
            Texture2D tex = (Texture2D)EditorGUIUtility.IconContent(textureName).image;
            if (!isReadable)
            {
                return tex;
            }
            return DuplicateTexture(tex);
        }


        public static Texture2D DarkerTexture(Texture2D tex, float percent = 0.2f)
        {
            Color[] color = tex.GetPixels();
            for (int i = 0; i < color.Length; i++)
            {
                color[i].r -= (color[i].r * percent);
                color[i].g -= (color[i].g * percent);
                color[i].b -= (color[i].b * percent);
            }

            Texture2D newTex = new Texture2D(tex.width, tex.height);
            newTex.SetPixels(color);
            newTex.Apply();
            return newTex;
        }


        public static Texture2D LighterTexture(Texture2D tex, float percent = 0.2f)
        {
            Color[] color = tex.GetPixels();
            for (int i = 0; i < color.Length; i++)
            {
                color[i].r += (color[i].r * percent);
                color[i].g += (color[i].g * percent);
                color[i].b += (color[i].b * percent);
            }

            Texture2D newTex = new Texture2D(tex.width, tex.height);
            newTex.SetPixels(color);
            newTex.Apply();
            return newTex;
        }


        public static GUIContent CreateGUIContent(string text, string textureName, string tooltips = "")
        {
            GUIContent content = new GUIContent();
            content.text = text;
            content.image = LoadBuiltInIcon(textureName);
            content.tooltip = tooltips;
            return content;
        }


        public static GUIContent CreateGUIContent(string text, Texture texture, string tooltips = "")
        {
            GUIContent content = new GUIContent();
            content.text = text;
            content.image = texture;
            content.tooltip = tooltips;
            return content;
        }
    }
}