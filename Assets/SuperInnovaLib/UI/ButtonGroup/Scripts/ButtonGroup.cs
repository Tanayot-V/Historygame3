using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonGroupEffectType
{
    ShowAndHideObject,
    SwapSprite,
    SwapColor,
    Animation,
    Scale,
    Translate,
    Rotation
}


public enum ButtonGroupTransition
{
    None,
    Lerp,
    MoveToward
}


[System.Serializable]
public class ButtonGroupEffect
{
    public ButtonGroupEffectType effectType;

    public Color                 effectColor;
    public Sprite                effectSwapSprite;
    public Vector2               effectVector2;
    public Vector3               effectVector3;
    public Animator              effectAnimator;
    public GameObject            effectShowAndHideTarget;

    public ButtonGroupTransition transition;
    public Graphic               targetEffect;
    public RectTransform         targetRect;
    public float                 transitionValue;

    public Sprite                savedSprite;
    public Color                 savedColor;
    public bool                  savedActive;
    public Vector2               savedVector2;
    public Vector3               savedVector3;

    // Use for temp variable
    public Vector2               vector2;     
    public Vector3               vector3;     
}


[RequireComponent(typeof(Button))]
public class ButtonGroup : MonoBehaviour
{
    public string              groupName;
    public string              key;
    public bool                autoUninteracable = true;
    public ButtonGroupEffect[] effects;

    private List<ButtonGroupEffect> effectsScale     = new List<ButtonGroupEffect>();
    private List<ButtonGroupEffect> effectsTranslate = new List<ButtonGroupEffect>();
    private List<ButtonGroupEffect> effectsRotation  = new List<ButtonGroupEffect>();

    private Button button;

    public void Awake()
    {
        button = GetComponent<Button>();
        Initialize();

        button.onClick.AddListener( () => 
        {
            ButtonGroupManager.Instance.Select(this);
        });

        ButtonGroupManager.Instance.SetupGroup(groupName, this);
    }


    public void Update()
    {
        ProcessingScale();
        ProcessingRotation();
        ProcessingTranslate();
    }


    public void OnDisable()
    {
        Deselect(true);
    }


    private void Initialize()
    {
        for(int i = 0; i < effects.Length; i++)
        {
            switch(effects[i].effectType)
            {
                case ButtonGroupEffectType.ShowAndHideObject:
                {
                    effects[i].savedActive = effects[i].effectShowAndHideTarget.activeSelf;
                    break;
                }
                case ButtonGroupEffectType.SwapSprite:
                {
                    if (effects[i].targetEffect == null) continue;
                    effects[i].savedSprite = ((Image)effects[i].targetEffect).sprite;
                    break;
                }
                case ButtonGroupEffectType.SwapColor:
                {
                    if (effects[i].targetEffect == null) continue;
                    effects[i].savedColor = effects[i].targetEffect.color;
                    break;
                }
                case ButtonGroupEffectType.Scale:
                {
                    if (effects[i].targetEffect == null) continue;
                    effects[i].savedVector2 = effects[i].targetRect.localScale;
                    effects[i].vector2      = effects[i].savedVector2;

                    effectsScale.Add(effects[i]);
                    break;
                }
                case ButtonGroupEffectType.Translate:
                {
                    if (effects[i].targetRect == null) continue;
                    effects[i].savedVector2 = effects[i].targetRect.anchoredPosition;
                    effects[i].vector2      = effects[i].savedVector2;

                    effectsTranslate.Add(effects[i]);
                    break;
                }
                case ButtonGroupEffectType.Rotation:
                {
                    if (effects[i].targetRect == null) continue;
                    effects[i].savedVector3 = effects[i].targetRect.localEulerAngles;
                    effects[i].vector3      = effects[i].savedVector3;

                    effectsRotation.Add(effects[i]);
                    break;
                }
            }
        }
    }

    
    public void Deselect(bool force = false)
    {
        if (autoUninteracable)
        {
            button.interactable = true;
        }

        for(int i = 0; i < effects.Length; i++)
        {
            switch(effects[i].effectType)
            {
                case ButtonGroupEffectType.ShowAndHideObject:
                {
                    if (effects[i].effectShowAndHideTarget == null) continue;
                    effects[i].effectShowAndHideTarget.SetActive(effects[i].savedActive);
                    break;
                }
                case ButtonGroupEffectType.SwapSprite:
                {
                    if (effects[i].targetEffect == null) continue;
                    ((Image)effects[i].targetEffect).sprite = effects[i].savedSprite;
                    break;
                }
                case ButtonGroupEffectType.SwapColor:
                {
                    if (effects[i].targetEffect == null) continue;
                    effects[i].targetEffect.color = effects[i].savedColor;
                    break;
                }
                case ButtonGroupEffectType.Animation:
                {
                    if (effects[i].effectAnimator == null) continue;
                    effects[i].effectAnimator.SetTrigger("Idle");
                    break;
                }
                case ButtonGroupEffectType.Scale:
                {
                    if (effects[i].targetRect == null) continue;
                    effects[i].vector2 = effects[i].savedVector2;
                    effects[i].targetRect.localScale = effects[i].vector2;
                    break;
                }
                case ButtonGroupEffectType.Translate:
                {
                    if (effects[i].targetRect == null) continue;
                    effects[i].vector2 = effects[i].savedVector2;
                    effects[i].targetRect.anchoredPosition = effects[i].vector2;
                    break;
                }
                case ButtonGroupEffectType.Rotation:
                {
                    if (effects[i].targetRect == null) continue;
                    effects[i].vector3 = effects[i].savedVector3;
                    effects[i].targetRect.localEulerAngles = effects[i].vector3;
                    break;
                }
            }
        }
    }


