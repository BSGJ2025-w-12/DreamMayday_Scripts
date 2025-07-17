using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dream0CharacterSpeechBubble : BaseCharacterSpeechBubble
{
    //そのシーンのマネージャーと異変の時に喋る内容を定義しておこう
    private Dream0Manager _manager;
    public List<string> fireAnomalySpeechLines;
    public List<string> iceAnomalySpeechLines;
    public List<string> lightAnomalySpeechLines;

    protected override void Start()
    {
        base.Start();
        //ここでそれぞれのシーンのマネージャーを取得しよう
        _manager = GameObject.Find("Dream0Manager").GetComponent<Dream0Manager>();
    }

    protected override IEnumerator SpeechScheduler()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDisplayInterval, maxDisplayInterval));
            
            //ここからICE時にコメントを出さない処理を追記
            if (_manager.isIce)
            {
                speechBubbleUI.SetActive(false);
                continue;
            }
            //ここまで
            
            var lines = GetCurrentSpeechLines();
            if (lines.Count > 0)
            {
                speechText.text = lines[Random.Range(0, lines.Count)];
                speechBubbleUI.SetActive(true);

                yield return new WaitForSeconds(displayDuration);
                speechBubbleUI.SetActive(false);
            }
        }
    }

    //条件によってどの内容を喋るかの切り替え
    protected override List<string> GetCurrentSpeechLines()
    {
        if (_manager.isHot) return fireAnomalySpeechLines;
        if (_manager.isIce) return iceAnomalySpeechLines;
        if (_manager.isLight) return lightAnomalySpeechLines;
        return normalSpeechLines;
    }
}