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

    // 粒子系统引用
    public ParticleSystem transitionParticles;

    // 淡入淡出速度
    public float fadeSpeed = 2f;

    // 当前正在运行的协程
    private Coroutine currentTransitionCoroutine;

    // 当前活动的文本索引
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
                // 初始化所有文本为透明
                sprites[i].color = new Color(1f, 1f, 1f, 0f);
                sprites[i].gameObject.SetActive(false);
            }

            // 激活第一个文本
            if (sprites.Length > 0)
            {
                sprites[0].gameObject.SetActive(true);
                sprites[0].color = new Color(1f, 1f, 1f, 1f);
                currentActiveIndex = 0;
            }
        }

        // 初始化时检查一次阈值
        CheckThresholdConditions();
    }

    public void Collect(int value)
    {
        totalCollected += value;

        // 使用协程更新文本状态
        if (currentTransitionCoroutine != null)
        {
            StopCoroutine(currentTransitionCoroutine);
        }
        currentTransitionCoroutine = StartCoroutine(TransitionTextSystem());

        CheckThresholdConditions();

        Debug.Log("已收集物品数：" + totalCollected);
    }

    // 文本系统过渡协程 - 修改顺序
    private IEnumerator TransitionTextSystem()
    {
        if (textSystem == null || totalCollected >= sprites.Length)
            yield break;

        int newIndex = totalCollected;

        // 开始淡出当前文本
        if (currentActiveIndex >= 0 && currentActiveIndex < sprites.Length)
        {
            // 启动淡出协程
            Coroutine fadeOutCoroutine = StartCoroutine(FadeSprite(sprites[currentActiveIndex], 1f, 0f));

            // 等待淡出进行到一半
            yield return new WaitForSeconds(0.3f); // 可以根据需要调整这个时间

            // 播放粒子特效
            if (transitionParticles != null)
            {
                // 确保粒子系统在正确的位置
                transitionParticles.transform.position = textSystem.transform.position;
                transitionParticles.Play();
            }

            // 激活新文本并开始淡入
            sprites[newIndex].gameObject.SetActive(true);
            StartCoroutine(FadeSprite(sprites[newIndex], 0f, 1f));

            // 等待淡出完成
            yield return fadeOutCoroutine;

            // 禁用旧文本
            sprites[currentActiveIndex].gameObject.SetActive(false);
        }
        else
        {
            // 如果没有当前活动文本，直接显示新文本
            sprites[newIndex].gameObject.SetActive(true);
            yield return StartCoroutine(FadeSprite(sprites[newIndex], 0f, 1f));
        }

        // 更新当前活动索引
        currentActiveIndex = newIndex;
    }

    // 淡入淡出协程
    private IEnumerator FadeSprite(SpriteRenderer sprite, float startAlpha, float targetAlpha)
    {
        float elapsedTime = 0f;
        float duration = 1f / fadeSpeed;

        // 设置初始透明度
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

    // 检查条件是否满足
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

    // 触发阈值效果
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

    // 检查特定阈值是否满足条件
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

    // 重置所有阈值状态
    public void ResetThresholds()
    {
        foreach (CollectionThreshold threshold in thresholds)
        {
            threshold.conditionMet = false;
            threshold.hasBeenTriggered = false;
        }
        CheckThresholdConditions();
    }

    // 直接设置收集值
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