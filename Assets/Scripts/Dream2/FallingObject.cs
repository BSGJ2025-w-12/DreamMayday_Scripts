using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FallingObject : MonoBehaviour
{
	public float fallSpeed = 3f;
	public float lifetime = 0.5f; // 放置時に自動削除
	public bool Rotate;
	public bool rotateZ; // Z軸回転用
	private float timer = 0f;
	private float randomX;
	private float randomY;
	private float randomZ;
	private float zRotateSpeed = 0f;
	// ゆらゆら用
	public float swayAmplitude = 0.5f; // 揺れ幅
	public float swayFrequency = 2f;   // 揺れ速さ
	private float swaySeed = 0f;
	private float baseX = 0f;

	private void Start()
	{
		transform.parent = null;
		randomX = Random.Range(-1, 1);
		randomY = Random.Range(-1, 1);
		randomZ = Random.Range(-1, 1);
		if (rotateZ)
		{
			// -180〜180度/秒の範囲でランダムな回転速度
			zRotateSpeed = Random.Range(-180f, 180f);
			swaySeed = Random.Range(0f, 1000f);
			baseX = transform.position.x;
		}
	}

	void Update()
	{
		// 下方向へ移動
		if (Rotate) transform.Rotate(randomX, randomY, randomZ);
		if (rotateZ)
		{
			float sway = Mathf.Sin(Time.time * swayFrequency + swaySeed) * swayAmplitude;
			Vector3 pos = transform.position;
			pos.x = baseX + sway;
			pos.y -= fallSpeed * Time.deltaTime;
			transform.position = pos;
			transform.Rotate(0, 0, zRotateSpeed * Time.deltaTime);
		}
		else
		{
			transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
		}
		// 一定時間後に自動削除
		timer += Time.deltaTime;
		if (timer >= lifetime)
		{
			Destroy(gameObject);
		}
	}
}