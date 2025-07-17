using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public GameObject[] cloudPrefabs; // 雲の種類
    public GameObject bikePrefab;
    public float spawnIntervalmin = 2f;
    public float spawnIntervalMax = 2f;
    public float moveSpeed = 1f;
    
    public float SpawnOffsetZ = 2f;

    public float spawnOffsetMin = -2f; // オフセットの最小値（public化）
    public float spawnOffsetMax = 2f;  // オフセットの最大値（public化）

    public Dream2Manager manager;

    void Start()
    
    {
        
        for (int i = 0; i < 5; i++)
        {
            SpawnInitialCloud(i);
        }
        // 最初の1回と、ランダム間隔でSpawnCloudを呼ぶ
        InvokeRepeating(nameof(SpawnCloud), 0f, Random.Range(spawnIntervalmin, spawnIntervalMax));
    }

    void SpawnCloud()
    {
        int index = Random.Range(0, cloudPrefabs.Length);

        // publicな範囲からランダムなX方向オフセットを取得
        float offsetX = Random.Range(spawnOffsetMin, spawnOffsetMax);
        Vector3 spawnPos = transform.position + new Vector3(offsetX, 0f, Random.Range(-SpawnOffsetZ,SpawnOffsetZ));

        if (manager.isNormal()||manager.isIce)
        {
            // 雲の生成
            GameObject cloud = Instantiate(cloudPrefabs[index], spawnPos, manager.transform.rotation, transform);
            CloudMover mover = cloud.GetComponent<CloudMover>();
            mover.speed = moveSpeed;
            mover.manager = manager;
        }

    }
    
    void SpawnInitialCloud(int index)
    {
        int prefabIndex = Random.Range(0, cloudPrefabs.Length);
        float offsetX = Random.Range(spawnOffsetMin, spawnOffsetMax);
    
        // 初期生成雲を左方向に一定間隔で配置（例：-5 ~ 0 の範囲に散らす）
        float spacing = 1f + (index * 2.5f); 
        Vector3 spawnPos = transform.position + new Vector3(spacing + offsetX, 0f, Random.Range(-SpawnOffsetZ,SpawnOffsetZ));

        if (manager.isNormal() || manager.isIce)
        {
            GameObject cloud = Instantiate(cloudPrefabs[prefabIndex], spawnPos, manager.transform.rotation, transform);
            CloudMover mover = cloud.GetComponent<CloudMover>();
            mover.speed = moveSpeed;
            mover.manager = manager;
        }
    }
}