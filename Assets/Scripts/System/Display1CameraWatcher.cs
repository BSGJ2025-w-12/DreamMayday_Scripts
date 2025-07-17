using UnityEngine;

public class Display1CameraWatcher : MonoBehaviour
{
    public Camera currentDisplay1Camera;

    void Update()
    {
        // Display1 に割り当てられ、有効なカメラを探す
        Camera[] allCameras = Camera.allCameras;

        foreach (Camera cam in allCameras)
        {
            if (cam.enabled && cam.targetDisplay == 0)
            {
                // 切り替わった場合
                if (currentDisplay1Camera != cam)
                {
                    currentDisplay1Camera = cam;
                    // Debug.Log($"Display1 カメラが切り替わりました: {cam.name}");
                }
                return; // 最初に見つけたカメラでOK
            }
        }

        // 有効な Display1 カメラが存在しない場合
        if (currentDisplay1Camera != null)
        {
            // Debug.Log("Display1 のカメラが無効になりました");
            currentDisplay1Camera = null;
        }
    }
}
