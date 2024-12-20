using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SwipeMenu : MonoBehaviour
{
    public GameObject scrollbar;
    private float scroll_pos = 0;
    private float[] pos;
    private float distance;
    //public float scrollSpeed = 0.1f;

    private void Start()
    {
    }

    void Update()
    {
        pos = new float[transform.childCount];
        distance = 1.25f / (pos.Length - 1f);

        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }
        /*
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScroll != 0)
        {
            scrollbar.GetComponent<Scrollbar>().value -= mouseScroll * scrollSpeed;
        }
        */

        if (Input.GetMouseButton(0))
        {
            scroll_pos = 1 - scrollbar.GetComponent<Scrollbar>().value;
        }
        else
        {
            for (int i = 0; i < pos.Length; i++)
            {
                if (scroll_pos > pos[i] - (distance / 2) && scroll_pos < pos[i] + (distance / 2))
                {
                    scrollbar.GetComponent<Scrollbar>().value = 1 - Mathf.Lerp(1 - scrollbar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                }
            }
        }

        for (int i = 0; i < pos.Length; i++)
        {
            if (scroll_pos > pos[i] - (distance / 2) && scroll_pos < pos[i] + (distance / 2))
            {
                transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1.25f, 1.25f), 0.1f);
            }
            else
            {
                transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(0.8f, 0.8f), 0.1f);
            }
        }
    }
}
