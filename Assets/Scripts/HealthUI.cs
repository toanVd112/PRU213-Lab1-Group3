using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HealthUI — Hiển thị số mạng sống dạng icon trái tim + số đếm.
/// Lắng nghe event OnHealthChanged từ PlayerHealth để tự cập nhật.
///
/// Setup trong Unity:
///   1. Tạo Canvas → tạo UI Image (trái tim) + TMP_Text (số)
///   2. Attach script này vào GameObject trong Canvas
///   3. Kéo TMP_Text vào field heartCountText trong Inspector
/// </summary>
public class HealthUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Text hiển thị số mạng sống — kéo TMP_Text object vào đây")]
    public TMP_Text heartCountText;

    [Header("Display Settings")]
    [Tooltip("Icon trái tim hiển thị trước số (có thể dùng emoji hoặc ký tự)")]
    public string heartIcon = "❤";

    private void OnEnable()
    {
        // Đăng ký lắng nghe event khi component được bật
        PlayerHealth.OnHealthChanged += UpdateHealthUI;
    }

    private void OnDisable()
    {
        // Hủy đăng ký khi component bị tắt — tránh memory leak
        PlayerHealth.OnHealthChanged -= UpdateHealthUI;
    }

    /// <summary>
    /// Được gọi tự động mỗi khi máu player thay đổi.
    /// Cập nhật text hiển thị số mạng sống.
    /// </summary>
    /// <param name="currentHealth">Máu hiện tại</param>
    /// <param name="maxHealth">Máu tối đa</param>
    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (heartCountText == null) return;

        // Hiển thị: ❤ 2  (icon + số mạng còn lại)
        heartCountText.text = heartIcon + " " + currentHealth;
    }
}
