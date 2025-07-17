using UnityEngine;

public class SkyObjectDrop : MonoBehaviour
{
    public float speed = 2f;
    public Vector3 moveDirection = Vector3.down;

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }
}
