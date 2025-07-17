using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CreditManager : MonoBehaviour
{
    [SerializeField] private string previousScene = "Title";
    [SerializeField] private bool changeSprite = false;
    [SerializeField] private Sprite[] birdSprite;
    private GameObject bird;
    private SpriteRenderer spriteRenderer;
    private bool isTransitioning = false;

    void Start()
    {
        if (birdSprite.Length < 2)
        {
            Debug.Log("Lack of Sprites. Check BirdSprite[]");
        }
        Transform _bird = transform.Find("Bird");
        bird = _bird.gameObject;
        spriteRenderer = bird.GetComponent<SpriteRenderer>();
        if (changeSprite)
        {
            StartCoroutine(ChangeBirdSprite());
        }
    }

    void Update()
    {
        if (isTransitioning) return;

        bool isGamepadPressed = Gamepad.current != null && (Gamepad.current.buttonSouth.wasPressedThisFrame || Gamepad.current.buttonEast.wasPressedThisFrame);
        bool isKeyboardPressed = Input.GetKeyDown(KeyCode.LeftArrow);

        if (isGamepadPressed || isKeyboardPressed)
        {
            StartCoroutine(PlaySEAndChangeScene());
        }
    }


    IEnumerator ChangeBirdSprite()
    {
        int index = 0;
        while (true)
        {
            spriteRenderer.sprite = birdSprite[index];
            index = (index + 1) % birdSprite.Length;
            yield return new WaitForSeconds(0.5f);
        }

    }

    IEnumerator PlaySEAndChangeScene()
    {
        isTransitioning = true;
        SoundManager.Instance.PlaySE("Go");
        yield return new WaitForSeconds(0.3f); // SEの長さに応じて調整
        SceneManager.LoadScene(previousScene);
    }
}
