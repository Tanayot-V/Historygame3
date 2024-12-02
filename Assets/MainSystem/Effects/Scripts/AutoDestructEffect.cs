using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AutoDestructEffect : MonoBehaviour
{
    private ParticleSystem particleSystem;
    public bool shouldDestroy = true; // ตัวแปร Boolean เพื่อเปิด/ปิดการทำลาย

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (shouldDestroy && particleSystem != null && !particleSystem.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
