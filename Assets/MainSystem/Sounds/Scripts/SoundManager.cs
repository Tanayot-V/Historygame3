using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _Instance;
    public static SoundManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                GameObject obj = new GameObject("[Script]: -- SoundManager Instance --");
                _Instance = obj.AddComponent<SoundManager>();
                DontDestroyOnLoad(obj);
            }
            return _Instance;
        }
    }

    [SerializeField]private AudioModelSO audioModelSO;
    public AudioSource sourceMusic;
    public float volume = 1;

    public void Init(AudioModelSO _audioModelSO)
    {
        audioModelSO = _audioModelSO;
        SetVolume(1);
        if (sourceMusic != null) Destroy(sourceMusic.gameObject);
    }

    public void PlayAudioSource(string _name)
    {
        AudioModel audioModel = audioModelSO.GetAudioModel(_name);
        AudioClip audioClip = audioModel.audioClip;
        AudioSource source = CreateAudioSource(_name, audioModel.audioType);

        if (audioModel.audioType != AudioType.BGM)
        {
            source.PlayOneShot(audioClip);
            StartCoroutine(DestroyAudioAfterDelay(source.gameObject, audioClip.length));
        }
        //BGM Sound
        else
        {
            //audioSources.Add(audio.GetComponent<AudioSource>());
            sourceMusic = source;
            sourceMusic.clip = audioClip;
            sourceMusic.playOnAwake = true;
            sourceMusic.loop = true;
            if (volume == 1) sourceMusic.volume = 0.5f;
            else sourceMusic.volume = volume;
            sourceMusic.Play();
        }
    }

    private AudioSource CreateAudioSource(string _name, AudioType _audioType)
    {
        GameObject audio = new GameObject(_name);
        audio.transform.position = Vector3.zero;
        audio.transform.rotation = Quaternion.identity;
        audio.transform.SetParent(this.gameObject.transform);
        AudioSource sourceMusic = audio.AddComponent<AudioSource>();
        sourceMusic.volume = volume;
        audio.name = _name;
        return audio.GetComponent<AudioSource>();
    }

    private IEnumerator DestroyAudioAfterDelay(GameObject audioObject, float delay)
    {
        yield return new WaitForSeconds(delay);

        // ทำลาย GameObject
        Destroy(audioObject);
    }

    public void SetVolume(float _volume)
    {
        volume = _volume;
        if (sourceMusic != null)
        {
            if(volume == 1) sourceMusic.volume = 0.5f;
            else sourceMusic.volume = volume;
        }
    }
}
