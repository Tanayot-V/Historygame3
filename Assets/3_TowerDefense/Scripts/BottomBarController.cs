using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class BottomBarController : MonoBehaviour
    {
        public GameObject dialogbar;
        [SerializeField] private Image charImg_Right;
        [SerializeField] private Image charImg_Left;
        [SerializeField] private TextMeshProUGUI barText;
        [SerializeField] private TextMeshProUGUI personNameText;
        
        private SpeakerData speakerData;
        private int sentenceIndex = -1;
        private Dialog currentScene;
        private State state = State.COMPLETED;
        private bool isHidden = false;
        public bool isSkipping = false;

        private enum State
        {
            PLAYING, COMPLETED
        }

        public void Hide()
        {
            if (!isHidden)
            {
                charImg_Right.gameObject.SetActive(false);
                dialogbar.GetComponent<CanvasGroupTransition>().FadeOut(() =>
                {
                    dialogbar.SetActive(false);
                });
                isHidden = true;
            }
        }

        public void Show()
        {
            dialogbar.SetActive(true);
            dialogbar.GetComponent<CanvasGroupTransition>().FadeIn();
            isHidden = false;
        }

        public bool IsHidden()
        {
            return isHidden;
        }

        public void ClearText()
        {
            personNameText.text = string.Empty;
            barText.text = string.Empty;
            charImg_Right.gameObject.SetActive(false);
        }

        public void PlayScene(Dialog scene)
        {
            currentScene = scene;
            sentenceIndex = -1;
            PlayNextSentence();
        }

        public void PlayNextSentence()
        {
            sentenceIndex++;
            Speaker speaker = speakerData.GetSpeaker(currentScene.sentences[sentenceIndex].speakerID);
            StartCoroutine(TypeText(currentScene.sentences[sentenceIndex].text));
            personNameText.text = speaker.speakerName;
            personNameText.color = speaker.textColor;

            charImg_Right.sprite = speaker.GetSpriteEmotion(currentScene.sentences[sentenceIndex].emotion);
            charImg_Left.sprite = charImg_Right.sprite;

            if (currentScene.sentences[sentenceIndex].emotion == Emotion.IDLE)
            {
                charImg_Right.gameObject.SetActive(true);
                charImg_Left.gameObject.SetActive(false);
            }

            if (currentScene.sentences[sentenceIndex].emotion == Emotion.HAPPY)
            {
                charImg_Right.gameObject.SetActive(false);
                charImg_Left.gameObject.SetActive(true);
            }
        }

        public void Skip()
        {
            isSkipping = true;
        }

        public bool IsCompleted()
        {
            return state == State.COMPLETED;
        }

        public bool IsLastSentence()
        {
            return sentenceIndex + 1 == currentScene.sentences.Count;
        }

        private IEnumerator TypeText(string text)
        {
            barText.text = string.Empty;
            state = State.PLAYING;
            int wordIndex = 0;

            while (state != State.COMPLETED)
            {
                if (isSkipping)
                {
                    barText.text = text;
                    state = State.COMPLETED;
                    isSkipping = false;
                    yield break;
                }

                barText.text += text[wordIndex];
                yield return new WaitForSeconds(0.05f);
                if (++wordIndex == text.Length)
                {
                    state = State.COMPLETED;
                    isSkipping = false;
                    break;
                }
            }
        }

        public void SetSpeakerData(SpeakerData _speakerData)
        {
            speakerData = _speakerData;
        }
    }
}
