using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public GameObject Player;

    void Start()
    {
        
    }

    void Update()
    {
        transform.position = new Vector3(0, Player.transform.position.y, -10);
        if (transform.position.y < 0)
        {
            transform.position = new Vector3(0, 0, -10);
        }
    }
}
