using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class DreamCameraChange : MonoBehaviour
{
    public List<Camera> cameraViews;
    private int currentIndex = 0;

    // 外部から呼び出す初期化メソッド
    public void InitializeCameras()
    {
        cameraViews = FindObjectsOfType<Camera>(true)
            .Where(cam => cam.targetDisplay == 1)
            .ToList();

        // Debug.Log($"Display2 用カメラ数：{cameraViews.Count}");

        if (cameraViews.Count > 0)
        {
            SetActiveCamera(0);
        }
        else
        {
            Debug.LogWarning("Display2 に設定されたカメラが見つかりませんでした．");
        }
    }

    void Update()
    {
        if (cameraViews.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.Alpha1) && cameraViews.Count >= 1)
        {
            SetActiveCamera(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && cameraViews.Count >= 2)
        {
            SetActiveCamera(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && cameraViews.Count >= 3)
        {
            SetActiveCamera(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && cameraViews.Count >= 4)
        {
            SetActiveCamera(3);
        }
    }

    void SetActiveCamera(int index)
    {
        currentIndex = index;
        for (int i = 0; i < cameraViews.Count; i++)
        {
            cameraViews[i].enabled = (i == index);
        }
    }
}
