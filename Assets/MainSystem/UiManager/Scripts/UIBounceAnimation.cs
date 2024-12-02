using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIBounceAnimation : MonoBehaviour
{
    public RectTransform uiElement; // ตัว UI ที่ต้องการทำอนิเมชั่น
    public float scaleMultiplier = 1.5f; // ระดับการขยาย (1.5 เท่า)
    public float duration = 0.5f; // ระยะเวลาในการขยาย-หด
    public Ease animationEase = Ease.InOutQuad; // รูปแบบการเคลื่อนไหว


    void Start()
    {
        uiElement = this.GetComponent<RectTransform>();
        // เริ่มอนิเมชั่นเด้งขึ้น-ลง
        ScaleUI();
    }

    void ScaleUI()
    {
        // เริ่มจากขนาดปัจจุบันแล้วขยายใหญ่ขึ้น จากนั้นหดลงกลับไปที่ขนาดเดิม
        uiElement.DOScale(Vector3.one * scaleMultiplier, duration)
                 .SetEase(animationEase) // ปรับรูปแบบการเคลื่อนไหว
                 .SetLoops(-1, LoopType.Yoyo) // ทำให้ขยาย-หดต่อเนื่อง
                 .SetUpdate(true); // ใช้การอัปเดตแบบไม่ขึ้นกับ Time.timeScale
    }
}
