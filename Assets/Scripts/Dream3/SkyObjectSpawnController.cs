using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

public class SkyObjectSpawnController : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject NormalCloudPrefab;
    public GameObject BirdPrefab;
    public GameObject ThunderCloudPrefab;
    public GameObject WaterDropPrefab;
    public GameObject SunPrefab;
    public GameObject MosquitoPrefab;

    [Header("Scripts")]
    public Dream3Manager SkyManager;

    [Header("Objects")]
    public GameObject RainSpawner;
    public GameObject SunFilter;

    [Header("DropBirdSprite")]
    public Sprite dropBirdSprite;
    [Header("Standard Position")]
    public Vector3 standardPos;

    [Header("Settings")]
    [SerializeField, Tooltip("オブジェクトの移動速度")]
    public float objectSpeed = 4f;
    [SerializeField, Tooltip("太陽の移動速度(小さいほど速い)")]
    private float sunSpeed = 5f;
    [SerializeField, Tooltip("太陽のサイズ")]
    public float minCloudScale = 0.5f;
    public float maxCloudScale = 5.0f;
    public int maxMosquitoCount = 150;
    public float SunTargetX = 0;
    public float SunTargetY = 15;
    private float SunCenterX = 0;
    private float SunCenterY = 0;
    public float SunStandardSize = 6f;
    public float maxSunSize = 6.5f;
    public float minSunSize = 5.5f;
    public float SunChangeValue = 0.005f;

    [Header("Spawn Timing")]
    public float intervalBetweenSpawn = 2f;
    public float intervalDropWater = 1f;
    public float intervalAfterCycle = 4f;
    public float mosquitoSpawnDelay = 0.25f;
    public float birdSpawnAnomalyDelay = 0.5f;
    public float minBirdDropTime = 1f;
    public float maxBirdDropTime = 10f;

    [Header("Spawn Settings")]
    public int initialSpawnCount = 5;
    public int cloudSpawnCount = 3;
    public int thunderSpawnCount = 5;
    public int birdSpawnCount = 2;
    [SerializeField, Tooltip("異変時の最大鳥の数")]
    public int birdSpawnAnomaly = 50;
    [SerializeField, Tooltip("鳥の墜落時の最大角度")]
    private float birdTargetRotationZ = 75f;
    [SerializeField, Tooltip("鳥の墜落時の加速タイミング")]
    private float accelTiming = 1f;
    [SerializeField, Tooltip("鳥の墜落時の加速度")]
    private float birdAccel = 0.25f;
    [SerializeField, Tooltip("鳥の墜落時の最大加速")]
    private float maxBirdAccel = 35f;
    [SerializeField, Tooltip("水滴の落下速度")]
    private float waterDropSpeed = 0.5f;

    [Header("画面中央からの相対位置の基準")]
    public float leftEnd = -45;
    public float rightEnd = 45;
    public float topEnd = 22;
    public float bottomEnd = -22;
    private float leftBound = -45;
    private float rightBound = 45;
    private float topBound = -22;
    private float bottomBound = 22;

    [Header("その他setting")]
    [SerializeField, Tooltip("雲の表示位置(大きいほど手前)")] private int cloudLayer = 2;
    [SerializeField, Tooltip("鳥の表示位置(大きいほど手前)")] private int birdLayer = 4;
    [SerializeField, Tooltip("雷雲の表示位置(大きいほど手前)")] private int thunderLayer = 3;
    [SerializeField, Tooltip("太陽の表示位置(大きいほど手前)")] private int sunLayer = 1;
    [SerializeField, Tooltip("蚊の表示位置(大きいほど手前)")] private int mosquitoLayer = 15;
    [SerializeField, Tooltip("Hierarchyにobjectを表示させるか")] private bool objAppear = false;
    public int currentMosquitoCount = 0;

    private bool hasSpawnSun = false;

    private Coroutine mosquitoRoutine;
    private Coroutine sunRiseRoutine;
    private Coroutine sunSetRoutine;
    private Coroutine thunderRoutine;
    private Coroutine birdRoutine;
    private Coroutine cloudRoutine;
    public bool isTop;      //太陽が一番上の場合にセリフを表示させることを伝える

    void Start()
    {
        leftBound = leftEnd + standardPos.x;
        rightBound = rightEnd + standardPos.x;
        topBound = topEnd + standardPos.y;
        bottomBound = bottomEnd + standardPos.y;
        SunCenterX = SunTargetX + standardPos.x;
        SunCenterY = SunTargetY + standardPos.y;

        // Debug.Log(standardPos);
        if (SkyManager == null)
        {
            SkyManager = FindAnyObjectByType<Dream3Manager>();
            if (SkyManager == null)
            {
                Debug.LogError("Dream3Manager がシーンに存在しません！");
                return;
            }
        }
        InitialObjectSpawn();
    }

    void Update()
    {
        //異変か起きているかのチェック

        // Normal Cloud
        if (!SkyManager.isThunder && !SkyManager.isSun && !SkyManager.isMosquito && cloudRoutine == null)
        {
            cloudRoutine = StartCoroutine(SpawnNormalCloud());
        }
        else if ((SkyManager.isThunder || SkyManager.isSun || SkyManager.isMosquito) && cloudRoutine != null)
        {
            StopCoroutine(cloudRoutine);
            cloudRoutine = null;
        }

        // Bird
        if (!SkyManager.isThunder && !SkyManager.isMosquito && birdRoutine == null)
        {
            birdRoutine = StartCoroutine(SpawnBird());
        }
        else if ((SkyManager.isThunder || SkyManager.isMosquito) && birdRoutine != null)
        {
            StopCoroutine(birdRoutine);
            birdRoutine = null;
        }

        //isThunderがtrueの時、雨を降らせる
        RainSpawner.SetActive(SkyManager.isThunder);

        // Thunder
        if (SkyManager.isThunder && thunderRoutine == null)
        {
            thunderRoutine = StartCoroutine(SpawnThunderCloud());
            //waterDropRoutine = StartCoroutine(SpawnWaterDrop());
        }
        else if (!SkyManager.isThunder && thunderRoutine != null)
        {
            StopCoroutine(thunderRoutine);
            thunderRoutine = null;
            //StopCoroutine(waterDropRoutine);
            //waterDropRoutine = null;
        }

        // Sun
        SunFilter.SetActive(SkyManager.isSun);

        if (SkyManager.isSun && sunRiseRoutine == null)
        {
            sunRiseRoutine = StartCoroutine(ActiveSunAnomaly());
        }
        else if (!SkyManager.isSun && sunRiseRoutine != null)
        {
            StopCoroutine(sunRiseRoutine);
            sunRiseRoutine = null;

            GameObject sun = GameObject.FindGameObjectWithTag("SkySun");
            sunSetRoutine = StartCoroutine(MoveInArcFromCenter(sun, sunSpeed));
        }

        // Mosquito
        if (SkyManager.isMosquito && mosquitoRoutine == null)
        {
            mosquitoRoutine = StartCoroutine(ActiveMosquitoAnomaly());
        }
        else if (!SkyManager.isMosquito && mosquitoRoutine != null)
        {
            StopCoroutine(mosquitoRoutine);
            mosquitoRoutine = null;

            currentMosquitoCount = 0;
        }
    }

    void InitialObjectSpawn()
    {
        for (int i = 0; i < initialSpawnCount; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(leftBound, rightBound), Random.Range(bottomBound, topBound), 0f);
            GameObject cloud = Instantiate(NormalCloudPrefab, randomPos, Quaternion.identity);  //ランダムな位置に初期スポーン
            // Debug.Log(randomPos + standardPos);
            CloudSpawnSettings(cloud);
        }

        for (int i = 0; i < initialSpawnCount; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(leftBound, rightBound), Random.Range(bottomBound, topBound), 0f);
            GameObject bird = Instantiate(BirdPrefab, randomPos, Quaternion.identity);  //ランダムな位置に初期スポーン
            BirdSpawnSettings(bird);
        }
    }

    IEnumerator SpawnNormalCloud()
    {
        while (true)
        {
            for (int i = 0; i < cloudSpawnCount; i++)
            {
                GameObject cloud = Instantiate(NormalCloudPrefab, new Vector3(rightBound, Random.Range(bottomBound, topBound), 0f), Quaternion.identity);
                CloudSpawnSettings(cloud);
                yield return new WaitForSeconds(intervalBetweenSpawn);
            }
            yield return new WaitForSeconds(intervalAfterCycle);
        }
    }

    IEnumerator SpawnBird()
    {
        //太陽の異変時は鳥の出現数を増やす
        int currentMaxBird;
        if (SkyManager.isSun)
        {
            currentMaxBird = birdSpawnAnomaly;
        }
        else
        {
            currentMaxBird = birdSpawnCount;
        }

        while (true)
        {
            for (int i = 0; i < currentMaxBird; i++)
            {
                GameObject bird = Instantiate(BirdPrefab, new Vector3(rightBound, Random.Range(bottomBound, topBound), 0f), Quaternion.identity);
                BirdSpawnSettings(bird);
                if (SkyManager.isSun)
                {
                    yield return new WaitForSeconds(birdSpawnAnomalyDelay);
                }
                else
                {
                    yield return new WaitForSeconds(intervalBetweenSpawn);
                }
            }
            yield return new WaitForSeconds(intervalAfterCycle);
        }
    }

    IEnumerator SpawnThunderCloud()
    {
        while (true)
        {
            for (int i = 0; i < thunderSpawnCount; i++)
            {
                GameObject thunder = Instantiate(ThunderCloudPrefab, new Vector3(rightBound, Random.Range(bottomBound, topBound), 0f), Quaternion.identity);
                ThunderSpawnSettings(thunder);
                yield return new WaitForSeconds(intervalBetweenSpawn);
            }
            yield return new WaitForSeconds(intervalAfterCycle);
        }
    }

    IEnumerator ActiveSunAnomaly()
    {
        while (true)
        {
            if (!hasSpawnSun)
            {
                hasSpawnSun = true;
                GameObject sun = Instantiate(SunPrefab, new Vector3(0, bottomBound, 0), Quaternion.identity);
                SunSpawnSettings(sun);
            }
            yield return new WaitForSeconds(intervalAfterCycle);
        }
    }

    IEnumerator ActiveMosquitoAnomaly()
    {
        while (true)
        {
            int spawnableCount = maxMosquitoCount - currentMosquitoCount;
            for (int i = 0; i < spawnableCount; i++)
            {
                GameObject mosquito = Instantiate(MosquitoPrefab, new Vector3(rightBound, Random.Range(bottomBound, topBound), 0f), Quaternion.identity);
                MosquitoSpawnSettings(mosquito);
                yield return new WaitForSeconds(mosquitoSpawnDelay);
            }
            yield return new WaitForSeconds(0);
        }
    }

    IEnumerator BirdMoveInArc(GameObject obj, float duration)
    {
        if (obj == null) yield break;   //エラーが出ないように確認

        float currentRotationZ = 0f;
        float stepAngle = 1f;
        float interval = 0.1f;

        Vector3 moveDir = Vector3.left;
        var birdDrop = obj.AddComponent<SkyObjectMoveLeft>();

        birdDrop.speed = objectSpeed;
        birdDrop.moveDirection = moveDir;

        yield return new WaitForSeconds(duration);

        if (SkyManager.isSun)
        {
            //子オブジェクトを有効化
            Transform child = obj.transform.Find("SpeechBubble");
            if (child != null)
            {
                child.gameObject.SetActive(true);
            }

            while (currentRotationZ < birdTargetRotationZ)
            {
                if (obj == null) yield break;

                currentRotationZ += stepAngle;
                if (currentRotationZ >= accelTiming && birdDrop.speed < maxBirdAccel)
                {
                    birdDrop.speed += birdAccel;
                }

                obj.transform.rotation = Quaternion.Euler(0f, 0f, currentRotationZ);
                float angleRad = currentRotationZ * Mathf.Deg2Rad;
                Vector3 newDirection = new Vector3(-Mathf.Cos(angleRad), -Mathf.Sin(angleRad), 0f);
                birdDrop.moveDirection = newDirection.normalized;

                if (obj.TryGetComponent<SpriteRenderer>(out var sr) && sr.sprite != dropBirdSprite)
                {
                    sr.sprite = dropBirdSprite;
                    Destroy(obj.GetComponent<BirdSpriteChanger>());
                }

                yield return new WaitForSeconds(interval);
            }
        }
        else
        {
            obj.AddComponent<SkyObjectSinMove>();
        }
    }


    IEnumerator MoveInArcToCenter(GameObject obj, float duration)
    {
        Vector3 startPos = new Vector3(leftBound, bottomBound, 0);  //sunの初期位置
        Vector3 endPos = new Vector3(SunCenterX, SunCenterY, 0);    //移動対象位置
        float arcHeight = 3f;
        float elapsed = 0f;

        obj.transform.position = startPos;

        while (elapsed < duration)
        {
            //円上を移動
            float t = elapsed / duration;
            float height = Mathf.Sin(t * Mathf.PI) * arcHeight;
            obj.transform.position = Vector3.Lerp(startPos, endPos, t) + Vector3.up * height;
            elapsed += Time.deltaTime;
            yield return null;

        }
        obj.transform.position = endPos;
    }

    IEnumerator MoveInArcFromCenter(GameObject obj, float duration)
    {
        Vector3 startPos = new Vector3(SunCenterX, SunCenterY, 0);  //sunの初期位置
        Vector3 endPos = new Vector3(rightBound, bottomBound, 0);    //移動対象位置
        float arcHeight = 3f;
        float elapsed = 0f;

        obj.transform.position = startPos;

        while (elapsed < duration)
        {
            //円上を移動
            float t = elapsed / duration;
            float height = Mathf.Sin(t * Mathf.PI) * arcHeight;
            obj.transform.position = Vector3.Lerp(startPos, endPos, t) + Vector3.down * height;
            elapsed += Time.deltaTime;
            yield return null;

        }
        obj.transform.position = endPos;
        Destroy(obj);
        hasSpawnSun = false;
        sunSetRoutine = null;
        StopCoroutine(MoveInArcFromCenter(obj, duration));
    }

    void CloudSpawnSettings(GameObject cloud)
    {
        //雲系統生成時の設定
        cloud.transform.localScale = Vector3.one * Random.Range(minCloudScale, maxCloudScale);    //サイズをランダムにする
        cloud.AddComponent<SkyObjectMoveLeft>().speed = objectSpeed;
        cloud.AddComponent<SkyObjectDestroy>().leftBound = leftBound;
        HierarchyAppear(cloud); //hierarchyに表示させるか
        SetSortingOrder(cloud, cloudLayer);     //表示優先度の設定
    }

    void BirdSpawnSettings(GameObject bird)
    {
        bird.AddComponent<SkyObjectDestroy>().leftBound = leftBound;
        HierarchyAppear(bird); //hierarchyに表示させるか

        if (SkyManager.isSun)
        {
            StartCoroutine(BirdMoveInArc(bird, Random.Range(minBirdDropTime, maxBirdDropTime)));
        }
        else
        {
            bird.AddComponent<SkyObjectSinMove>().speed = objectSpeed;
        }
        SetSortingOrder(bird, birdLayer);   //表示優先度の設定
    }

    void ThunderSpawnSettings(GameObject thunder)
    {
        //雲系統生成時の設定
        thunder.transform.localScale = Vector3.one * Random.Range(minCloudScale, maxCloudScale);    //サイズをランダムにする
        thunder.AddComponent<SkyObjectMoveLeft>().speed = objectSpeed;
        thunder.AddComponent<SkyObjectDestroy>().leftBound = leftBound;
        HierarchyAppear(thunder); //hierarchyに表示させるか
        SetSortingOrder(thunder, thunderLayer);     //表示優先度の設定
    }

    void SunSpawnSettings(GameObject sun)
    {
        sun.transform.localScale = new Vector3(SunStandardSize, SunStandardSize, SunStandardSize);    //サイズ設定
        sun.AddComponent<SkyObjectDestroy>().leftBound = leftBound;
        HierarchyAppear(sun); //hierarchyに表示させるか
        SetSortingOrder(sun, sunLayer);  //表示優先度の設定
        StartCoroutine(MoveInArcToCenter(sun, sunSpeed));  //sunSpeedの時間をかけて中央へ
    }

    void MosquitoSpawnSettings(GameObject mosquito)
    {
        mosquito.transform.localScale = GetRandomMosquitoScale();
        mosquito.transform.Rotate(0, 180, 0);
        HierarchyAppear(mosquito); //hierarchyに表示させるか
        var mover = mosquito.AddComponent<SkyObjectSinMove>();
        mover.speed = objectSpeed;
        mover.dropMosquito = true;
        mover.frequency = 2f;   //移動の滑らかさ
        mover.magnitude = 0.5f;     //sinグラフの振幅
        mosquito.AddComponent<SkyObjectDestroy>().leftBound = leftBound;
        SetSortingOrder(mosquito, mosquitoLayer);   //表示優先度の設定
        currentMosquitoCount++;
    }

    Vector3 GetRandomMosquitoScale()
    {
        Vector3[] scales = {
            new Vector3(0.5f, 0.5f, 1f),
            new Vector3(0.8f, 0.8f, 1f),
            new Vector3(1.3f, 1.3f, 1f)
        };
        return scales[Random.Range(0, scales.Length)];
    }

    //表示位置の調整
    void SetSortingOrder(GameObject obj, int order)
    {
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sortingOrder = order;
    }

    void HierarchyAppear(GameObject obj)
    {
        //falseの時Hierarchyに生成オブジェクトを表示させない
        if (!objAppear)
        {
            obj.hideFlags = HideFlags.HideInHierarchy;    //Hierarchyに表示させない
        }
    }
}
