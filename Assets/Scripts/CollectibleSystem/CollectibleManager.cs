using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class CollectionThreshold
{
    public int valueRequired;
    public GameObject[] objectsToShow;
    public GameObject[] objectsToHide;
    public AudioClip musicToPlay; // 新增：要播放的音乐
    public bool stopCurrentMusic = true; // 新增：是否停止当前音乐
    [HideInInspector] public bool conditionMet = false;
    [HideInInspector] public bool hasBeenTriggered = false;
}

public class CollectibleManager : MonoBehaviour
{
    public GameObject textSystem;
    private SpriteRenderer[] sprites;
    public static CollectibleManager Instance;

    public int totalCollected = 0;
    
    // 新增：音频源引用
    public AudioSource backgroundMusicSource;
    public AudioSource soundEffectSource;
    
    // 当前播放的音乐
    private AudioClip currentMusic;

    public List<CollectionThreshold> thresholds = new List<CollectionThreshold>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
            
        // 确保有音频源组件
        if (backgroundMusicSource == null)
        {
            backgroundMusicSource = gameObject.AddComponent<AudioSource>();
            backgroundMusicSource.loop = true;
            backgroundMusicSource.playOnAwake = false;
        }
        
        if (soundEffectSource == null)
        {
            soundEffectSource = gameObject.AddComponent<AudioSource>();
            soundEffectSource.playOnAwake = false;
        }
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

