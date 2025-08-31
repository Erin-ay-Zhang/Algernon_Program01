using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CollectionThreshold
{
    public int valueRequired;
    public GameObject[] objectsToShow;
    public GameObject[] objectsToHide;
    [HideInInspector] public bool conditionMet = false; // 条件已满足但尚未触发
    [HideInInspector] public bool hasBeenTriggered = false; // 已经触发过
}

public class CollectibleManager : MonoBehaviour
{
    public GameObject textSystem;
    private SpriteRenderer[] sprites;
    public static CollectibleManager Instance;

    public int totalCollected = 0;

    // 新增：阈值配置列表
    public List<CollectionThreshold> thresholds = new List<CollectionThreshold>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (textSystem != null)
        {
            sprites = new SpriteRenderer[textSystem.transform.childCount];
            for (int i = 0; i < textSystem.transform.childCount; ++i)
            {
                var a = textSystem.transform.GetChild(i);
                sprites[i] = a.GetComponent<SpriteRenderer>();
            }
        }

        // 初始化时检查一次阈值
        CheckThresholdConditions();
    }

    public void Collect(int value)
    {
        totalCollected += value;

        UpdateTextState();
        CheckThresholdConditions(); // 检查阈值条件是否满足

        Debug.Log("已收集物品数：" + totalCollected);
    }

    private void UpdateTextState()
    {
        if (textSystem == null || totalCollected >= sprites.Length)
            return;

        for (int i = 0; i < sprites.Length; ++i)
        {
            sprites[i].gameObject.SetActive(false);
        }

        sprites[totalCollected].gameObject.SetActive(true);
    }

    // 修改：只检查条件是否满足，但不立即触发
    private void CheckThresholdConditions()
    {
        foreach (CollectionThreshold threshold in thresholds)
        {
            // 如果达到阈值且尚未满足条件
            if (totalCollected >= threshold.valueRequired && !threshold.conditionMet)
            {
                threshold.conditionMet = true;
                Debug.Log($"Threshold {threshold.valueRequired} condition met, waiting for trigger zone.");
            }
            // 如果低于阈值但已经满足条件（用于重置情况）
            else if (totalCollected < threshold.valueRequired && threshold.conditionMet)
            {
                threshold.conditionMet = false;
                // 这里可以根据需要添加重置逻辑
            }
        }
    }

    // 新增：在触发点调用此方法来实际触发阈值效果
    public void TriggerThresholdEffects(int thresholdValue)
    {
        foreach (CollectionThreshold threshold in thresholds)
        {
            if (threshold.valueRequired == thresholdValue && threshold.conditionMet && !threshold.hasBeenTriggered)
            {
                // 显示需要显示的对象
                foreach (GameObject obj in threshold.objectsToShow)
                {
                    if (obj != null)
                    {
                        obj.SetActive(true);
                    }
                }

                // 隐藏需要隐藏的对象
                foreach (GameObject obj in threshold.objectsToHide)
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);
                    }
                }

                threshold.hasBeenTriggered = true;
                Debug.Log($"Threshold {threshold.valueRequired} effects triggered!");
            }
        }
    }

    // 新增：检查特定阈值是否满足条件
    public bool IsThresholdConditionMet(int thresholdValue)
    {
        foreach (CollectionThreshold threshold in thresholds)
        {
            if (threshold.valueRequired == thresholdValue)
            {
                return threshold.conditionMet;
            }
        }
        return false;
    }

    // 修改：重置所有阈值状态的方法
    public void ResetThresholds()
    {
        foreach (CollectionThreshold threshold in thresholds)
        {
            threshold.conditionMet = false;
            threshold.hasBeenTriggered = false;
        }
        CheckThresholdConditions(); // 重新检查当前状态
    }

    // 修改：直接设置收集值的方法
    public void SetCollectedValue(int value)
    {
        totalCollected = value;
        UpdateTextState();
        CheckThresholdConditions();
    }
}