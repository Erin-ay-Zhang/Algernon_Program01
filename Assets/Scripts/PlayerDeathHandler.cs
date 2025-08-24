using UnityEngine;
using System.Collections;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("死亡设置")]
    public float respawnDelay = 1f; // 重生延迟时间
    public GameObject deathEffect; // 死亡特效（可选）
    public AudioClip deathSound; // 死亡音效（可选）

    [Header("组件引用")]
    public MonoBehaviour playerController; // 玩家控制器引用
    public Collider2D playerCollider; // 玩家碰撞器引用
    public SpriteRenderer playerSprite; // 玩家精灵渲染器引用

    private Vector3 _respawnPoint;
    private bool _isDead = false;
    private Rigidbody2D _rb;

    void Awake()
    {
        // 获取组件引用
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        if (playerCollider == null) playerCollider = GetComponent<Collider2D>();
        if (playerSprite == null) playerSprite = GetComponent<SpriteRenderer>();

        // 设置初始重生点
        _respawnPoint = transform.position;
    }

    // 设置重生点
    public void SetRespawnPoint(Vector3 point)
    {
        _respawnPoint = point;
    }

    // 死亡并重生
    public void DieAndRespawn(Vector3 respawnPosition)
    {
        if (_isDead) return;

        StartCoroutine(DeathAndRespawnCoroutine(respawnPosition));
    }

    // 死亡并重生协程
    private IEnumerator DeathAndRespawnCoroutine(Vector3 respawnPosition)
    {
        _isDead = true;

        // 禁用玩家控制
        if (playerController != null)
            playerController.enabled = false;

        // 停止所有运动
        if (_rb != null)
        {
            _rb.velocity = Vector2.zero;
            _rb.isKinematic = true; // 防止物理交互
        }

        // 禁用碰撞
        if (playerCollider != null)
            playerCollider.enabled = false;

        // 播放死亡效果
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        // 播放死亡音效
        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position);

        // 隐藏玩家
        if (playerSprite != null)
            playerSprite.enabled = false;

        // 等待重生延迟
        yield return new WaitForSeconds(respawnDelay);

        // 重生玩家
        Respawn(respawnPosition);
    }

    // 重生玩家
    public void Respawn(Vector3 respawnPosition)
    {
        // 移动到重生点
        transform.position = respawnPosition;

        // 重新启用玩家控制
        if (playerController != null)
            playerController.enabled = true;

        // 重新启用物理
        if (_rb != null)
            _rb.isKinematic = false;

        // 重新启用碰撞
        if (playerCollider != null)
            playerCollider.enabled = true;

        // 显示玩家
        if (playerSprite != null)
            playerSprite.enabled = true;

        _isDead = false;
    }

    // 检查玩家是否死亡
    public bool IsDead()
    {
        return _isDead;
    }

    // 当玩家被攻击击中时调用
    void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否被攻击图片击中
        if (other.CompareTag("AttackImage") && !_isDead)
        {
            // 通知战斗区域控制器玩家被击中
            CombatZoneController zoneController = FindObjectOfType<CombatZoneController>();
            if (zoneController != null)
            {
                zoneController.OnPlayerHit(gameObject);
            }
        }
    }
}