using UnityEngine;
using System.Collections;

public class ObjectShakeAndMove : MonoBehaviour
{
    [Header("Ŀ������")]
    public Vector3 targetPosition;       // ����Ҫ�ƶ�����λ��
    public float moveDuration = 2f;      // �ƶ�����ʱ��
    public float rotationSpeed = 180f;   // ��ת�ٶȣ���/�룩

    [Header("��������")]
    public float shakeDuration = 1f;     // ����ʱ��
    public float shakeMagnitude = 0.1f;  // ��������

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
        // -------- �����׶� --------
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
            transform.position = initialPosition + new Vector3(randomOffset.x, randomOffset.y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // �ָ�����ʼλ��
        transform.position = initialPosition;

        // -------- �ƶ� + ��ת�׶� --------
        Vector3 startPosition = transform.position;
        elapsed = 0f;
        while (elapsed < moveDuration)
        {
            // �ƶ�
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / moveDuration);

            // ��ת
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ��֤���յ���Ŀ��λ��
        transform.position = targetPosition;
    }
}
