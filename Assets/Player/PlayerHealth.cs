using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// PlayerHealth — Quản lý hệ thống máu của Player.
/// 
/// Tính năng:
///   - Nhận sát thương từ EnemyPatrol và Trap
///   - Invincibility Frames (i-frames): bất tử tạm thời sau mỗi đòn, sprite nhấp nháy
///   - Event OnHealthChanged: thông báo UI cập nhật mà không tạo coupling chặt
///   - Khi máu = 0: disable input, đợi 1 giây, load GameOver Scene
///   - Fall Death: rơi xuống dưới deathZoneY → chết ngay lập tức
/// 
/// Cần attach: PlayerController phải nằm trên cùng GameObject
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // INSPECTOR FIELDS
    // ─────────────────────────────────────────────

    [Header("Health Settings")]

    /// <summary>
    /// Số máu tối đa. Mặc định 3 = 3 trái tim.
    /// Số lẻ phù hợp với cách hiển thị icon tim trong UI.
    /// </summary>
    [Tooltip("Số máu tối đa của Player")]
    public int maxHealth = 3;

    /// <summary>Tên scene sẽ load khi Player chết — phải khớp tên trong Build Settings</summary>
    [Tooltip("Tên Scene Game Over (phải thêm vào Build Settings)")]
    public string gameOverSceneName = "GameOverScene";

    [Header("Invincibility Frames")]

    /// <summary>
    /// Thời gian bất tử sau mỗi đòn (giây).
    /// 1.5s: đủ để player phản ứng và thoát khỏi vùng nguy hiểm.
    /// Quá ngắn (0.3s) → frustrating. Quá dài (3s) → game quá dễ.
    /// </summary>
    [Tooltip("Giây bất tử sau khi bị trúng đòn")]
    public float invincibleDuration = 1.5f;

    /// <summary>
    /// Tốc độ nhấp nháy sprite khi bất tử (giây/lần).
    /// 0.1s → nhấp nháy 10 lần/giây → trông mượt mà, dễ nhận ra.
    /// </summary>
    [Tooltip("Tốc độ nhấp nháy — giây giữa mỗi lần bật/tắt sprite")]
    public float blinkRate = 0.1f;

    [Header("Fall Death Zone")]

    /// <summary>
    /// Ngưỡng Y tối thiểu — Player rơi xuống dưới giá trị này → chết ngay lập tức.
    /// Mặc định -10: đủ sâu để không trigger khi nhảy bình thường,
    /// nhưng đủ gần để xử lý nhanh khi rơi khỏi mặt đất.
    /// Chỉnh trong Inspector nếu layout bản đồ khác.
    /// </summary>
    [Tooltip("Rơi xuống dưới Y này → chết ngay (instant death, bỏ qua i-frames)")]
    public float deathZoneY = -10f;

    // ─────────────────────────────────────────────
    // PRIVATE STATE
    // ─────────────────────────────────────────────

    /// <summary>Máu hiện tại — thay đổi khi TakeDamage() hoặc Heal() được gọi</summary>
    private int _currentHealth;

    /// <summary>
    /// Flag kiểm soát i-frames.
    /// true → Player bất tử, mọi TakeDamage() đều bị bỏ qua.
    /// Được bật bởi InvincibilityCoroutine và tự tắt sau invincibleDuration giây.
    /// </summary>
    private bool _isInvincible = false;

    /// <summary>SpriteRenderer để thay đổi alpha khi nhấp nháy</summary>
    private SpriteRenderer _sr;

    /// <summary>
    /// Flag tránh gọi Die() nhiều lần cùng lúc
    /// (ví dụ: rơi + bị enemy cùng một frame).
    /// </summary>
    private bool _isDead = false;

    // ─────────────────────────────────────────────
    // EVENT (Observer Pattern)
    // ─────────────────────────────────────────────

    /// <summary>
    /// Event thông báo khi máu thay đổi.
    /// Signature: Action(int currentHealth, int maxHealth)
    /// 
    /// UI script đăng ký lắng nghe: PlayerHealth.OnHealthChanged += UpdateUI;
    /// PlayerHealth không cần biết UI nào đang lắng nghe → loose coupling.
    /// 
    /// static: cho phép đăng ký mà không cần tham chiếu đến instance cụ thể.
    /// </summary>
    public static System.Action<int, int> OnHealthChanged;

    // ─────────────────────────────────────────────
    // UNITY LIFECYCLE
    // ─────────────────────────────────────────────

    /// <summary>
    /// Start(): khởi tạo máu về maxHealth và gửi event để UI hiển thị đúng ngay từ đầu.
    /// </summary>
    private void Start()
    {
        _sr = GetComponent<SpriteRenderer>();

        // RESTORE máu từ scene trước nếu có
        // savedHealth == -1 → game mới → dùng maxHealth
        // savedHealth > 0  → đang chuyển cảnh giữa chừng → giữ nguyên máu
        if (PlayerSpawnData.savedHealth > 0)
        {
            _currentHealth = PlayerSpawnData.savedHealth;
        }
        else
        {
            _currentHealth = maxHealth;
        }

        // Thông báo UI hiển thị đúng từ đầu
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);
    }

    /// <summary>
    /// Update() chạy mỗi frame — kiểm tra Player có rơi khỏi mặt đất không.
    /// Dùng transform.position.y thay vì Rigidbody2D.position để đọc chính xác hơn.
    /// </summary>
    private void Update()
    {
        // Nếu đã chết rồi → không kiểm tra thêm
        if (_isDead) return;

        // Kiểm tra ngưỡng Y — rơi xuống dưới deathZoneY → thua NGAY LẬP TỨC
        if (transform.position.y < deathZoneY)
        {
            Debug.Log($"[PlayerHealth] Rơi xuống vực! Y = {transform.position.y:F2}");

            _isDead = true;

            // Xóa savedHealth → khi replay sẽ bắt đầu lại từ đầu
            PlayerSpawnData.savedHealth = -1;

            // Load GameOver NGAY LẬP TỨC — không delay
            SceneManager.LoadScene(gameOverSceneName);
        }
    }

    // ─────────────────────────────────────────────
    // PUBLIC API — được gọi từ EnemyPatrol, TrapController...
    // ─────────────────────────────────────────────

    /// <summary>
    /// Nhận sát thương từ nguồn bên ngoài (Enemy, Trap...).
    /// 
    /// Luồng xử lý:
    ///   1. Đang bất tử (i-frames)? → bỏ qua hoàn toàn
    ///   2. Trừ máu + clamp về 0 (không để máu âm)
    ///   3. Thông báo UI qua OnHealthChanged event
    ///   4. Máu = 0 → Die() | Còn máu → kích hoạt i-frames + nhấp nháy
    /// </summary>
    /// <param name="amount">Lượng máu bị trừ (thường là 1)</param>
    public void TakeDamage(int amount)
    {
        // BƯỚC 1: Bỏ qua nếu đang trong i-frames
        // _isInvincible được set true bởi InvincibilityCoroutine
        if (_isInvincible) return;

        // BƯỚC 2a: Trừ máu
        _currentHealth -= amount;

        // BƯỚC 2b: Clamp — đảm bảo máu không xuống dưới 0
        // Mathf.Max(a, b) trả về giá trị lớn hơn → Max(currentHealth, 0) = không bao giờ âm
        // Dùng <= 0 thay vì == 0 vì boss có thể gây nhiều damage hơn máu còn lại
        _currentHealth = Mathf.Max(_currentHealth, 0);

        // BƯỚC 3: Thông báo UI cập nhật icon tim / health bar
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);

        Debug.Log($"[PlayerHealth] Bị đánh! Máu còn: {_currentHealth}/{maxHealth}");

        // BƯỚC 4: Phân nhánh theo kết quả
        if (_currentHealth <= 0)
        {
            // Hết máu → xóa data đã lưu (bắt đầu lại từ đầu khi chơi lại)
            PlayerSpawnData.savedHealth = -1;
            Die();
        }
        else
        {
            // Còn máu → lưu máu hiện tại để giữ qua scene
            PlayerSpawnData.savedHealth = _currentHealth;

            // Kích hoạt i-frames (nhấp nháy + bất tử tạm thời)
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    /// <summary>
    /// Hồi máu cho Player — dùng cho item hồi máu nếu cần sau này.
    /// Clamp lên maxHealth để không vượt quá giới hạn.
    /// </summary>
    /// <param name="amount">Lượng máu hồi</param>
    public void Heal(int amount)
    {
        // Mathf.Min(a, b): lấy giá trị nhỏ hơn → không vượt quá maxHealth
        _currentHealth = Mathf.Min(_currentHealth + amount, maxHealth);

        // Thông báo UI cập nhật
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);

        Debug.Log($"[PlayerHealth] Hồi máu! Máu hiện tại: {_currentHealth}/{maxHealth}");
    }

    // ─────────────────────────────────────────────
    // PRIVATE LOGIC
    // ─────────────────────────────────────────────

    /// <summary>
    /// Xử lý khi Player chết (máu = 0).
    /// 
    /// Thứ tự:
    ///   1. Tắt script này → không nhận damage nữa
    ///   2. Tắt PlayerController → không nhận input bàn phím nữ
    ///      (KHÔNG tắt Rigidbody2D → để player rơi xuống tự nhiên, tạo cảm giác chết)
    ///   3. Chờ 1 giây (dramatism) → chuyển sang GameOver Scene
    /// </summary>
    private void Die()
    {
        // Guard: tránh gọi Die() nhiều lần cùng lúc
        if (_isDead) return;
        _isDead = true;

        Debug.Log("[PlayerHealth] Player đã chết!");

        // Tắt script này để không nhận thêm damage
        enabled = false;

        // Tắt input — Player không di chuyển được khi đang chết
        // Dùng GetComponent thay vì reference trực tiếp để tránh circular dependency
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null) pc.enabled = false;

        // Coroutine: đợi rồi load scene (không block thread chính)
        StartCoroutine(LoadGameOverScene());
    }

    /// <summary>
    /// Coroutine chờ 1 giây rồi chuyển sang Game Over Scene.
    /// Dùng Coroutine thay vì Invoke() vì dễ đọc và kiểm soát hơn.
    /// 
    /// 1 giây chờ: người chơi thấy được khoảnh khắc "chết",
    /// animation rơi/ngã có thể phát trong thời gian này.
    /// </summary>
    private IEnumerator LoadGameOverScene()
    {
        // WaitForSeconds: tạm dừng Coroutine, KHÔNG block thread chính
        // (game vẫn tiếp tục chạy, gravity vẫn kéo player rơi)
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(gameOverSceneName);
    }

    /// <summary>
    /// Coroutine tạo hiệu ứng Invincibility Frames (i-frames):
    ///   - Bật _isInvincible = true → chặn mọi TakeDamage()
    ///   - Nhấp nháy sprite bằng cách toggle alpha (không dùng SetActive — tránh tắt Collider)
    ///   - Sau invincibleDuration giây → tắt i-frames, khôi phục sprite
    /// 
    /// Tại sao Coroutine?
    ///   → Coroutine có thể "chờ" (yield) mà không block code khác.
    ///   → Nếu dùng Update(), phải tự quản lý timer và state phức tạp hơn.
    /// </summary>
    private IEnumerator InvincibilityCoroutine()
    {
        // BẮT ĐẦU: Kích hoạt i-frames
        _isInvincible = true;

        float elapsed = 0f; // Thời gian đã trôi qua trong i-frames

        // Vòng lặp nhấp nháy cho đến khi hết thời gian bất tử
        while (elapsed < invincibleDuration)
        {
            // NHẤP NHÁY: Toggle alpha giữa 0.2 (mờ) và 1.0 (rõ)
            // Đọc màu hiện tại → sửa alpha → gán lại (vì Color là struct, không thể sửa trực tiếp)
            if (_sr != null)
            {
                Color currentColor = _sr.color;

                // Nếu đang rõ → chuyển mờ, và ngược lại
                currentColor.a = (currentColor.a > 0.5f) ? 0.2f : 1.0f;

                _sr.color = currentColor;
            }

            // Chờ blinkRate giây rồi lặp lại — tạo hiệu ứng nhấp nháy
            yield return new WaitForSeconds(blinkRate);
            elapsed += blinkRate; // Cộng thời gian đã chờ vào elapsed
        }

        // KẾT THÚC: Đảm bảo sprite về trạng thái rõ ràng (không bị kẹt ở alpha 0.2)
        if (_sr != null)
        {
            Color finalColor = _sr.color;
            finalColor.a = 1.0f;
            _sr.color = finalColor;
        }

        // Tắt i-frames — Player có thể nhận damage bình thường trở lại
        _isInvincible = false;
    }

    // ─────────────────────────────────────────────
    // READ-ONLY PROPERTIES (cho UI, GameManager truy cập)
    // ─────────────────────────────────────────────

    /// <summary>Máu hiện tại — readonly từ bên ngoài, chỉ thay đổi qua TakeDamage/Heal</summary>
    public int CurrentHealth => _currentHealth;

    /// <summary>Máu tối đa — để UI tính tỉ lệ hiển thị health bar</summary>
    public int MaxHealth => maxHealth;
}
