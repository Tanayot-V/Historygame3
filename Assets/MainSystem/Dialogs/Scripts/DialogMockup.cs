using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogMockup : MonoBehaviour
{
    private static DialogMockup _Instance;
    public static DialogMockup Instance
    {
        get
        {
            if (_Instance == null)
            {
                GameObject obj = new GameObject("[Script]: -- DialogMakeUp Instance --");
                _Instance = obj.AddComponent<DialogMockup>();
                DontDestroyOnLoad(obj);
            }
            return _Instance;
        }
    }

    public List<Dialog> SetDialogMockUp()
    {
        List<Dialog> dialogs = new List<Dialog>();

        #region QuizFight
        List<Dialog.Sentence> sentences1 = new List<Dialog.Sentence>
        {
            new Dialog.Sentence { text = "สวัสดีเจ้ากะปิปลาร้า!", speakerID = "Monkey", emotion = Emotion.HAPPY },
            new Dialog.Sentence { text = "สบายดีไหม?", speakerID = "Monkey", emotion = Emotion.IDLE },
            new Dialog.Sentence { text = "สบายมาก เจ้าลิงสุดหล่อ", speakerID = "Capybara", emotion = Emotion.HAPPY }
        };

        List<Dialog.Sentence> sentences2 = new List<Dialog.Sentence>
        {
            new Dialog.Sentence { text = "มีคำถาม ตอบได้ให้เลย 10 บาท", speakerID = "Capybara", emotion = Emotion.IDLE },
            new Dialog.Sentence { text = "ขอ 20 เลย", speakerID = "Monkey", emotion = Emotion.HAPPY }
        };

        List<Dialog.Sentence> sentences3 = new List<Dialog.Sentence>
        {
            new Dialog.Sentence { text = "ถูกต้อง!! เจ้ามังครี้ เก่งมาก", speakerID = "Capybara", emotion = Emotion.IDLE },
            new Dialog.Sentence { text = "อิอิ แน่นอน ข้าเก่งอยู่แล้ว 20 บาท", speakerID = "Monkey", emotion = Emotion.HAPPY },
            new Dialog.Sentence { text = ". . . ", speakerID = "Capybara", emotion = Emotion.IDLE }
        };
        Dialog dialog1 = new Dialog("dialog1", sentences1, null, DialogTakeAction.NEXTDIALOG, "dialog2");
        Dialog dialog2 = new Dialog("dialog2", sentences2, null, DialogTakeAction.QUIZ, "question3");
        Dialog dialog3 = new Dialog("dialog3", sentences3, null, DialogTakeAction.NEXTDIALOG, "dialog2");

        dialogs.Add(dialog1);
        dialogs.Add(dialog2);
        dialogs.Add(dialog3);

        #endregion

        #region CityTycoon
        List<Dialog.Sentence> sentences4 = new List<Dialog.Sentence>
        {
            new Dialog.Sentence { text = "ผิดแล้ว!! เจ้ามังครี้ อด. . .", speakerID = "Capybara", emotion = Emotion.IDLE },
            new Dialog.Sentence { text = "มุแง๊!!!!", speakerID = "Monkey", emotion = Emotion.HAPPY },
            new Dialog.Sentence { text = "ลองใหม่ๆๆ เจ้าเก่งอยู่แล้ว", speakerID = "Capybara", emotion = Emotion.IDLE },
        };

        List<Dialog.Sentence> sentences5 = new List<Dialog.Sentence>
        {
            new Dialog.Sentence { text = "เจ้าลิง เจ้าคืออารยธรรมที่สุดยอด!!", speakerID = "Capybara", emotion = Emotion.IDLE },
            new Dialog.Sentence { text = "แน่นอน!!!!", speakerID = "Monkey", emotion = Emotion.HAPPY },
            new Dialog.Sentence { text = "เริ่มสร้างเมืองกันได้เลย", speakerID = "Monkey", emotion = Emotion.IDLE },
        };
        Dialog dialog4 = new Dialog("dialog4", sentences4, null, DialogTakeAction.NEXTDIALOG, "dialog2");
        Dialog dialog5 = new Dialog("dialog5", sentences5, null, DialogTakeAction.NONE, null);

        dialogs.Add(dialog4);
        dialogs.Add(dialog5);
        #endregion

        #region SelectChoice
        List<Dialog.Sentence> sentences6 = new List<Dialog.Sentence>
        {
            new Dialog.Sentence { text = "เจ้าลิง ข้ารู้ว่าเจ้าชอบกิน!!", speakerID = "Capybara", emotion = Emotion.IDLE },
            new Dialog.Sentence { text = "แน่นอน!! เพราะของกินประเทศไทยอร่อย", speakerID = "Monkey", emotion = Emotion.HAPPY },
            new Dialog.Sentence { text = "แล้วเจ้าอยากร่วมมือเอาเสบียงอาหารไปนครวานรไหม. . .", speakerID = "Capybara", emotion = Emotion.IDLE },
        };

        List<Dialog.Sentence> sentences7 = new List<Dialog.Sentence>
        {
            new Dialog.Sentence { text = "เยี่ยมเลยว่าแต่...", speakerID = "Capybara", emotion = Emotion.IDLE },
            new Dialog.Sentence { text = "ว่าแต่อะไรเหรอ", speakerID = "Monkey", emotion = Emotion.HAPPY },
            new Dialog.Sentence { text = "เอารถม้าสีอะไรดีล่ะ?", speakerID = "Capybara", emotion = Emotion.IDLE },
        };

        List<Dialog.Sentence> sentences8 = new List<Dialog.Sentence>
        {
            new Dialog.Sentence { text = "ไม่ส่งเสบียงอาหารงั้นจะทำอะไรล่ะ", speakerID = "Capybara", emotion = Emotion.IDLE },
            new Dialog.Sentence { text = "หาอะไรกินไหมล่ะ", speakerID = "Monkey", emotion = Emotion.HAPPY },
            new Dialog.Sentence { text = "ดีเลยงั้นกินอะไรกันดี?", speakerID = "Capybara", emotion = Emotion.IDLE },
        };
        Dialog dialog6 = new Dialog("dialog6", sentences6, null, DialogTakeAction.SITUATION, "situation1");
        Dialog dialog7 = new Dialog("dialog7", sentences7, null, DialogTakeAction.SITUATION, "situation2");
        Dialog dialog8 = new Dialog("dialog8", sentences8, null, DialogTakeAction.SITUATION, "situation3");

        dialogs.Add(dialog6);
        dialogs.Add(dialog7);
        dialogs.Add(dialog8);
        #endregion

        return dialogs;
    }

    public List<Dialog> SetDialogMockUpGame3()
    {
        List<Dialog> dialogs = new List<Dialog>();
        List<Dialog.Sentence> sentences1 = new List<Dialog.Sentence>
        {
            new Dialog.Sentence { text = "ท่านทำได้ท่านแม่ทัพ! ความสำเร็จในครั้งนี้ช่างน่าอัศจรรย์ใจยิ่งนัก แม้ว่าจะเกิดเหตุไม่คาดฝันขึ้น แต่ท่านก็พิสูจน์ว่าความสามารถของท่านเป็นของจริง ขอให้ท่านจำไว้ว่า ความน่ากลัวของสนามรบไม่ใช่ความเก่งของศัตรู แต่เป็นการแตกความสามัคคีของคนในอาณาจักร", speakerID = "General", emotion = Emotion.IDLE },
        };

        List<Dialog.Sentence> sentences2 = new List<Dialog.Sentence>
        {
            new Dialog.Sentence { text = "ครั้งนี้", speakerID = "General", emotion = Emotion.IDLE },
        };

        Dialog dialog1 = new Dialog("dialog1", sentences1, null, DialogTakeAction.NEXTDIALOG, "Tutorial_1");
        Dialog dialog2 = new Dialog("dialog2", sentences2, null, DialogTakeAction.QUIZ, "Tutorial_2");

        dialogs.Add(dialog1);
        dialogs.Add(dialog2);

        return dialogs;
    }
    /*
    public List<Dialog> SetDialogVisNovel(VisualNovel.GameManager _gameManager)
    {
        List<Dialog> dialogs = new List<Dialog>();
        List<Dialog.Sentence> sentences1 = new List<Dialog.Sentence>
        {
            new Dialog.Sentence { text = "สวัสดีเจ้าลิงสุดหล่อ!", speakerID = "Capybara", emotion = Emotion.IDLE },
            new Dialog.Sentence { text = "สวัสดีเจ้าคาปิบาร่า ยินดียินรับสู่ป่า", speakerID = "Monkey", emotion = Emotion.IDLE },
            new Dialog.Sentence { text = "เดี๋ยววันนี้ลิงจะพาเดินเที่ยวเอง", speakerID = "Monkey", emotion = Emotion.HAPPY }
        };

        List<Dialog.Sentence> sentences2 = new List<Dialog.Sentence>
        {
            new Dialog.Sentence { text = "ข้าไม่ชอบเดินเที่ยวเลย", speakerID = "Capybara", emotion = Emotion.IDLE },
            new Dialog.Sentence { text = "ถ้าไม่เดินเที่ยวจะทำอะไรดีล่ะ", speakerID = "Monkey", emotion = Emotion.IDLE },
            new Dialog.Sentence { text = "นั้นสิ . . . ", speakerID = "Capybara", emotion = Emotion.IDLE }
        };

        List<Dialog.Sentence> sentences3 = new List<Dialog.Sentence>
        {
            new Dialog.Sentence { text = "นอนทั้งวันเลยเหรอ", speakerID = "Capybara", emotion = Emotion.IDLE },
            new Dialog.Sentence { text = "ช่ายๆๆ ง่วงแล้ว คร๊อก. . . ", speakerID = "Monkey", emotion = Emotion.IDLE },
        };

        List<Dialog.Sentence> sentences4 = new List<Dialog.Sentence>
        {
            new Dialog.Sentence { text = "ไม่ทำอะไรเลยเหรอ", speakerID = "Monkey", emotion = Emotion.IDLE },
            new Dialog.Sentence { text = "ช่ายยยย ", speakerID = "Capybara", emotion = Emotion.IDLE },
            new Dialog.Sentence { text = ". . . . .", speakerID = "Monkey", emotion = Emotion.IDLE }
        };

        List<Dialog.Sentence> sentences5 = new List<Dialog.Sentence>
        {
            new Dialog.Sentence { text = "ไปเดินเล่นก็ได้ รับลมกันหน่อย", speakerID = "Capybara", emotion = Emotion.IDLE },
            new Dialog.Sentence { text = "โอเคไปกันเลย", speakerID = "Monkey", emotion = Emotion.IDLE },
            new Dialog.Sentence { text = "ยิปปี้ เย่ๆๆ", speakerID = "Capybara", emotion = Emotion.IDLE }
        };

        Dialog dialog1 = new Dialog("dialog1", sentences1, _gameManager.BackgroundData().GetBackground("bg_1").sprite, DialogTakeAction.NEXTDIALOG, "dialog2");
        Dialog dialog2 = new Dialog("dialog2", sentences2, _gameManager.BackgroundData().GetBackground("bg_2").sprite, DialogTakeAction.NOVELCHOICE, "choose_1");
        Dialog dialog3 = new Dialog("dialog3", sentences3, _gameManager.BackgroundData().GetBackground("bg_1").sprite, DialogTakeAction.NOVELCHOICE, "choose_1");
        Dialog dialog4 = new Dialog("dialog4", sentences4, _gameManager.BackgroundData().GetBackground("bg_2").sprite, DialogTakeAction.NOVELCHOICE, "choose_1");
        Dialog dialog5 = new Dialog("dialog5", sentences5, _gameManager.BackgroundData().GetBackground("bg_2").sprite, DialogTakeAction.NOVELCHOICE, "choose_1");

        dialogs.Add(dialog1);
        dialogs.Add(dialog2);
        dialogs.Add(dialog3);
        dialogs.Add(dialog4);
        dialogs.Add(dialog5);
        return dialogs;
    }*/
}
