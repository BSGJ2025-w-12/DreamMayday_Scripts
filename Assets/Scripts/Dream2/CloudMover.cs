using UnityEngine;

public class CloudMover : MonoBehaviour
{
	public float speed = 1f;
	public Dream2Manager manager;
	public GameObject rainPrefab;
	public GameObject fireArrowPrefab;
	public GameObject icePrefab;
	public GameObject flowerPrefab;
	public bool isBike;

	public float dropIntervalMin = 0.3f; // ランダム間隔の最小値
	public float dropIntervalMax = 0.8f; // ランダム間隔の最大値

	float dropInterval;
	float timer = 0f;

	void Start()
	{
		dropInterval = Random.Range(dropIntervalMin, dropIntervalMax);
	}

	void Update()
	{
		transform.Translate(Vector3.right * speed * Time.deltaTime);
		timer += Time.deltaTime;

		if (timer >= dropInterval && !isBike)
		{
			DropObject();
			timer = 0f;
			dropInterval = Random.Range(dropIntervalMin, dropIntervalMax); // 次の間隔を再設定
		}

		if (transform.localPosition.x > 18f)
		{
			Destroy(gameObject);
		}
	}

	void DropObject()
	{
		if (manager.isHot) return;
		GameObject toDrop = null;
		if (manager.isIce) { toDrop = icePrefab; }
		else
		{
			toDrop = rainPrefab;
		}
		if (toDrop != null)
			Instantiate(toDrop, transform.position, transform.parent.rotation, transform);
	}
}