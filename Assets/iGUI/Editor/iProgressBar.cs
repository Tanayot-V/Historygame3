using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace InnovaFramework.iGUI
{
    public class iProgressBar : iObject
    {
        private float _value;
        public float  value
        {
            get { return _value; }
            set 
            {
                _value = value;
                _value = Mathf.Clamp01(_value);
            }
        }

        private iBox   progressBackground;
        private iBox   progressValue;
        private iLabel label;


        public iProgressBar()
        {
            progressBackground = new iBox();
            progressValue      = new iBox();
            label              = new iLabel();

            size         = new Vector2(200, 24);
            color        = Color.white;
            contentColor = Color.green;
        }


        public override void Start()
        {
            base.Start();
            progressBackground.style   = new GUIStyle(EditorStyles.textArea);
            progressBackground.enabled = false;
            progressBackground.RelativePosition(iRelativePosition.TOP , this);
            progressBackground.RelativePosition(iRelativePosition.LEFT, this);

            progressValue.style = new GUIStyle(GUI.skin.button);
            progressValue.RelativePosition(iRelativePosition.TOP , this);
            progressValue.RelativePosition(iRelativePosition.LEFT, this);

            label.SetText(text);
            label.RelativePosition(iRelativePosition.CENTER_X_OF, this);
            label.RelativePosition(iRelativePosition.CENTER_Y_OF, this);
        }


        public override void UpdateRelative()
        {
            base.UpdateRelative();
            progressBackground.UpdateRelative();
            progressValue.UpdateRelative();
            label.UpdateRelative();
        }


        protected override void BeginProcessProperty()
        {
            progressBackground.size            = this.size;
            progressBackground.backgroundColor = this.backgroundColor;
            progressValue.backgroundColor      = this.contentColor;

            progressValue.size.y = progressBackground.size.y;
            progressValue.size.x = progressBackground.size.x * value;

            label.contentColor = color;

            GUI.enabled = enabled;
        }


        protected override void EndProcessProperty()
        {
            GUI.enabled = true;
        }


        public override void Render()
        {
            base.Render();

            if (!active) return;

            BeginProcessProperty();

            label.SetText(text);
            progressBackground.Render();
            if (value > 0)
            {
                progressValue.Render();
            }
            label.Render();

            EndProcessProperty();
        }
    }
}