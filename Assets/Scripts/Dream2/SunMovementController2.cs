using UnityEngine;

public class SunMovementController2 : MonoBehaviour
{
	public GameObject sun;
	public float standardSize = 6f;
	public float sunMaxSize = 6.5f;
	public float sunMinSize = 5.5f;
	public float changeSize = 0.005f;
	public Light Light;
	private float currentSize;
	private int sizeDir = 0; // 0 = 大きくなる, 1 = 小さくなる

	[Header("Sun Sprite Switch")]
	public Sprite sunSprite1;
	public Sprite sunSprite2;
	public float switchInterval = 1.0f;
	private float switchTimer = 0f;
	private bool isSprite1 = true;
	private SpriteRenderer sunRenderer;

	void Start()
	{
		currentSize = standardSize;
		sun.transform.localScale = new Vector3(currentSize, currentSize, currentSize);
		sunRenderer = sun.GetComponent<SpriteRenderer>();
		if (sunRenderer != null && sunSprite1 != null)
		{
			sunRenderer.sprite = sunSprite1;
			isSprite1 = true;
		}
	}

	void Update()
	{
		// スプライト切り替え
		if (sunRenderer != null && sunSprite1 != null && sunSprite2 != null)
		{
			switchTimer += Time.deltaTime;
			if (switchTimer >= switchInterval)
			{
				sunRenderer.sprite = isSprite1 ? sunSprite2 : sunSprite1;
				isSprite1 = !isSprite1;
				switchTimer = 0f;
			}
		}

		if (sizeDir == 0)
		{
			currentSize += changeSize;
			if (currentSize >= sunMaxSize)
			{
				currentSize = sunMaxSize;
				sizeDir = 1;
			}
		}
		else
		{
			currentSize -= changeSize;
			if (currentSize <= sunMinSize)
			{
				currentSize = sunMinSize;
				sizeDir = 0;
			}
		}

		sun.transform.localScale = new Vector3(currentSize, currentSize, currentSize);
	}
}
