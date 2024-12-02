 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System;

public class DialogsData : MonoBehaviour
{
    public TextAsset csvFile;
    public List<Dialog> dialogList = new List<Dialog>();

    public void InitData()
    {
        LoadDialogsFromCSV();
        //dialogList = DialogMockup.Instance.SetDialogMockUpGame3();
    }
    /*
    public void InitData_Novel(VisualNovel.GameManager _gameManager)
    {
        dialogList = DialogMockup.Instance.SetDialogVisNovel(_gameManager);
    }*/

    public Dictionary<string, Dialog> dialogDic = new Dictionary<string, Dialog>();
    public Dialog GetDialog(string _id)
    {
        Debug.Log("_id"+_id);
        if (dialogDic.ContainsKey(_id))
        {
            return dialogDic[_id];
        }
        else
        {
            Dialog foundDialog = dialogList.ToList().Find(o => o.dialogID == _id);
            if (foundDialog != null)
            {
                dialogDic[_id] = foundDialog;
                return foundDialog;
            }
            else
            {
                Debug.LogError($"Dialog not found:{_id}");
                return null;
            }  
        }
    }

    public void LoadDialogsFromCSV()
    {
        if (csvFile == null)
        {
            Debug.LogError("CSV file is null. Please assign a valid TextAsset.");
            return;
        }
        Debug.Log($"File Content: {csvFile.text}");

        // อ่านไฟล์ทั้งหมดด้วย Encoding.UTF8
        //string fileContent = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetBytes(csvFile.text));

        bool isFirstLine = true; // Skip the first line
        string currentDialogID = null;
        List<Dialog.Sentence> currentSentences = new List<Dialog.Sentence>();
        Sprite currentBackground = null;
        DialogTakeAction currentNextAction = DialogTakeAction.NONE;
        string currentNextActionID = null;

        var lines = csvFile.text.Split('\n');
        //var lines = fileContent.Split('\n');
        foreach (var line in lines)
        {
            if (isFirstLine) { isFirstLine = false; continue; }

            var columns = line.Split(',');
            var dialogID = columns[0].Trim();
            var sentenceID = columns[1].Trim();
            var text = columns[2];
            var speakerID = columns[3].Trim();
            var emotion = !string.IsNullOrEmpty(columns[4]) ? (Emotion)System.Enum.Parse(typeof(Emotion), columns[4]) : Emotion.IDLE;
            var background = columns[5];
            var nextAction = columns[6];
            var nextActionID = columns[7].Trim();

            if (!string.IsNullOrEmpty(dialogID))
            {
                if (currentDialogID != null)
                {
                    dialogList.Add(new Dialog(currentDialogID, new List<Dialog.Sentence>(currentSentences), currentBackground, currentNextAction, currentNextActionID));
                    currentSentences.Clear();
                }
                currentDialogID = dialogID;
                currentBackground = !string.IsNullOrEmpty(background) ? BackgroundData.Instance.GetBackground(background).sprite : null;
                currentNextAction = !string.IsNullOrEmpty(nextAction) ? (DialogTakeAction)System.Enum.Parse(typeof(DialogTakeAction), nextAction) : DialogTakeAction.NONE;
                currentNextActionID = nextActionID;
            }

            if (!string.IsNullOrEmpty(sentenceID))
            {
                var sentence = new Dialog.Sentence() { text = text , speakerID = speakerID , emotion = emotion };
                currentSentences.Add(sentence);
            }
        }
        if (!string.IsNullOrEmpty(currentDialogID))
        {
            dialogList.Add(new Dialog(currentDialogID, currentSentences, currentBackground, currentNextAction, currentNextActionID));
        }
        dialogDic = dialogList.ToDictionary(dialog => dialog.dialogID, dialog => dialog);
    }
}
