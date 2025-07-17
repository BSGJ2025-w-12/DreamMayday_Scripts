using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    //Audioミキサーを入れるとこです
    [SerializeField] AudioMixer audioMixer;

    //それぞれのスライダーを入れるとこです。
    //多い場合は配列にしてもいいですね。
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SESlider;

    private void Start()
    {
        //ミキサーのvolumeにスライダーのvolumeを入れてます。

        //BGM
        audioMixer.GetFloat("BGM", out float bgmVolume);
        BGMSlider.value = bgmVolume;
        //SE
        audioMixer.GetFloat("SE", out float seVolume);
        SESlider.value = seVolume;
    }

    //スライダーで使う部分です。
    public void SetBGM(float volume)
    {
        audioMixer.SetFloat("BGM", volume);
    }

    public void SetSE(float volume)
    {
        audioMixer.SetFloat("SE", volume);
        //ここで確認用の音を鳴らしても良さそう。
    }
}