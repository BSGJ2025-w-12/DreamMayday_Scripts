using UnityEngine;
using System.Collections;
using UnityEngine.Serialization; // Coroutineを使用するために必要

public class AnomalyObject : MonoBehaviour
{
    [Header("通常時の動き設定")]
    public float moveRangeY = 0.5f; // 上下移動の範囲
    public float moveSpeedY = 1f; // 上下移動の速さ

    [Header("異変オブジェクトのプレハブ")]
    public GameObject firePrefab;
    public GameObject icePrefab;
    public GameObject lightPrefab;

    [Header("火の異変設定")]
    public float fireExpandDuration = 1.5f; // 火のオブジェクトが拡大し続ける時間
    public float fireExpandRate = 0.5f; // 1秒あたりの拡大率 (例: 0.5は1秒で0.5倍拡大)

    [Header("光の異変設定")]
    public float lightBlinkDuration = 3f; // 光の点滅を繰り返す全体時間
    public float lightBlinkInterval = 0.5f; // 点滅の間隔 (Active/Disactiveそれぞれ0.5秒)

    private Vector3 originalPosition; // 初期位置
    private GameObject currentAnomalyEffect; // 現在生成されている異変エフェクトの参照
    private bool isAnomalyActive = false; // 異変がアクティブかどうか

    // DreamManagerへの参照 (Inspectorで設定推奨)
    [FormerlySerializedAs("dream1Manager")] public Dream0Manager dream0Manager; 

    void Start()
    {
        originalPosition = transform.position; // 初期位置を保存

        // AnomalyTesterがInspectorで割り当てられていない場合、シーンから探す
        if (dream0Manager == null)
        {
            dream0Manager = FindObjectOfType<Dream0Manager>(); 
            if (dream0Manager == null)
            {
                Debug.LogError("AnomalyTesterが見つかりません！シーンに存在するか確認してください。");
                enabled = false; // スクリプトを無効にする
                return;
            }
        }
    }

    void Update()
    {
        if (dream0Manager == null)
        {
            Debug.LogError("AnomalyTesterの参照が失われました！");
            enabled = false;
            return;
        }

        if (dream0Manager.isIce)
        {
            transform.position = originalPosition;
            StopAllCoroutines();
        }

        if (!isAnomalyActive || (!dream0Manager.isHot && !dream0Manager.isIce && !dream0Manager.isLight))
        {
            float newY = originalPosition.y + Mathf.Sin(Time.time * moveSpeedY) * moveRangeY;
            transform.position = new Vector3(originalPosition.x, newY, originalPosition.z);
        }

        CheckAndApplyAnomaly();
    }
    // 前回の異変タイプを追跡 (AnomalyTesterのenumを参照)
    private CharacterMovementCircle.AnomalyType previousAnomaly = CharacterMovementCircle.AnomalyType.None;

    void CheckAndApplyAnomaly()
    {
        bool currentFire = dream0Manager.isHot;
        bool currentIce = dream0Manager.isIce;
        bool currentLight = dream0Manager.isLight;

        if (currentFire && !isAnomalyActive)
        {
            isAnomalyActive = true;
            StartCoroutine(FireAnomalyRoutine());
        }
        else if (currentIce && !isAnomalyActive)
        {
            isAnomalyActive = true;
            IceAnomalyRoutine();
        }
        else if (currentLight && !isAnomalyActive)
        {
            isAnomalyActive = true;
            StartCoroutine(LightAnomalyRoutine());
        }
        else if (!currentFire && !currentIce && !currentLight && isAnomalyActive)
        {
            CleanupAnomalyEffect();
            isAnomalyActive = false;
        }
    }
    /// <summary>
    /// 現在表示中の異変エフェクトをクリーンアップします。（氷のエフェクトは対象外にするなど調整が必要な場合あり）
    /// </summary>
    void CleanupAnomalyEffect()
    {
        // 氷の異変エフェクトをDestroyしない場合は、ここを調整
        // if (currentAnomalyEffect != null && previousAnomaly != CharacterMovementCircle.AnomalyType.Ice) // 氷以外をクリーンアップ
        if (currentAnomalyEffect != null) // デフォルトでは全てクリーンアップ
        {
            Destroy(currentAnomalyEffect);
            currentAnomalyEffect = null;
        }
    }

    // --- 各異変のコルーチン ---

    IEnumerator FireAnomalyRoutine()
    {
        if (firePrefab == null)
        {
            Debug.LogWarning("Fire Prefabが割り当てられていません。");
            yield break;
        }

        currentAnomalyEffect = Instantiate(firePrefab, transform.position + Vector3.down*2.8f + Vector3.back * 0.1f, Quaternion.identity, transform); // スリッパの子として生成
        Vector3 initialScale = currentAnomalyEffect.transform.localScale;

        float timer = 0f;
        while (timer < fireExpandDuration)
        {
            timer += Time.deltaTime;
            currentAnomalyEffect.transform.localScale += initialScale * fireExpandRate * Time.deltaTime;
            yield return null;
        }
    }

    void IceAnomalyRoutine()
    {
        if (icePrefab == null)
        {
            Debug.LogWarning("Ice Prefabが割り当てられていません。");
            return;
        }

        // 既に氷のエフェクトがある場合は生成しない
        if (currentAnomalyEffect != null && previousAnomaly == CharacterMovementCircle.AnomalyType.Ice)
        {
            // 既に存在する場合は何もしない
            return;
        }

        currentAnomalyEffect = Instantiate(icePrefab, transform.position + Vector3.back * 0.1f, Quaternion.identity, transform); // スリッパの子として生成
    }

    IEnumerator LightAnomalyRoutine()
    {
        if (lightPrefab == null)
        {
            Debug.LogWarning("Light Prefabが割り当てられていません。");
            yield break;
        }

        currentAnomalyEffect = Instantiate(lightPrefab, transform.position + Vector3.back * 0.1f, Quaternion.identity, transform); // スリッパの子として生成

        float totalBlinkTimer = 0f;
        while (totalBlinkTimer < lightBlinkDuration)
        {
            // Active
            if (currentAnomalyEffect != null) currentAnomalyEffect.SetActive(true);
            yield return new WaitForSeconds(lightBlinkInterval);
            totalBlinkTimer += lightBlinkInterval;

            // Disactive
            if (totalBlinkTimer < lightBlinkDuration && currentAnomalyEffect != null)
            {
                currentAnomalyEffect.SetActive(false);
                yield return new WaitForSeconds(lightBlinkInterval);
                totalBlinkTimer += lightBlinkInterval;
            }
        }

        // 3秒間繰り返し後、Activeになる
        if (currentAnomalyEffect != null)
        {
            currentAnomalyEffect.SetActive(true);
        }
    }
}