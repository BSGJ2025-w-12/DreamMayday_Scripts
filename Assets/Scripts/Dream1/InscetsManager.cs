using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 複数の虫キャラクターを管理し、セリフやジャンプなどの演出を制御するクラス。
/// </summary>
public class InsectsManager : MonoBehaviour
{
    public List<InsectController> insects = new List<InsectController>();

    [Header("Group Speech Settings")]
    public string[] groupSpeeches = {
        "僕と一緒に飛ぼう",
        "君という光に\n集まっちゃった",
        "虫も人間も\n関係ない！"
    };
    public float intervalBetweenJumps = 5f;

    private int _currentIndex = 0;
    private Coroutine _loopCoroutine;

    void Start()
    {
        AssignSpeeches();
        StartJumpSequence();
    }

    public void AssignSpeeches()
    {
        for (int i = 0; i < insects.Count; i++)
        {
            if (insects[i] != null)
            {
                string line = i < groupSpeeches.Length ? groupSpeeches[i] : "......";
                insects[i].SetSpeech(line);
            }
        }
    }

    public void StartJumpSequence()
    {
        if (_loopCoroutine != null)
        {
            StopCoroutine(_loopCoroutine);
        }
        _loopCoroutine = StartCoroutine(JumpLoop());
    }

    public void StopJumpSequence()
    {
        if (_loopCoroutine != null)
        {
            StopCoroutine(_loopCoroutine);
            _loopCoroutine = null;
        }

        foreach (var insect in insects)
        {
            if (insect != null && insect.insectBubble != null)
            {
                insect.insectBubble.HideSpeech();
            }
        }
    }

    IEnumerator JumpLoop()
    {
        while (true)
        {
            if (insects.Count == 0) yield break;

            InsectController currentInsect = insects[_currentIndex];
            if (currentInsect != null && currentInsect.HasReachedTarget())
            {
                currentInsect.JumpAndSpeak();
            }

            _currentIndex = (_currentIndex + 1) % insects.Count;
            yield return new WaitForSeconds(intervalBetweenJumps);
        }
    }

    [ContextMenu("Start Room")]
    public void StartRoom()
    {
        foreach (var insect in insects)
        {
            if (insect != null) insect.EnterRoom();
        }

        StartJumpSequence();
    }

    [ContextMenu("End Room")]
    public void EndRoom()
    {
        StopJumpSequence();

        foreach (var insect in insects)
        {
            if (insect != null) insect.ExitRoom();
        }
    }
}
