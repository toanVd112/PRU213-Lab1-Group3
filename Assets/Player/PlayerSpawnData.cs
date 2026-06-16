using UnityEngine;

/// <summary>
/// PlayerSpawnData — Lớp static lưu dữ liệu player giữa các scene.
/// Vì là static class, dữ liệu tồn tại suốt vòng đời ứng dụng (không bị reset khi load scene).
/// </summary>
public static class PlayerSpawnData
{
    /// <summary>Vị trí spawn của player khi chuyển scene.</summary>
    public static Vector3 spawnPosition;

    /// <summary>
    /// Máu đã lưu khi rời scene trước.
    /// -1 = chưa có data (bắt đầu game mới) → PlayerHealth sẽ dùng maxHealth.
    /// > 0 = máu thực tế cần restore khi vào scene mới.
    /// </summary>
    public static int savedHealth = -1;

    /// <summary>Reset toàn bộ data — gọi khi bắt đầu game mới từ MainMenu.</summary>
    public static void Reset()
    {
        spawnPosition = Vector3.zero;
        savedHealth = -1;
    }
}