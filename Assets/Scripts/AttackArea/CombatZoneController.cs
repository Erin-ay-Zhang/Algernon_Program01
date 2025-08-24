using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatZoneController : MonoBehaviour
{
    [Header("��������")]
    public GameObject attackImagePrefab; // ����ͼƬԤ����
    public int attackCount = 5; // ��������
    public float attackInterval = 1.5f; // �������
    public float fallSpeed = 5f; // �����ٶ�
    public float spawnHeight = 5f; // ���ɸ߶ȣ���������򶥲���

    [Header("��������")]
    public Transform respawnPoint; // ������

    private BoxCollider2D _fightAreaCollider;
    private bool isAttacking = false;
    private int currentAttackCount = 0;
    private List<GameObject> activeAttackImages = new List<GameObject>();

    void Start()
    {
        // ��ȡս���������ײ��
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
            // �������������
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

        // ��ȡս������ı߽�
        Bounds areaBounds = _fightAreaCollider.bounds;

        // ��ս�����������������λ��
        float randomX = Random.Range(
            areaBounds.center.x - areaBounds.extents.x,
            areaBounds.center.x + areaBounds.extents.x
        );

        // ��ս�������Ϸ�����
        Vector3 spawnPosition = new Vector3(
            randomX,
            areaBounds.max.y + spawnHeight,
            0
        );

        // ʵ��������ͼƬ
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
        // ��ȡ��ҵ�����������
        PlayerDeathHandler deathHandler = player.GetComponent<PlayerDeathHandler>();
        if (deathHandler != null && respawnPoint != null)
        {
            deathHandler.DieAndRespawn(respawnPoint.position);
        }

        // ֹͣ���й���
        StopAllCoroutines();
        isAttacking = false;

        // ������й���ͼƬ
        foreach (GameObject attackImage in activeAttackImages)
        {
            if (attackImage != null)
            {
                Destroy(attackImage);
            }
        }
        activeAttackImages.Clear();
    }

    // ��Scene��ͼ�л�������Χ������λ��
    void OnDrawGizmosSelected()
    {
        if (_fightAreaCollider == null)
            _fightAreaCollider = GetComponent<BoxCollider2D>();

        if (_fightAreaCollider != null)
        {
            // ����ս������
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(_fightAreaCollider.bounds.center, _fightAreaCollider.bounds.size);

            // ������������
            Gizmos.color = Color.yellow;
            float spawnY = _fightAreaCollider.bounds.max.y + spawnHeight;
            Gizmos.DrawLine(
                new Vector3(_fightAreaCollider.bounds.min.x, spawnY, 0),
                new Vector3(_fightAreaCollider.bounds.max.x, spawnY, 0)
            );

            // ����������
            if (respawnPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(respawnPoint.position, 0.5f);
                Gizmos.DrawLine(transform.position, respawnPoint.position);
            }
        }
    }
}