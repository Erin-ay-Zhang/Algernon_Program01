using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public GameObject textSystem;
    private SpriteRenderer[] sprites;
    public static CollectibleManager Instance;

    public int totalCollected = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        sprites = new SpriteRenderer[textSystem.transform.childCount];
        for (int i = 0; i < textSystem.transform.childCount; ++i)
        {
            var a = textSystem.transform.GetChild(i);
            sprites[i] = a.GetComponent<SpriteRenderer>();
        }
    }

    public void Collect(int value)
    {
        totalCollected += value;

        UpdateTextState();

        Debug.Log("已收集物品数：" + totalCollected);
        // 更新文字状态
    }

    private void UpdateTextState()
    {
        if(totalCollected>= sprites.Length)
            return;

        for (int i = 0; i < sprites.Length; ++i)
        {
            sprites[i].gameObject.SetActive(false);// .enabled = false;
        }

        sprites[totalCollected].gameObject.SetActive(true);
    }
}
