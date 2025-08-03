using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("�ƶ�����")]
    public Transform startPoint;  // ���
    public Transform endPoint;   // �յ�
    public float speed = 2f;     // �ƶ��ٶ�
    public float waitTime = 1f;  // ������ĵȴ�ʱ��

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

        // �ƶ�ƽ̨
        transform.position = Vector3.MoveTowards(
            transform.position,
            _targetPos,
            speed * Time.deltaTime
        );

        // ����Ƿ񵽴�Ŀ���
        if (Vector3.Distance(transform.position, _targetPos) < 0.01f)
        {
            _isWaiting = true;
            // �л�Ŀ���
            _targetPos = (_targetPos == (Vector3)endPoint.position) ?
                startPoint.position : endPoint.position;
        }
    }
}