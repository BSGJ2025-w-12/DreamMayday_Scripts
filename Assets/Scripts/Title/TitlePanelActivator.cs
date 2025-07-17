using UnityEngine;

public class TitlePanelActivator : MonoBehaviour
{
    [SerializeField] private GameObject LocalizationPanel;
    [SerializeField] private GameObject VolumePanel;

    /// <summary>
    /// ローカライズパネルの表示切り替え
    /// </summary>
    public void ActivateLocalizationPanel()
    {
        if (!VolumePanel.activeSelf) {
            LocalizationPanel.SetActive(true);
        }
    }

    public void DeactivateLocalizationPanel()
    {
        LocalizationPanel.SetActive(false);
    }

    public void ToggleLocalizationPanel()
    {
        if (LocalizationPanel.activeSelf)
        {
            LocalizationPanel.SetActive(false);
        }
        else if (!VolumePanel.activeSelf)
        {
            LocalizationPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 音量調整パネルの表示切り替え
    /// </summary>
    public void ActivateVolumePanel()
    {
        if (!LocalizationPanel.activeSelf) {
            VolumePanel.SetActive(true);
        }
    }

    public void DeactivateVolumePanel()
    {
        VolumePanel.SetActive(false);
    }

    public void ToggleVolumePanel()
    {
        if (VolumePanel.activeSelf)
        {
            VolumePanel.SetActive(false);
        }
        else if (!LocalizationPanel.activeSelf)
        {
            VolumePanel.SetActive(true);
        }
    }
}
