using UnityEngine;

public class SkyObjectMoveLeft : MonoBehaviour
{
    public float speed = 2f;
    public Vector3 moveDirection = Vector3.left;

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }
}
