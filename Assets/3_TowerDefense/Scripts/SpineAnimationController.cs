using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using static SpineSkinModelSO;
using System.Linq;

namespace TowerDefense
{
    public class SpineAnimationController : MonoBehaviour
    {
        public SkeletonAnimation[] skeletonAnimations;
        private SpineSkinModelSO spineSkinModelSO;
        private SpineSkinModelSO.SpineAnimation[] animations;
        private List<string> attacksAnim = new List<string>();
        public SpineSkinModelSO.SpineAnimation currentAnimType;

        void Start() {}

        public void SetSpineAnimation(SpineSkinModelSO _spineSkinModelSO)
        {
            spineSkinModelSO = _spineSkinModelSO;
            animations = _spineSkinModelSO.animations;
            //skeletonAnimation = this.transform.GetChild(0).GetComponent<SkeletonAnimation>();

            animations.ToList().ForEach(o => {
                if (SpineAnimationType.Attack == o.type)
                {
                    attacksAnim.Add(o.animationName);
                }
            });
        }

        public bool GetCurrentAnimationName(string _animationName)
        {
            TrackEntry currentTrack = skeletonAnimations[0].state.GetCurrent(0); // ตรวจสอบ Track ที่ 0
            if (currentTrack != null)
            {
                if (currentTrack.Animation.Name == _animationName) return true;
            }
            return false;
        }

        public void ChangeAnimation(SpineAnimationType _animType, float _timeScale = 1 ,bool _isLoop = true)
        {
            if (_animType == SpineAnimationType.Attack)
            {
                currentAnimType.animationName = GetRandomSpineAttack();
            }
            else
            {
                currentAnimType.animationName = spineSkinModelSO.GetSpineAnimationPath(_animType);
            }
            currentAnimType = spineSkinModelSO.GetSpineAnimationType(currentAnimType.animationName);

            // เช็คว่ามีอนิเมชั่นที่ระบุหรือไม่
            if (skeletonAnimations[0].Skeleton.Data.FindAnimation(currentAnimType.animationName) != null)
            {
                // ถ้ามี เปลี่ยนไปเป็นอนิเมชั่นที่ต้องการ
                skeletonAnimations.ToList().ForEach(o => {
                    o.state.SetAnimation(0, currentAnimType.animationName, _isLoop);
                });
            }
            else
            {
                // ถ้าไม่มี ใช้อนิเมชั่น idle แทน
                skeletonAnimations.ToList().ForEach(o => {
                    o.state.SetAnimation(0, spineSkinModelSO.GetSpineAnimationPath(SpineAnimationType.Idle), _isLoop);
                });
            }

            if (_timeScale <= 0) _timeScale = 1;
            skeletonAnimations[0].state.TimeScale = _timeScale;
            //Debug.Log("ChangeAnimation: "+ currentAnimType.animationName);
            string GetRandomSpineAttack()
            {
                string _path = string.Empty;

                if (attacksAnim.Count > 0)
                {
                    _path = attacksAnim[Random.Range(0, attacksAnim.Count)];
                }
                else
                {
                    _path = attacksAnim[0];
                }
                return _path;
            }
        }

        public void DeadAnim()
        {
            string deadName = spineSkinModelSO.GetSpineAnimationPath(SpineAnimationType.Dead);
            if (skeletonAnimations[0].Skeleton.Data.FindAnimation(deadName) != null)
            {
                // ถ้ามี เปลี่ยนไปเป็นอนิเมชั่นที่ต้องการ
                skeletonAnimations.ToList().ForEach(o => {
                    o.state.TimeScale = 0.75f;
                    o.state.SetAnimation(0, deadName, false).TrackEnd = float.PositiveInfinity;
                });
            }
            /*
            if (skeletonAnimations[0].Skeleton.Data.FindAnimation("dead") != null)
            {
                // ถ้ามี เปลี่ยนไปเป็นอนิเมชั่นที่ต้องการ
                skeletonAnimations.ToList().ForEach(o => {
                    o.state.TimeScale = 0.75f;
                    o.state.SetAnimation(0, "dead", false).TrackEnd = float.PositiveInfinity;
                });
            }*/
        }
    }
}
