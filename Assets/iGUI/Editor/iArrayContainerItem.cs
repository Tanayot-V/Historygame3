using UnityEditor;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public class iArrayContainerItem<T> : iObject where T : iObject
    {
        public T main;
        public iLabel labelIndex;
        public iButton btnDelete;
        public iArrayContainer<T> owner;
        public int currentIndex;

        private string mainText = "";

        public iArrayContainerItem(T main, iArrayContainer<T> owner)
        {
            this.main  = main;
            this.owner = owner;

            labelIndex      = new iLabel();
            btnDelete       = new iButton();
            btnDelete.size  = new Vector2(16, 16);
            btnDelete.style = new GUIStyle();
            btnDelete.style.LoadClickableTexture("CollabDeleted Icon");

            btnDelete.OnClicked = OnClickDelete;
            size.y = main.height;
        }


        public float SetIndex(int index)
        {
            this.currentIndex = index;
            labelIndex.SetText($"Index: {index}");

            if (mainText == "")
            {
                mainText = main.text;
            }

            main.text = mainText.Replace("{INDEX}", index.ToString());

            return labelIndex.widthEx;
        }


        public void ReChildrenPosition()
        {
            size.y = main.height;

            main.size.x = size.x - btnDelete.widthEx - (owner.showIndex ? owner.indexWidth : 0);
            main.RelativePosition(iRelativePosition.TOP, this);

            labelIndex.RelativePosition(iRelativePosition.LEFT, this);
            labelIndex.RelativePosition(iRelativePosition.CENTER_Y_OF, main);

            btnDelete.RelativePosition(iRelativePosition.CENTER_Y_OF, main);
            btnDelete.RelativePosition(iRelativePosition.RIGHT, this);

            main.RelativePosition(iRelativePosition.LEFT_OF, btnDelete);
        }


        private void OnClickDelete(iObject sender)
        {
            bool ok = true;

            if (!iEvent.IsKeyPressed(KeyCode.LeftAlt))
            {
                ok = EditorUtility.DisplayDialog("Delete", owner.messageDelete, "Confirm", "Cancel");
            }

            if (ok)
            {
                owner.RemoveAt(currentIndex);
            }
        }


        public override void Render()
        {
            base.Render();
            size.y = main.height;

            if (owner.showIndex)
            {
                labelIndex.Render();
            }

            main.enabled       = this.enabled;
            btnDelete.enabled  = this.enabled;
            labelIndex.enabled = this.enabled;

            main.Render();
            btnDelete.Render();
        }
    }
}