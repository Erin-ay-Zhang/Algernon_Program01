using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance;

    public int totalCollected = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Collect(int value)
    {
        totalCollected += value;
        Debug.Log("���ռ���Ʒ����" + totalCollected);
        // ��������״̬
    }
}
