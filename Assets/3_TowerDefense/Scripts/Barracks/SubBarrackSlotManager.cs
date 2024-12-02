using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TowerDefense
{
    public class SubBarrackSlotManager : MonoBehaviour
    {
        public SpriteRenderer towerSR;
        public SubBarrackSlot[] subBarrackSlots;
        public Dictionary<BarrackType, SubBarrackSlot> barrackSlotsDic = new Dictionary<BarrackType, SubBarrackSlot>();

        //ลำดับ defalut
        public FortressSlot[] fortressSlotDefault;
        //ลำดับการสร้าง
        public List<FortressSlot> fortressSlotConstruct = new List<FortressSlot>();

        public void Init()
        {
            barrackSlotsDic = subBarrackSlots.ToDictionary(barrack => barrack.barrackType, barrack => barrack);
            subBarrackSlots.ToList().ForEach(o => {
                //o.InitBarrack();
            });

            //สีของป้อม
            fortressSlotDefault.ToList().ForEach(o => {
                o.SetDefault();
                o.SetSprite();
                if (o.fortressType != BarrackType.Empty)
                {
                    AddFortressConstruction(o);
                }
                o.SetQuantity();
            });
        }

        #region Fortress
        //หาป้อมให้ทหาร
        private List<FortressSlot> fortressSlotList = new List<FortressSlot>();

        public void AddFortressConstruction(FortressSlot _fortressSlot)
        {
            if (fortressSlotConstruct.Contains(_fortressSlot))
            {
                fortressSlotConstruct.Remove(_fortressSlot);
            }
            fortressSlotConstruct.Add(_fortressSlot);
        }

        //หาไทป์ของตัวป้อมที่มีอยู่
        public FortressSlot GetFortressType(BarrackType _barrackType)
        {
            fortressSlotList.Clear();
            fortressSlotConstruct.ToList().ForEach(o => {
                if (o.fortressType == _barrackType)
                {
                    fortressSlotList.Add(o);
                }
            });
            if (fortressSlotList.Count > 0)
            {
                return fortressSlotList[0];
            }
            else
            {
                return null;
            }
        }

        //หาป้อมประเภทเดียวกันที่ว่างอยู่
        public FortressEmpty GetFortress(BarrackType _barrackType)
        {
            FortressEmpty fortressEmpty;
            fortressEmpty = null;

            if (GetFortressType(_barrackType))
            {
                foreach (var o in fortressSlotList)
                {
                    foreach (var state in o.targetEntitys.ToList())
                    {
                        if (state.targetEntity == null && !o.isAdded)
                        {
                            fortressEmpty = new FortressEmpty(o, state);
                            break; // Exit the inner loop once a match is found
                        }
                    }

                    // Exit the outer loop if fortressEmpty has been assigned
                    if (fortressEmpty != null)
                    {
                        break;
                    }
                }
            }
            return fortressEmpty;
        }

        public void CheckIsLastFotress(FortressSlot _fortressSlot)
        {
            if (_fortressSlot == fortressSlotList[fortressSlotList.Count - 1])
            {
                fortressSlotList.ForEach(o => {
                    o.isAdded = false;
                });
            }
        }
        #endregion

        public void ClearFortressBetroyal()
        {
            if(fortressSlotConstruct.Count > 0)
            {
                fortressSlotConstruct.ForEach(o =>
                {
                    o.SetDefault();
                });
            }
        }
    }
}
