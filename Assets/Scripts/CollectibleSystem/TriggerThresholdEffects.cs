using UnityEngine;

public class ThresholdTrigger : MonoBehaviour
{
    public int thresholdValue; // 需要触发的阈值数值

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 通知管理器触发阈值效果
            CollectibleManager.Instance.TriggerThresholdEffects(thresholdValue);
        }
    }
}