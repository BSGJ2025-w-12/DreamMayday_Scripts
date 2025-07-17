using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;  // TextMeshPro を使っている場合に必要

/// <summary>
/// 虫キャラクター専用のセリフ吹き出し管理クラス。
/// ランダムまたは指定のセリフを表示し、一定時間後に自動で非表示にする。
/// </summary>
public class InsectSpeechBubble : BaseCharacterSpeechBubble
{
    [Header("Insect Specific Settings")]
    public List<string> InsectSpeechLines = new List<string>();

    private bool _disableAutoSpeech = true;

    // 以下は明示的に InsectSpeechBubble に定義（BaseCharacterSpeechBubble を変更しない前提）
    [Header("UI References")]
    [SerializeField] private GameObject _speechBubbleUI;
    [SerializeField] private TextMeshProUGUI _speechText;
    [SerializeField] private float _displayDuration = 2f;
    private Coroutine _speechCoroutine;

    protected override void Start()
    {
        base.Start();

        if (_speechCoroutine != null)
        {
            StopCoroutine(_speechCoroutine);
            _speechCoroutine = null;
        }
    }

    protected override List<string> GetCurrentSpeechLines()
    {
        return new List<string>();
    }

    /// <summary>
    /// ランダムなセリフを表示。
    /// </summary>
    public void ShowRandomSpeech()
    {
        if (InsectSpeechLines.Count == 0 || _speechBubbleUI == null || _speechText == null) return;

        string line = InsectSpeechLines[Random.Range(0, InsectSpeechLines.Count)];
        ShowSpeech(line);
    }

    /// <summary>
    /// 指定のセリフを表示。
    /// </summary>
    public void ShowSpeech(string line)
    {
        if (_speechBubbleUI == null || _speechText == null) return;

        if (_speechCoroutine != null)
        {
            StopCoroutine(_speechCoroutine);
        }

        _speechText.text = line;
        _speechBubbleUI.SetActive(false);
        _speechBubbleUI.SetActive(true);

        _speechCoroutine = StartCoroutine(HideAfterDelay());
    }

    /// <summary>
    /// インデックス指定でセリフを表示。
    /// </summary>
    public void ShowSpeechByIndex(int index)
    {
        if (index >= 0 && index < InsectSpeechLines.Count)
        {
            ShowSpeech(InsectSpeechLines[index]);
        }
    }

    /// <summary>
    /// 吹き出しを非表示。
    /// </summary>
    public void HideSpeech()
    {
        if (_speechBubbleUI != null)
        {
            _speechBubbleUI.SetActive(false);
        }

        if (_speechCoroutine != null)
        {
            StopCoroutine(_speechCoroutine);
            _speechCoroutine = null;
        }
    }

    /// <summary>
    /// 一定時間後に自動で非表示。
    /// </summary>
    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(_displayDuration);

        if (_speechBubbleUI != null)
            _speechBubbleUI.SetActive(false);

        _speechCoroutine = null;
    }

    /// <summary>
    /// セリフが表示中かどうか。
    /// </summary>
    public bool IsSpeaking()
    {
        return _speechBubbleUI != null && _speechBubbleUI.activeSelf;
    }
}
