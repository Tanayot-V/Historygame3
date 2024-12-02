using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VidPlayer : MonoBehaviour
{
    [SerializeField] private string url = "";
    public GameObject rawOdj;
    private VideoPlayer videoPlayer;
    //https://github.com/mediaelement/mediaelement-files/blob/master/big_buck_bunny.mp4?raw=true
    //https://github.com/mediaelement/mediaelement-files/blob/master/big_buck_bunny.webm?raw=true
    //https://drive.google.com/uc?export=download&id=12ZF_yikKOtFhU8BUHYg6_bch4vb64HLf
    //https://drive.google.com/uc?export=download&id=1vaBBxMYEzhbg4w_zbRYbVF3IzFoFJF5Z
    //https://drive.google.com/uc?export=download&id=1s06rV16o4p2qUn6JXhLho21TaSBoT-to

    //https://github.com/SupapornThipnan/backup/blob/main/2bc2e25bccc749f7.mp4?raw=true
    //https://github.com/SupapornThipnan/backup/blob/main/aae2144a0ff88966.mp4?raw=true
    //https://github.com/SupapornThipnan/backup/blob/main/WEMB_2bc2e25bccc749f7.webm?raw=true

    //http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4
    //https://historygame-e98c2.web.app/result/2bc2e25bccc749f7.mp4
    //https://historygame-e98c2.web.app/result/aae2144a0ff88966.mp4

    private void Awake()
    {
        videoPlayer = rawOdj.GetComponent<VideoPlayer>(); 
    }
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            url = "https://historygame-e98c2.web.app/result/2bc2e25bccc749f7.mp4";
            if (videoPlayer)
            {
                rawOdj.gameObject.SetActive(true);
                videoPlayer.url = url;
                videoPlayer.playOnAwake = false;
                videoPlayer.Prepare();

                videoPlayer.errorReceived += OnVideoError;
                videoPlayer.prepareCompleted += OnVideoPrepared;
                videoPlayer.loopPointReached += OnVideoFinished;
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            url = "https://historygame-e98c2.web.app/result/aae2144a0ff88966.mp4";
            if (videoPlayer)
            {
                rawOdj.gameObject.SetActive(true);
                videoPlayer.url = url;
                videoPlayer.playOnAwake = false;
                videoPlayer.Prepare();

                videoPlayer.errorReceived += OnVideoError;
                videoPlayer.prepareCompleted += OnVideoPrepared;
                videoPlayer.loopPointReached += OnVideoFinished;
            }
        }
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer source) // ฟังก์ชันนี้จะถูกเรียกเมื่อวิดีโอเล่นเสร็จ
    {
        Debug.Log("OnVideoFinished");
        rawOdj.gameObject.SetActive(false); // ซ่อนวัตถุ
    }

    private void OnVideoError(VideoPlayer source, string message)
    {
        Debug.LogError("VideoPlayer Error: " + message);
    }
}
