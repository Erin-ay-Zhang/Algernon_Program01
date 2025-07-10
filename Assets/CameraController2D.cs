using UnityEngine;

public class CameraController2D : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("摄像头移动速度")]
    public float moveSpeed = 5f;  // 默认移动速度

    [Tooltip("移动平滑度")]
    [Range(0, 1)] public float smoothFactor = 0.5f;

    private Vector3 targetPosition;  // 目标位置

    void Start()
    {
        // 初始化目标位置为当前位置
        targetPosition = transform.position;
    }

    void Update()
    {
        HandleInput();  // 处理键盘输入
        MoveCamera();   // 移动摄像机
    }

    void HandleInput()
    {
        // 获取WASD输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 计算移动方向
        Vector3 moveDirection = new Vector3(horizontal, vertical, 0).normalized;

        // 更新目标位置（保持Z轴不变）
        targetPosition += moveDirection * moveSpeed * Time.deltaTime;
    }

    void MoveCamera()
    {
        // 使用Lerp实现平滑移动
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothFactor * Time.deltaTime * 10f
        );
    }
}