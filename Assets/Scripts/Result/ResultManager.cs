using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // 追加

public class ResultManager : MonoBehaviour
{
    public Image[] stars;         
    public Sprite starOn;
    public Sprite starOff;
    public Image titleImage; // Button → Image に変更

    public TextMeshProUGUI supportText;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI kaiText;

    public int supportCount = 10;
    public int firstStarBorder = 1;
    public int SecondStarBorder = 5;
    public int ThirdStarBorder = 10;
    private int starCount = 0;
    private bool skipped = false;
    private Coroutine currentCoroutine;
    private bool alreadyShown = false;

    void Start()
    {
        foreach (var s in stars)
        {
            s.sprite = starOff;
            s.color = new Color(1f, 1f, 1f, 0f); 
        }

        supportText.alpha = 0f;
        countText.alpha = 0f;
        kaiText.alpha = 0f;
        titleImage.enabled = false; // SetActive(false) → enabled = false
    }

    public void ShowResult(int score)
    {
        if (alreadyShown) return;
        alreadyShown = true;
        supportCount = score;

        foreach (var s in stars)
        {
            s.sprite = starOff;
            s.color = new Color(1f, 1f, 1f, 0f);
        }

        supportText.alpha = 0f;
        countText.alpha = 0f;
        kaiText.alpha = 0f;
        titleImage.enabled = false; // SetActive(false) → enabled = false

        if (supportCount >= ThirdStarBorder)
            starCount = 3;
        else if (supportCount >= SecondStarBorder)
            starCount = 2;
        else if (supportCount >= firstStarBorder)
            starCount = 1;
        else
            starCount = 0;

        currentCoroutine = StartCoroutine(PlayResultSequence());
    }

    void Update()
    {
        if (alreadyShown && !skipped && Input.GetKeyDown(KeyCode.Space))//skip
        {
            skipped = true;
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            ShowAllUI();
        }

        // TitleImageが表示された後にゲームパッドSouthボタンでTitleシーンへ
        if (titleImage.enabled && Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Title");
        }
    }

    IEnumerator PlayResultSequence()
    {
        yield return new WaitForSecondsRealtime(1f);

        SoundManager.Instance.PlaySE("Show");
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].sprite = starOff;
            stars[i].color = new Color(1f, 1f, 1f, 1f);
        }

        for (int i = 0; i < starCount; i++)
        {
            yield return new WaitForSecondsRealtime(1f);
            stars[i].sprite = starOn;
            SoundManager.Instance.PlaySE("Star");
        }

        yield return new WaitForSecondsRealtime(1f);
        SoundManager.Instance.PlaySE("Show");
        supportText.alpha = 1f;

        yield return new WaitForSecondsRealtime(1f);
        SoundManager.Instance.PlaySE("Show");
        countText.text = supportCount.ToString();
        countText.alpha = 1f;
        kaiText.alpha = 1f;

        yield return new WaitForSecondsRealtime(1f);
        SoundManager.Instance.PlaySE("Show");
        titleImage.enabled = true; // SetActive(true) → enabled = true
    }

    void ShowAllUI()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].sprite = (i < starCount) ? starOn : starOff;
            stars[i].color = new Color(1f, 1f, 1f, 1f);
        }

        supportText.alpha = 1f;
        countText.text = supportCount.ToString();
        countText.alpha = 1f;
        kaiText.alpha = 1f;
        titleImage.enabled = true; // SetActive(true) → enabled = true
    }
}
