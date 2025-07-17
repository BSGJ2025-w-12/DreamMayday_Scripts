using UnityEngine;

public class TitleCameraController : MonoBehaviour
{
    public GameObject Player;
    [SerializeField] private float upLimit = 2.6f;
    [SerializeField] private float downLimit = -2.6f;
    void Start()
    {

    }

    void Update()
    {
        transform.position = new Vector3(0, Player.transform.position.y, -7);
        if (transform.position.y < downLimit)
        {
            transform.position = new Vector3(0, downLimit, -7);
        }
        if (transform.position.y > upLimit)
        {
            transform.position = new Vector3(0, upLimit, -7);
        }
    }
}
