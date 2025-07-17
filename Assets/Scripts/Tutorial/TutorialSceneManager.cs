using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TutorialSceneManager : MonoBehaviour
{
    [SerializeField] private string previousScene = "Title";
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int tutorialCount = 3;
    private GameObject[] Tutorial;
    private float appearX = 0f;
    private float hideX = 12.8f;
    private int nowAppear = 0;
    private int nextAppear;
    private bool isShowNext = false;
    private bool isShowBack = false;

    void Start()
    {
        Tutorial = new GameObject[tutorialCount];
        for (int i = 0; i < tutorialCount; i++)
        {
            Tutorial[i] = transform.Find($"tutorial_{i + 1}").gameObject;

            if (i <= nowAppear)
            {
                Tutorial[i].transform.position = new Vector2(appearX, 0);
            }
            else
            {
                Tutorial[i].transform.position = new Vector2(hideX, 0);
            }
        }
    }

    void Update()
    {
        if (isShowNext)
        {
            Vector2 Pos = Tutorial[nextAppear].transform.position;
            Pos.x -= Time.deltaTime * moveSpeed;
            Tutorial[nextAppear].transform.position = new Vector2(Pos.x, 0);
            if (Pos.x < appearX)
            {
                Pos.x = appearX;
                Tutorial[nextAppear].transform.position = new Vector2(Pos.x, 0);
                nowAppear = nextAppear;
                isShowNext = false;
            }
        }
        else if (isShowBack)
        {
            Vector2 Pos = Tutorial[nowAppear].transform.position;
            Pos.x += Time.deltaTime * moveSpeed;
            Tutorial[nowAppear].transform.position = new Vector2(Pos.x, 0);
            if (Pos.x > hideX)
            {
                Pos.x = hideX;
                Tutorial[nowAppear].transform.position = new Vector2(Pos.x, 0);
                nowAppear = nextAppear;
                isShowBack = false;
            }
        }
        else
        {
            bool next = Gamepad.current?.buttonSouth.wasPressedThisFrame == true || Input.GetKeyDown(KeyCode.RightArrow);
            bool back = Gamepad.current?.buttonEast.wasPressedThisFrame == true || Input.GetKeyDown(KeyCode.LeftArrow);

            if (next)
            {
                SoundManager.Instance.PlaySE("Go");
                nextAppear = nowAppear + 1;
                if (nextAppear > tutorialCount - 1)
                {
                    StartCoroutine(LoadSceneWithDelay());
                }
                else
                {
                    isShowNext = true;
                }
            }
            else if (back)
            {
                SoundManager.Instance.PlaySE("Back");
                nextAppear = nowAppear - 1;
                if (nextAppear < 0)
                {
                    StartCoroutine(LoadSceneWithDelay());
                }
                else
                {
                    isShowBack = true;
                }
            }
        }
    }

    private System.Collections.IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(0.5f); // 0.5秒待つ（必要に応じて調整）
        SceneManager.LoadScene(previousScene);
    }
}
