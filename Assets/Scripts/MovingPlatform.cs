using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("移动设置")]
    public Transform startPoint;  // 起点
    public Transform endPoint;   // 终点
    public float speed = 2f;     // 移动速度
    public float waitTime = 1f;  // 到达点后的等待时间

    private Vector3 _targetPos;
    private bool _isWaiting;
    private float _waitTimer;

    void Start()
    {
        transform.position = startPoint.position;
        _targetPos = endPoint.position;
    }

    void Update()
    {
        if (_isWaiting)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= waitTime)
            {
                _isWaiting = false;
                _waitTimer = 0;
            }
            return;
        }

        // 移动平台
        transform.position = Vector3.MoveTowards(
            transform.position,
            _targetPos,
            speed * Time.deltaTime
        );

        // 检查是否到达目标点
        if (Vector3.Distance(transform.position, _targetPos) < 0.01f)
        {
            _isWaiting = true;
            // 切换目标点
            _targetPos = (_targetPos == (Vector3)endPoint.position) ?
                startPoint.position : endPoint.position;
        }
    }
}