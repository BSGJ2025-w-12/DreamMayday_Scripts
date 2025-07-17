using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 吹き出し表示の共通基底クラス。異変タイプの判定やUI制御は継承先で実装する。
/// </summary>
public abstract class BaseCharacterSpeechBubble : MonoBehaviour
{
    [Header("UI References")]
    public GameObject speechBubbleUI;                 // 吹き出しのUIオブジェクト（Image＋Text）
    public TextMeshProUGUI speechText;                // 吹き出し内のテキスト

    [Header("Speech Settings")]
    public List<string> normalSpeechLines = new List<string>(); // 通常時のセリフ一覧
    public float minDisplayInterval = 1f;             // セリフが表示される間隔（最小）
    public float maxDisplayInterval = 3f;             // セリフが表示される間隔（最大）
    public float displayDuration = 2f;                // セリフが表示される時間

    [Header("Positioning Settings (Local Space)")]
    public Vector2 localOffsetFromCharacter = new Vector3(0f, 1f); // キャラからの相対位置
    public Vector2 bubbleAnchoredOffsetFlipLeft = new Vector2(0.5f, 0f);   // 左
    public Vector2 bubbleAnchoredOffsetFlipRight = new Vector2(-0.5f, 0f); // 右

    // 内部参照
    protected RectTransform speechBubbleRectTransform;
    protected Canvas speechBubbleCanvas;
    protected Coroutine speechCoroutine;
    protected Camera mainCamera;
    protected SpriteRenderer spriteRenderer;

    protected virtual void Start()
    {
        mainCamera = Camera.main;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (speechBubbleUI != null)
        {
            speechBubbleRectTransform = speechBubbleUI.GetComponent<RectTransform>();
            speechBubbleCanvas = speechBubbleUI.GetComponentInParent<Canvas>();

            if (speechBubbleCanvas == null || speechBubbleCanvas.renderMode != RenderMode.WorldSpace)
            {
                Debug.LogError("Speech Bubble UIのCanvasがWorld Spaceではありません！");
                enabled = false;
                return;
            }

            speechBubbleUI.SetActive(false);
        }

        if (speechText == null)
        {
            Debug.LogError("Speech Textが割り当てられていません！");
            enabled = false;
            return;
        }

        speechCoroutine = StartCoroutine(SpeechScheduler());
    }

    protected virtual void Update()
    {
        UpdateSpeechBubblePosition();
    }

    /// <summary>
    /// カメラ位置に応じて吹き出しの反転方向と表示位置を決定
    /// </summary>
    protected virtual void UpdateSpeechBubblePosition()
    {
        if (speechBubbleUI == null || !speechBubbleUI.activeSelf) return;


        Vector2 screenPosition = mainCamera.WorldToScreenPoint(transform.position);
        float screenCenterX = Screen.width / 2f;

        // キャラクターが画面の右側にいれば flip する
        bool isFlipped = screenPosition.x > screenCenterX;

        Vector2 offset = isFlipped ? bubbleAnchoredOffsetFlipRight : bubbleAnchoredOffsetFlipLeft;

        // 中央上部に固定
        speechBubbleRectTransform.pivot = new Vector2(0.5f, 1f);
        speechBubbleRectTransform.anchorMin = speechBubbleRectTransform.anchorMax = new Vector2(0.5f, 1f);
        speechBubbleRectTransform.anchoredPosition = offset+localOffsetFromCharacter;
    }

    /// <summary>
    /// 継承先でセリフ候補を定義する
    /// </summary>
    protected abstract List<string> GetCurrentSpeechLines();

    /// <summary>
    /// セリフ更新コルーチン
    /// </summary>
    protected virtual IEnumerator SpeechScheduler()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDisplayInterval, maxDisplayInterval));

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

    /// <summary>
    /// 無効化時にセリフコルーチンを停止
    /// </summary>
    protected virtual void OnDisable()
    {
        if (speechCoroutine != null) StopCoroutine(speechCoroutine);
    }
}
