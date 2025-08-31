using UnityEngine;

public class ObscuringObject : MonoBehaviour
{
    [Header("��������")]
    public float fadeRadius = 2f; // ��ȫ�����ľ���
    public float fullVisibleRadius = 0.5f; // ��ʼ�����ľ���
    public float fadeSpeed = 2f; // �����ٶ�

    private SpriteRenderer spriteRenderer;
    private Transform player;
    private float initialAlpha; // ��ʼ͸����

    void Start()
    {
        // ��ȡ�����SpriteRenderer���
        spriteRenderer = GetComponent<SpriteRenderer>();
        

        // ������Ҷ���
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("δ�ҵ�����'Player'��ǩ�Ķ�����ȷ����Ҷ�������ȷ��ǡ�");
        }

        // �����ʼ͸����
        initialAlpha = spriteRenderer.color.a;
    }

    void Update()
    {
        if (player == null || spriteRenderer == null) return;

        // ����������ڵ���ľ���
        float distance = Vector2.Distance(transform.position, player.position);

        // ����Ŀ��͸����
        float targetAlpha = CalculateTargetAlpha(distance);

        // ƽ�����ɵ�Ŀ��͸����
        Color currentColor = spriteRenderer.color;
        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
        spriteRenderer.color = currentColor;
    }

    float CalculateTargetAlpha(float distance)
    {
        // ����������ȫ�ɼ��뾶�ڣ���ȫ͸��
        if (distance <= fullVisibleRadius)
        {
            return 0f;
        }
        // �������ڵ����뾶�⣬��ȫ��͸��
        else if (distance >= fadeRadius)
        {
            return initialAlpha;
        }
        // ������֮�䣬���Բ�ֵ
        else
        {
            // �����ڵ�����Χ�ڵı��� (0��1)
            float ratio = (distance - fullVisibleRadius) / (fadeRadius - fullVisibleRadius);
            // ���ݱ�������͸���� (0��initialAlpha)
            return Mathf.Lerp(0f, initialAlpha, ratio);
        }
    }

    // ���ӻ���ⷶΧ����Scene��ͼ����ʾ��
    void OnDrawGizmosSelected()
    {
        // ���Ƶ����뾶
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fadeRadius);

        // ������ȫ�ɼ��뾶
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, fullVisibleRadius);
    }
}