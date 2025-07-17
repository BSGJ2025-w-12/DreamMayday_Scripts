using UnityEngine;

public class SkyObjectSinMove : MonoBehaviour
{
    [Header("移動設定")]
    public float speed = 2f;
    public float frequency = 1f;
    public float magnitude = 1f;
    public bool dropMosquito = false;

    private Vector3 startPos;
    private float elapsedX = 0f;
    private float elapsedY = 0f;
    private float dropSpeed;
    private bool wasDropping = false; // 状態切り替え検知用
    private Dream3Manager Dream3Manager;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        startPos = transform.position;
        dropSpeed = speed * 10f;

        Dream3Manager = FindAnyObjectByType<Dream3Manager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        bool mosquitoDrop = dropMosquito && !Dream3Manager.isMosquito;

        // 状態が false → true に切り替わった瞬間
        if (mosquitoDrop && !wasDropping)
        {
            startPos = transform.position;  // 今の位置を新しい基準点に
            elapsedX = 0f; // 揺れリセット
        }

        if (mosquitoDrop)
        {
            spriteRenderer.flipY = true;
            elapsedX += Time.deltaTime;
            float xOffset = Mathf.Sin(elapsedX * frequency) * magnitude;
            float newX = startPos.x - xOffset;
            //float newX = startPos.x;
            float newY = transform.position.y - dropSpeed * Time.deltaTime;
            transform.position = new Vector3(newX, newY, 0f);
        }
        else
        {
            elapsedY += Time.deltaTime;
            float yOffset = Mathf.Sin(elapsedY * frequency) * magnitude;
            float newX = transform.position.x - speed * Time.deltaTime;
            float newY = startPos.y + yOffset;
            transform.position = new Vector3(newX, newY, 0f);
        }

        wasDropping = mosquitoDrop; // 状態を更新
    }
}
