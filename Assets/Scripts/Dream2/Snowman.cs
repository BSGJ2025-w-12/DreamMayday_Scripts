using UnityEngine;

public class Snowman : MonoBehaviour
{
	public Dream2Manager manager;
	public SpriteRenderer spriteRenderer;

	// 追加: 小・中・大スノーマン用スプライト
	public Sprite smallSnowmanSprite;
	public Sprite mediumSnowmanSprite;
	public Sprite largeSnowmanSprite;

	// 段階管理用
	private int snowmanStage = 0; // 0:無し, 1:小, 2:中, 3:大
	private float stageTimer = 0f;
	public float stageInterval = 0.5f; // 段階ごとの切り替え時間（秒）
	private bool wasIce = false;

	void Start()
	{
		if (spriteRenderer != null) spriteRenderer.sprite = null;
	}

	void Update()
	{
		if (manager == null || spriteRenderer == null) return;

		if (manager.isIce)
		{
			if (!wasIce)
			{
				wasIce = true;
				stageTimer = 0f;
			}
			stageTimer += Time.deltaTime;
			if (snowmanStage < 3 && stageTimer >= stageInterval)
			{
				snowmanStage++;
				stageTimer = 0f;
			}
		}
		else
		{
			if (wasIce)
			{
				wasIce = false;
				stageTimer = 0f;
			}
			stageTimer += Time.deltaTime;
			if (snowmanStage > 0 && stageTimer >= stageInterval)
			{
				snowmanStage--;
				stageTimer = 0f;
			}
		}

		// スプライトの切り替え
		if (manager.isIce)
		{
			switch (snowmanStage)
			{
				case 1:
					spriteRenderer.sprite = smallSnowmanSprite;
					break;
				case 2:
					spriteRenderer.sprite = mediumSnowmanSprite;
					break;
				case 3:
					spriteRenderer.sprite = largeSnowmanSprite;
					break;
				default:
					spriteRenderer.sprite = null;
					break;
			}
		}
		else
		{
			if (snowmanStage == 0)
			{
				spriteRenderer.sprite = null;
			}
			else if (snowmanStage == 1)
			{
				spriteRenderer.sprite = smallSnowmanSprite;
			}
			else if (snowmanStage == 2)
			{
				spriteRenderer.sprite = mediumSnowmanSprite;
			}
			else if (snowmanStage == 3)
			{
				spriteRenderer.sprite = largeSnowmanSprite;
			}
		}
	}
}