        CheckThresholdConditions();
    }

    public void Collect(int value)
    {
        totalCollected += value;

        
        CheckThresholdConditions();

        Debug.Log("已收集物品数：" + totalCollected);
    }

    public class TextSystemController : MonoBehaviour
    {
        [SerializeField] private GameObject[] textSystems;
        [SerializeField] private float transitionDuration = 1.0f;

        private int currentSystemIndex = -1;
        private bool isTransitioning = false;

        private void UpdateTextState(int newSystemIndex)
        {
            if (isTransitioning || currentSystemIndex == newSystemIndex)
                return;

            StartCoroutine(TransitionTextSystems(newSystemIndex));
        }

        //文字切换
        private IEnumerator TransitionTextSystems(int newSystemIndex)
        {
            isTransitioning = true;

            // 获取当前和新的文本系统
            GameObject currentSystem = currentSystemIndex >= 0 ? textSystems[currentSystemIndex] : null;
            GameObject newSystem = textSystems[newSystemIndex];

            // 激活新系统但设置其溶解值为1（完全溶解）
            newSystem.SetActive(true);
            SetDissolveAmount(newSystem, 1f);

            // 同时进行两个动画：当前系统溶解消失，新系统溶解出现
            float elapsedTime = 0f;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / transitionDuration;

                if (currentSystem != null)
                {
                    SetDissolveAmount(currentSystem, t); // 从0到1溶解
                }

                SetDissolveAmount(newSystem, 1 - t); // 从1到0显示
                yield return null;
            }

            // 确保最终值准确
            if (currentSystem != null)
            {
                SetDissolveAmount(currentSystem, 1f);
                currentSystem.SetActive(false);
            }

            SetDissolveAmount(newSystem, 0f);
            currentSystemIndex = newSystemIndex;
            isTransitioning = false;
        }

        private void SetDissolveAmount(GameObject textSystem, float amount)
        {
            // 获取所有子物体的子物体（孙子物体及更深层级）的SpriteRenderer
            var grandChildren = textSystem.GetComponentsInChildren<Transform>()
                .Where(t => t != textSystem.transform && t.parent != textSystem.transform);

            foreach (Transform child in grandChildren)
            {
                SpriteRenderer renderer = child.GetComponent<SpriteRenderer>();
                if (renderer != null && renderer.material.HasProperty("_DissolveAmount"))
                {
                    renderer.material.SetFloat("_DissolveAmount", amount);
                }
            }
        }

        // 示例方法：如何调用切换
        public void SwitchToSystem(int systemIndex)
        {
            if (systemIndex >= 0 && systemIndex < textSystems.Length)
            {
                UpdateTextState(systemIndex);
            }
        }
    }

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

    public void TriggerThresholdEffects(int thresholdValue)
    {
        foreach (CollectionThreshold threshold in thresholds)
        {
            if (threshold.valueRequired == thresholdValue && threshold.conditionMet && !threshold.hasBeenTriggered)
            {
                foreach (GameObject obj in threshold.objectsToShow)
                {
                    if (obj != null) obj.SetActive(true);
                }

                foreach (GameObject obj in threshold.objectsToHide)
                {
                    if (obj != null) obj.SetActive(false);
                }
                
                // 新增：处理音乐播放
                if (threshold.musicToPlay != null)
                {
                    if (threshold.stopCurrentMusic)
                    {
                        // 直接播放新音乐
                        PlayMusic(threshold.musicToPlay);
                    }
                    else
                    {
                        // 淡入淡出过渡
                        StartCoroutine(CrossFadeToNewMusic(threshold.musicToPlay));
                    }
                }

                threshold.hasBeenTriggered = true;
                Debug.Log($"Threshold {threshold.valueRequired} effects triggered!");
            }
        }
    }
    
    // 新增：直接播放音乐的方法
    public void PlayMusic(AudioClip music)
    {
        if (music == null) return;
        
        if (backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
        }
        
        backgroundMusicSource.clip = music;
        backgroundMusicSource.Play();
        currentMusic = music;
    }
    
    // 新增：淡入淡出过渡到新音乐
    private IEnumerator CrossFadeToNewMusic(AudioClip newMusic)
    {
        if (newMusic == null) yield break;
        
        // 创建临时音频源用于过渡
        AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
        newAudioSource.clip = newMusic;
        newAudioSource.volume = 0f;
        newAudioSource.loop = true;
        newAudioSource.Play();
        
        float duration = 3.0f; // 过渡时间
        float timer = 0f;
        
        float originalVolume = backgroundMusicSource.volume;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float ratio = timer / duration;
            
            // 降低旧音乐音量，提高新音乐音量
            backgroundMusicSource.volume = Mathf.Lerp(originalVolume, 0f, ratio);
            newAudioSource.volume = Mathf.Lerp(0f, originalVolume, ratio);
            
            yield return null;
        }
        
        // 过渡完成后清理
        backgroundMusicSource.Stop();
        Destroy(backgroundMusicSource);
        
        // 将新音频源设置为主音频源
        backgroundMusicSource = newAudioSource;
        backgroundMusicSource.volume = originalVolume;
        currentMusic = newMusic;
    }

    // 新增：播放音效
    public void PlaySoundEffect(AudioClip sound)
    {
        if (sound == null || soundEffectSource == null) return;
        
        soundEffectSource.PlayOneShot(sound);
    }

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

    public void ResetThresholds()
    {
        foreach (CollectionThreshold threshold in thresholds)
        {
            threshold.conditionMet = false;
            threshold.hasBeenTriggered = false;
        }
        CheckThresholdConditions();
    }

    public void SetCollectedValue(int value)
    {
        totalCollected = value;
        CheckThresholdConditions();
    }

    // 修改后的交叉淡入淡出方法，现在可以被调用
    public void CrossFadeMusic(AudioClip newMusic, float fadeDuration = 6.0f)
    {
        if (newMusic == null || newMusic == currentMusic) return;
        
        StartCoroutine(CrossFadeCoroutine(backgroundMusicSource, newMusic, fadeDuration));
    }
    
    private IEnumerator CrossFadeCoroutine(AudioSource currentSource, AudioClip newMusic, float fadeDuration)
    {
        // 创建新的音频源用于播放新音乐
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = newMusic;
        newSource.volume = 0f;
        newSource.loop = true;
        newSource.Play();

        float timer = 0f;
        float currentVolume = currentSource.volume;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float ratio = timer / fadeDuration;

            // 降低当前音乐音量，提高新音乐音量
            // 使用对数曲线而不是线性变化
            float logRatio = Mathf.Log10(ratio * 9 + 1); // 将0-1映射到0-1但对数分布

            currentSource.volume = Mathf.Lerp(currentVolume, 0.001f, logRatio);
            newSource.volume = Mathf.Lerp(0.001f, currentVolume, logRatio);
            
            yield return null;
        }

        // 过渡完成后清理
        currentSource.Stop();
        Destroy(currentSource);
        
        // 将新音频源设置为主音频源
        backgroundMusicSource = newSource;
        backgroundMusicSource.volume = currentVolume;
        currentMusic = newMusic;
    }
}