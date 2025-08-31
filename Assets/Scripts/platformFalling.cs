using UnityEngine;
using System.Collections;

public class ObjectShakeAndMove : MonoBehaviour
{
    [Header("目标设置")]
    public Vector3 targetPosition;       // 最终要移动到的位置
    public float moveDuration = 2f;      // 移动持续时间
    public float rotationSpeed = 180f;   // 旋转速度（度/秒）

    [Header("抖动设置")]
    public float shakeDuration = 1f;     // 抖动时长
    public float shakeMagnitude = 0.1f;  // 抖动幅度

    private Vector3 initialPosition;
    private bool isTriggered = false;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            StartCoroutine(ShakeThenMove());
        }
    }

    private IEnumerator ShakeThenMove()
    {
        // -------- 抖动阶段 --------
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
            transform.position = initialPosition + new Vector3(randomOffset.x, randomOffset.y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 恢复到初始位置
        transform.position = initialPosition;

        // -------- 移动 + 旋转阶段 --------
        Vector3 startPosition = transform.position;
        elapsed = 0f;
        while (elapsed < moveDuration)
        {
            // 移动
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / moveDuration);

            // 旋转
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 保证最终到达目标位置
        transform.position = targetPosition;
    }
}
