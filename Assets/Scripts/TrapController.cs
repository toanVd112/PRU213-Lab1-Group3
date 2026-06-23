using UnityEngine;

/// <summary>
/// TrapController — Gắn lên GameObject Trap1 trong Hierarchy.
///
/// Khi Player chạm vào Trap:
///   1. Gọi PlayerHealth.TakeDamage(1) → trừ 1 máu (i-frames tự xử lý bên PlayerHealth)
///   2. Đẩy Player lùi theo hướng ngược lại (knockback)
///
/// Yêu cầu setup trong Unity:
///   - Trap1 phải có Collider2D với "Is Trigger" = true  (hoặc dùng OnCollisionEnter2D)
///   - Player phải có tag "Player"
///   - Player phải có component PlayerHealth và Rigidbody2D
/// </summary>
public class TrapController : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // INSPECTOR FIELDS
    // ─────────────────────────────────────────────

    [Header("Knockback Settings")]

    /// <summary>
    /// Lực đẩy ngang khi player bị knockback.
    /// </summary>
    [Tooltip("Lực đẩy ngang khi Player chạm bẫy")]
    public float knockbackForceX = 8f;

    /// <summary>
    /// Lực đẩy dọc (nhảy lên) khi bị knockback.
    /// </summary>
    [Tooltip("Lực đẩy dọc (nảy lên) khi Player chạm bẫy")]
    public float knockbackForceY = 5f;

    /// <summary>
    /// Thời gian (giây) PlayerController bị khóa input sau knockback.
    /// Nếu quá ngắn → velocity bị ghi đè ngay. Thường đặt bằng 0.2–0.3s.
    /// </summary>
    [Tooltip("Thời gian (giây) Player mất kiểm soát sau knockback")]
    public float knockbackDuration = 0.25f;

    [Header("Damage Settings")]

    /// <summary>
    /// Lượng máu bị trừ mỗi lần chạm bẫy. Mặc định 1.
    /// </summary>
    [Tooltip("Lượng máu trừ mỗi lần chạm (thường là 1)")]
    public int damageAmount = 1;

    // ─────────────────────────────────────────────
    // TRIGGER — Collider2D "Is Trigger = true"
    // ─────────────────────────────────────────────

    /// <summary>
    /// OnTriggerEnter2D: gọi khi một Collider2D khác bước vào vùng trigger của Trap.
    ///
    /// Dùng Trigger (Is Trigger = true) để:
    ///   - Player không bị "đứng" trên bẫy (xuyên qua về mặt vật lý)
    ///   - Chỉ nhận event khi bước vào vùng nguy hiểm
    ///
    /// Nếu muốn Trap là vật thể rắn (player đứng được), đổi sang OnCollisionEnter2D.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null) return;

        // BƯỚC 1: Trừ máu
        health.TakeDamage(damageAmount);

        // BƯỚC 2: Knockback
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        PlayerController pc = other.GetComponent<PlayerController>();
        if (rb != null)
        {
            // Khóa input trước — phải gọi TRƯỚC khi set velocity
            // để Update() frame này không ghi đè ngay
            if (pc != null) pc.SetKnockbackState(knockbackDuration);

            ApplyKnockback(rb, other.transform.position);
        }

        Debug.Log($"[TrapController] {gameObject.name} gây {damageAmount} st cho Player!");
    }

    // ─────────────────────────────────────────────
    // PRIVATE HELPER
    // ─────────────────────────────────────────────

    /// <summary>
    /// Tính hướng knockback và áp lực lên Rigidbody2D của Player.
    ///
    /// Hướng ngang:
    ///   - Tính vector từ Trap → Player (playerPos - trapPos)
    ///   - Nếu Player ở BÊN PHẢI trap → đẩy sang phải (+X)
    ///   - Nếu Player ở BÊN TRÁI trap  → đẩy sang trái  (-X)
    ///
    /// Dùng linearVelocity thay vì AddForce để có kiểm soát tốt hơn:
    ///   - AddForce phụ thuộc mass của Rigidbody2D → không nhất quán
    ///   - linearVelocity gán thẳng → dễ điều chỉnh trong Inspector
    /// </summary>
    private void ApplyKnockback(Rigidbody2D rb, Vector2 playerPosition)
    {
        // Xác định hướng đẩy:
        // So sánh vị trí X của Player với Trap
        //   Player ở BÊN PHẢI trap (playerX > trapX) → đẩy sang phải (+1)
        //   Player ở BÊN TRÁI trap (playerX < trapX) → đẩy sang trái  (-1)
        //   player đứng đúng giữa (rất hiếm)  → nhìn vào SpriteRenderer.flipX để đoán hướng
        float directionX;
        float deltaX = playerPosition.x - transform.position.x;

        if (Mathf.Abs(deltaX) > 0.05f)
        {
            // Trường hợp bình thường: lấy dấu của deltaX
            directionX = Mathf.Sign(deltaX);
        }
        else
        {
            // Player đúng ở giữa (ví dụ nhảy rơi thẳng xuống trên bẫy)
            // Dùng hướng ngược với SpriteRenderer để đẩy ra phía sau player
            SpriteRenderer sr = rb.GetComponent<SpriteRenderer>();
            if (sr != null)
                directionX = sr.flipX ? 1f : -1f; // flipX=true → đang nhìn trái → đẩy sang phải
            else
                directionX = 1f;
        }

        // Ghi đè velocity: đẩy ngang + nẩy lên
        rb.linearVelocity = new Vector2(directionX * knockbackForceX, knockbackForceY);
    }
}
