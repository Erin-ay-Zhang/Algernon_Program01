using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int value = 1; // ÿ����Ʒ�ļ�ֵ������1�����

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ֪ͨ���������ӷ���
            CollectibleManager.Instance.Collect(value);

            // �����Լ�
            Destroy(gameObject);

        }
    }
}


    

