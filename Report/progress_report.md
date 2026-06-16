# 📊 Báo Cáo Tiến Độ — Lab 1 PRU213
## Game: Treasure Hunter (Unity 6 — 6000.4.7f1)

---

## 1. Tổng Quan Dự Án

| Thông Tin | Chi Tiết |
|-----------|---------|
| **Môn học** | PRU213 — Lập Trình Game Unity |
| **Engine** | Unity 6 (6000.4.7f1) |
| **Thể loại** | 2D Platformer |
| **Tên game** | Treasure Hunter |
| **Ngôn ngữ** | C# |

**Mục tiêu Lab 1:** Xây dựng hệ thống AI kẻ thù tuần tra và hệ thống máu của Player.

---

## 2. Các Tính Năng Đã Implement

### ✅ 2.1 — Hệ Thống AI Kẻ Thù Tuần Tra (EnemyPatrol)

**File:** `Assets/Enemy/EnemyPatrol.cs`

**Mô tả:**
Kẻ thù tự động di chuyển qua lại giữa 2 điểm tuần tra (PointA ↔ PointB) theo thuật toán **Ping-Pong Patrol**. Khi Player bước vào vùng của enemy, sẽ bị trừ máu.

**Thuật toán chính:**
```
1. Di chuyển về phía _currentTarget bằng Vector2.MoveTowards
2. Chỉ di chuyển theo trục X (giữ Y cố định) → không bị chìm xuống
3. Khi đến gần mục tiêu (< 0.1f đơn vị) → đổi target
4. Flip sprite tự động theo hướng đang đi
```

**Cơ chế gây sát thương:**
- Dùng `OnTriggerEnter2D` + `OnTriggerStay2D` để phát hiện va chạm
- **Damage Cooldown:** 1 giây/lần → tránh trừ máu 60 lần/giây
- Kiểm tra tag `"Player"` trước khi gây damage → bỏ qua các object khác

**Inspector Parameters:**

| Tham Số | Mặc Định | Mô Tả |
|---------|---------|-------|
| Move Speed | 2 | Tốc độ di chuyển |
| Damage Amount | 1 | Sát thương mỗi lần chạm |
| Damage Cooldown | 1s | Thời gian chờ giữa 2 lần damage |

---

### ✅ 2.2 — Hệ Thống Máu Player (PlayerHealth)

**File:** `Assets/Player/PlayerHealth.cs`

**Mô tả:**
Quản lý toàn bộ vòng đời máu của Player: nhận sát thương, hồi máu, và xử lý khi chết.

**Thuật toán chính:**
```
TakeDamage(amount):
  1. Nếu đang bất tử (i-frames) → bỏ qua
  2. Trừ máu + clamp về 0 (không để âm)
  3. Thông báo UI qua Event OnHealthChanged
  4. Máu = 0 → Die() | Còn máu → InvincibilityCoroutine()
```

**Tính năng Invincibility Frames (I-frames):**
- Sau khi bị đánh → Player **bất tử 1.5 giây**
- Sprite **nhấp nháy** (alpha toggle 0.2 ↔ 1.0) → trực quan
- Dùng Coroutine → không block thread chính

**Cơ chế thông báo UI:**
```csharp
// Event Pattern (Observer) — UI không cần biết PlayerHealth
public static Action<int, int> OnHealthChanged;
OnHealthChanged?.Invoke(currentHealth, maxHealth);
```

**Inspector Parameters:**

| Tham Số | Mặc Định | Mô Tả |
|---------|---------|-------|
| Max Health | 3 | Số máu tối đa |
| Game Over Scene | GameOverScene | Scene load khi chết |
| Invincible Duration | 1.5s | Thời gian bất tử |
| Blink Rate | 0.1s | Tốc độ nhấp nháy |

---

### ✅ 2.3 — Giao Diện Máu (HealthUI)

**File:** `Assets/Scripts/HealthUI.cs`

**Mô tả:**
Hiển thị số mạng sống dưới dạng icon trái tim + số đếm ở **góc trên trái màn hình**.

