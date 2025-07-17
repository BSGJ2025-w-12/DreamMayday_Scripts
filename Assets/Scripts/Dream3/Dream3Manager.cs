using UnityEngine;

public class Dream3Manager : MonoBehaviour,IDreamAnomaly
{

    public string[] GetAnomalyNames()
    {
        return new string[] { "Ice", "Hot", "Insect" };   
    }
    //名前によって異変のboolを起こす
    public void SetAnomalyState(string anomalyName, bool value)
    {
        ResetSkyAnomaly();
        // まず全ての異変をリセット
        isThunder = isSun = isMosquito = false;

        // 指定された異変をアクティブにする
        switch (anomalyName)
        {
            case "Ice":
                isThunder = value;
                break;
            case "Hot":
                isSun = value;
                break;
            case "Insect":
                isMosquito = value;
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
            case "Ice": return isThunder;
            case "Hot": return isSun;
            case "Insect": return isMosquito;
            default: return false;
        }
    }
    [Header("Anomaly Flag")]
    public bool isThunder;
    public bool isSun;
    public bool isMosquito;

    void Start()
    {
        if (isThunder || isSun || isMosquito)
        {
            // Debug.Log("Anomaly Active");
        }
        else
        {
            // Debug.Log("No Anomaly");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            ResetSkyAnomaly();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ResetSkyAnomaly();
            isThunder = true;
            Debug.Log("Active Thunder");
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            ResetSkyAnomaly();
            isSun = true;
            Debug.Log("Active Sun");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ResetSkyAnomaly();
            isMosquito = true;
            Debug.Log("Active mosquito");
        }
    }

    void ResetSkyAnomaly()
    {
        Debug.Log("Reset Anomaly");
        isThunder = false;
        isSun = false;
        isMosquito = false;
    }
}