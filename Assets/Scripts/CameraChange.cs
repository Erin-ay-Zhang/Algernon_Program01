using UnityEngine;
using Cinemachine;

public class DualCameraZoneTrigger2D : MonoBehaviour
{
    [Header("跟随相机")]
    [SerializeField] private CinemachineVirtualCamera followVCam;

    [Header("第一个固定相机区域")]
    [SerializeField] private Collider2D zone1Trigger;
    [SerializeField] private CinemachineVirtualCamera fixedCam1;
    [SerializeField] private float zone1BlendTime = 1f;

    [Header("第二个固定相机区域")]
    [SerializeField] private Collider2D zone2Trigger;
    [SerializeField] private CinemachineVirtualCamera fixedCam2;
    [SerializeField] private float zone2BlendTime = 1f;

    [Header("默认设置")]
    [SerializeField] private float defaultBlendTime = 1f;

    private CinemachineVirtualCamera currentActiveCamera;

    private void Start()
    {
        // 初始化相机优先级
        if (followVCam != null) followVCam.Priority = 10;
        if (fixedCam1 != null) fixedCam1.Priority = 5;
        if (fixedCam2 != null) fixedCam2.Priority = 5;

        currentActiveCamera = followVCam;
        SetBlendTime(defaultBlendTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 检查进入了哪个区域
            if (zone1Trigger != null && zone1Trigger.IsTouching(other) && fixedCam1 != null)
            {
                SwitchToCamera(fixedCam1, zone1BlendTime);
            }
            else if (zone2Trigger != null && zone2Trigger.IsTouching(other) && fixedCam2 != null)
            {
                SwitchToCamera(fixedCam2, zone2BlendTime);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 检查是否离开了所有区域
            bool inZone1 = zone1Trigger != null && zone1Trigger.IsTouching(other);
            bool inZone2 = zone2Trigger != null && zone2Trigger.IsTouching(other);

            if (!inZone1 && !inZone2 && followVCam != null)
            {
                SwitchToCamera(followVCam, defaultBlendTime);
            }
        }
    }

    private void SwitchToCamera(CinemachineVirtualCamera targetCamera, float blendTime)
    {
        if (targetCamera == null || targetCamera == currentActiveCamera) return;

        // 设置过渡时间
        SetBlendTime(blendTime);

        // 切换相机优先级
        if (currentActiveCamera != null) currentActiveCamera.Priority = 5;
        targetCamera.Priority = 15;
        currentActiveCamera = targetCamera;
    }

    private void SetBlendTime(float time)
    {
        var brain = CinemachineCore.Instance.FindPotentialTargetBrain(currentActiveCamera);
        if (brain != null)
        {
            brain.m_DefaultBlend.m_Time = time;
        }
    }

    // 可视化区域
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);

        if (zone1Trigger is BoxCollider2D box1)
        {
            Gizmos.DrawCube(zone1Trigger.transform.position + (Vector3)box1.offset, box1.size);
        }

        if (zone2Trigger is BoxCollider2D box2)
        {
            Gizmos.DrawCube(zone2Trigger.transform.position + (Vector3)box2.offset, box2.size);
        }
    }
}