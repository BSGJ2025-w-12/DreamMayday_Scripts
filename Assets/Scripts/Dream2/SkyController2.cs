using UnityEngine;
using System.Collections;

public class SkyController2 : MonoBehaviour
{
	public Dream2Manager dream2Manager;

	[Header("Sun Settings")]
	public GameObject sunPrefab;
	public float sunMoveDuration = 3f;
	public float sunOrbitRadius = 10f;

	[Header("Flower Spawn Settings")]
	public GameObject flowerPrefab;
	public Transform flowerSpawnOrigin;
	public float flowerRangeX = 10f;
	public float flowerRangeZ = 10f;
	public float flowerSpawnInterval = 1.0f;

	[Header("Ice Settings")]
	public GameObject icePrefab;
	public Transform[] iceSpawnPoints = new Transform[2];

	private bool isIce;
	private GameObject spawnedSun;
	private GameObject spawnedIce;
	private GameObject spawnedIce2;
	private float flowerSpawnTimer = 0f;

	private Coroutine sunRoutine;
	private Coroutine sunDespawnRoutine;

	void Update()
	{
		if (dream2Manager == null) return;

		// 🌞 太陽出現（Hot時のみ）
		if (dream2Manager.isHot && !dream2Manager.isFlower && sunRoutine == null && spawnedSun == null)
		{
			sunRoutine = StartCoroutine(RotateAroundSelfZ(20f, -90f, false));
		}
		else if ((!dream2Manager.isHot || dream2Manager.isFlower) && sunRoutine == null && spawnedSun != null && sunDespawnRoutine == null)
		{
			sunDespawnRoutine = StartCoroutine(RotateAroundSelfZ(-90f, -200f, true));
		}

		// 🌸 花スポーン
		if (dream2Manager.isFlower)
		{
			flowerSpawnTimer += Time.deltaTime;
			if (flowerSpawnTimer >= flowerSpawnInterval)
			{
				SpawnFlower();
				flowerSpawnTimer = 0f;
			}
		}
		else
		{
			flowerSpawnTimer = 0f;
		}

		// ❄ 氷スポーン
		if (dream2Manager.isIce && !isIce)
		{
			SpawnIce();
		}
		else if (!dream2Manager.isIce && isIce)
		{
			StartCoroutine(DestroyIce());
		}
	}

	IEnumerator RotateAroundSelfZ(float fromAngle, float toAngle, bool isDespawn)
	{
		if (!isDespawn)
		{
			Vector3 startOffset = Quaternion.Euler(0, 0, fromAngle) * Vector3.left * sunOrbitRadius;
			spawnedSun = Instantiate(sunPrefab, transform.position + startOffset, Quaternion.identity);
		}

		// 🔥 Light取得（昇る or 既に存在してる場合）
		Light sunLight = null;
		if (spawnedSun != null)
			sunLight = spawnedSun.GetComponentInChildren<Light>();

		float elapsed = 0f;
		while (elapsed < sunMoveDuration)
		{
			float t = elapsed / sunMoveDuration;
			float angle = Mathf.Lerp(fromAngle, toAngle, t);
			Quaternion rotation = Quaternion.Euler(0, 0, angle);
			Vector3 offset = rotation * Vector3.left * sunOrbitRadius;

			if (spawnedSun != null)
			{
				spawnedSun.transform.position = transform.position + offset;

				// ☀ Lightのintensityを昇る・降りるに応じて変化
				if (sunLight != null)
					sunLight.intensity = isDespawn ? 1f - t : t;
			}

			elapsed += Time.deltaTime;
			yield return null;
		}

		if (isDespawn && spawnedSun != null)
		{
			Destroy(spawnedSun);
			spawnedSun = null;
			sunDespawnRoutine = null;
		}

		if (!isDespawn)
		{
			sunRoutine = null;
		}
	}

	void SpawnFlower()
	{
		if (flowerPrefab == null || flowerSpawnOrigin == null)
		{
			Debug.LogWarning("flowerPrefab または flowerSpawnOrigin が設定されていません。");
			return;
		}

		float offsetX = Random.Range(-flowerRangeX / 2f, flowerRangeX / 2f);
		float offsetZ = Random.Range(-flowerRangeZ / 2f, flowerRangeZ / 2f);
		Vector3 spawnPos = flowerSpawnOrigin.position + new Vector3(offsetX, 0, offsetZ);
		Instantiate(flowerPrefab, spawnPos, Quaternion.identity);
	}

	void SpawnIce()
	{
		if (icePrefab == null || iceSpawnPoints[0] == null)
		{
			Debug.LogWarning("icePrefab または iceSpawnPoint が設定されていません。");
			return;
		}

		spawnedIce = Instantiate(icePrefab, iceSpawnPoints[0].position + Vector3.back * 0.1f + Vector3.up * 2, Quaternion.identity);
		spawnedIce.transform.localScale *= 1.5f;
		spawnedIce2 = Instantiate(icePrefab, iceSpawnPoints[1].position + Vector3.back * 0.1f, Quaternion.identity);
		spawnedIce2.transform.localScale *= 0.6f;
		isIce = true;
	}

	IEnumerator DestroyIce()
	{
		float duration = 3f;
		float elapsed = 0f;
		Renderer iceRenderer1 = spawnedIce != null ? spawnedIce.GetComponentInChildren<Renderer>() : null;
		Renderer iceRenderer2 = spawnedIce2 != null ? spawnedIce2.GetComponentInChildren<Renderer>() : null;
		Color color1 = iceRenderer1 != null ? iceRenderer1.material.color : Color.white;
		Color color2 = iceRenderer2 != null ? iceRenderer2.material.color : Color.white;

		while (elapsed < duration)
		{
			float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
			if (iceRenderer1 != null)
			{
				Color c = color1;
				c.a = alpha;
				iceRenderer1.material.color = c;
			}
			if (iceRenderer2 != null)
			{
				Color c = color2;
				c.a = alpha;
				iceRenderer2.material.color = c;
			}
			elapsed += Time.deltaTime;
			yield return null;
		}
		if (spawnedIce != null) Destroy(spawnedIce);
		if (spawnedIce2 != null) Destroy(spawnedIce2);
		spawnedIce = null;
		spawnedIce2 = null;
		isIce = false;
	}
}
