using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Image Arrow;
    [SerializeField] private GameObject InitialText;
    [SerializeField] private TextMeshProUGUI SettingArrow;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SESlider;

    [SerializeField] private Transform BGMBarParent;
    [SerializeField] private Transform SEBarParent;

    [SerializeField] private Vector3 BGMStartPos = new Vector3(-3f, 4f, 0f);
    [SerializeField] private Vector3 SEStartPos = new Vector3(-3f, 2.7f, 0f);

    [SerializeField] private bool isShowSetting = false;

    [Header("Effect")]
    [SerializeField] private Image[] blackPanels; // ← 配列に変更
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float blackDuration = 2f; 
    private GameObject SettingPanel;
    private LangageChanger languageChanger;

    private string startScene = "Product";
    private string tutorialScene = "Tutorial";
    private string CreditScene = "Credit";
    private float[] height = { 2f, 0.8f, -0.4f, -1.6f };
    private float[] settingsPosX = { -6.4f, -5.4f, 0.4f, -1.5f };
    private float[] settingsPosY = { 5f, 3.6f, 2.3f, 0.4f, -0.9f };
    private int arrowPos = 0;
    private int settingArrowX = 0;
    private int settingArrowY = 0;
    private int buttonNum = 4;  //選択肢の数
    private bool isUseSlider = false;    //スライダーの値を変更しているかどうか
    private float prevStickY = 0f; // 追加
    private float prevStickX = 0f; // 追加

    private Color blockStartColor = new Color(1f, 0.8f, 0.3f);  // 濃いオレンジ
    private Color blockEndColor = new Color(1f, 0.4f, 0f); // 明るめのオレンジ（薄め）
    private Color blockOffColor = Color.gray;

    private GameObject[] bgmCubes;
    private GameObject[] seCubes;
    private const int blockCount = 10;
    private float spacing = 0.6f;

    private Shader urpLitShader; // 追加

    void Start()
    {
        // URP Lit Shaderを取得
        urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLitShader == null)
        {
            Debug.LogError("URP Lit Shaderが見つかりません。Graphics Settings を確認してください。");
        }

        languageChanger = this.AddComponent<LangageChanger>();
        Time.timeScale = 1;

        Transform _child = transform.Find("Settings");
        SettingPanel = _child.gameObject;

        GenerateVolumeCubes(ref bgmCubes, BGMBarParent, BGMStartPos);
        GenerateVolumeCubes(ref seCubes, SEBarParent, SEStartPos);

        AppearSettingPanel(isShowSetting);
    }

    void Update()
    {
        Vector2 stick = Gamepad.current?.leftStick.ReadValue() ?? Vector2.zero;

        // スティック上下のトリガー判定
        bool up = Gamepad.current?.dpad.up.wasPressedThisFrame == true || Input.GetKeyDown(KeyCode.UpArrow)
            || (prevStickY < 0.5f && stick.y > 0.5f); // 上に倒した瞬間
        bool down = Gamepad.current?.dpad.down.wasPressedThisFrame == true || Input.GetKeyDown(KeyCode.DownArrow)
            || (prevStickY > -0.5f && stick.y < -0.5f); // 下に倒した瞬間

        // スライダー調整時 or 言語選択時のみ左右スティックを有効化
        bool right = false;
        bool left = false;
        if (isShowSetting && (isUseSlider || settingArrowX == 1 || settingArrowX == 2))
        {
            right = Gamepad.current?.dpad.right.wasPressedThisFrame == true 
                || Input.GetKeyDown(KeyCode.RightArrow) 
                || (prevStickX < 0.2f && stick.x > 0.2f); // 右に倒した瞬間

            left = Gamepad.current?.dpad.left.wasPressedThisFrame == true 
                || Input.GetKeyDown(KeyCode.LeftArrow) 
                || (prevStickX > -0.2f && stick.x < -0.2f); // 左に倒した瞬間
        }
        else
        {
            right = Gamepad.current?.dpad.right.wasPressedThisFrame == true || Input.GetKeyDown(KeyCode.RightArrow);
            left = Gamepad.current?.dpad.left.wasPressedThisFrame == true || Input.GetKeyDown(KeyCode.LeftArrow);
        }

        bool select = Gamepad.current?.buttonSouth.wasPressedThisFrame == true || Input.GetKeyDown(KeyCode.Return);
        bool back = Gamepad.current?.buttonEast.wasPressedThisFrame == true || Input.GetKeyDown(KeyCode.RightShift);

        prevStickY = stick.y; // 最後に必ず保存
        prevStickX = stick.x; // ←追加

        if (!isShowSetting)     //通常
        {
            if (up)
            {
                SoundManager.Instance.PlaySE("Cursor");
                arrowPos = (arrowPos - 1 + buttonNum) % buttonNum;
                UpdateArrowPos();
            }

            if (down)
            {
                SoundManager.Instance.PlaySE("Cursor");
                arrowPos = (arrowPos + 1) % buttonNum;
                UpdateArrowPos();
            }

            if (select)
            {
                SoundManager.Instance.PlaySE("Go");
                switch (arrowPos)
                {
                    case 0:
                        StartCoroutine(LoadSceneWithStartEffect(startScene));
                        break;
                    case 1:
                        StartCoroutine(LoadSceneWithDelay(tutorialScene));
                        break;
                    case 2:
                        StartCoroutine(LoadSceneWithDelay(CreditScene));
                        break;
                    case 3:
                        AppearSettingPanel(true);
                        break;
                    default:
                        Debug.Log("Error of arrowPos");
                        break;
                }
            }
        }
        else
        {
            if (up || down)
            {
                SoundManager.Instance.PlaySE("Cursor");
                if (settingArrowX == 0)
                {
                    settingArrowY = settingArrowY == 0 ? 3 : 0;
                }
                else if (settingArrowY < 3)
                {
                    settingArrowY = settingArrowY == 1 ? 2 : 1;
                }
            }

            if (right || left)
            {
                if (isUseSlider)
                {
                    SoundManager.Instance.PlaySE("VolumeChange");
                    Slider _slider;
                    _slider = settingArrowY == 1 ? BGMSlider : SESlider;
                    _slider.value += right ? 5 : -5;
                }
                // ▼ここを修正
                else if (settingArrowX == 1 || settingArrowX == 2)
                {
                    SoundManager.Instance.PlaySE("Cursor");
                    // 1⇔2をトグル
                    settingArrowX = (settingArrowX == 1) ? 2 : 1;
                }
            }

            if (select)
            {
                SoundManager.Instance.PlaySE("Go");
                if (!isUseSlider)
                {
                    if (settingArrowX == 0)
                    {
                        settingArrowX = 1;
                        settingArrowY = settingArrowY == 0 ? 1 : 4;
                    }
                    else if (settingArrowY == 1 || settingArrowY == 2)
                    {
                        isUseSlider = true;
                        settingArrowX = 3;
                    }
                    else if (settingArrowX == 1)
                    {
                        languageChanger.SetJapanese();
                    }
                    else
                    {
                        languageChanger.SetEnglish();
                    }
                }
                else
                {
                    isUseSlider = false;
                    settingArrowX = 1;
                }
            }

            if (back)
            {
                SoundManager.Instance.PlaySE("Back");
                if (!isUseSlider)
                {
                    if (settingArrowX == 0)
                    {
                        AppearSettingPanel(false);
                    }
                    else
                    {
                        settingArrowX = 0;
                        settingArrowY = settingArrowY != 4 ? 0 : 3;
                    }
                }
                else
                {
                    isUseSlider = false;
                    settingArrowX = 1;
                }
            }

            UpdateSettingArrowPos();
        }

        UpdateCubeVolumeBar(bgmCubes, BGMSlider);
        UpdateCubeVolumeBar(seCubes, SESlider);
    }

    void UpdateArrowPos()
    {
        Vector3 pos = Arrow.transform.localPosition;
        pos.y = height[arrowPos];
        Arrow.transform.localPosition = pos;
    }

    void UpdateSettingArrowPos()
    {
        Vector3 pos = SettingArrow.transform.localPosition;
        pos.x = settingsPosX[settingArrowX];
        pos.y = settingsPosY[settingArrowY];
        SettingArrow.transform.localPosition = pos;
    }

    void AppearSettingPanel(bool appear)
    {
        isShowSetting = appear;
        SettingPanel.SetActive(appear);
        Arrow.gameObject.SetActive(!appear);
        InitialText.SetActive(!appear);
    }

    void GenerateVolumeCubes(ref GameObject[] targetArray, Transform parent, Vector3 startPos)
    {
        targetArray = new GameObject[blockCount];

        for (int i = 0; i < blockCount; i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(parent, false);
            cube.transform.localPosition = startPos + new Vector3(i * spacing, 0, 0);
            cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.01f);

            Renderer rend = cube.GetComponent<Renderer>();

            // URP Lit Shaderを使ったマテリアルを割り当て
            Material mat = new Material(urpLitShader);
            mat.color = blockOffColor;
            rend.material = mat;

            targetArray[i] = cube;
        }
    }

    void UpdateCubeVolumeBar(GameObject[] cubes, Slider slider)
    {
        float normalized = Mathf.InverseLerp(slider.minValue, slider.maxValue, slider.value);

        int count = Mathf.RoundToInt(normalized * cubes.Length);
        for (int i = 0; i < cubes.Length; i++)
        {
            Renderer rend = cubes[i].GetComponent<Renderer>();
            if (rend == null) continue;

            if (i < count)
            {
                float t = (float)i / (cubes.Length - 1);
                rend.material.color = Color.Lerp(blockStartColor, blockEndColor, t);
            }
            else
            {
                rend.material.color = blockOffColor;
            }
        }
    }
    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        yield return new WaitForSeconds(0.3f); // 0.3秒待つ（必要に応じて調整）
        SceneManager.LoadScene(sceneName);
    }
    //effect
    private IEnumerator LoadSceneWithStartEffect(string sceneName)
    {
        InitialText.SetActive(false);
        Arrow.gameObject.SetActive(false);
        
        yield return StartCoroutine(FadeBlack(0f,1f,fadeOutDuration));
        SoundManager.Instance.PlaySE("Fade");
        yield return new WaitForSeconds(blackDuration);
        SceneManager.LoadScene(sceneName);
    }
    private IEnumerator FadeBlack(float fromAlpha, float toAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            foreach (var blackPanel in blackPanels)
            {
                if (blackPanel != null)
                {
                    Color color = blackPanel.color;
                    color.a = Mathf.Lerp(fromAlpha, toAlpha, t);
                    blackPanel.color = color;
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        foreach (var blackPanel in blackPanels)
        {
            if (blackPanel != null)
            {
                Color color = blackPanel.color;
                color.a = toAlpha;
                blackPanel.color = color;
            }
        }
    }
}
