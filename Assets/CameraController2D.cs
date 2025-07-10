using UnityEngine;

public class CameraController2D : MonoBehaviour
{
    [Header("�ƶ�����")]
    [Tooltip("����ͷ�ƶ��ٶ�")]
    public float moveSpeed = 5f;  // Ĭ���ƶ��ٶ�

    [Tooltip("�ƶ�ƽ����")]
    [Range(0, 1)] public float smoothFactor = 0.5f;

    private Vector3 targetPosition;  // Ŀ��λ��

    void Start()
    {
        // ��ʼ��Ŀ��λ��Ϊ��ǰλ��
        targetPosition = transform.position;
    }

    void Update()
    {
        HandleInput();  // �����������
        MoveCamera();   // �ƶ������
    }

    void HandleInput()
    {
        // ��ȡWASD����
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // �����ƶ�����
        Vector3 moveDirection = new Vector3(horizontal, vertical, 0).normalized;

        // ����Ŀ��λ�ã�����Z�᲻�䣩
        targetPosition += moveDirection * moveSpeed * Time.deltaTime;
    }

    void MoveCamera()
    {
        // ʹ��Lerpʵ��ƽ���ƶ�
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothFactor * Time.deltaTime * 10f
        );
    }
}