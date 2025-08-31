using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CollectionThreshold
{
    public int valueRequired;
    public GameObject[] objectsToShow;
    public GameObject[] objectsToHide;
    [HideInInspector] public bool conditionMet = false; // ���������㵫��δ����
    [HideInInspector] public bool hasBeenTriggered = false; // �Ѿ�������
}

public class CollectibleManager : MonoBehaviour
{
    public GameObject textSystem;
    private SpriteRenderer[] sprites;
    public static CollectibleManager Instance;

    public int totalCollected = 0;

    // ��������ֵ�����б�
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

        // ��ʼ��ʱ���һ����ֵ
        CheckThresholdConditions();
    }

    public void Collect(int value)
    {
        totalCollected += value;

        UpdateTextState();
        CheckThresholdConditions(); // �����ֵ�����Ƿ�����

        Debug.Log("���ռ���Ʒ����" + totalCollected);
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

    // �޸ģ�ֻ��������Ƿ����㣬������������
    private void CheckThresholdConditions()
    {
        foreach (CollectionThreshold threshold in thresholds)
        {
            // ����ﵽ��ֵ����δ��������
            if (totalCollected >= threshold.valueRequired && !threshold.conditionMet)
            {
                threshold.conditionMet = true;
                Debug.Log($"Threshold {threshold.valueRequired} condition met, waiting for trigger zone.");
            }
            // ���������ֵ���Ѿ������������������������
            else if (totalCollected < threshold.valueRequired && threshold.conditionMet)
            {
                threshold.conditionMet = false;
                // ������Ը�����Ҫ��������߼�
            }
        }
    }

    // �������ڴ�������ô˷�����ʵ�ʴ�����ֵЧ��
    public void TriggerThresholdEffects(int thresholdValue)
    {
        foreach (CollectionThreshold threshold in thresholds)
        {
            if (threshold.valueRequired == thresholdValue && threshold.conditionMet && !threshold.hasBeenTriggered)
            {
                // ��ʾ��Ҫ��ʾ�Ķ���
                foreach (GameObject obj in threshold.objectsToShow)
                {
                    if (obj != null)
                    {
                        obj.SetActive(true);
                    }
                }

                // ������Ҫ���صĶ���
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

    // ����������ض���ֵ�Ƿ���������
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

    // �޸ģ�����������ֵ״̬�ķ���
    public void ResetThresholds()
    {
        foreach (CollectionThreshold threshold in thresholds)
        {
            threshold.conditionMet = false;
            threshold.hasBeenTriggered = false;
        }
        CheckThresholdConditions(); // ���¼�鵱ǰ״̬
    }

    // �޸ģ�ֱ�������ռ�ֵ�ķ���
    public void SetCollectedValue(int value)
    {
        totalCollected = value;
        UpdateTextState();
        CheckThresholdConditions();
    }
}