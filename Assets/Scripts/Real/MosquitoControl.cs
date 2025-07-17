using UnityEngine;

// 蚊の動きを制御するスクリプト
public class MosquitoControl : MonoBehaviour
{
    Animator animator;
    float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // speed = 1f + Time.time / 100;
        speed = 1f;
        animator.SetFloat("Speed", speed);
    }
}
