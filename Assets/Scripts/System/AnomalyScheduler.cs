using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement; // For .ToArray() and other LINQ methods

public class AnomalyScheduler : MonoBehaviour
{
    [System.Serializable]
    public class AnomalyScheduleEntry
    {
        public float delaySeconds;
        bool activateAnomaly = true;
        [Tooltip("このスケジュールで使用するマネージャーの名前（GameObject名）")]
        public string targetManagerName;

        [Tooltip("このマネージャーでアクティブにする異変の名前を選択してください。")]
        public string anomalyName;

        [Tooltip("このスケジュールエントリでマネージャーをランダムに選択します。")]
        public bool selectRandomManager = false;

        [Tooltip("このスケジュールエントリで異変をランダムに選択します。")]
        public bool selectRandomAnomaly = false;


        // インスペクタでプルダウン表示するための変数 (エディタ拡張で使用)
        [HideInInspector] public List<string> availableAnomalyNames = new List<string>();
        [HideInInspector] public List<MonoBehaviour> availableManagers = new List<MonoBehaviour>();
    }

    public List<AnomalyScheduleEntry> anomalySchedules = new List<AnomalyScheduleEntry>();

    [Header("Global Settings")]
    [Tooltip("シーン内の全てのIDreamAnomalyを実装したマネージャーを自動検出します。")]
    public bool autoDetectAllManagers = true;

    // シーン内の全てのIDreamAnomalyマネージャーを保持するリスト
    public List<IDreamAnomaly> allAnomalyManagers = new List<IDreamAnomaly>();
    public List<Camera> AllSceneCameras = new List<Camera>();
    
    // Display1CameraWatcher参照用
    private Display1CameraWatcher display1CameraWatcher;
    
    [Tooltip("読み込むシーン名をここに入力してください（Additiveで読み込まれます）")]
    [SerializeField]
    private string[] sceneNames = { };

    // Unityエディタでのみ実行される特別なメソッド
    void OnValidate()
    {
        if (autoDetectAllManagers)
        {
            // エディタ上でマネージャーを再度検出
            allAnomalyManagers.Clear();
            IDreamAnomaly[] managers = FindObjectsOfType<MonoBehaviour>().OfType<IDreamAnomaly>().ToArray();
            allAnomalyManagers.AddRange(managers);
        }

        foreach (var entry in anomalySchedules)
        {
            // ランダムマネージャー選択時、利用可能なマネージャーを更新
            if (entry.selectRandomManager)
            {
                entry.availableManagers.Clear();
                foreach (var manager in allAnomalyManagers)
                {
                    if (manager is MonoBehaviour mono) // MonoBehaviourにキャストできるもののみ
                    {
                        entry.availableManagers.Add(mono);
                    }
                }
            }
            else
            {
                entry.availableManagers.Clear(); // ランダム選択しない場合はリストをクリア
            }

            // ランダム異変選択時、または特定のマネージャーが指定されている場合
            if (entry.selectRandomAnomaly || (entry.targetManagerName != null && !entry.selectRandomManager))
            {
                IDreamAnomaly anomalyManager = null;
                if (entry.selectRandomAnomaly && entry.selectRandomManager)
                {
                    // ランダムなマネージャーの異変リストは事前に分からないため、ここでは提供できない
                    // 実行時にランダム選択する
                    entry.availableAnomalyNames.Clear();
                    entry.availableAnomalyNames.Add("Random Anomaly"); // インスペクタ表示用
                }
                else if (!string.IsNullOrEmpty(entry.targetManagerName))
                {
                    anomalyManager = allAnomalyManagers
                        .FirstOrDefault(m => ((MonoBehaviour)m).name == entry.targetManagerName);
                }

                if (anomalyManager != null)
                {
                    entry.availableAnomalyNames = new List<string>(anomalyManager.GetAnomalyNames());
                    entry.availableAnomalyNames.Insert(0, "None"); // 「None」オプションを追加
                    if (entry.selectRandomAnomaly)
                    {
                        entry.availableAnomalyNames.Insert(1, "Random Anomaly"); // 「Random Anomaly」オプションを追加
                    }
                }
                else
                {
                    entry.availableAnomalyNames.Clear();
                    if (!entry.selectRandomManager)
                    {
                        Debug.LogWarning($"{entry.targetManagerName?? "N/A"} does not implement IDreamAnomaly, or target not assigned. Please assign a script that implements IDreamAnomaly.");
                    }
                }
            }
            else
            {
                entry.availableAnomalyNames.Clear();
            }
        }
    }

    void Start()
    {
        // Display1CameraWatcherをシーンから探す
        display1CameraWatcher = FindObjectOfType<Display1CameraWatcher>();
        StartCoroutine(LoadAndFetchAnomalies());
    }

