using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

// ↓これを呼べばどのスクリプトからでも音源をならせる
// SoundManager.Instance.PlaySE("BGM名");
// SoundManager.Instance.PlayBGM("SE名");

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] AudioSource seAudioSource;
    [SerializeField] AudioMixerGroup mixerGroup;

    [SerializeField] List<BGMSoundData> bgmSoundDatas;
    [SerializeField] List<SESoundData> seSoundDatas;

    public float masterVolume = 1;
    public float bgmMasterVolume = 1;
    public float seMasterVolume = 1;

    public static SoundManager Instance { get; private set; }
    private AudioClip currentBGM = null;

    // シングルトンの決まり文句
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(string clipName)
    {
        BGMSoundData data = bgmSoundDatas.Find(d => d.audioClip != null && d.audioClip.name == clipName);

        if (data == null)
        {
            Debug.LogWarning($"BGM AudioClip名 '{clipName}' に対応するBGMが見つかりませんでした．");
            return;
        }

        if (currentBGM == data.audioClip) return;

        bgmAudioSource.clip = data.audioClip;
        bgmAudioSource.volume = data.volume * bgmMasterVolume * masterVolume;
        bgmAudioSource.Play();

        currentBGM = data.audioClip;
    }


    public void PlaySE(string clipName)
    {
        SESoundData data = seSoundDatas.Find(d => d.audioClip != null && d.audioClip.name == clipName);

        if (data == null)
        {
            Debug.LogWarning($"SE AudioClip名 '{clipName}' に対応するSEが見つかりませんでした．");
            return;
        }

        GameObject tempGO = new GameObject("TempSE_" + data.audioClip.name);
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();
        tempSource.outputAudioMixerGroup = mixerGroup;

        tempSource.clip = data.audioClip;
        tempSource.volume = data.volume * seMasterVolume * masterVolume;
        tempSource.Play();

        Destroy(tempGO, data.audioClip.length);
    }

    // 再生中のループSEを管理する辞書
    private Dictionary<string, GameObject> loopedSEs = new Dictionary<string, GameObject>();

    public void PlayLoopSE(string clipName)
    {
        if (loopedSEs.ContainsKey(clipName))
        {
            // すでに再生中ならスキップ（または再生し直すならDestroyしてもOK）
            return;
        }

        SESoundData data = seSoundDatas.Find(d => d.audioClip != null && d.audioClip.name == clipName);
        if (data == null)
        {
            Debug.LogWarning($"Loop SE '{clipName}' が見つかりませんでした。");
            return;
        }

        GameObject loopGO = new GameObject("LoopSE_" + data.audioClip.name);
        AudioSource audioSource = loopGO.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixerGroup;

        audioSource.clip = data.audioClip;
        audioSource.volume = data.volume * seMasterVolume * masterVolume;
        audioSource.loop = true;
        audioSource.Play();

        loopedSEs[clipName] = loopGO;
    }

    public void StopLoopSE(string clipName)
    {
        if (!loopedSEs.TryGetValue(clipName, out GameObject loopGO))
        {
            Debug.LogWarning($"Loop SE '{clipName}' は再生されていません。");
            return;
        }

        loopedSEs.Remove(clipName);
        Destroy(loopGO);
    }


    void OnEnable()
    {
        // シーンがロードされたときに呼び出されるイベントに登録
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // イベント登録を解除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // シーン切り替え時に実行される関数
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMByScene();
    }


    void Start()
    {
        PlayBGMByScene();
    }

    void PlayBGMByScene()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Product":
                SoundManager.Instance.PlayBGM("audiostock_215418");
                break;
            default:
                SoundManager.Instance.PlayBGM("audiostock_114122");
                break;
        }
    }
}

[System.Serializable]
public class BGMSoundData
{
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 1;
}


[System.Serializable]
public class SESoundData
{
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 1;
}
