using UnityEngine;

public class Dream0Manager : MonoBehaviour, IDreamAnomaly
{
    public CharacterMovementCircle[] characterMovement;

    // 内部変数
    private bool _isHot;
    private bool _isIce;
    private bool _isLight;

    public bool isHot
    {
        get => _isHot;
        set
        {
            if (_isHot == value) return;
            _isHot = value;
            if (value)
            {
                foreach (var chara in characterMovement)
                    chara.ActivateFireAnomaly();
                _isIce = _isLight = false;
            }
            else if (!IsAnyAnomalyActive())
            {
                ResetAllAnomalies();
            }
        }
    }

    public bool isIce
    {
        get => _isIce;
        set
        {
            if (_isIce == value) return;
            _isIce = value;
            if (value)
            {
                foreach (var chara in characterMovement)
                    chara.ActivateIceAnomaly();
                _isHot = _isLight = false;
            }
            else if (!IsAnyAnomalyActive())
            {
                ResetAllAnomalies();
            }
        }
    }

    public bool isLight
    {
        get => _isLight;
        set
        {
            if (_isLight == value) return;
            _isLight = value;
            if (value)
            {
                foreach (var chara in characterMovement)
                    chara.ActivateLightAnomaly();
                _isHot = _isIce = false;
            }
            else if (!IsAnyAnomalyActive())
            {
                ResetAllAnomalies();
            }
        }
    }

    void ResetAllAnomalies()
    {
        foreach (var chara in characterMovement)
            chara.ResetAnomaly();
    }

    bool IsAnyAnomalyActive() => _isHot || _isIce || _isLight;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            isHot = isIce = isLight = false; // Reset
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            isHot = true;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            isIce = true;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            isLight = true;
        }
    }

    public string[] GetAnomalyNames()
    {
        return new string[] { "Hot", "Ice", "Light" };
    }

    public void SetAnomalyState(string anomalyName, bool value)
    {
        if (!value)
        {
            isHot = isIce = isLight = false;
            return;
        }

        switch (anomalyName)
        {
            case "Hot":
                isHot = true;
                break;
            case "Ice":
                isIce = true;
                break;
            case "Light":
                isLight = true;
                break;
            default:
                isHot = isIce = isLight = false;
                break;
        }
    }

    public bool IsAnomalyActive(string anomalyName)
    {
        return anomalyName switch
        {
            "Hot" => isHot,
            "Ice" => isIce,
            "Light" => isLight,
            _ => false,
        };
    }

    public bool isNormal() => isHot || isIce || isLight;
}
