using UnityEngine;
using System.Collections;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("��������")]
    public float respawnDelay = 1f; // �����ӳ�ʱ��
    public GameObject deathEffect; // ������Ч����ѡ��
    public AudioClip deathSound; // ������Ч����ѡ��

    [Header("�������")]
    public MonoBehaviour playerController; // ��ҿ���������
    public Collider2D playerCollider; // �����ײ������
    public SpriteRenderer playerSprite; // ��Ҿ�����Ⱦ������

    private Vector3 _respawnPoint;
    private bool _isDead = false;
    private Rigidbody2D _rb;

    void Awake()
    {
        // ��ȡ�������
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        if (playerCollider == null) playerCollider = GetComponent<Collider2D>();
        if (playerSprite == null) playerSprite = GetComponent<SpriteRenderer>();

        // ���ó�ʼ������
        _respawnPoint = transform.position;
    }

    // ����������
    public void SetRespawnPoint(Vector3 point)
    {
        _respawnPoint = point;
    }

    // ����������
    public void DieAndRespawn(Vector3 respawnPosition)
    {
        if (_isDead) return;

        StartCoroutine(DeathAndRespawnCoroutine(respawnPosition));
    }

    // ����������Э��
    private IEnumerator DeathAndRespawnCoroutine(Vector3 respawnPosition)
    {
        _isDead = true;

        // ������ҿ���
        if (playerController != null)
            playerController.enabled = false;

        // ֹͣ�����˶�
        if (_rb != null)
        {
            _rb.velocity = Vector2.zero;
            _rb.isKinematic = true; // ��ֹ������
        }

        // ������ײ
        if (playerCollider != null)
            playerCollider.enabled = false;

        // ��������Ч��
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        // ����������Ч
        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position);

        // �������
        if (playerSprite != null)
            playerSprite.enabled = false;

        // �ȴ������ӳ�
        yield return new WaitForSeconds(respawnDelay);

        // �������
        Respawn(respawnPosition);
    }

    // �������
    public void Respawn(Vector3 respawnPosition)
    {
        // �ƶ���������
        transform.position = respawnPosition;

        // ����������ҿ���
        if (playerController != null)
            playerController.enabled = true;

        // ������������
        if (_rb != null)
            _rb.isKinematic = false;

        // ����������ײ
        if (playerCollider != null)
            playerCollider.enabled = true;

        // ��ʾ���
        if (playerSprite != null)
            playerSprite.enabled = true;

        _isDead = false;
    }

    // �������Ƿ�����
    public bool IsDead()
    {
        return _isDead;
    }

    // ����ұ���������ʱ����
    void OnTriggerEnter2D(Collider2D other)
    {
        // ����Ƿ񱻹���ͼƬ����
        if (other.CompareTag("AttackImage") && !_isDead)
        {
            // ֪ͨս�������������ұ�����
            CombatZoneController zoneController = FindObjectOfType<CombatZoneController>();
            if (zoneController != null)
            {
                zoneController.OnPlayerHit(gameObject);
            }
        }
    }
}