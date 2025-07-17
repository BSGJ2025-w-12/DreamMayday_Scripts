using UnityEngine;

public class SkySpeechBubbleController : MonoBehaviour
{
    [SerializeField] private Dream3Manager skyManager;

    [SerializeField] private GameObject sunTextPanel;
    [SerializeField] private GameObject thunderTextPanel;
    [SerializeField] private GameObject defaultTextPanel;

    private bool wasSun = false;
    private bool wasThunder = false;
    private bool isShowingExitText = false;
    private float exitTextTimer = 0f;

    [SerializeField] private float textChangeInterval = 3f;

    void Update()
    {
        bool isSun = skyManager?.isSun ?? false;
        bool isThunder = skyManager?.isThunder ?? false;

        if ((wasSun && !isSun) || (wasThunder && !isThunder))
        {
            ShowOnly(defaultTextPanel);
            isShowingExitText = true;
            exitTextTimer = 0f;
        }

        if (isShowingExitText)
        {
            exitTextTimer += Time.deltaTime;
            if (exitTextTimer >= textChangeInterval + 2f)
            {
                HideAllPanels();
                isShowingExitText = false;
            }

            wasSun = isSun;
            wasThunder = isThunder;
            return;
        }

        if (isSun)
        {
            ShowOnly(sunTextPanel);
        }
        else if (isThunder)
        {
            ShowOnly(thunderTextPanel);
        }
        else
        {
            HideAllPanels();
        }

        wasSun = isSun;
        wasThunder = isThunder;
    }

    private void ShowOnly(GameObject targetPanel)
    {
        sunTextPanel.SetActive(targetPanel == sunTextPanel);
        thunderTextPanel.SetActive(targetPanel == thunderTextPanel);
        defaultTextPanel.SetActive(targetPanel == defaultTextPanel);
    }

    private void HideAllPanels()
    {
        sunTextPanel.SetActive(false);
        thunderTextPanel.SetActive(false);
        defaultTextPanel.SetActive(false);
    }
}
