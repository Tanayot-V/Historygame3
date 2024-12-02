using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextLoop : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI displayText;
    [SerializeField] private string[] messages;
    [SerializeField] private float delay;
    private int currentIndex = 0; // ตำแหน่งปัจจุบันในอาเรย์

    void Start()
    {
        StartCoroutine(LoopMessages(1f)); // เริ่มวนลูปเปลี่ยนข้อความทุก 1 วินาที
    }

    private IEnumerator LoopMessages(float delay)
    {
        while (true)
        {
            displayText.text = messages[currentIndex]; // แสดงข้อความปัจจุบัน
            currentIndex = (currentIndex + 1) % messages.Length; // เลื่อนไปตำแหน่งถัดไปในอาเรย์ (วนกลับมาเริ่มที่ 0 เมื่อถึงตำแหน่งสุดท้าย)
            yield return new WaitForSeconds(delay); // รอเวลาตามที่กำหนดก่อนเปลี่ยนข้อความ
        }
    }
}
