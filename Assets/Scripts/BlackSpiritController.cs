using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BlackSpiritController : MonoBehaviour
{
    [Header("Ʈ������")]
    [Tooltip("Ʈ�������뾶")]
    public float maxRadius = 1f; // ���Ʈ���뾶

    [Tooltip("�ƶ��ٶ�")]
    public float moveSpeed = 0.5f;

    [Tooltip("����仯Ƶ��")]
    public float directionChangeInterval = 2f;

    [Tooltip("����仯ƽ����")]
    [Range(0.1f, 1f)]
    public float directionSmoothness = 0.5f;

    [Tooltip("�Ƿ�������ʱ��ʼƮ��")]
    public bool playOnStart = true;

    private Vector3 originalPosition; // ԭʼ����λ��
    private Vector2 currentDirection;
    private float nextDirectionChangeTime;
    private Tween moveTween;

    void Start()
    {
        originalPosition = transform.position;
        currentDirection = Random.insideUnitCircle.normalized;
        nextDirectionChangeTime = Time.time + directionChangeInterval;

        if (playOnStart)
        {
            StartFloating();
        }
    }

    void Update()
    {
        if (moveTween != null && moveTween.IsActive())
        {
            // ����Ƿ���Ҫ�ı䷽��
            if (Time.time >= nextDirectionChangeTime)
            {
                ChangeDirection();
                nextDirectionChangeTime = Time.time + directionChangeInterval;
            }

            // ����Ƿ�ӽ��߽磬���������ǰ�ı䷽��
            Vector3 toCenter = originalPosition - transform.position;
            if (toCenter.magnitude > maxRadius * 0.8f)
            {
                // �����ķ���ƫת
                Vector2 centerDir = new Vector2(toCenter.x, toCenter.y).normalized;
                currentDirection = Vector2.Lerp(currentDirection, centerDir, 0.5f).normalized;
            }
        }
    }

    public void StartFloating()
    {
        StopFloating();

        // ��ʼ�������
        currentDirection = Random.insideUnitCircle.normalized;

        // ��ʼ�����ƶ�
        MoveToNextPosition();
    }

    private void MoveToNextPosition()
    {
        // ����Ŀ��λ��
        Vector3 currentPos = transform.position;
        Vector3 targetPos = currentPos + (Vector3)currentDirection * moveSpeed;

        // ȷ�����ᳬ�����뾶
        Vector3 toTargetCenter = targetPos - originalPosition;
        if (toTargetCenter.magnitude > maxRadius)
        {
            // ��������뾶�������߷���ƫת
            targetPos = originalPosition + toTargetCenter.normalized * maxRadius;
            currentDirection = Vector2.Perpendicular(currentDirection) * Random.Range(0.8f, 1.2f);
        }

        // ����ʵ���ƶ������ʱ��
        float distance = Vector3.Distance(currentPos, targetPos);
        float duration = distance / moveSpeed;

        // �����ƶ�����
        moveTween = transform.DOMove(targetPos, duration)
            .SetEase(Ease.InCubic)
            .OnComplete(MoveToNextPosition);
    }

    private void ChangeDirection()
    {
        // ����ı䷽�򣬵�����һ����������
        Vector2 newDirection = Random.insideUnitCircle.normalized;
        currentDirection = Vector2.Lerp(currentDirection, newDirection, 1f - directionSmoothness).normalized;
    }

    public void StopFloating()
    {
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }
    }

    void OnDestroy()
    {
        StopFloating();
    }

    void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying ? originalPosition : transform.position;
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.3f);
        Gizmos.DrawWireSphere(center, maxRadius);

        // ���Ƶ�ǰ�ƶ�����
        if (Application.isPlaying && moveTween != null && moveTween.IsActive())
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)currentDirection * 0.5f);
        }
    }
}
