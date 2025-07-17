using UnityEngine;
using System.Collections;

public class TrainerController : MonoBehaviour
{
    public Transform entryPoint;
    public Transform targetPoint;
    public float moveSpeed = 2f;

    public TrainerSpeechBubble TSB;

    private bool hasStartedTalking = false;
    private bool isMoving = false;
    private bool isExiting = false;

    private Coroutine exitCoroutine;
    private SpriteRenderer sr;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(entryPoint!=null)
        {
            transform.position = entryPoint.position;
        }
        if(TSB!=null)
        {
            TSB.gameObject.SetActive(false);
        }
        sr = GetComponent<SpriteRenderer>();
    }
    public void EnterRoom()
    {
        if(sr!=null)
        {
            sr.flipX = false;
        }
        
        isMoving = true;
        hasStartedTalking = false;
        isExiting = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving || targetPoint == null) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
        {
            isMoving = false;
        }
        if(!hasStartedTalking&&TSB !=null)
        {
            TSB.gameObject.SetActive(true);
            hasStartedTalking = true;
        }
    }
    public void ExitRoom()
    {
       
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Trainer is inactive, skipping ExitRoom");
            return; // 🚫 不执行协程
        }
        Debug.Log("Trainer ExitRoom");
        if(exitCoroutine!=null)
        {
            StopCoroutine(exitCoroutine);
        }
        exitCoroutine = StartCoroutine(ExitSequence());
    }
    private IEnumerator ExitSequence()
    {
        isExiting = true;

        if(sr!=null)
        {
            sr.flipX = true;
        }
        yield return new WaitForSeconds(0.5f);

        while (Vector3.Distance(transform.position, entryPoint.position)>0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, entryPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        if (TSB != null)
        {
            TSB.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }
    
}
