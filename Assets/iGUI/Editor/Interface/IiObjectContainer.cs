using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public interface IiObjectContainer
    {
        public void AddChild(iObject obj, int index);
        public void RemoveChild(iObject obj, int index = -99);
    }
}
