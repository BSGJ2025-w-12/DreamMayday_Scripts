using UnityEngine;

public class SunMovementController : MonoBehaviour
{
    public GameObject sun;
    public SkyObjectSpawnController SkyController;
    public float standardSize = 6f;
    public float sunMaxSize = 6.5f;
    public float sunMinSize = 5.5f;
    public float changeSize = 0.005f;

    private float currentSize;
    private int sizeDir = 0; // 0 = 大きくなる, 1 = 小さくなる

    void Start()
    {
        SkyController = FindAnyObjectByType<SkyObjectSpawnController>();
        if (SkyController == null)
        {
            Debug.LogError("SkyObjectSpawnController が見つかりません！");
            return;
        }

        standardSize = SkyController.SunStandardSize;
        sunMaxSize = SkyController.maxSunSize;
        sunMinSize = SkyController.minSunSize;
        changeSize = SkyController.SunChangeValue;
        currentSize = standardSize;
        sun.transform.localScale = new Vector3(currentSize, currentSize, currentSize);
    }

    void Update()
    {
        if (sizeDir == 0)
        {
            currentSize += changeSize;
            if (currentSize >= sunMaxSize)
            {
                currentSize = sunMaxSize;
                sizeDir = 1;
            }
        }
        else
        {
            currentSize -= changeSize;
            if (currentSize <= sunMinSize)
            {
                currentSize = sunMinSize;
                sizeDir = 0;
            }
        }

        sun.transform.localScale = new Vector3(currentSize, currentSize, currentSize);
    }
}
