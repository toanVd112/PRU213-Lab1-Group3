# LAB 1 (Odd) — 2D Unity Game: Treasure Hunter

## Objective

Develop a 2D adventure game named **"Treasure Hunter"** to practice Unity Interface, Scene Management, Rigidbody2D Physics, Collision Detection, UI Systems, and C# scripting in Unity.

---

## Game Concept

The player controls a treasure hunter exploring ancient ruins. The goal is to collect treasure chests, avoid enemies and traps, and escape through the exit portal before time runs out.

---

## Game Elements

### Treasure Hunter (Player Object)

A 2D player character controlled by keyboard input.

- Move left and right.
- Jump over obstacles.
- Collect treasure chests.

### Treasure Chests

Collectible objects hidden throughout the level.

- Increase score when collected.
- Play sound or animation effects.

### Enemies

Moving enemies guarding the ruins.

- Patrol between platforms.
- Cause the player to lose health on collision.

### Traps

Hazardous objects such as spikes and falling rocks.

- Trigger Game Over if the player touches them.

### Exit Portal

The level completion object.

- Loads the Victory Scene when all treasures are collected.

---

## Game Flow

### 1. Main Menu Scene

- Start Game Button.
- Instructions Button.
- Exit Button.

### 2. Gameplay Scene

- Explore the ancient ruins.
- Collect treasure chests.
- Avoid enemies and traps.
- Reach the exit portal before the timer ends.

### 3. Victory Scene

- Display total score and completion message.
- Restart Game or Return to Main Menu.

### 4. Game Over Scene

- Display Game Over message.
- Retry or Quit options.

---

## Lab Assignment

> Complete all the following tasks:

- [ ] Create all required scenes (Main Menu, Gameplay, Victory, Game Over).
- [ ] Implement player movement using Rigidbody2D and keyboard input.
- [ ] Create enemy patrol behavior using C# scripts.
- [ ] Implement treasure collection and scoring system.
- [ ] Create trap collision handling using triggers and colliders.
- [ ] Add UI elements: score, timer, and health display.
- [ ] Implement scene transitions and game state management.

---

## Technical Requirements

- Use **Rigidbody2D** and **Collider2D** components.
- Use `OnTriggerEnter2D` or `OnCollisionEnter2D` methods.
- Implement at least one **animation**.
- Use at least one **audio effect**.
- Use **C# scripts** for all gameplay logic.

---

## Grading Criteria

### Functionality

- Player movement.
- Enemy behavior.
- Collision handling.
- Scoring and timer system.
- Scene transitions.

### Creativity

- Visual design.
- UI layout.
- Sound and animation effects.

### Documentation

- Well-structured code.
- Proper comments in scripts.
- Clear explanation of game mechanics.

---

## Submission Requirements

- [ ] Submit the complete Unity project folder.
- [ ] Include all scripts and game assets.
- [ ] Provide screenshots of all scenes.
- [ ] Export executable build for bonus points.