**Cách hoạt động:**
- Lắng nghe event `PlayerHealth.OnHealthChanged`
- Tự động cập nhật text khi máu thay đổi
- Hiển thị: `♥ 3` → `♥ 2` → `♥ 1` → `♥ 0`

**Thiết kế (Loose Coupling):**
```
PlayerHealth ──[Event]──► HealthUI
(không cần biết UI là gì)   (không cần biết Player ở đâu)
```

---

### ✅ 2.4 — Cập Nhật PlayerController

**File:** `Assets/Player/PlayerController.cs`

- Tích hợp với `PlayerHealth`: **tắt input khi Player chết**
- Refactor code: đổi naming convention, thêm XML doc comments

---

### ✅ 2.5 — Enemy Cho Scene Ban Đêm (Game_Night)

**File:** `Assets/Scenes/Game_Night.unity`

**Mô tả:**
Thêm 2 enemy vào scene `Game_Night` (scene thứ 2 — trời tối) với cùng cơ chế như scene ban ngày.

**Chi tiết Enemy:**

| Enemy | Vị Trí | Patrol Range | Speed |
|-------|--------|-------------|-------|
| Enemy1 | x=18, y=-3.06 | x=10 ↔ x=28 | 2.0 |
| Enemy2 | x=60, y=-3.06 | x=50 ↔ x=72 | 2.5 |

**Vấn đề phát sinh & Fix:**
- **Root cause:** `Player.prefab` thiếu `PlayerHealth` component → `GetComponent<PlayerHealth>()` luôn trả về `null` → enemy không thể gây damage
- **Fix:** Thêm `PlayerHealth` component trực tiếp vào `Player.prefab` (áp dụng cho toàn bộ scene dùng prefab)
- **Fix thêm:** Thêm Canvas + HealthUI vào `Game_Night.unity` để UI hiển thị máu hoạt động đúng

---

### ✅ 2.6 — Giữ Máu Xuyên Suốt Các Scene (Health Persistence)

**Files:** `PlayerSpawnData.cs`, `PlayerHealth.cs`, `ExitPortal.cs`, `SceneController.cs`

**Vấn đề:**
Khi chuyển từ scene ban ngày (`game`) sang scene ban đêm (`Game_Night`), máu Player bị reset về 3 dù đã mất máu ở scene trước. Vì đây là cùng 1 màn chơi liên tục, cần giữ nguyên máu.

**Giải pháp — Static Persistence Pattern:**
Tận dụng `PlayerSpawnData` (đã là `static class`) để lưu thêm `savedHealth`:

```
[MainMenu] → PlayGame()
    └── PlayerSpawnData.Reset()     ← savedHealth = -1 (game mới)

[game scene] → PlayerHealth.Start()
    └── savedHealth == -1           → dùng maxHealth (3) ✅

[Player bị đánh] → TakeDamage()
    └── savedHealth = currentHealth ← cập nhật liên tục

[ExitPortal] → chạm Portal
    └── savedHealth = currentHealth ← lưu lần cuối trước khi load

[Game_Night] → PlayerHealth.Start()
    └── savedHealth > 0             → restore máu từ scene trước ✅

[Hết máu] → Die()
    └── savedHealth = -1            ← reset để Replay bắt đầu lại
```

**Inspector Parameters mới trong `PlayerSpawnData`:**

| Field | Giá Trị | Ý nghĩa |
|-------|---------|---------|
| `savedHealth` | -1 | Chưa có data (game mới) |
| `savedHealth` | > 0 | Máu cần restore từ scene trước |

---

## 3. Kiến Trúc Kỹ Thuật

```
┌──────────────────────────────────────────────────────┐
│                GAME SCENE (game)                     │
│                                                      │
│  [Enemy] ──collision──► [PlayerHealth]               │
│    │                        │                        │
│  EnemyPatrol             TakeDamage()                │
│  (Ping-Pong)             I-frames                    │
│                          savedHealth ◄── lưu máu     │
│                          Die()                       │
│                             │                        │
│                         [Event]                      │
│                             │                        │
│                        [HealthUI] ♥ 3 → ♥ 0          │
│                                                      │
│  [ExitPortal] ──trigger──► lưu savedHealth           │
│                             └──► LoadScene(Game_Night)│
└──────────────────────────────────────────────────────┘
          ↓ PlayerSpawnData.savedHealth ↓
┌──────────────────────────────────────────────────────┐
│              GAME SCENE (Game_Night)                 │
│                                                      │
│  [PlayerHealth.Start()] ──► restore savedHealth      │
│  [Enemy1] ──collision──► [PlayerHealth]              │
│  [Enemy2] ──collision──► [PlayerHealth]              │
│                        [HealthUI] ♥ (tiếp tục)       │
└──────────────────────────────────────────────────────┘
```

