using UnityEditor;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public class iPreviewObject : iObject
    {
        private Object _targetObject;
        public Object   targetObject
        {
            get { return _targetObject;}
            set 
            {
                _targetObject = value;

                if (objectPreviewEditor != null)
                {
                    Editor.DestroyImmediate(objectPreviewEditor);
                }
                objectPreviewEditor = Editor.CreateEditor(_targetObject);
                objectPreviewEditor.HasPreviewGUI();
            }
        }

        private iBox         box;
        private Editor       objectPreviewEditor;
        private iPreviewType previewType;


        public iPreviewObject()
        { 
            box         = new iBox();
            size        = new Vector2(300, 300);
            previewType = iPreviewType.PREVIEW;
        }


        public override void Start()
        {
            if (style == null)
            {
                style = new GUIStyle(EditorStyles.helpBox);
            }
        }


        public Editor GetEditor()
        {
            return objectPreviewEditor;
        }

        
        public override void Dispose()
        {
            if (objectPreviewEditor != null)
            {
                try
                {
                    Editor.DestroyImmediate(objectPreviewEditor);
                }
                catch {}
            }
        }


        public override void Render()
        {
            base.Render();

            if(!active) return;

            BeginProcessProperty();

            box.size     = size;
            box.position = this.position;
            box.Render();
            if (_targetObject != null && objectPreviewEditor != null)
            {
                switch(previewType)
                {
                    case iPreviewType.PREVIEW:             objectPreviewEditor.OnPreviewGUI(rect, style);            break;
                    case iPreviewType.INTERACTIVE_PREVIEW: objectPreviewEditor.OnInteractivePreviewGUI(rect, style); break;
                    case iPreviewType.INSPECTOR:           objectPreviewEditor.OnInspectorGUI();                     break;
                }
            }

            EndProcessProperty();
        }


        public void Repaint()
        {
            if (objectPreviewEditor != null)
            {
                objectPreviewEditor.ReloadPreviewInstances();
            }
        }
    }

}