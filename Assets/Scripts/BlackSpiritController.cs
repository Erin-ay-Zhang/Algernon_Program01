using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BlackSpiritController : MonoBehaviour
{
    [Header("飘动设置")]
    [Tooltip("飘动的最大半径")]
    public float maxRadius = 1f; // 最大飘动半径

    [Tooltip("移动速度")]
    public float moveSpeed = 0.5f;

    [Tooltip("方向变化频率")]
    public float directionChangeInterval = 2f;

    [Tooltip("方向变化平滑度")]
    [Range(0.1f, 1f)]
    public float directionSmoothness = 0.5f;

    [Tooltip("是否在启动时开始飘动")]
    public bool playOnStart = true;

    private Vector3 originalPosition; // 原始中心位置
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
            // 检查是否需要改变方向
            if (Time.time >= nextDirectionChangeTime)
            {
                ChangeDirection();
                nextDirectionChangeTime = Time.time + directionChangeInterval;
            }

            // 检查是否接近边界，如果是则提前改变方向
            Vector3 toCenter = originalPosition - transform.position;
            if (toCenter.magnitude > maxRadius * 0.8f)
            {
                // 向中心方向偏转
                Vector2 centerDir = new Vector2(toCenter.x, toCenter.y).normalized;
                currentDirection = Vector2.Lerp(currentDirection, centerDir, 0.5f).normalized;
            }
        }
    }

    public void StartFloating()
    {
        StopFloating();

        // 初始随机方向
        currentDirection = Random.insideUnitCircle.normalized;

        // 开始连续移动
        MoveToNextPosition();
    }

    private void MoveToNextPosition()
    {
        // 计算目标位置
        Vector3 currentPos = transform.position;
        Vector3 targetPos = currentPos + (Vector3)currentDirection * moveSpeed;

        // 确保不会超出最大半径
        Vector3 toTargetCenter = targetPos - originalPosition;
        if (toTargetCenter.magnitude > maxRadius)
        {
            // 如果超出半径，向切线方向偏转
            targetPos = originalPosition + toTargetCenter.normalized * maxRadius;
            currentDirection = Vector2.Perpendicular(currentDirection) * Random.Range(0.8f, 1.2f);
        }

        // 计算实际移动距离和时间
        float distance = Vector3.Distance(currentPos, targetPos);
        float duration = distance / moveSpeed;

        // 创建移动动画
        moveTween = transform.DOMove(targetPos, duration)
            .SetEase(Ease.InCubic)
            .OnComplete(MoveToNextPosition);
    }

    private void ChangeDirection()
    {
        // 随机改变方向，但保持一定的连续性
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

        // 绘制当前移动方向
        if (Application.isPlaying && moveTween != null && moveTween.IsActive())
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)currentDirection * 0.5f);
        }
    }
}
