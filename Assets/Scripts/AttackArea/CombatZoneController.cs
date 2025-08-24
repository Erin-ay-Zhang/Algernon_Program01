using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatZoneController : MonoBehaviour
{
    [Header("攻击设置")]
    public GameObject attackImagePrefab; // 攻击图片预制体
    public int attackCount = 5; // 攻击次数
    public float attackInterval = 1.5f; // 攻击间隔
    public float fallSpeed = 5f; // 下落速度
    public float spawnHeight = 5f; // 生成高度（相对于区域顶部）

    [Header("区域设置")]
    public Transform respawnPoint; // 重生点

    private BoxCollider2D _fightAreaCollider;
    private bool isAttacking = false;
    private int currentAttackCount = 0;
    private List<GameObject> activeAttackImages = new List<GameObject>();

    void Start()
    {
        // 获取战斗区域的碰撞器
        _fightAreaCollider = GetComponent<BoxCollider2D>();

        if (_fightAreaCollider == null)
        {
            Debug.LogError("CombatZoneController requires a BoxCollider2D component!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isAttacking && _fightAreaCollider != null)
        {
            // 设置玩家重生点
            PlayerDeathHandler deathHandler = other.GetComponent<PlayerDeathHandler>();
            if (deathHandler != null && respawnPoint != null)
            {
                deathHandler.SetRespawnPoint(respawnPoint.position);
            }

            StartCoroutine(StartAttackSequence());
        }
    }

    IEnumerator StartAttackSequence()
    {
        isAttacking = true;
        currentAttackCount = 0;

        while (currentAttackCount < attackCount)
        {
            SpawnAttackImage();
            currentAttackCount++;
            yield return new WaitForSeconds(attackInterval);
        }

        isAttacking = false;
    }

    void SpawnAttackImage()
    {
        if (_fightAreaCollider == null) return;

        // 获取战斗区域的边界
        Bounds areaBounds = _fightAreaCollider.bounds;

        // 在战斗区域宽度内随机生成位置
        float randomX = Random.Range(
            areaBounds.center.x - areaBounds.extents.x,
            areaBounds.center.x + areaBounds.extents.x
        );

        // 在战斗区域上方生成
        Vector3 spawnPosition = new Vector3(
            randomX,
            areaBounds.max.y + spawnHeight,
            0
        );

        // 实例化攻击图片
        GameObject attackImage = Instantiate(attackImagePrefab, spawnPosition, Quaternion.identity);
        FallingAttack attackScript = attackImage.GetComponent<FallingAttack>();

        if (attackScript != null)
        {
            attackScript.Initialize(this, fallSpeed, areaBounds.min.y);
        }

        activeAttackImages.Add(attackImage);
    }

    public void OnPlayerHit(GameObject player)
    {
        // 获取玩家的死亡处理器
        PlayerDeathHandler deathHandler = player.GetComponent<PlayerDeathHandler>();
        if (deathHandler != null && respawnPoint != null)
        {
            deathHandler.DieAndRespawn(respawnPoint.position);
        }

        // 停止所有攻击
        StopAllCoroutines();
        isAttacking = false;

        // 清除所有攻击图片
        foreach (GameObject attackImage in activeAttackImages)
        {
            if (attackImage != null)
            {
                Destroy(attackImage);
            }
        }
        activeAttackImages.Clear();
    }

    // 在Scene视图中绘制区域范围和生成位置
    void OnDrawGizmosSelected()
    {
        if (_fightAreaCollider == null)
            _fightAreaCollider = GetComponent<BoxCollider2D>();

        if (_fightAreaCollider != null)
        {
            // 绘制战斗区域
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(_fightAreaCollider.bounds.center, _fightAreaCollider.bounds.size);

            // 绘制生成区域
            Gizmos.color = Color.yellow;
            float spawnY = _fightAreaCollider.bounds.max.y + spawnHeight;
            Gizmos.DrawLine(
                new Vector3(_fightAreaCollider.bounds.min.x, spawnY, 0),
                new Vector3(_fightAreaCollider.bounds.max.x, spawnY, 0)
            );

            // 绘制重生点
            if (respawnPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(respawnPoint.position, 0.5f);
                Gizmos.DrawLine(transform.position, respawnPoint.position);
            }
        }
    }
}