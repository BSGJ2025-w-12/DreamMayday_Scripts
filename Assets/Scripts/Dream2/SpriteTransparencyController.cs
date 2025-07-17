using System;
using UnityEngine;

public class SpriteTransparencyController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Dream2Manager manager;

    public ConditionType conditionType = ConditionType.Normal;
    public Color aColor = new Color(1f, 1f, 1f, 0f);
    public Color bColor = new Color(1f, 1f, 1f, 1f);
    public float fadeSpeed = 1.0f;

    private Color targetColor;
    private Func<bool> conditionFunc;

    public enum ConditionType
    {
        Normal,
        Hot,
        Ice,
        Bike,
        Flower
    }

    void Start()
    {
        // 一度だけ設定すればOK
        switch (conditionType)
        {
            case ConditionType.Normal:
                conditionFunc = () => manager.isNormal();
                break;
            case ConditionType.Hot:
                conditionFunc = () => manager.isHot;
                break;
            case ConditionType.Ice:
                conditionFunc = () => manager.isIce;
                break;
            case ConditionType.Flower:
                conditionFunc = () => manager.isFlower;
                break;
        }
    }

    void Update()
    {
        if (conditionFunc == null) return;

        bool conditionMet = conditionFunc.Invoke();
        targetColor = conditionMet ? bColor : aColor;
        spriteRenderer.color = Color.Lerp(spriteRenderer.color, targetColor, fadeSpeed * Time.deltaTime);
    }
}