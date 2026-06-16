# 🏺 Treasure Hunter — 2D Unity Game

> **Môn học:** PRU213 — Unity Game Development  
> **Trường:** Đại học FPT  
> **Loại bài:** LAB 1 (Odd Week)  
> **Nền tảng:** Unity 2D (Universal Render Pipeline)

---

## 📖 Bối Cảnh Dự Án

**Treasure Hunter** là một game phiêu lưu 2D được xây dựng bằng Unity, trong đó người chơi điều khiển một thợ săn kho báu khám phá những tàn tích cổ đại. Mục tiêu là thu thập tất cả các rương báu, tránh kẻ thù và bẫy, rồi thoát qua cổng trước khi hết thời gian.

Dự án này được thực hiện nhằm thực hành các kỹ năng lập trình game cốt lõi trong Unity:

- Unity Interface & Scene Management
- Rigidbody2D Physics & Collision Detection
- UI Systems (Score, Timer, Health)
- C# Scripting trong môi trường Unity

---

## 🎮 Cơ Chế Gameplay

| Yếu Tố | Mô Tả |
|--------|--------|
| 🧍 **Player** | Di chuyển trái/phải (A/D hoặc ←/→), nhảy (Space), hỗ trợ double-jump |
| 💎 **Rương Báu** | Thu thập để tăng điểm, có hiệu ứng âm thanh/animation |
| 👾 **Kẻ Thù** | Tuần tra trên các platform, gây sát thương khi va chạm |
| ⚠️ **Bẫy** | Gai nhọn, đá rơi — chạm vào là Game Over ngay lập tức |
| 🌀 **Cổng Thoát** | Kích hoạt khi thu thập đủ báu vật, chuyển sang màn chiến thắng |

---

## 🗺️ Luồng Game (Game Flow)

```
[Main Menu]
     │
     ├─ Start Game ──► [Gameplay Scene]
     │                       │
     │              Thu thập đủ báu vật
     │                       │
     │                  [Win Scene] ──► Restart / Menu
     │
     │              Hết máu / Chạm bẫy
     │                       │
     │               [Game Over Scene] ──► Retry / Quit
     │
     └─ Quit ──► Thoát game
```

---

## 📁 Cấu Trúc Dự Án

```
lab-1-2026-06-10_03-44-27-main/
│
├── Assets/
│   ├── Scenes/
│   │   ├── MainMenu.unity          # Scene menu chính
│   │   ├── game.unity              # Scene gameplay chính
│   │   ├── Game_Night.unity        # Phiên bản gameplay (chủ đề đêm)
│   │   ├── WinScene.unity          # Scene chiến thắng
│   │   └── GameOverScene.unity     # Scene thua cuộc
│   │
│   ├── Scripts/
│   │   ├── SceneController.cs      # Điều hướng giữa các scene
│   │   ├── BackgroundMusic.cs      # Singleton quản lý nhạc nền
│   │   └── ResultScoreDisplay.cs   # Hiển thị điểm trên màn kết quả
│   │
│   ├── Player/
│   │   ├── PlayerController.cs     # Di chuyển, nhảy, animation player
│   │   ├── ExitPortal.cs           # Trigger cổng thoát level
│   │   ├── CameraFollow.cs         # Camera bám theo player
│   │   ├── PlayerSpawn.cs          # Quản lý spawn điểm player
│   │   ├── PlayerSpawnData.cs      # Dữ liệu vị trí spawn
│   │   ├── Player.controller       # Animator Controller
│   │   ├── PlayerIdle.anim         # Animation đứng yên
│   │   ├── PlayerRun.anim          # Animation chạy
│   │   └── PlayerJump.anim         # Animation nhảy
│   │
│   ├── Enemy/                      # ⚠️ Chưa có script (cần triển khai)
│   ├── Audio/                      # Assets âm thanh
│   └── TextMesh Pro/               # Package hỗ trợ UI text
│
├── Introduction/
│   └── LAB_1(Odd)_Treasure_Hunter_Unity.md   # Đề bài Lab
│
└── README.md                       # File này
```

---

## 📜 Scripts Hiện Có