    public void Select()
    {
        if (autoUninteracable)
        {
            button.interactable = false;
        }

        for(int i = 0; i < effects.Length; i++)
        {
            switch(effects[i].effectType)
            {
                case ButtonGroupEffectType.ShowAndHideObject:
                {
                    if (effects[i].effectShowAndHideTarget == null) continue;
                    effects[i].effectShowAndHideTarget.SetActive(!effects[i].savedActive);
                    break;
                }
                case ButtonGroupEffectType.SwapSprite:
                {
                    if (effects[i].targetEffect == null) continue;
                    ((Image) effects[i].targetEffect).sprite = effects[i].effectSwapSprite;
                    break;
                }
                case ButtonGroupEffectType.SwapColor:
                {
                    if (effects[i].targetEffect == null) continue;
                    effects[i].targetEffect.color = effects[i].effectColor;
                    break;
                }
                case ButtonGroupEffectType.Animation:
                {
                    if (effects[i].effectAnimator == null) continue;
                    effects[i].effectAnimator.SetTrigger("Select");
                    break;
                }
                case ButtonGroupEffectType.Scale:
                {
                    if (effects[i].targetRect == null) continue;
                    effects[i].vector2 = effects[i].savedVector2 * effects[i].effectVector2;
                    break;
                }
                case ButtonGroupEffectType.Translate:
                {
                    if (effects[i].targetRect == null) continue;
                    effects[i].vector2 = effects[i].savedVector2 + effects[i].effectVector2;
                    break;
                }
                case ButtonGroupEffectType.Rotation:
                {
                    if (effects[i].targetRect == null) continue;
                    effects[i].vector3 = effects[i].savedVector3 + effects[i].effectVector3;
                    break;
                }
            }
        }
    }


    private void ProcessingScale()
    {
        if (effectsScale.Count == 0) return;

        for(int i = 0; i < effectsScale.Count; i++)
        {
            if (effectsScale[i].targetRect == null) continue;

            RectTransform target = effectsScale[i].targetRect;
            switch(effectsScale[i].transition)
            {
                case ButtonGroupTransition.None:
                {
                    if (!target.localScale.Equals(effectsScale[i].vector2))
                    {
                        target.localScale = effectsScale[i].vector2;
                    }
                    break;
                }
                case ButtonGroupTransition.Lerp:
                {
                    if (!IsEqualsVector(target.localScale, effectsScale[i].vector2))
                    {
                        target.localScale = Vector2.Lerp(target.localScale, effectsScale[i].vector2, effectsScale[i].transitionValue * Time.deltaTime);
                    }
                    break;
                }
                case ButtonGroupTransition.MoveToward:
                {
                    if (!IsEqualsVector(target.localScale, effectsScale[i].vector2))
                    {
                        target.localScale = Vector2.MoveTowards(target.localScale, effectsScale[i].vector2, effectsScale[i].transitionValue * Time.deltaTime);
                    }
                    break;
                }
            }
        }
    }