    IEnumerator ProcessAnomalySchedule(AnomalyScheduleEntry schedule)
    {
        int entryIndex = anomalySchedules.IndexOf(schedule);
        yield return new WaitForSeconds(schedule.delaySeconds);

        // 除外するManager名を決定
        string excludeManagerName = null;
        if (display1CameraWatcher != null && display1CameraWatcher.currentDisplay1Camera != null)
        {
            string camName = display1CameraWatcher.currentDisplay1Camera.name;
            if (camName.StartsWith("R") && camName.Length >= 2 && camName[1] >= '0' && camName[1] <= '3')
            {
                int idx = camName[1] - '0';
                excludeManagerName = $"Dream{idx}Manager";
            }
        }

        IDreamAnomaly managerToActivate = null;
        string anomalyToActivate = schedule.anomalyName;

        // Step 1: マネージャーの選択
        if (schedule.selectRandomManager)
        {
            // 除外対象を除いたリストを作成
            List<IDreamAnomaly> availableManagers = new List<IDreamAnomaly>(allAnomalyManagers);
            if (!string.IsNullOrEmpty(excludeManagerName))
            {
                int beforeCount = availableManagers.Count;
                availableManagers = availableManagers.Where(m => ((MonoBehaviour)m).name != excludeManagerName).ToList();
                int afterCount = availableManagers.Count;
                if (beforeCount != afterCount)
                {
                    Debug.Log($"Display1CameraWatcher: 現在のカメラ({display1CameraWatcher.currentDisplay1Camera.name})に対応するマネージャー {excludeManagerName} を除外しました。");
                }
            }

            if (availableManagers.Count == 0)
            {
                Debug.LogWarning($"除外後、選択可能なマネージャーがありません: {excludeManagerName}");
                yield break;
            }

            int maxTries = availableManagers.Count;
            int attempt = 0;

            do
            {
                managerToActivate = availableManagers[Random.Range(0, availableManagers.Count)];
                string[] anomalyNames = managerToActivate.GetAnomalyNames();

                // すでに1つでもアクティブな異変があるかチェック
                bool hasActiveAnomaly = false;
                foreach (string name in anomalyNames)
                {
                    if (managerToActivate.IsAnomalyActive(name))
                    {
                        hasActiveAnomaly = true;
                        break;
                    }
                }

                if (hasActiveAnomaly)
                {
                    availableManagers.Remove(managerToActivate); // 再抽選から除外
                    managerToActivate = null;
                }

                attempt++;
            } while (managerToActivate == null && attempt < maxTries);

            if (managerToActivate != null)
            {
                Debug.Log(
                    $"AnomalyScheduler: Selected manager without active anomalies: {((MonoBehaviour)managerToActivate).name}");
            }
            else
            {
                Debug.Log("AllAnomalyActive");
                yield break;
            }
        }
        else
        {
            if (string.IsNullOrEmpty(schedule.targetManagerName))
            {
                Debug.LogWarning("Target Manager Name is empty and random selection is off. Skipping this entry.");
                yield break;
            }
            // 除外対象ならスキップ
            if (!string.IsNullOrEmpty(excludeManagerName) && schedule.targetManagerName == excludeManagerName)
            {
                Debug.Log($"Display1CameraWatcher: 現在のカメラ({display1CameraWatcher.currentDisplay1Camera.name})に対応するマネージャー {excludeManagerName} を除外したためスキップします。");
                yield break;
            }
            managerToActivate = allAnomalyManagers
                .FirstOrDefault(m => ((MonoBehaviour)m).name == schedule.targetManagerName);

            if (managerToActivate == null)
            {
                Debug.LogError($"Manager with name '{schedule.targetManagerName}' not found or does not implement IDreamAnomaly.");
                yield break;
            }
        }

        // Step 2: 異変の選択
        if (schedule.selectRandomAnomaly)
        {
            string[] availableAnomalies = managerToActivate.GetAnomalyNames();
            if (availableAnomalies.Length == 0)
            {
                Debug.LogWarning($"Selected manager {((MonoBehaviour)managerToActivate).name} has no anomalies. Skipping this entry.");
                yield break;
            }
            anomalyToActivate = availableAnomalies[Random.Range(0, availableAnomalies.Length)];
            Debug.Log($"AnomalyScheduler: Randomly selected anomaly for {((MonoBehaviour)managerToActivate).name}: {anomalyToActivate}");
        }

        // Step 3: 異変の適用
        if (anomalyToActivate == "None")
        {
            foreach (string name in managerToActivate.GetAnomalyNames())
            {
                managerToActivate.SetAnomalyState(name, false);
            }
            Debug.Log($"AnomalyScheduler: All anomalies on {((MonoBehaviour)managerToActivate).name} have been reset.");
            Debug.Log($"異変発生：エントリー{entryIndex} マネージャー「{((MonoBehaviour)managerToActivate).name}」に異変「{anomalyToActivate}」を適用しました．");
        }
        else
        {
            managerToActivate.SetAnomalyState(anomalyToActivate, true);
            Debug.Log($"異変発生：エントリー{entryIndex} マネージャー「{((MonoBehaviour)managerToActivate).name}」に異変「{anomalyToActivate}」を適用しました．");
        }
    }
    IEnumerator LoadAndFetchAnomalies()
    {
        List<AsyncOperation> loadOperations = new List<AsyncOperation>();

        // sceneNames の中身をすべて Additive で読み込む
        foreach (string sceneName in sceneNames)
        {
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            loadOperations.Add(op);
        }

        // すべての読み込み完了を待つ
        yield return new WaitUntil(() => loadOperations.All(op => op.isDone));

        // シーン内のルート GameObject を取得し，異変マネージャとカメラを探す
        foreach (string sceneName in sceneNames)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
            {
                Debug.LogWarning($"Scene {sceneName} is not loaded properly.");
                continue;
            }

            foreach (var root in scene.GetRootGameObjects())
            {
                allAnomalyManagers.AddRange(root.GetComponentsInChildren<IDreamAnomaly>(true));
                AllSceneCameras.AddRange(root.GetComponentsInChildren<Camera>(true));
            }
        }

        // すべてのシーン読み込み完了後にDreamCameraChangeの初期化を呼ぶ
        var cameraChanger = FindObjectOfType<DreamCameraChange>();
        if (cameraChanger != null)
        {
            cameraChanger.InitializeCameras();
            Debug.Log("DreamCameraChange.InitializeCameras() を呼び出しました");
        }
        else
        {
            Debug.LogWarning("DreamCameraChangeがシーン内に見つかりませんでした");
        }

        foreach (var schedule in anomalySchedules)
        {
            StartCoroutine(ProcessAnomalySchedule(schedule));
        }
    }
}