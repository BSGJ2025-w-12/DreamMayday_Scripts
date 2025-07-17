using UnityEngine;
using UnityEngine.Serialization;

public class Dream2Manager : MonoBehaviour,IDreamAnomaly
{
    //IDreamAnomaly用の設定1
    //異変の名前
    public string[] GetAnomalyNames()
    {
        return new string[] { "Hot", "Ice", "Flower" };   
    }
    //名前によって異変のboolを起こす
    public void SetAnomalyState(string anomalyName, bool value)
    {
        // まず全ての異変をリセット
        isHot = isIce = isFlower = false;

        // 指定された異変をアクティブにする
        switch (anomalyName)
        {
            case "Hot":
                isHot = value;
                break;
            case "Ice":
                isIce = value;
                break;
            case "Flower":
                isFlower = value;
                break;
            default: // "None"が指定された場合や、未知の異変名の場合
                // 何もしないか、あるいは全ての異変をfalseのままにする
                break;
        }
    }
    //異変が起こっているかのboolを返す
    public bool IsAnomalyActive(string anomalyName)
    {
        switch (anomalyName)
        {
            case "Hot": return isHot;
            case "Ice": return isIce;
            case "Flower": return isFlower;
            default: return false;
        }
    }

    

    // 各異変をboolで管理するだけのスクリプト
    public bool isHot;
    public bool isIce;
    public bool isFlower;
    
    public bool isNormal()
    {
        return !(isHot || isIce || isFlower);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            isHot = isIce = isFlower = false;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            isHot = true;
            isIce = isFlower = false;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            isIce = true;
            isHot  = isFlower = false;
        }


        if (Input.GetKeyDown(KeyCode.F))
        {
            isFlower = true;
            isHot = isIce  = false;
        }
    }

}