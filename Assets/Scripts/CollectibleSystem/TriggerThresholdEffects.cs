using UnityEngine;

public class ThresholdTrigger : MonoBehaviour
{
    public int thresholdValue; // ��Ҫ��������ֵ��ֵ

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ֪ͨ������������ֵЧ��
            CollectibleManager.Instance.TriggerThresholdEffects(thresholdValue);
        }
    }
}