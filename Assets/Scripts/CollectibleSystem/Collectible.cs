using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int value = 1; // 每个物品的价值，比如1个金币

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 通知管理器增加分数
            CollectibleManager.Instance.Collect(value);
            // 通知管理器触发阈值效果
            CollectibleManager.Instance.TriggerThresholdEffects(3);

            // 销毁自己
            Destroy(gameObject);

        }
    }
}


    

