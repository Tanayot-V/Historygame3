using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InnovaFramework.iGUI.Icons
{
    public partial class iGUIMaterialIcons
    {
        private static Texture2D mainTex_24;
        private static Texture2D mainTex_48;
        private static Dictionary<string, Texture2D> cached_x24 = new Dictionary<string, Texture2D>();
        private static Dictionary<string, Texture2D> cached_x48 = new Dictionary<string, Texture2D>();


        public static string[] GetAllIcons()
        {
            Setup2x();
            return iconsFactory_2x.Keys.ToArray();
        }


        public static Texture2D LoadImage(string name, bool x2)
        {
            if (x2)
            {
                return LoadImage_2X(name);
            }

            return LoadImage_1X(name);
        }

        private static Texture2D LoadImage_2X(string name)
        {
            if (mainTex_48 == null) mainTex_48 = Resources.Load<Texture2D>("iGUI/material_icon_48x48");
            if (mainTex_48 == null) return null;

            Setup2x();

            if (!iconsFactory_2x.ContainsKey(name))
            {
                return null;
            }

            if (cached_x48.ContainsKey(name))
            {
                if (cached_x48[name] != null)
                {
                    return cached_x48[name];
                }
            }

            Texture2D image = new Texture2D(48, 48, TextureFormat.RGBA32, false);
            iPoint point = iconsFactory_2x[name];
            image.SetPixels(mainTex_48.GetPixels(point.x, point.y, 48, 48));
            image.Apply();

            cached_x48[name] = image;

            return image;
        }


        private static Texture2D LoadImage_1X(string name)
        {
            if (mainTex_24 == null) mainTex_24 = Resources.Load<Texture2D>("iGUI/material_icon_24x24");
            if (mainTex_24 == null) return null;

            Setup1x();

            if (!iconsFactory_1x.ContainsKey(name))
            {
                return null;
            }

            if (cached_x24.ContainsKey(name))
            {
                if (cached_x24[name] != null)
                {
                    return cached_x24[name];
                }
            }

            Texture2D image = new Texture2D(24, 24, TextureFormat.RGBA32, false);
            iPoint point = iconsFactory_1x[name];
            image.SetPixels(mainTex_24.GetPixels(point.x, point.y, 24, 24));
            image.Apply();

            cached_x24[name] = image;

            return image;
        }
    }
}