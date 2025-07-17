using UnityEngine;
using System.Collections;

public class ResultCameraMover : MonoBehaviour
{
    public Transform cameraTransform;
    public Vector3 startPos;
    public Vector3 endPos;
    public float duration = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void BeginMove()
    {
        if(cameraTransform==null)
        {
            cameraTransform = transform;
        }
        Vector3 start = cameraTransform.position;
        Vector3 end = endPos;
        // Debug.Log($"[Camera] Moving from {startPos} to {endPos}");
        StartCoroutine(MoveCamera(start,end));
    }
    IEnumerator MoveCamera(Vector3 start,Vector3 end)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / duration;
            cameraTransform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
        cameraTransform.position = endPos;
    }

}