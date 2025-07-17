using UnityEngine;

public class BackGroundSpriteChanger : MonoBehaviour
{
    public Sprite RainBackGroundSprite;
    public Sprite NormalBackGroundSprite;
    private SpriteRenderer BackGroundSpriteRenderer;
    public Dream3Manager SkyManager;

    void Start()
    {
        BackGroundSpriteRenderer = GetComponent<SpriteRenderer>();

        if (SkyManager == null)
        {
            SkyManager = FindAnyObjectByType<Dream3Manager>();
        }

        if (BackGroundSpriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer がこの GameObject に存在しません！");
        }
    }

    void Update()
    {
        if (SkyManager == null || BackGroundSpriteRenderer == null)
        {
            Debug.Log("必要なものがアタッチされていません");
        }

        if (SkyManager.isThunder)
        {
            BackGroundSpriteRenderer.sprite = RainBackGroundSprite;
        }
        else
        {
            BackGroundSpriteRenderer.sprite = NormalBackGroundSprite;
        }
    }
}
