using UnityEngine;

public class SkyObjectDestroy : MonoBehaviour
{
    public float leftBound = -55f;
    public float bottomBound = -30f;
    private SkyObjectSpawnController SkyController;

    void Start()
    {
        SkyController = FindAnyObjectByType<SkyObjectSpawnController>();
    }
    void Update()
    {
        if (transform.position.x < leftBound - 2f || transform.position.y < bottomBound - 2f)
        {
            if (gameObject.CompareTag("Mosquito"))
            {
                SkyController.currentMosquitoCount--;
            }
            Destroy(gameObject);
        }
    }
}