---

## 4. Các Vấn Đề Đã Giải Quyết

| Vấn Đề | Giải Pháp |
|--------|----------|
| Enemy bị chìm xuống khi di chuyển | Chỉ MoveTowards theo trục X, giữ Y cố định |
| Enemy chạy mãi không dừng | PointA/B là object độc lập, không phải con của Enemy |
| Damage quá nhanh (60 lần/giây) | Cooldown timer dùng `Time.time` |
| Player không biết đang bất tử | I-frames + sprite nhấp nháy trực quan |
| UI bị coupling chặt | Dùng `static Action` event pattern |
| Sprite sheet không tách được | Dùng Sprite Editor → Automatic Slice |
| Game_Night không có enemy | Thêm 2 enemy thủ công vào file `.unity` (YAML) |
| Enemy không gây damage ở Game_Night | `Player.prefab` thiếu `PlayerHealth` → thêm component vào prefab |
| Game_Night không có UI máu | Thêm Canvas + HeartCountText + HealthUI vào scene |
| Máu reset khi chuyển scene | `PlayerSpawnData.savedHealth` lưu/đọc máu xuyên scene |

---

## 5. Kết Quả Demo

| Test Case | Kết Quả |
|-----------|---------|
| Enemy tuần tra A↔B liên tục | ✅ Pass |
| Enemy flip sprite đúng hướng | ✅ Pass |
| Player mất 1 máu khi chạm enemy | ✅ Pass |
| Player nhấp nháy sau khi bị đánh | ✅ Pass |
| UI cập nhật ♥ 3 → ♥ 2 → ♥ 1 | ✅ Pass |
| Player chết → GameOver Scene | ✅ Pass |
| Nhiều enemy hoạt động cùng lúc | ✅ Pass |
| Enemy xuất hiện ở Game_Night | ✅ Pass |
| Enemy gây damage ở Game_Night | ✅ Pass |
| UI máu hiển thị đúng ở Game_Night | ✅ Pass |
| Máu giữ nguyên khi chuyển scene | ✅ Pass |
| Game mới → máu reset về 3 | ✅ Pass |

---

## 6. Files Đã Tạo / Chỉnh Sửa

| File | Hành Động | Mô Tả |
|------|-----------|-------|
| `Assets/Enemy/EnemyPatrol.cs` | ✅ Tạo mới | AI tuần tra + gây damage |
| `Assets/Player/PlayerHealth.cs` | 🔄 Cập nhật | Quản lý máu + i-frames + **persist qua scene** |
| `Assets/Scripts/HealthUI.cs` | ✅ Tạo mới | UI hiển thị trái tim |
| `Assets/Player/PlayerController.cs` | 🔄 Cập nhật | Tích hợp với PlayerHealth |
| `Assets/Enemy/Enemy.prefab` | ✅ Tạo mới | Prefab tái sử dụng cho enemy |
| `Assets/Scenes/Player.prefab` | 🔄 Cập nhật | Thêm `PlayerHealth` component |
| `Assets/Scenes/Game_Night.unity` | 🔄 Cập nhật | Thêm Enemy1, Enemy2, Canvas, HealthUI |
| `Assets/Player/PlayerSpawnData.cs` | 🔄 Cập nhật | Thêm `savedHealth` + `Reset()` |
| `Assets/Player/ExitPortal.cs` | 🔄 Cập nhật | Lưu máu trước khi chuyển scene |
| `Assets/Scripts/SceneController.cs` | 🔄 Cập nhật | Reset data khi bắt đầu game mới |
| `README.md` | ✅ Tạo mới | Tài liệu dự án |
