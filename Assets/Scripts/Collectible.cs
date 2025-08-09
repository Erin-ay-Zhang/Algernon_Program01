using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int value = 1; // 每个物品的价值，比如1个金币

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 通知管理器增加分数
            CollectibleManager.Instance.Collect(value);

            // 销毁自己
            Destroy(gameObject);

        }


        Debug.Log("触发器触发，碰撞到：");
        if (other.CompareTag("Player"))
        {
            Debug.Log("收集到了玩家");
            Destroy(gameObject);
        }
    }

    
   
}


    

