using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class DialogController : MonoBehaviour
    {
        [Header("Data")]
        private Dialog currentDialog;

        [Header("References")]
        private BottomBarController dialogBar;
        private DialogsData dialogsData;

        private State state = State.IDLE;
        private enum State
        {
            IDLE, ANIMATE
        }

        public void DialogControllerInit()
        {
            dialogBar = GameManager.Instance.BottomBarController();
            dialogsData = GameManager.Instance.DialogsData();
            dialogsData.InitData();
            GameManager.Instance.BottomBarController().SetSpeakerData(GameManager.Instance.SpeakerData());
        }

        public void PlaySceneInit(string _dialogID)
        {
            //currentDialog = dialogsData.GetDialog(_dialogID);
            currentDialog = dialogsData.dialogDic[_dialogID];
            state = State.IDLE;
            dialogBar.Show();
            dialogBar.PlayScene(currentDialog);
        }

        public void EnterDialogBarButton()
        {
            if (dialogBar.IsHidden()) return;
            if (state == State.IDLE)
            {
                if (dialogBar.IsCompleted())
                {
                    if (dialogBar.IsLastSentence())
                    {
                        PlaySwitchScene(currentDialog);
                    }
                    else
                    {
                        dialogBar.PlayNextSentence();
                    }
                }
                else
                {
                    dialogBar.Skip();
                }
            }
        }

        public void PlaySwitchScene(Dialog scene)
        {
            StartCoroutine(SwitchScene(scene));
        }

        private IEnumerator SwitchScene(Dialog scene)
        {
            state = State.ANIMATE;
            currentDialog = scene;
            dialogBar.Hide();
            yield return new WaitForSeconds(1f);

            switch (scene.nextAction)
            {
                case DialogTakeAction.NEXTDIALOG:
                    if (scene.nextActionID != null) StartCoroutine(NEXTDIALOG(scene.nextActionID));
                    break;
                case DialogTakeAction.NONE: NextActionID(); break;
            }

            void NextActionID()
            {
                if (scene.nextActionID == "EndGame")
                {
                    GameManager.Instance.LevelStageUI().triviaUI.SetActive(true);
                }
                else if (scene.nextActionID == "ResetGame")
                {
                    GameManager.Instance.LevelStageUI().LoadNewScene("3_TowerDefense");
                }

                if (scene.nextActionID.Contains("Tutorial"))
                {
                    _SetTutorial(scene.nextActionID);
                }

                void _SetTutorial(string _id)
                {
                    if (scene.nextActionID == _id)
                    {
                        TutorialStatge1.Instance.SetTutorial(_id);
                    }
                }
            }

            IEnumerator NEXTDIALOG(string _dialogID)
            {
                //currentDialog = dialogsData.GetDialog(_dialogID);
                currentDialog = dialogsData.dialogDic[_dialogID];
                yield return new WaitForSeconds(1f);
                dialogBar.ClearText();
                dialogBar.Show();
                yield return new WaitForSeconds(1f);
                dialogBar.PlayScene(currentDialog);
                state = State.IDLE;
            }

        }
    }
}
