using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ExitPortal — Kích hoạt chuyển sang scene tiếp theo khi Player bước vào.
/// Lưu máu hiện tại của Player vào PlayerSpawnData để scene mới có thể restore.
/// </summary>
public class ExitPortal : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Lưu máu hiện tại trước khi chuyển scene
            // → PlayerHealth.Start() ở scene mới sẽ đọc giá trị này
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                PlayerSpawnData.savedHealth = playerHealth.CurrentHealth;
            }

            SceneManager.LoadScene(nextSceneName);
        }
    }
}