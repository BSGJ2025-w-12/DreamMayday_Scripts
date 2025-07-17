using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 虫キャラクターの制御クラス。
/// 登場・退場・ジャンプ・セリフなどの演出を管理。
/// </summary>
public class InsectController : MonoBehaviour
{
    private Coroutine _exitCoroutine;
    private SpriteRenderer _spriteRenderer;

    [Header("Jump")]
    public float jumpForce = 3f;
    private bool _isJumping = false;

    [Header("Movement")]
    public Transform entryPoint;
    public Transform targetPoint;
    public float moveSpeed = 1.5f;

    private bool _hasReachedPoint = false;
    private bool _isMoving = false;
    private Vector3 _moveTarget;
    private bool _shouldHideOnArrival = false;

    [Header("Speech Bubble")]
    public InsectSpeechBubble insectBubble;

    [Header("Speech")]
    public string mySpecialSpeech = "";

    public void SetSpeech(string line)
    {
        mySpecialSpeech = line;
        if (insectBubble != null)
        {
            insectBubble.InsectSpeechLines = new List<string> { mySpecialSpeech };
        }
    }

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (entryPoint != null)
        {
            transform.position = entryPoint.position;
        }
    }

    void Update()
    {
        if (_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _moveTarget, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _moveTarget) < 0.01f)
            {
                _isMoving = false;
                _hasReachedPoint = true;

                if (_shouldHideOnArrival)
                {
                    gameObject.SetActive(false);
                    _shouldHideOnArrival = false;
                }
            }
        }
    }

    public void EnterRoom()
    {
        if (entryPoint == null || targetPoint == null) return;

        transform.position = entryPoint.position;
        _moveTarget = targetPoint.position;
        _isMoving = true;
        _hasReachedPoint = false;
        _shouldHideOnArrival = false;

        gameObject.SetActive(true);
        if (_spriteRenderer != null) _spriteRenderer.flipX = false;
    }

    public void ExitRoom()
    {
        if (entryPoint == null) return;

        if (_exitCoroutine != null)
            StopCoroutine(_exitCoroutine);

        _exitCoroutine = StartCoroutine(ExitSequence());
    }

    IEnumerator ExitSequence()
    {
        if (_spriteRenderer != null) _spriteRenderer.flipX = true;

        yield return new WaitForSeconds(0.5f);
        if (insectBubble != null)
            insectBubble.HideSpeech();

        _moveTarget = entryPoint.position;
        _isMoving = true;
        _hasReachedPoint = false;
        _shouldHideOnArrival = true;
    }

    public bool HasReachedTarget() => _hasReachedPoint;

    public void JumpAndSpeak()
    {
        if (!gameObject.activeInHierarchy || !_hasReachedPoint || _isJumping) return;

        if (insectBubble != null && !string.IsNullOrEmpty(mySpecialSpeech))
        {
            insectBubble.ShowSpeech(mySpecialSpeech);
        }

        StartCoroutine(JumpRoutine());
    }

    public void Jump()
    {
        if (!gameObject.activeInHierarchy || !_hasReachedPoint || _isJumping) return;

        if (insectBubble != null)
        {
            insectBubble.ShowRandomSpeech();
        }

        StartCoroutine(JumpRoutine());
    }

    IEnumerator JumpRoutine()
    {
        _isJumping = true;

        Vector3 originalPos = transform.position;
        Vector3 jumpTarget = originalPos + Vector3.up * 1f;
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(originalPos, jumpTarget, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(jumpTarget, originalPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
        _isJumping = false;
    }
}
