using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CollectionThreshold
{
    public int valueRequired;
    public GameObject[] objectsToShow;
    public GameObject[] objectsToHide;
    [HideInInspector] public bool conditionMet = false;
    [HideInInspector] public bool hasBeenTriggered = false;
}

public class CollectibleManager : MonoBehaviour
{
    public GameObject textSystem;
    private SpriteRenderer[] sprites;
    public static CollectibleManager Instance;

    public int totalCollected = 0;

    public List<CollectionThreshold> thresholds = new List<CollectionThreshold>();

    // ����ϵͳ����
    public ParticleSystem transitionParticles;

    // ���뵭���ٶ�
    public float fadeSpeed = 2f;

    // ��ǰ�������е�Э��
    private Coroutine currentTransitionCoroutine;

    // ��ǰ����ı�����
    private int currentActiveIndex = 0;

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
                // ��ʼ�������ı�Ϊ͸��
                sprites[i].color = new Color(1f, 1f, 1f, 0f);
                sprites[i].gameObject.SetActive(false);
            }

            // �����һ���ı�
            if (sprites.Length > 0)
            {
                sprites[0].gameObject.SetActive(true);
                sprites[0].color = new Color(1f, 1f, 1f, 1f);
                currentActiveIndex = 0;
            }
        }

        // ��ʼ��ʱ���һ����ֵ
        CheckThresholdConditions();
    }

    public void Collect(int value)
    {
        totalCollected += value;

        // ʹ��Э�̸����ı�״̬
        if (currentTransitionCoroutine != null)
        {
            StopCoroutine(currentTransitionCoroutine);
        }
        currentTransitionCoroutine = StartCoroutine(TransitionTextSystem());

        CheckThresholdConditions();

        Debug.Log("���ռ���Ʒ����" + totalCollected);
    }

    // �ı�ϵͳ����Э�� - �޸�˳��
    private IEnumerator TransitionTextSystem()
    {
        if (textSystem == null || totalCollected >= sprites.Length)
            yield break;

        int newIndex = totalCollected;

        // ��ʼ������ǰ�ı�
        if (currentActiveIndex >= 0 && currentActiveIndex < sprites.Length)
        {
            // ��������Э��
            Coroutine fadeOutCoroutine = StartCoroutine(FadeSprite(sprites[currentActiveIndex], 1f, 0f));

            // �ȴ��������е�һ��
            yield return new WaitForSeconds(0.3f); // ���Ը�����Ҫ�������ʱ��

            // ����������Ч
            if (transitionParticles != null)
            {
                // ȷ������ϵͳ����ȷ��λ��
                transitionParticles.transform.position = textSystem.transform.position;
                transitionParticles.Play();
            }

            // �������ı�����ʼ����
            sprites[newIndex].gameObject.SetActive(true);
            StartCoroutine(FadeSprite(sprites[newIndex], 0f, 1f));

            // �ȴ��������
            yield return fadeOutCoroutine;

            // ���þ��ı�
            sprites[currentActiveIndex].gameObject.SetActive(false);
        }
        else
        {
            // ���û�е�ǰ��ı���ֱ����ʾ���ı�
            sprites[newIndex].gameObject.SetActive(true);
            yield return StartCoroutine(FadeSprite(sprites[newIndex], 0f, 1f));
        }

        // ���µ�ǰ�����
        currentActiveIndex = newIndex;
    }

    // ���뵭��Э��
    private IEnumerator FadeSprite(SpriteRenderer sprite, float startAlpha, float targetAlpha)
    {
        float elapsedTime = 0f;
        float duration = 1f / fadeSpeed;

        // ���ó�ʼ͸����
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, startAlpha);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
            yield return null;
        }

        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, targetAlpha);
    }

    // ��������Ƿ�����
    private void CheckThresholdConditions()
    {
        foreach (CollectionThreshold threshold in thresholds)
        {
            if (totalCollected >= threshold.valueRequired && !threshold.conditionMet)
            {
                threshold.conditionMet = true;
                Debug.Log($"Threshold {threshold.valueRequired} condition met, waiting for trigger zone.");
            }
            else if (totalCollected < threshold.valueRequired && threshold.conditionMet)
            {
                threshold.conditionMet = false;
            }
        }
    }

    // ������ֵЧ��
    public void TriggerThresholdEffects(int thresholdValue)
    {
        foreach (CollectionThreshold threshold in thresholds)
        {
            if (threshold.valueRequired == thresholdValue && threshold.conditionMet && !threshold.hasBeenTriggered)
            {
                foreach (GameObject obj in threshold.objectsToShow)
                {
                    if (obj != null) obj.SetActive(true);
                    // if it has audiosource yes play 
                    if(obj.transform.TryGetComponent<AudioSource>(out AudioSource result))
                    {
                        result.PlayDelayed(0.3f);
                    }
                }

                foreach (GameObject obj in threshold.objectsToHide)
                {
                    if (obj != null) obj.SetActive(false);
                }

                threshold.hasBeenTriggered = true;
                Debug.Log($"Threshold {threshold.valueRequired} effects triggered!");
            }
        }
    }

    // ����ض���ֵ�Ƿ���������
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

    // ����������ֵ״̬
    public void ResetThresholds()
    {
        foreach (CollectionThreshold threshold in thresholds)
        {
            threshold.conditionMet = false;
            threshold.hasBeenTriggered = false;
        }
        CheckThresholdConditions();
    }

    // ֱ�������ռ�ֵ
    public void SetCollectedValue(int value)
    {
        totalCollected = value;

        if (currentTransitionCoroutine != null)
        {
            StopCoroutine(currentTransitionCoroutine);
        }
        currentTransitionCoroutine = StartCoroutine(TransitionTextSystem());

        CheckThresholdConditions();
    }
}