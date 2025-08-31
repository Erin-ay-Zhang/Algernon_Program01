using UnityEngine;
using UnityEngine.SceneManagement;  // ���볡�����������ռ�

public class NextLevelTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName; // ��Inspector��ָ����һ������������

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ȷ����������Player
        if (other.CompareTag("Player"))
        {
            // ������һ������
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