    private void ProcessingTranslate()
    {
        if (effectsTranslate.Count == 0) return;

        for(int i = 0; i < effectsTranslate.Count; i++)
        {
            if (effectsTranslate[i].targetRect == null) continue;

            RectTransform target = effectsTranslate[i].targetRect;
            switch(effectsTranslate[i].transition)
            {
                case ButtonGroupTransition.None:
                {
                    if (!target.anchoredPosition.Equals(effectsTranslate[i].vector2))
                    {
                        target.anchoredPosition = effectsTranslate[i].vector2;
                    }
                    break;
                }
                case ButtonGroupTransition.Lerp:
                {
                    if (!IsEqualsVector(target.anchoredPosition, effectsTranslate[i].vector2))
                    {
                        target.anchoredPosition = Vector2.Lerp(target.anchoredPosition, effectsTranslate[i].vector2, effectsTranslate[i].transitionValue * Time.deltaTime);
                    }
                    break;
                }
                case ButtonGroupTransition.MoveToward:
                {
                    if (!IsEqualsVector(target.anchoredPosition, effectsTranslate[i].vector2))
                    {
                        target.anchoredPosition = Vector2.MoveTowards(target.anchoredPosition, effectsTranslate[i].vector2, effectsTranslate[i].transitionValue * Time.deltaTime);
                    }
                    break;
                }
            }
        }
    }


    private void ProcessingRotation()
    {
        if (effectsRotation.Count == 0) return;

        for(int i = 0; i < effectsRotation.Count; i++)
        {
            if (effectsRotation[i].targetRect == null) continue;

            RectTransform target = effectsRotation[i].targetRect;
            switch(effectsRotation[i].transition)
            {
                case ButtonGroupTransition.None:
                {
                    if (!target.localEulerAngles.Equals(effectsRotation[i].vector3))
                    {
                        target.localEulerAngles = effectsRotation[i].vector3;
                    }
                    break;
                }
                case ButtonGroupTransition.Lerp:
                {
                    if (!IsEqualsVector(target.localEulerAngles, effectsRotation[i].vector3))
                    {
                        target.localEulerAngles = Vector3.Lerp(target.localRotation.eulerAngles, effectsRotation[i].vector3, effectsRotation[i].transitionValue * Time.deltaTime);
                    }
                    break;
                }
                case ButtonGroupTransition.MoveToward:
                {
                    if (!IsEqualsVector(target.localEulerAngles, effectsRotation[i].vector3))
                    {
                        target.localEulerAngles = Vector3.MoveTowards(target.localRotation.eulerAngles, effectsRotation[i].vector3, effectsRotation[i].transitionValue * Time.deltaTime);
                    }
                    break;
                }
            }
        }
    }


    private bool IsEqualsVector(Vector3 a, Vector3 b, float epsilon = 0.001f)
    {
        return Mathf.Abs(a.x - b.x) <= epsilon && Mathf.Abs(a.y - b.y) <= epsilon && Mathf.Abs(a.z - b.z) <= epsilon;
    }


    private bool IsEqualsVector(Vector3 a, Vector2 b, float epsilon = 0.001f)
    {
        return IsEqualsVector((Vector2)a, b, epsilon);
    }


    private bool IsEqualsVector(Vector2 a, Vector2 b, float epsilon = 0.001f)
    {
        return Mathf.Abs(a.x - b.x) <= epsilon && Mathf.Abs(a.y - b.y) <= epsilon;
    }
}
