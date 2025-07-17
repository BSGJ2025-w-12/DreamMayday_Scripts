using UnityEngine;
using System.Collections;

public class Dream2Character : MonoBehaviour
{
	public Dream2Manager manager;
	public Animator animator;

	[Header("トリガー名")]
	public string crouchTrigger = "Crouch";
	public string flowerTrigger = "FlowerReact";
	public string backToIdleTrigger = "Idle"; // Idleに戻す用

	[Header("花用ビジュアル回転")]
	public Transform spriteVisualsTransform;
	public float turnRotationDuration = 0.5f;

	[Header("震え設定（しゃがみ中）")]
	public Transform shakeTarget;
	public float shakeMagnitude = 0.05f;
	public float shakeSpeed = 50f;

	public GameObject sweatPrefab;
	private GameObject spawnedSweat;

	private bool isTurning = false;
	private float currentVisualYRotationAngle = 0f;
	private Coroutine currentRoutine;
	private Coroutine shakeRoutine;

	private bool isIdle;

	private float sweatSwitchInterval = 0.5f; // 切り替え間隔（秒）
	private float sweatSwitchTimer = 0f;
	private bool sweatAngleFlag = false;

	void Update()
	{
		// 汗生成・削除
		if (manager != null && sweatPrefab != null)
		{
			if (manager.isHot && spawnedSweat == null)
			{
				spawnedSweat = Instantiate(sweatPrefab, transform.position + Vector3.up * 2f + Vector3.right * 0.9f, Quaternion.identity, transform);
				sweatSwitchTimer = 0f;
				sweatAngleFlag = false;
				SetSweatRotation(9f);
			}
			else if (!manager.isHot && spawnedSweat != null)
			{
				Destroy(spawnedSweat);
				spawnedSweat = null;
			}
		}

		// 汗の角度切り替え
		if (spawnedSweat != null)
		{
			sweatSwitchTimer += Time.deltaTime;
			if (sweatSwitchTimer >= sweatSwitchInterval)
			{
				sweatAngleFlag = !sweatAngleFlag;
				SetSweatRotation(sweatAngleFlag ? -28f : 9f);
				sweatSwitchTimer = 0f;
			}
		}

		if (currentRoutine == null && manager != null)
		{
			if (manager.isHot || manager.isIce)
			{
				currentRoutine = StartCoroutine(CrouchAfterDelay(4f));
			}
			else if (manager.isFlower)
			{
				currentRoutine = StartCoroutine(HandleFlowerState());
			}
		}

		if (currentRoutine != null && manager.isNormal())
		{
			if (currentRoutine != null)
			{
				StopCoroutine(currentRoutine);
				currentRoutine = null;
			}

			if (shakeRoutine != null)
			{
				StopCoroutine(shakeRoutine);
				shakeTarget.localPosition = Vector3.zero + Vector3.up * 1.49f;
				shakeRoutine = null;
			}

			// 花の状態が終わった時に回転を元に戻す
			if (spriteVisualsTransform != null && currentVisualYRotationAngle != 0f)
			{
				StartCoroutine(RotateVisuals(currentVisualYRotationAngle, 0f));
			}

			// 状態が終わったらIdleに戻す
			animator.ResetTrigger(backToIdleTrigger);
			animator.SetTrigger(backToIdleTrigger);

		}
	}

	private IEnumerator CrouchAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		animator.Play("crouch");
		if (shakeRoutine == null)
		{
			shakeRoutine = StartCoroutine(ShakeWhileCrouching());
		}

		currentRoutine = null;
	}

	private IEnumerator ShakeWhileCrouching()
	{
		Vector3 originalPos = shakeTarget.localPosition;

		while (true)
		{
			// Debug.Log("ShakeWhileCrouching");
			float x = Mathf.Sin(Time.time * shakeSpeed) * shakeMagnitude;
			float y = Mathf.Cos(Time.time * shakeSpeed * 1.1f) * shakeMagnitude;
			shakeTarget.localPosition = originalPos + new Vector3(x, y, 0);
			yield return null;
		}
	}

	private IEnumerator HandleFlowerState()
	{
		yield return new WaitForSeconds(2f);

		animator.Play("lookup");

		while (manager.isFlower)
		{
			yield return new WaitForSeconds(Random.Range(1f, 3f));

			if (!isTurning)
			{
				float targetAngle = (currentVisualYRotationAngle + 180f) % 360f;
				StartCoroutine(RotateVisuals(currentVisualYRotationAngle, targetAngle));
			}
		}

		currentRoutine = null;
	}

	private IEnumerator RotateVisuals(float startYAngle, float endYAngle)
	{
		isTurning = true;
		float timer = 0f;

		Quaternion startRotation = Quaternion.Euler(0, startYAngle, 0);
		Quaternion endRotation = Quaternion.Euler(0, endYAngle, 0);

		while (timer < turnRotationDuration)
		{
			timer += Time.deltaTime;
			float progress = timer / turnRotationDuration;
			spriteVisualsTransform.localRotation = Quaternion.Slerp(startRotation, endRotation, progress);
			yield return null;
		}

		spriteVisualsTransform.localRotation = endRotation;
		currentVisualYRotationAngle = endYAngle;
		isTurning = false;
	}

	void SetSweatRotation(float zAngle)
	{
		if (spawnedSweat != null)
		{
			Vector3 euler = spawnedSweat.transform.localEulerAngles;
			euler.z = zAngle;
			spawnedSweat.transform.localEulerAngles = euler;
		}
	}
}
