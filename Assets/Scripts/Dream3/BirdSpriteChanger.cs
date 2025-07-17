using System.Collections;
using UnityEngine;

public class BirdSpriteChanger : MonoBehaviour
{
    public Sprite[] BirdSprites; // 切り替えるスプライトをここにセット
    public float switchInterval = 0.2f; // 切り替え間隔（秒）
    public bool spriteChange = true;

    private SpriteRenderer spriteRenderer;
    private int currentIndex = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (BirdSprites.Length > 0)
        {
            StartCoroutine(SwitchSprites());
        }
    }

    IEnumerator SwitchSprites()
    {
        while (true)
        {
            if (spriteChange)
            {
                currentIndex = (currentIndex + 1) % BirdSprites.Length;
                spriteRenderer.sprite = BirdSprites[currentIndex];
            }
            else
            {
                spriteRenderer.sprite = BirdSprites[0];

            }
            yield return new WaitForSeconds(switchInterval);
        }
    }
}
