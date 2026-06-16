using UnityEngine;

/// <summary>
/// EnemyPatrol — AI kẻ thù tự động tuần tra qua lại giữa 2 điểm (Ping-Pong Patrol).
/// Gây sát thương cho Player khi va chạm, có cooldown để tránh trừ máu liên tục.
/// 
/// Setup trong Unity Editor:
///   1. Attach script này vào Enemy GameObject
///   2. Tạo 2 Empty GameObject làm PointA và PointB, kéo vào Inspector
///   3. Enemy cần có Collider2D (Trigger hoặc thường đều hoạt động)
/// </summary>
public class EnemyPatrol : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // INSPECTOR FIELDS
    // ─────────────────────────────────────────────

    [Header("Patrol Settings")]

    /// <summary>Điểm tuần tra bên trái — kéo Empty GameObject vào đây</summary>
    [Tooltip("Điểm tuần tra đầu tiên (thường là bên trái)")]
    public Transform pointA;

    /// <summary>Điểm tuần tra bên phải — kéo Empty GameObject vào đây</summary>
    [Tooltip("Điểm tuần tra thứ hai (thường là bên phải)")]
    public Transform pointB;

    /// <summary>
    /// Tốc độ di chuyển của kẻ thù (units/giây).
    /// Player chạy 5f → enemy 2f = chậm hơn 2.5 lần, player có cơ hội thoát.
    /// </summary>
    [Tooltip("Tốc độ di chuyển — nên thấp hơn moveSpeed của Player")]
    public float moveSpeed = 2f;

    [Header("Combat Settings")]

    /// <summary>Lượng máu trừ mỗi lần chạm — chỉnh trong Inspector tùy loại enemy</summary>
    [Tooltip("Sát thương gây ra cho Player mỗi lần va chạm")]
    public int damageAmount = 1;

    /// <summary>
    /// Thời gian chờ (giây) giữa 2 lần gây sát thương.
    /// Không có cooldown → OnTriggerStay2D gọi 60 lần/giây → mất toàn bộ máu tức thì.
    /// </summary>
    [Tooltip("Giây giữa 2 lần gây sát thương — ngăn trừ máu quá nhanh")]
    public float damageCooldown = 1f;

    // ─────────────────────────────────────────────
    // PRIVATE STATE — không hiện trong Inspector
    // ─────────────────────────────────────────────

    /// <summary>Mục tiêu đang hướng tới trong vòng tuần tra (pointA hoặc pointB)</summary>
    private Transform _currentTarget;

    /// <summary>SpriteRenderer để flip hướng nhìn của enemy khi đổi chiều</summary>
    private SpriteRenderer _sr;

    /// <summary>
    /// Lưu Time.time tại thời điểm gây damage lần cuối.
    /// So sánh (Time.time - _lastDamageTime) với damageCooldown để quyết định có gây damage không.
    /// </summary>
    private float _lastDamageTime = -999f; // -999f để lần đầu tiên không bị cooldown

    // ─────────────────────────────────────────────
    // UNITY LIFECYCLE
    // ─────────────────────────────────────────────

    /// <summary>
    /// Start() chạy 1 lần khi object được tạo ra.
    /// Dùng Start() thay vì Awake() vì pointA/pointB cần được gán xong trước.
    /// </summary>
    private void Start()
    {
        // Lấy SpriteRenderer để flip sprite theo hướng đi
        _sr = GetComponent<SpriteRenderer>();

        // Bắt đầu hành trình từ pointA
        // → Enemy sẽ đi từ vị trí hiện tại đến pointA trước
        _currentTarget = pointA;
    }

    /// <summary>
    /// Update() chạy mỗi frame — gọi logic tuần tra liên tục.
    /// </summary>
    private void Update()
    {
        Patrol();
    }

    // ─────────────────────────────────────────────
    // PATROL ALGORITHM (Ping-Pong)
    // ─────────────────────────────────────────────

    /// <summary>
    /// Thuật toán Ping-Pong Patrol:
    ///   1. Di chuyển về phía _currentTarget bằng Vector2.MoveTowards
    ///   2. Flip sprite theo hướng đang đi
    ///   3. Khi đến gần mục tiêu (< 0.1f) → đổi mục tiêu sang điểm kia
    /// </summary>
    private void Patrol()
    {
        // Guard clause: nếu chưa gán điểm tuần tra → thoát ngay, tránh NullReferenceException
        if (pointA == null || pointB == null) return;

        // BƯỚC 1: Tạo vị trí đích CHỈ THAY ĐỔI TRỤC X
        // Giữ nguyên Y và Z của enemy → tránh bị chìm xuống khi PointA/PointB lệch Y
        // FIX: thay vì dùng _currentTarget.position trực tiếp (có thể lệch Y),
        //      chỉ lấy X từ target, giữ Y hiện tại của enemy
        Vector2 targetPosition = new Vector2(
            _currentTarget.position.x,  // X từ điểm tuần tra
            transform.position.y        // Y giữ nguyên của enemy → không bị chìm!
        );

        // Di chuyển về phía targetPosition (chỉ theo X)
        // × Time.deltaTime: đảm bảo tốc độ nhất quán dù FPS thay đổi
        transform.position = Vector2.MoveTowards(
            transform.position,        // Vị trí hiện tại
            targetPosition,            // Vị trí đích (chỉ khác X)
            moveSpeed * Time.deltaTime // Khoảng di chuyển tối đa trong frame này
        );

        // BƯỚC 2: Flip sprite theo hướng đang đi
        // So sánh X của target với X hiện tại để xác định hướng
        if (_sr != null)
        {
            _sr.flipX = (_currentTarget.position.x < transform.position.x);
        }

        // BƯỚC 3: Kiểm tra đã đến điểm đích chưa (chỉ so sánh trục X)
        // Dùng Mathf.Abs(deltaX) thay vì Vector2.Distance để chỉ xét trục X
        float distanceX = Mathf.Abs(transform.position.x - _currentTarget.position.x);
        if (distanceX < 0.1f)
        {
            // Đổi mục tiêu: đang đi pointA → chuyển pointB, và ngược lại
            _currentTarget = (_currentTarget == pointA) ? pointB : pointA;
        }
    }

    // ─────────────────────────────────────────────
    // DAMAGE DETECTION
    // ─────────────────────────────────────────────

    /// <summary>
    /// Được gọi khi enemy va chạm với object khác (Collider không phải Trigger).
    /// Dùng khi enemy có BoxCollider2D với IsTrigger = false.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Truyền gameObject của object bị va chạm vào hàm xử lý chung
        DealDamageIfPlayer(collision.gameObject);
    }

    /// <summary>
    /// Được gọi KHI VỪAO vùng trigger (Collider với IsTrigger = true).
    /// Dùng khi enemy có BoxCollider2D với IsTrigger = true.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        DealDamageIfPlayer(other.gameObject);
    }

    /// <summary>
    /// Được gọi LIÊN TỤC mỗi frame khi player đứng trong vùng trigger.
    /// Nhờ có damageCooldown, sẽ không gây damage 60 lần/giây.
    /// </summary>
    private void OnTriggerStay2D(Collider2D other)
    {
        DealDamageIfPlayer(other.gameObject);
    }

    /// <summary>
    /// Logic xử lý damage chung — được gọi bởi cả 3 hàm collision ở trên.
    /// 
    /// Luồng kiểm tra (Early Return Pattern):
    ///   1. Không phải Player? → return (bỏ qua object khác: platform, wall...)
    ///   2. Đang trong cooldown? → return (chưa đến lượt gây damage)
    ///   3. Không tìm thấy PlayerHealth? → return (tránh NullReferenceException)
    ///   4. Tất cả OK → gây damage + reset cooldown timer
    /// </summary>
    /// <param name="target">GameObject vừa va chạm với enemy</param>
    private void DealDamageIfPlayer(GameObject target)
    {
        // BƯỚC 1: Tag check
        // CompareTag() không tạo string allocation mới → tốt hơn (target.tag == "Player") về hiệu năng
        // Player phải được gán tag "Player" trong Inspector
        if (!target.CompareTag("Player")) return;

        // BƯỚC 2: Cooldown check
        // Time.time: thời gian (giây) kể từ khi game bắt đầu
        // Nếu chưa đủ thời gian chờ kể từ lần damage cuối → bỏ qua
        if (Time.time - _lastDamageTime < damageCooldown) return;

        // BƯỚC 3: Tìm component PlayerHealth trên Player
        // GetComponent<>() trả về null nếu không tìm thấy → kiểm tra null trước khi dùng
        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        // BƯỚC 4: Gây sát thương và reset cooldown timer
        playerHealth.TakeDamage(damageAmount);
        _lastDamageTime = Time.time; // Lưu thời điểm này → cooldown bắt đầu tính
    }

    // ─────────────────────────────────────────────
    // EDITOR VISUALIZATION
    // ─────────────────────────────────────────────

    /// <summary>
    /// OnDrawGizmos() chỉ chạy trong Unity Editor, không ảnh hưởng game build.
    /// Vẽ đường tuần tra để designer thấy rõ phạm vi mà không cần Play.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (pointA == null || pointB == null) return;

        Gizmos.color = Color.red;

        // Vẽ hình cầu nhỏ tại mỗi điểm tuần tra
        Gizmos.DrawSphere(pointA.position, 0.15f);
        Gizmos.DrawSphere(pointB.position, 0.15f);

        // Vẽ đường thẳng nối 2 điểm — thể hiện đường di chuyển
        Gizmos.DrawLine(pointA.position, pointB.position);
    }
}
