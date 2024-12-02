using UnityEditor;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public class iBox : iObject
    {
        public iLabel label;

        public iBox()
        {
            size = new Vector2(64, 64);
            label = new iLabel();
        }

        public override void Start()
        {
            if (style == null)
            {
                style = new GUIStyle(EditorStyles.helpBox);
            }

            label.RelativePosition(iRelativePosition.CENTER_X_OF, this);
            label.RelativePosition(iRelativePosition.CENTER_Y_OF, this);
        }

        protected override void BeginProcessProperty()
        {
            base.BeginProcessProperty();
            label.fontSize = fontSize;
        }

        public override void Render()
        {
            base.Render();

            if(!active) return;

            BeginProcessProperty();

            GUI.Box(rect, new GUIContent("", texture, tooltips), style);
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text))
            {
                label.CopyProperty(this);
                label.SetText(text);
                label.Render();
            }

            EndProcessProperty();
        }
    }

}