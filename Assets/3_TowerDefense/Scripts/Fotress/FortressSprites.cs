using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace TowerDefense
{
    public class FortressSprites : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] spriteRenderers;
        public void SetSpriteRenderer(Sprite[] _sprites)
        {
            int index = 0;
            spriteRenderers.ToList().ForEach(o => {
                if(o.sprite != null) o.sprite = _sprites[index];
                index++;
            });
        }

        public void SetSpriteRendererNull()
        {
            spriteRenderers.ToList().ForEach(o => {
                o.sprite = null;
            });
            Destroy(this.gameObject);
        }
    }
}
