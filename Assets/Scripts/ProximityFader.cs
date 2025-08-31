using UnityEngine;

public class ObscuringObject : MonoBehaviour
{
    [Header("淡化设置")]
    public float fadeRadius = 2f; // 完全淡出的距离
    public float fullVisibleRadius = 0.5f; // 开始淡出的距离
    public float fadeSpeed = 2f; // 淡化速度

    private SpriteRenderer spriteRenderer;
    private Transform player;
    private float initialAlpha; // 初始透明度

    void Start()
    {
        // 获取自身的SpriteRenderer组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        

        // 查找玩家对象
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("未找到带有'Player'标签的对象，请确保玩家对象已正确标记。");
        }

        // 保存初始透明度
        initialAlpha = spriteRenderer.color.a;
    }

    void Update()
    {
        if (player == null || spriteRenderer == null) return;

        // 计算玩家与遮挡物的距离
        float distance = Vector2.Distance(transform.position, player.position);

        // 计算目标透明度
        float targetAlpha = CalculateTargetAlpha(distance);

        // 平滑过渡到目标透明度
        Color currentColor = spriteRenderer.color;
        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
        spriteRenderer.color = currentColor;
    }

    float CalculateTargetAlpha(float distance)
    {
        // 如果玩家在完全可见半径内，完全透明
        if (distance <= fullVisibleRadius)
        {
            return 0f;
        }
        // 如果玩家在淡出半径外，完全不透明
        else if (distance >= fadeRadius)
        {
            return initialAlpha;
        }
        // 在两者之间，线性插值
        else
        {
            // 计算在淡出范围内的比例 (0到1)
            float ratio = (distance - fullVisibleRadius) / (fadeRadius - fullVisibleRadius);
            // 根据比例返回透明度 (0到initialAlpha)
            return Mathf.Lerp(0f, initialAlpha, ratio);
        }
    }

    // 可视化检测范围（在Scene视图中显示）
    void OnDrawGizmosSelected()
    {
        // 绘制淡出半径
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fadeRadius);

        // 绘制完全可见半径
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, fullVisibleRadius);
    }
}