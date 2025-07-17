using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
// using static UnityEditor.SceneView;

public class Timer : MonoBehaviour
{
    // インスペクタで編集可能なパラメータ
    [Header("現実時間の何秒がゲーム内時間に何サイクル値するか")]
    [SerializeField] private float realSecondsPerGameMinute = 5f; // 実時間5秒でゲーム時間15分
    [Header("1サイクルで加算するゲーム内時間（分）")]
    [SerializeField] private int gameMinutesToAdd = 15;           // 1サイクルで加算するゲーム内分数
    [Header("60分で1時間")]
    [SerializeField] private int minutesPerHour = 60;             // 60分で1時間
    [Header("ゲーム終了時刻")]
    [SerializeField] private int maxGameHours = 9;                // 9時間でゲーム終了
    [Header("スタートパネル表示時間（秒）")]
    [SerializeField] private float startPanelDisplaySeconds = 3f; // スタートパネル表示時間
    [Header("リザルトカメラの追加待機時間（秒）")]
    [SerializeField] private float resultCameraWaitBuffer = 0.5f; // リザルトカメラの追加待機時間
    [Header("黒フェード")]
    [SerializeField] private Image blackPanel;
    [SerializeField] private float fadeInDuration = 1f;

    [SerializeField] private Image startText;
    [Header("スタートテキスト拡大時間（秒）")]
    [SerializeField] private float startTextZoomDuration = 1f;

    [SerializeField] private GameObject dreamOverlayCanvas;

    private float time = 0;
    private int h = 0, m = 0;
    public bool isResult = false;
    public TextMeshProUGUI Text_h, Text_m;
    public GameObject TimeCanvas;
    public GameObject StartPannel;
    public GameObject FinishPannel;

    public ResultManager resultManager;
    public RealToDream realToDream;

    GameObject GetActiveCamera()
    {
        string[] camNames = {
        "Entrance Camera",
        "R0 Camera",
        "R1 Camera",
        "R2 Camera",
        "R3 Camera",
        "R4 Camera",
        "R5 Camera"
    };

        foreach (var name in camNames)
        {
            GameObject cam = GameObject.Find(name);
            if (cam != null && cam.activeInHierarchy)
            {
                return cam;
            }
        }
        return null;
    }


    void Start()
    {
        
        Time.timeScale = 0;
        StartCoroutine("GameStart");
    }

    void Update()
    {
        // 実時間
        time += Time.deltaTime;

        // 実時間5秒でゲーム時間15分
        if (time > realSecondsPerGameMinute)
        {
            m += gameMinutesToAdd;
            time = 0;
        }
        // if (time > 1)
        // {
        //     m += 30;
        //     time = 0;
        // }
        // 60分経ったら1時間に変換
        if (m == minutesPerHour)
        {
            h += 1;
            m = 0;
        }
        // 9時間経ったらゲーム終了

        if (h == maxGameHours && m == 0 && !isResult)
        {
            isResult = true;
            SoundManager.Instance.PlaySE("Result");
            StartCoroutine(GameFinish());
        }

        // 時間を表示
        Text_h.text = h.ToString("0");
        Text_m.text = m.ToString("00");
    }

    IEnumerator GameStart()
    {
        StartPannel.SetActive(true);
        startText.gameObject.SetActive(true);
        startText.transform.localScale = Vector3.zero;

        // SoundManager.Instance.PlaySE("Go");

        // 並列処理用コルーチン開始
        Coroutine fadeCoroutine = null;
        if (blackPanel != null)
        {
            fadeCoroutine = StartCoroutine(FadeBlack(1f, 0f, fadeInDuration));
        }

        float zoomDuration = startTextZoomDuration;
        float elapsed = 0f;
        Vector3 targetScale = new Vector3(9f, 4f, 4f);
        while (elapsed < zoomDuration)
        {
            float t = elapsed / zoomDuration;
            float eased = Mathf.Sin(t * Mathf.PI * 0.5f);
            startText.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, eased);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        startText.transform.localScale = Vector3.one;

        // フェードが終わるまで待つ
        if (fadeCoroutine != null)
        {
            yield return fadeCoroutine;
            blackPanel.gameObject.SetActive(false);
        }

        StartPannel.SetActive(false);
        Time.timeScale = 1;
    }
    private IEnumerator FadeBlack(float fromAlpha, float toAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = blackPanel.color;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            color.a = Mathf.Lerp(fromAlpha, toAlpha, t);
            blackPanel.color = color;
            elapsed += Time.unscaledDeltaTime; 
            yield return null;
        }

        color.a = toAlpha;
        blackPanel.color = color;
    }


    IEnumerator GameFinish()
    {
        Time.timeScale = 0;
        if (dreamOverlayCanvas != null)
        {
            dreamOverlayCanvas.gameObject.SetActive(true);
        }

        int score = realToDream.score;
        //FinishPannel.SetActive(true);
        if (TimeCanvas != null)
        {
            TimeCanvas.SetActive(false);
        }
        GameObject cam = GetActiveCamera();
        if (cam != null)
        {
            ResultCameraMover mover = cam.GetComponent<ResultCameraMover>();
            if (mover != null)
            {
                mover.BeginMove();
                yield return new WaitForSecondsRealtime(mover.duration + resultCameraWaitBuffer);
            }
            
            resultManager.ShowResult(score);
        }
    }
}
