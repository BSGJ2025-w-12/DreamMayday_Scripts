using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TitleCharacterController : MonoBehaviour
{
    [SerializeField] private float moveDuration = 5.0f;
    [SerializeField] private float upLimit = 4f;
    [SerializeField] private float downLimit = -5f;
    [SerializeField] private float rightLimit = 6f;
    [SerializeField] private float leftLimit = -6f;
    [SerializeField] private float waitTime = 0.5f;
    [SerializeField] private bool useBigMap = true;
    [SerializeField] private float inactivityThreshold = 5f;

    private Dictionary<int, Vector2> movePos;
    private Dictionary<int, int[]> BigNeighbors;
    private Dictionary<int, int[]> SmallNeighbors;
    private Animator animator;
    private SpriteRenderer sprite;

    private float inactivityTimer = 0f;
    private Coroutine movementCoroutine;
    private bool isIdle = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        float centerX = (rightLimit + leftLimit) / 2;
        float centerY = (upLimit + downLimit) / 2;

        movePos = new Dictionary<int, Vector2>
        {
            {0, new Vector2(leftLimit, downLimit)},
            {1, new Vector2(centerX, downLimit)},
            {2, new Vector2(rightLimit, downLimit)},
            {3, new Vector2(leftLimit, centerY)},
            {4, new Vector2(centerX, centerY)},
            {5, new Vector2(rightLimit, centerY)},
        };

        BigNeighbors = new Dictionary<int, int[]>
        {
            {0, new int[] {1} },
            {1, new int[] {0, 2}},
            {2, new int[] {1, 5}},
            {3, new int[] {4}},
            {4, new int[] {3, 5}},
            {5, new int[] {2, 4}},
        };

        SmallNeighbors = new Dictionary<int, int[]>
        {
            {0, new int[] {2} },
            {1, new int[] {0, 2}},
            {2, new int[] {0, 5}},
            {3, new int[] {5}},
            {4, new int[] {3, 5}},
            {5, new int[] {2, 3}},
        };
    }

    void Update()
    {
        if (IsPlayerInputDetected())
        {
            inactivityTimer = 0f;

            /*
                if (isIdle)
                {
                    if (movementCoroutine != null)
                        StopCoroutine(movementCoroutine);

                    isIdle = false;
                }
                */
        }
        else
        {
            inactivityTimer += Time.deltaTime;

            if (inactivityTimer >= inactivityThreshold && !isIdle)
            {
                movementCoroutine = StartCoroutine(CharacterMovement());
                isIdle = true;
            }
        }
    }

    private bool IsPlayerInputDetected()
    {
        return Input.anyKey
            || Input.GetAxis("Horizontal") != 0
            || Input.GetAxis("Vertical") != 0
            || Input.GetMouseButton(0)
            || Input.GetMouseButton(1);
    }

    IEnumerator CharacterMovement()
    {
        int currentNum = 0;
        int[] neighborNum;
        int targetNum;
        Vector2 currentPos;
        Vector2 targetPos;

        while (true)
        {
            if (useBigMap)
            {
                neighborNum = BigNeighbors[currentNum];
            }
            else
            {
                neighborNum = SmallNeighbors[currentNum];
            }

            targetNum = neighborNum[Random.Range(0, neighborNum.Length)];

            currentPos = transform.position;
            targetPos = movePos[targetNum];

            CheckMoveDir(currentNum, targetNum);

            float elapsed = 0f;
            while (elapsed < moveDuration)
            {
                float t = elapsed / moveDuration;
                transform.position = Vector2.Lerp(currentPos, targetPos, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPos;
            currentNum = targetNum;
            ResetAnimatorBool();

            yield return new WaitForSeconds(waitTime);
        }
    }

    void CheckMoveDir(int current, int target)
    {
        int diff = current - target;

        ResetAnimatorBool();

        if (diff == -3)
        {
            animator.SetBool("up", true);
        }
        else if (diff == 3)
        {
            animator.SetBool("down", true);
        }
        else if (diff < 0)
        {
            animator.SetBool("side", true);
            sprite.flipX = true;
        }
        else if (diff > 0)
        {
            animator.SetBool("side", true);
            sprite.flipX = false;
        }
        else
        {
            Debug.Log("Error: Check AnimatorBool");
        }
    }

    void ResetAnimatorBool()
    {
        animator.SetBool("up", false);
        animator.SetBool("down", false);
        animator.SetBool("side", false);
    }
}
