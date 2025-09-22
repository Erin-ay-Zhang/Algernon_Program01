using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectionThreshold
{
    public int valueRequired;
    public GameObject[] objectsToShow;
    public GameObject[] objectsToHide;
    public AudioClip musicToPlay; // ������Ҫ���ŵ�����
    public bool stopCurrentMusic = true; // �������Ƿ�ֹͣ��ǰ����
    [HideInInspector] public bool conditionMet = false;
    [HideInInspector] public bool hasBeenTriggered = false;
}

public class CollectibleManager : MonoBehaviour
{
    public GameObject textSystem;
    private SpriteRenderer[] sprites;
    public static CollectibleManager Instance;

    public int totalCollected = 0;
    
    // ��������ƵԴ����
    public AudioSource backgroundMusicSource;
    public AudioSource soundEffectSource;
    
    // ��ǰ���ŵ�����
    private AudioClip currentMusic;

    public List<CollectionThreshold> thresholds = new List<CollectionThreshold>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
            
        // ȷ������ƵԴ���
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

        Debug.Log("���ռ���Ʒ����" + totalCollected);
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

        //�����л�
        private IEnumerator TransitionTextSystems(int newSystemIndex)
        {
            isTransitioning = true;

            // ��ȡ��ǰ���µ��ı�ϵͳ
            GameObject currentSystem = currentSystemIndex >= 0 ? textSystems[currentSystemIndex] : null;
            GameObject newSystem = textSystems[newSystemIndex];

            // ������ϵͳ���������ܽ�ֵΪ1����ȫ�ܽ⣩
            newSystem.SetActive(true);
            SetDissolveAmount(newSystem, 1f);

            // ͬʱ����������������ǰϵͳ�ܽ���ʧ����ϵͳ�ܽ����
            float elapsedTime = 0f;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / transitionDuration;

                if (currentSystem != null)
                {
                    SetDissolveAmount(currentSystem, t); // ��0��1�ܽ�
                }

                SetDissolveAmount(newSystem, 1 - t); // ��1��0��ʾ
                yield return null;
            }

            // ȷ������ֵ׼ȷ
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
            // ��ȡ�����������SpriteRenderer
            SpriteRenderer[] renderers = textSystem.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer renderer in renderers)
            {
                if (renderer.material.HasProperty("_DissolveAmount"))
                {
                    renderer.material.SetFloat("_DissolveAmount", amount);
                }
            }
        }

        // ʾ����������ε����л�
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
                
                // �������������ֲ���
                if (threshold.musicToPlay != null)
                {
                    if (threshold.stopCurrentMusic)
                    {
                        // ֱ�Ӳ���������
                        PlayMusic(threshold.musicToPlay);
                    }
                    else
                    {
                        // ���뵭������
                        StartCoroutine(CrossFadeToNewMusic(threshold.musicToPlay));
                    }
                }

                threshold.hasBeenTriggered = true;
                Debug.Log($"Threshold {threshold.valueRequired} effects triggered!");
            }
        }
    }
    
    // ������ֱ�Ӳ������ֵķ���
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
    
    // ���������뵭�����ɵ�������
    private IEnumerator CrossFadeToNewMusic(AudioClip newMusic)
    {
        if (newMusic == null) yield break;
        
        // ������ʱ��ƵԴ���ڹ���
        AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
        newAudioSource.clip = newMusic;
        newAudioSource.volume = 0f;
        newAudioSource.loop = true;
        newAudioSource.Play();
        
        float duration = 3.0f; // ����ʱ��
        float timer = 0f;
        
        float originalVolume = backgroundMusicSource.volume;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float ratio = timer / duration;
            
            // ���;������������������������
            backgroundMusicSource.volume = Mathf.Lerp(originalVolume, 0f, ratio);
            newAudioSource.volume = Mathf.Lerp(0f, originalVolume, ratio);
            
            yield return null;
        }
        
        // ������ɺ�����
        backgroundMusicSource.Stop();
        Destroy(backgroundMusicSource);
        
        // ������ƵԴ����Ϊ����ƵԴ
        backgroundMusicSource = newAudioSource;
        backgroundMusicSource.volume = originalVolume;
        currentMusic = newMusic;
    }

    // ������������Ч
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

    // �޸ĺ�Ľ��浭�뵭�����������ڿ��Ա�����
    public void CrossFadeMusic(AudioClip newMusic, float fadeDuration = 6.0f)
    {
        if (newMusic == null || newMusic == currentMusic) return;
        
        StartCoroutine(CrossFadeCoroutine(backgroundMusicSource, newMusic, fadeDuration));
    }
    
    private IEnumerator CrossFadeCoroutine(AudioSource currentSource, AudioClip newMusic, float fadeDuration)
    {
        // �����µ���ƵԴ���ڲ���������
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

            // ���͵�ǰ�����������������������
            // ʹ�ö������߶��������Ա仯
            float logRatio = Mathf.Log10(ratio * 9 + 1); // ��0-1ӳ�䵽0-1�������ֲ�

            currentSource.volume = Mathf.Lerp(currentVolume, 0.001f, logRatio);
            newSource.volume = Mathf.Lerp(0.001f, currentVolume, logRatio);
            
            yield return null;
        }

        // ������ɺ�����
        currentSource.Stop();
        Destroy(currentSource);
        
        // ������ƵԴ����Ϊ����ƵԴ
        backgroundMusicSource = newSource;
        backgroundMusicSource.volume = currentVolume;
        currentMusic = newMusic;
    }
}