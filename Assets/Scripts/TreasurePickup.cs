using UnityEngine;

/// <summary>
/// TreasurePickup — Xử lý logic khi Player nhặt được Treasure (Kho báu).
/// 
/// Tính năng:
///   - Phát hiện va chạm với Player (dùng Trigger)
///   - Gọi hàm Heal(1) của PlayerHealth để cộng 1 máu
///   - (Tùy chọn) Cộng điểm hoặc phát âm thanh
///   - Biến mất (Destroy) sau khi nhặt
/// 
/// Setup:
///   1. Gắn script này vào GameObject Treasure (hòm kho báu, đồng xu, v.v.)
///   2. Đảm bảo Treasure có Collider2D và được tick chọn "Is Trigger" = true
/// </summary>
public class TreasurePickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [Tooltip("Số máu sẽ hồi khi nhặt được item này")]
    public int healAmount = 1;

    /// <summary>
    /// Được gọi tự động khi một GameObject khác đi vào vùng Trigger của Treasure.
    /// </summary>
    /// <param name="other">Collider của object vừa đi vào</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Kiểm tra xem object chạm vào có phải là Player không
        if (other.CompareTag("Player"))
        {
            // 2. Tìm component PlayerHealth trên Player
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            // 3. Nếu tìm thấy PlayerHealth, tiến hành hồi máu
            if (playerHealth != null)
            {
                // Chỉ hồi máu nếu máu chưa đầy 
                if (playerHealth.CurrentHealth < playerHealth.MaxHealth)
                {
                    playerHealth.Heal(healAmount);
                    
                    Debug.Log($"[TreasurePickup] Player nhặt được Treasure! Hồi {healAmount} máu.");

                    // 4. Hủy (xóa) object Treasure khỏi Scene sau khi đã nhặt
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("[TreasurePickup] Player nhặt Treasure nhưng máu đã đầy!");
                    Destroy(gameObject);
                }
            }
        }
    }
}