### `PlayerController.cs`
- Xử lý input bàn phím (A/D, ←/→, Space)
- Di chuyển bằng `Rigidbody2D.linearVelocity`
- Hỗ trợ **double-jump** với biến `jumpCount`
- Ground detection bằng `Physics2D.OverlapCircle`
- Flip sprite & điều khiển Animator theo hướng di chuyển

### `SceneController.cs`
- `PlayGame()` — chuyển sang scene gameplay (`game`)
- `ShowHuongDan()` / `CloseHuongDan()` — mở/đóng panel hướng dẫn
- `BackToMenu()` — về Main Menu
- `QuitGame()` — thoát ứng dụng

### `BackgroundMusic.cs`
- Singleton pattern với `DontDestroyOnLoad`
- Đảm bảo nhạc nền không bị ngắt khi chuyển scene

### `ResultScoreDisplay.cs`
- Đọc điểm từ `PlayerPrefs.GetInt("FinalScore")`
- Hiển thị lên `TMP_Text` ở màn hình kết quả

### `ExitPortal.cs`
- Dùng `OnTriggerEnter2D` để phát hiện player
- Gọi `SceneManager.LoadScene(nextSceneName)` khi player chạm vào

### `CameraFollow.cs`
- Camera theo dõi vị trí player theo thời gian thực

---

## ✅ Checklist Bài Lab

- [x] Tạo đủ các scene (Main Menu, Gameplay, Win, Game Over)
- [x] Lập trình di chuyển player bằng Rigidbody2D
- [x] Animation player (Idle, Run, Jump)
- [x] Nhạc nền không bị ngắt khi đổi scene
- [x] Hiển thị điểm trên màn kết quả
- [x] Chuyển scene và cổng thoát
- [ ] Script AI kẻ thù tuần tra (Enemy Patrol)
- [ ] Hệ thống thu thập rương báu + tính điểm
- [ ] Xử lý va chạm bẫy (Trap Collision)
- [ ] Hệ thống máu (Health System)
- [ ] Đồng hồ đếm ngược (Timer)
- [ ] UI hiển thị Score / Timer / Health trong gameplay

---

## ⚙️ Yêu Cầu Kỹ Thuật

| Yêu Cầu | Trạng Thái |
|---------|-----------|
| Rigidbody2D & Collider2D | ✅ Đã dùng |
| `OnTriggerEnter2D` / `OnCollisionEnter2D` | ✅ Có trong ExitPortal |
| Ít nhất 1 Animation | ✅ Có 3 (Idle, Run, Jump) |
| Ít nhất 1 Audio Effect | ✅ Có thư mục Audio |
| Toàn bộ logic bằng C# | ✅ |

---

## 🐛 Lỗi Đã Biết

| # | Vị trí | Mô tả | Mức độ |
|---|--------|--------|--------|
| 1 | `SceneController.cs` dòng 27 | `ReplayGame()` load scene `"GameScene"` nhưng tên thực là `"game"` | 🔴 Lỗi |
| 2 | `PlayerController.cs` dòng 52 | `Debug.Log("isGrounded")` chưa xóa trước khi nộp | 🟡 Cần dọn |
| 3 | `ExitPortal.cs` | Chưa kiểm tra điều kiện thu thập đủ báu vật trước khi thoát | 🟡 Logic thiếu |

---

## 🚀 Hướng Dẫn Mở Dự Án

1. Mở **Unity Hub**
2. Chọn **Open > Add project from disk**
3. Trỏ đến thư mục `lab-1-2026-06-10_03-44-27-main/`
4. Chọn đúng **Unity version** (khuyến nghị: 2022.3 LTS trở lên với URP)
5. Mở scene `MainMenu.unity` và nhấn **Play**

---

## 📋 Yêu Cầu Nộp Bài

- [ ] Nộp toàn bộ thư mục Unity project
- [ ] Bao gồm tất cả scripts và game assets
- [ ] Cung cấp screenshots của tất cả các scene
- [ ] *(Bonus)* Export file build chạy được (.exe)

---

*Dự án được thực hiện bởi sinh viên Đại học FPT — Môn PRU213, Kỳ 7.*
