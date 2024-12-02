using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace TowerDefense
{
    public class ClickManager : MonoBehaviour
    {
        public List<GameObject> clickGOList = new List<GameObject>();
        LevelStageManager levelStageManager;
        LevelStageUI levelStageUI;
        TutorialUI tutorial;

        public void Start()
        {
            levelStageManager = GameManager.Instance.LevelStageManager();
            levelStageUI = GameManager.Instance.LevelStageUI();
            if(LevelStageSlot.Instance.IsStageLevelType(LevelType.Level1Tutorial)) tutorial = TutorialUI.Instance;
        }

        public void Update()
        {
            /*
            if (UiController.IsPointerOverUIObject()) return;
            StartCoroutine(UiController.Instance.WaitForSecond(0.2f, () => {
                if (CameraSmooth.Instance.isDragging) return;

            }));*/
            if (Input.GetMouseButtonDown(0) && !UiController.IsPointerOverUIObject())
            {
                if(levelStageUI == null ) GameManager.Instance.LevelStageUI();
                if (!UiController.IsPointerOverUIObject())
                {
                    if (levelStageUI.selectFortessUI.selectFortessUI.activeSelf) levelStageUI.selectFortessUI.Hide();
                    if (levelStageUI.fortessOptionsUI.optionUI.activeSelf) levelStageUI.fortessOptionsUI.Hide();
                }

                ObjectClick obj = GetGameObjectClick();
                if (obj != null)
                {
                    switch (obj.clickType)
                    {
                        //คลิกป้อม
                        case ClickType.Fortress:
                            bool isCanTBuild = false;
                            if (obj.GetComponent<FortressSlot>() != null)
                            {
                                if (isCanTBuild) return;

                                //โหมด Tutorial จนกว่าจะถึง Event ไอดถึง "Tutorial_3" ถึงจะคลิกได้
                                if (LevelStageSlot.Instance.isTutorial)
                                {
                                    if (!TutorialStatge1.Instance.IsCurrentTutorialID("Tutorial_7"))
                                    {
                                        return;
                                    }

                                    if (obj.GetComponent<FortressSlot>() != TutorialStatge1.Instance.fortressSlot_1)
                                    {
                                        return;
                                    }
                                }

                                if (GameManager.Instance.BottomBarController().dialogbar.activeSelf) return;
                                if (obj.GetComponent<FortressSlot>().fortressType == BarrackType.Empty)
                                {
                                    obj.GetComponent<FortressSlot>().Demolition();
                                }
                                else
                                {
                                    if (LevelStageSlot.Instance.isTutorial) return;
                                    levelStageUI.selectFortessUI.currentFortressSlot = obj.GetComponent<FortressSlot>();
                                    levelStageUI.fortessOptionsUI.Show();
                                    /*
                                    levelStageUI.selectFortessUI.currentFortressSlot = obj.GetComponent<FortressSlot>();
                                    levelStageUI.demolitionUI.GetComponent<WorldToUIPos>().targetTransform = obj.transform;
                                    levelStageUI.demolitionUI.GetComponent<WorldToUIPos>().offset = 30;
                                    levelStageUI.demolitionUI.SetActive(true);*/
                                }
                                SoundManager.Instance.PlayAudioSource("Click2");
                            }
                            break;
                        //คลิกฐาน
                        case ClickType.Barrack:
                            if (obj.GetComponent<BarrackSlot>() != null)
                            {

                            }
                            break;

                        case ClickType.TownEnemy:

                            if (levelStageUI.towerFightUI.activeSelf) return;

                            //โหมด Tutorial จนกว่าจะถึง Event ไอดถึง "Tutorial_3" ถึงจะคลิกได้
                            if (LevelStageSlot.Instance.isTutorial)
                            {
                                if (TutorialStatge1.Instance.IsCurrentTutorialID("Tutorial_4"))
                                {
                                    TutorialUI.Instance.arrowFightTownUI.SetActive(false);
                                    TutorialStatge1.Instance.entityMovement.baseType = BaseType.Offensive;
                                    TutorialStatge1.Instance.entityMovement.SetRunPath(LevelStageSlot.Instance.GetPathTransforms("Entity-EnemyGate-Path"));
                                    GameManager.Instance.LevelStageUI().towerFightUI.SetActive(true);
                                }
                                if (TutorialStatge1.Instance.IsCurrentTutorialID("Tutorial_8"))
                                {
                                    TutorialUI.Instance.arrowFightTownUI.SetActive(false);
                                    TutorialStatge1.Instance.entityMovement.baseType = BaseType.Offensive;

                                    TutorialStatge1.Instance.enemyMovement.isStopAttack = false;
                                    TutorialStatge1.Instance.entityMovement.fortressSlot.OffensiveBarrackClick(TutorialStatge1.Instance.entityMovement);
                                    GameManager.Instance.LevelStageUI().towerFightUI.SetActive(true);
                                }
                                
                                return;
                            }
                            else
                            {
                                //กดบุกในเกม
                                ATKEnemyTown();
                                levelStageUI.towerFightUI.SetActive(true);
                                Debug.Log("Click Attack Tower.");
                            }
                            break;

                        case ClickType.TownEntity:
                            UpgradeView upgradeView = GameManager.Instance.LevelStageUI().upgradeViewUI.GetComponent<UpgradeView>();
                            upgradeView.Show();
                            break;
                    }
                    Debug.Log("ClickObject: "+GetGameObjectClick().name);
                }
            }
        }

        //กดบุกในเกม
        public void ATKEnemyTown()
        {
            levelStageManager.levelStageSlot.mainBarrack.barrackSlots[0].entityPool.ForEach(o =>
            {
                o.baseType = BaseType.Offensive;
            });

            Transform sodlierParent = LevelStageSlot.Instance.heroSpwan.transform;
            foreach (Transform child in sodlierParent)
            {
                EntityMovement movement = child.GetComponent<EntityMovement>();
                movement.baseType = BaseType.Offensive;

                if (movement.comboType != EntityComboType.Combo1 && movement.fortressSlot != null)
                {
                    movement.fortressSlot.OffensiveBarrackClick(movement);
                }
            }
            LevelStageSlot.Instance.stageModelSO.levelStageModel.baseType = BaseType.Offensive;
            levelStageManager.levelStageSlot.mainBarrack.barrackSlots[0].general.GetComponent<EntityMovement>().SetRunPath(levelStageManager.levelStageSlot.GetPathTransforms("Entity-EnemyGate-Path"));
        }

        private void GetGameObject()
        {
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(Input.mousePosition));
            //var objHits = hits.Where(hit => hit.collider != null && hit.collider.CompareTag("Furniture")).ToArray();

            clickGOList.Clear();
            if (hits.Length > 0)
            {
                //List<GameObject> goList = new List<GameObject>();
                clickGOList = new List<GameObject>();
                hits.ToList().ForEach(o => {
                    clickGOList.Add(o.collider.gameObject);
                });
            }
        }

        public ObjectClick GetGameObjectClick()
        {
            ObjectClick obj = null;
            GetGameObject();
            if (clickGOList.Count > 0)
            {
                clickGOList.ForEach(o =>
                {
                    if (o.GetComponent<ObjectClick>() != null)
                    {
                        obj = o.GetComponent<ObjectClick>();
                    }
                });
            }
            return obj;
        }

        public void ClickPlaySound(string _id)
        {
            SoundManager.Instance.PlayAudioSource(_id);
        }
    }
}
