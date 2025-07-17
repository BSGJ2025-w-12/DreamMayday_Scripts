using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Dream2SpeechBubble : BaseCharacterSpeechBubble
{
    [Header("セリフリスト")]
    public List<string> hotLines;
    public List<string> coldLines;
    public List<string> bikeLines;
    public List<string> flowerLines;

    private Dream2Manager _manager;

    void Start()
    {
        base.Start();
        _manager = FindObjectOfType<Dream2Manager>();
    }

    //リストの書き込み
    protected override List<string> GetCurrentSpeechLines()
    {
        if (_manager.isHot) return hotLines;
        if (_manager.isIce) return coldLines;
        if (_manager.isFlower) return flowerLines;
        return normalSpeechLines;
    }
}