using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class TutorialCharacter : MonoBehaviour
    {
        public string currentID;

        void Start()
        {

        }

        void Update()
        {

        }

        public void TutorialAction()
        {
            switch (currentID)
            {
                //ตอนศัตรูธรรมดามาตี แล้วตาย => ปล่อยชั้นดีออกมา
                case "Enemy_Lv1_G1":
                    TutorialStatge1.Instance.SetTutorial("Tutorial_2_1");
                    break;

                //ตอนทหารธรรมดาตาย โดนศัตรูชั้นดีตบตาย => ไดอารอคบอกให้อัพเกรด
                case "Sword_Lv1_G1":
                    //หยุดสปอนทหาร
                    TutorialStatge1.Instance.barrack_SwordSlot.isStopSpawning = true;
                    //ขึ้นไดอารอค
                    GameManager.Instance.DialogController().PlaySceneInit("Dialog_2");
                    //หยุดการโจมตีของศัตรูชั้นดี
                    break;

                //ศัตรูชั้นดีตาย => ขึ้นไดอาร๊อกให้บุก
                case "Enemy_Lv2_G1":
                    //TutorialStatge1.Instance.SetTutorial("Tutorial_4");
                    GameManager.Instance.DialogController().PlaySceneInit("Dialog_3");
                    break;

                //เลือดศัครู
                case "Sword_Lv2_G1":
                    GameManager.Instance.DialogController().PlaySceneInit("Dialog_4");
                    currentID = "Sword_Lv2_G1_2";
                    break;

                case "Tutorial_3":
                    TutorialStatge1.Instance.barrack_SwordSlot.isStopSpawning = true;
                    GameManager.Instance.DialogController().PlaySceneInit("Dialog_4");
                    break;

                //ตายแล้วหยุดศัตรูหน้าป้อมก่อน
                case "Sword_Lv2_G1_2":
                    TutorialStatge1.Instance.enemyMovement.isStopAttack = true;
                    TutorialStatge1.Instance.barrack_SwordSlot.isStopSpawning = true;
                    GameManager.Instance.DialogController().PlaySceneInit("Dialog_6");
                    break;

                case "Sword_lv2_G3":
                    GameManager.Instance.DialogController().PlaySceneInit("Dialog_7");
                    break;
                case "Enemy_Lv2_G3":
                    GameManager.Instance.DialogController().PlaySceneInit("Dialog_8");
                    TutorialStatge1.Instance.entityMovement.health = 0;
                    TutorialStatge1.Instance.entityMovement.deadQuantity = 0;
                    TutorialStatge1.Instance.entityMovement.GetComponent<SpineAnimationController>().DeadAnim();
                    StartCoroutine(UiController.Instance.WaitForSecond(1f, () => {
                        Destroy(TutorialStatge1.Instance.entityMovement.gameObject);
                    }));
                    break;
            }
        }       
        
    }
}
