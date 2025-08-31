using UnityEngine;
using UnityEngine.SceneManagement;  // 引入场景管理命名空间

public class NextLevelTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName; // 在Inspector里指定下一个场景的名字

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 确认碰到的是Player
        if (other.CompareTag("Player"))
        {
            // 加载下一个场景
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
