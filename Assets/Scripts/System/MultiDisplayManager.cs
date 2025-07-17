using UnityEngine;

public class MultiDisplayManager : MonoBehaviour
{
    void Start()
    {
        // 接続されているディスプレイ数を確認
        Debug.Log("Connected Displays: " + Display.displays.Length);

        // 2番目のディスプレイを有効にする（Display.displays[1] = Display 2）
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
        }

        // 追加：3台目以降も対応するなら以下を追加
        /*
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
        */
    }
}
