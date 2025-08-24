using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingAttack : MonoBehaviour
{
    private CombatZoneController zoneController;
    private float speed;
    private float bottomY;
    private bool hasHit = false;

    public void Initialize(CombatZoneController controller, float fallSpeed, float bottomPositionY)
    {
        zoneController = controller;
        speed = fallSpeed;
        bottomY = bottomPositionY;
    }

    void Update()
    {
        if (!hasHit)
        {
            // �����ƶ�
            transform.Translate(Vector3.down * speed * Time.deltaTime);

            // ����Ƿ񵽴�ײ�
            if (transform.position.y <= bottomY)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasHit)
        {
            hasHit = true;
            zoneController.OnPlayerHit(other.gameObject);
            Destroy(gameObject);
        }

        // �������ʱ����
        //if (other.CompareTag("Ground") && !hasHit)
        //{
        //    hasHit = true;
        //    Destroy(gameObject);
        //}
    }
}