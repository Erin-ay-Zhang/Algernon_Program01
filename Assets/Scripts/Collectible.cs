using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int value = 1; // ÿ����Ʒ�ļ�ֵ������1�����

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ֪ͨ���������ӷ���
            CollectibleManager.Instance.Collect(value);

            // �����Լ�
            Destroy(gameObject);

        }


        Debug.Log("��������������ײ����");
        if (other.CompareTag("Player"))
        {
            Debug.Log("�ռ��������");
            Destroy(gameObject);
        }
    }

    
   
}


    

