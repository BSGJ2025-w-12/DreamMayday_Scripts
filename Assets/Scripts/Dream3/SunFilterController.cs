using UnityEngine;

public class SunFilterController : MonoBehaviour
{
    public Dream3Manager skyManager;
    private Material filterMaterial;
    private SpriteRenderer spriteRenderer;

    public float fadeDuration = 2.0f; // 上がる or 下がるのにかかる時間

    public Color32 minColor = new Color32(255, 0, 0, 50);
    public Color32 maxColor = new Color32(255, 0, 0, 185);

    private float elapsedTime = 0f;
    private bool isIncreasing = true;

    void Start()
    {
        // SkyManagerの取得
        if (skyManager == null)
        {
            skyManager = GetComponent<Dream3Manager>();
        }

        // SpriteRendererの取得とチェック
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            filterMaterial = spriteRenderer.material;
            if (filterMaterial != null)
            {
                filterMaterial.color = minColor;
            }
            else
            {
                Debug.LogWarning("Material が SpriteRenderer に設定されていません。");
            }
        }
        else
        {
            Debug.LogError("SpriteRenderer がアタッチされていません。SunFilterController を無効化します。");
            enabled = false;
        }
    }

    void Update()
    {
        if (filterMaterial == null || skyManager == null || !skyManager.isSun) return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / fadeDuration);

        byte newAlpha = isIncreasing
            ? (byte)Mathf.Lerp(minColor.a, maxColor.a, t)
            : (byte)Mathf.Lerp(maxColor.a, minColor.a, t);

        Color32 currentColor = new Color32(255, 0, 0, newAlpha);
        filterMaterial.color = currentColor;

        if (t >= 1f)
        {
            isIncreasing = !isIncreasing;
            elapsedTime = 0f;
        }
    }
}
