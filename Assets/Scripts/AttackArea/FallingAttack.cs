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
            // 向下移动
            transform.Translate(Vector3.down * speed * Time.deltaTime);

            // 检查是否到达底部
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

        // 到达地面时销毁
        //if (other.CompareTag("Ground") && !hasHit)
        //{
        //    hasHit = true;
        //    Destroy(gameObject);
        //}
    }
}