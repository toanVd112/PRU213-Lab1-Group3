using UnityEngine;

/// <summary>
/// PlayerController: Xử lý input di chuyển và nhảy của player.
/// Tích hợp với PlayerHealth để dừng input khi player chết.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public int maxJumps = 2;

    // ── Internal ──
    private int _jumpCount = 0;
    private bool _isGrounded;
    private Rigidbody2D _rb;
    private Animator _anim;
    private SpriteRenderer _sr;

    // ── Knockback State ──
    // Khi bị đẩy lùi, tạm thời tắt input để velocity không bị ghi đè
    private bool _isKnockedBack = false;
    private float _knockbackTimer = 0f;

    void Start()
    {
        _rb   = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _sr   = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // ── Xử lý timer knockback ──
        // Trong thời gian bị đẩy lùi: đếm ngược timer, KHÔNG nhận input di chuyển
        // để velocity do TrapController set không bị ghi đè ngay frame sau
        if (_isKnockedBack)
        {
            _knockbackTimer -= Time.deltaTime;
            if (_knockbackTimer <= 0f)
                _isKnockedBack = false;

            // Vẫn cập nhật animation và ground check, nhưng KHÔNG ghi đè velocity
            _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            _anim.SetFloat("Speed", 0f);
            return; // ← Bỏ qua phần xử lý input bên dưới
        }

        float move = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            move = -1f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            move = 1f;

        _rb.linearVelocity = new Vector2(move * moveSpeed, _rb.linearVelocity.y);

        // Flip sprite theo hướng di chuyển
        if (move != 0)
            _sr.flipX = move < 0;

        _anim.SetFloat("Speed", Mathf.Abs(move));

        // Kiểm tra đang đứng trên mặt đất
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (_isGrounded)
            _jumpCount = 0;

        // Nhảy (hỗ trợ double-jump)
        if (Input.GetKeyDown(KeyCode.Space) && _jumpCount < maxJumps)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
            _jumpCount++;
        }
    }

    /// <summary>
    /// Được gọi từ TrapController khi Player bị knockback.
    /// Khóa input trong <duration> giây để velocity knockback không bị ghi đè.
    /// </summary>
    public void SetKnockbackState(float duration)
    {
        _isKnockedBack = true;
        _knockbackTimer = duration;
    }
}