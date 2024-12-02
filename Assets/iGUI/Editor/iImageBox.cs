using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public class iImageBox : iObject
    {
        public bool      alphaBlend;
        public bool      showBackground;
        public float     padding;
        public float     imageAspect;
        public iBox      background;
        public Texture   image;
        public ScaleMode scaleMode;


        public iImageBox()
        {
            background = new iBox();
            size       = new Vector2(64, 64);
            padding    = 6;
        }


        public override void Start()
        {
            base.Start();
            background.size = size;
            background.RelativePosition(iRelativePosition.TOP , this);
            background.RelativePosition(iRelativePosition.LEFT, this);
        }


        public override void Render()
        {
            base.Render();

            if (!active) return;

            BeginProcessProperty();

            if (image == null || showBackground)
            {
                background.Render();
            }

            if (image != null)
            {
                Rect r = rect;
                if (showBackground)
                {
                    r.size      = rect.size - new Vector2(padding * 2, padding * 2);
                    r.position += new Vector2(padding, padding);
                }
                GUI.DrawTexture(r, image, scaleMode, alphaBlend, imageAspect);
            }

            EndProcessProperty();
        }


        public override void UpdateRelative()
        {
            base.UpdateRelative();
            background.UpdateRelative();
        }


        public void SetImage(Texture image, ScaleMode scaleMode = ScaleMode.ScaleToFit, bool alphaBlend = true, float imageAspect = 0)
        {
            this.image       = image;
            this.scaleMode   = scaleMode;
            this.alphaBlend  = alphaBlend;
            this.imageAspect = imageAspect;
            GUI.changed      = true;
        }


        public void SetImage(string image, ScaleMode scaleMode = ScaleMode.ScaleToFit, bool alphaBlend = true, float imageAspect = 0)
        {
            this.image       = AssetDatabase.LoadAssetAtPath<Texture2D>(image);
            this.scaleMode   = scaleMode;
            this.alphaBlend  = alphaBlend;
            this.imageAspect = imageAspect;
            GUI.changed      = true;
        }


        public void SetImageFromResources(string path, ScaleMode scaleMode = ScaleMode.ScaleToFit, bool alphaBlend = true, float imageAspect = 0)
        {
            this.image       = Resources.Load<Texture2D>(path);
            this.scaleMode   = scaleMode;
            this.alphaBlend  = alphaBlend;
            this.imageAspect = imageAspect;
            GUI.changed      = true;
        }
    }
}