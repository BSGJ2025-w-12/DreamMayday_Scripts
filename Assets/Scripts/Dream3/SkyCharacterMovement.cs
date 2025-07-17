using UnityEngine;

public class SkyCharacterMovement : MonoBehaviour
{
    [Header("キャラスプライト")]
    public Sprite NormalCharaSprite;
    public Sprite CryCharaSprite;

    [Header("移動設定")]
    public float centerHeight = -1f;
    public float maxHeight = 0f;
    public float minHeight = -2f;
    public float speed = 0.5f;
    public float thunderHeight = -4f;

    [SerializeField, Tooltip("thunder中の上下揺れの振れ幅")]
    private float thunderMaxOffset = 0.3f;

    [SerializeField, Tooltip("スムーズな移動時間")]
    private float smoothTime = 0.1f;

    [Header("参照")]
    public Dream3Manager SkyManager;

    private SpriteRenderer CharaSpriteRenderer;

    private float currentYVelocity = 0f;
    private bool wasThunderLastFrame = false;
    private bool returningToCenter = false;
    private bool movingToThunder = false;

    void Start()
    {
        CharaSpriteRenderer = GetComponent<SpriteRenderer>();
        if (CharaSpriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer がこの GameObject に存在しません！");
        }

        if (SkyManager == null)
        {
            SkyManager = FindAnyObjectByType<Dream3Manager>();
        }
    }

    void Update()
    {
        if (SkyManager == null || CharaSpriteRenderer == null)
        {
            Debug.LogWarning("必要なコンポーネントがアタッチされていません");
            return;
        }

        Vector3 currentPosition = transform.position;
        float newY;

        bool isThunder = SkyManager.isThunder;

        // 状態変化検出
        if (isThunder && !wasThunderLastFrame)
        {
            movingToThunder = true;
            currentYVelocity = 0f;
        }
        else if (!isThunder && wasThunderLastFrame)
        {
            returningToCenter = true;
            currentYVelocity = 0f;
        }

        // 移動ロジック
        if (movingToThunder)
        {
            newY = Mathf.SmoothDamp(currentPosition.y, thunderHeight, ref currentYVelocity, smoothTime);

            if (Mathf.Abs(currentPosition.y - thunderHeight) < 0.01f)
            {
                movingToThunder = false;
                currentYVelocity = 0f;
            }
        }
        else if (isThunder)
        {
            // thunder中に上下揺れ
            float sin = Mathf.Sin(Time.time * speed);
            float offsetY = sin * thunderMaxOffset;
            newY = thunderHeight + offsetY;
        }
        else if (returningToCenter)
        {
            newY = Mathf.SmoothDamp(currentPosition.y, centerHeight, ref currentYVelocity, smoothTime);

            if (Mathf.Abs(currentPosition.y - centerHeight) < 0.01f)
            {
                returningToCenter = false;
                currentYVelocity = 0f;
            }
        }
        else
        {
            // 通常揺れ
            float sin = Mathf.Sin(Time.time * speed);
            float offsetY = sin >= 0 ? sin * maxHeight : sin * minHeight;
            newY = centerHeight + offsetY;
        }

        // 位置更新
        transform.position = new Vector3(currentPosition.x, newY, currentPosition.z);

        // スプライト切り替え
        bool isCrying = SkyManager.isThunder || SkyManager.isSun || SkyManager.isMosquito;
        CharaSpriteRenderer.sprite = isCrying ? CryCharaSprite : NormalCharaSprite;

        wasThunderLastFrame = isThunder;
    }
}
