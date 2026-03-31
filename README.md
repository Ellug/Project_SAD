# TanKsante
**보스의 패턴을 피하며 전투하는 3D 탄막 슈팅 게임**

> 패턴 중심의 액션 RPG를 탑뷰 탄막 슈터로 재해석.
> 각 스테이지·페이즈마다 보스의 패턴을 익히고 파훼하며 실력 성장의 재미를 추구합니다.

[![YouTube](https://img.shields.io/badge/YouTube-Demo-red?logo=youtube)](https://youtu.be/Es8QreV_6Ow)

---

<details>
<summary><strong>👑 팀장 (Ellug) 작업물</strong></summary>

## 팀장 담당 구현 목록

### 전체 아키텍처 설계 & GitHub 형상 관리 총괄

---

### 1. OOD : Stat Node Tree (특전 시스템)

Player/Weapon 능력치를 계층 구조로 관리하는 핵심 설계.

```
PerksTree
  └── Stage 0 ~ N
        ├── Left  Node  (StatMod[] + TriggeredBuff[])
        └── Right Node
```

| 계층 | 역할 |
|------|------|
| `Stat` | 적용할 최소 스탯 단위 (`StatId` + 수치 + 연산 방식) |
| `Node` | 특전 하나 단위. 버프·디버프로도 재사용 가능한 모듈 |
| `Tree` | 트리 구조 컨테이너. UI 바인딩 및 최종 스탯 합산 진입점 |

- **[StatId.cs](Assets/Scripts/Perks/StatId.cs)** — 플레이어/무기/무기모드 전체 능력치 열거형
- **[PerksNode.cs](Assets/Scripts/Perks/PerksNode.cs)** — 노드 단위 데이터 (StatMod[], TriggeredBuff[])
- **[PerksTree.cs](Assets/Scripts/Perks/PerksTree.cs)** — 2×N 트리 컨테이너, OnChanged 이벤트
- **[PerksCalculator.cs](Assets/Scripts/Perks/PerksCalculator.cs)** — Override → Add → Mul 순 누적 계산 엔진
- **[PlayerRuntimeStats.cs](Assets/Scripts/Perks/PlayerRuntimeStats.cs)** / **[WeaponRuntimeStats.cs](Assets/Scripts/Perks/WeaponRuntimeStats.cs)** — 런타임 스탯 래퍼
- **[TriggeredBuff.cs](Assets/Scripts/Perks/TriggeredBuff.cs)** — OnDodgeUsed / OnSpecialUsed 트리거 버프
- **[PlayerStatsContext.cs](Assets/Scripts/Player/PlayerStatsContext.cs)** — Rebuild() 로 특전 → 최종 스탯 반영

**설계 의도**: 버프·디버프·무기 진화 등 이후 시스템 추가 시 Node 재사용만으로 빠른 구현 가능.

---

### 2. Generic Object Pooling

서로 다른 타입의 오브젝트를 단일 매니저에서 관리하는 범용 풀링 시스템.

```
PoolManager (Generic Dictionary<Prefab, Queue<T>>)
  ├── Prewarm<T>(prefab, count)
  ├── Spawn<T>(prefab, pos, rot)
  └── Despawn<T>(instance)
```

- **[PoolManager.cs](Assets/Scripts/Bullets/ObjectPool/PoolManager.cs)** — 제네릭 Dictionary+Queue 기반 싱글톤
- **[IPoolable.cs](Assets/Scripts/Bullets/ObjectPool/IPoolable.cs)** — `OnSpawned()` / `OnDespawned()` 인터페이스
- **[PoolMember.cs](Assets/Scripts/Bullets/ObjectPool/PoolMember.cs)** — 오브젝트가 자신의 매니저·프리팹 키를 기억
- **[AutoDespawnParticle.cs](Assets/Scripts/Bullets/ObjectPool/AutoDespawnParticle.cs)** — 파티클 자동 반환

| 자료구조 | 선택 이유 |
|----------|-----------|
| `Dictionary<Prefab, Queue>` | 프리팹 키 직접 접근 O(1), 다양한 타입 동시 관리 |
| `Queue<T>` | 반복문 없는 선입선출, List 풀링 대비 탐색 비용 제거 |

---

### 3. Player 시스템 (MVC)

- **[PlayerController.cs](Assets/Scripts/Player/PlayerController.cs)** — 입력 처리, 이동·회피·공격·특수공격 호출
- **[PlayerModel.cs](Assets/Scripts/Player/PlayerModel.cs)** — HP, 쿨타임, 상태이상(화상·냉기), 무기 장착 관리
- **[PlayerView.cs](Assets/Scripts/Player/PlayerView.cs)** — Rigidbody 이동, 포신·본체 회전, 장애물 겹침 해결
- **[PlayerFinalStats.cs](Assets/Scripts/Player/PlayerFinalStats.cs)** — 최종 스탯 래퍼 (특전 합산 결과 보관)
- **[PlayerCameraController.cs](Assets/Scripts/Player/PlayerCameraController.cs)** — 탑뷰 카메라 추적

---

### 4. 무기 시스템 설계 개선 (초기 → 개선)

**초기 문제**: `Player ↔ Weapon` 간 직접 참조 + `GameManager` 책임 과중

```
[Before]  GameManager ─┬─ Player ↔ Weapon
                       └─ (무기 선택 데이터 보관)
```

```
[After]   EquipManager → PlayerStatsContext → FinalStats
                                  ↑
               Perks (Tree/Node/Stat) 합산 후 Player & Weapon 에 반영
```

- **[EquipManager.cs](Assets/Scripts/System/EquipManager.cs)** — 무기 선택·장착·특전 바인딩 전담
- **[WeaponPresenter.cs](Assets/Scripts/Weapon/WeaponPresenter.cs)** — MVP Presenter, View 갱신 중재
- **[WeaponModel.cs](Assets/Scripts/Weapon/WeaponModel.cs)** — 무기 순수 데이터 모델
- **[WeaponView.cs](Assets/Scripts/Weapon/WeaponView.cs)** — 무기 시각 처리
- **[WeaponData.cs](Assets/Scripts/Weapon/WeaponData.cs)** — ScriptableObject 설정값

---

### 5. UI 정적/동적 분리 설계 (드로우콜 최적화)

- **[StageStaticUI.cs](Assets/Scripts/UI/StageStaticUI.cs)** — 변경이 적은 UI 요소 (Static Batching)
- **[StageDynamicUI.cs](Assets/Scripts/UI/StageDynamicUI.cs)** — 매 프레임 갱신되는 HP바·쿨타임 등
- **[UIManager.cs](Assets/Scripts/System/UIManager.cs)** — UI 스택 관리, 입력맵 전환 (Player ↔ UI)

</details>

---

## 게임 개요

| 항목 | 내용 |
|------|------|
| **장르** | 3D 탑뷰 탄막 슈팅 |
| **개발 기간** | 2025.12 |
| **엔진** | Unity 6000.2.10f1 (Universal 3D) |
| **팀 구성** | 개발 4인 / 기획 5인 |
| **핵심 기술** | OOD: Stat Node Tree, Generic Object Pooling |

### 플레이 방식

- 탑뷰 시점에서 탱크를 조종하여 보스와 전투
- 라이플 / 샷건 / 스나이프 라이플 중 무기 선택 후 스테이지 입장
- 보스의 패턴을 피하고, **카운터 어택** 타이밍에 반격하여 강력한 패턴을 저지
- 스테이지 클리어 후 특전(Perks)을 선택해 무기와 플레이어 능력치 성장

---

## 시스템 구조

### 무기 & 특전 (Weapon & Perks)

```
EquipManager
  └── PlayerStatsContext.Rebuild()
        ├── PerksCalculator.ApplyToPlayer(PerksTree)
        └── PerksCalculator.ApplyToWeapon(PerksTree)
              └── FinalStats (Player / Weapon)
```

**무기 3종**

| 무기 | 특징 |
|------|------|
| [Rifle](Assets/Scripts/Weapon/Weapons/Rifle.cs) | 연사형. 지속 사격 시 스택 축적 → NoBrain/Minigun 모드 전환 |
| [Shotgun](Assets/Scripts/Weapon/Weapons/Shotgun.cs) | 산탄형. 근거리 광역 피해 |
| [Sniper](Assets/Scripts/Weapon/Weapons/Sniper.cs) | 고위력 단발. 레이저 특수공격 |

특전은 5레벨 트리로 구성되며, 각 레벨마다 좌·우 선택지 중 하나를 고릅니다.
선택에 따라 **공격력·속도·투사체 수** 등 세분화된 스탯이 변동됩니다.

---

### 보스 페이즈 & 패턴

```
BossController (HP 모니터링)
  └── PhaseManager
        ├── Phase 1 → [Pattern 1-A, 1-B, 1-C, 1-D] (동시 실행)
        └── Phase 2 → [Pattern 2-A, 2-B, 2-C, 2-D]  ← HP% 미만시 전환
```

- **[PatternBase.cs](Assets/Scripts/Patterns/PatternBase.cs)** — 경고 장판, 카운터 타이밍, 주기 사이클 공통 처리
- 각 패턴은 독립 코루틴으로 실행되며 Inspector에서 기획자가 직접 수치 조정 가능
- 파티클 시스템 & VFX로 시각 연출 강화

**구현된 패턴 목록**

| 패턴 | 파일 |
|------|------|
| 일반 포탄 | [CannonPattern.cs](Assets/Scripts/Patterns/CannonPattern.cs) |
| 화염구 | [FireBallPattern.cs](Assets/Scripts/Patterns/FireBallPattern.cs) |
| 화염 포탄 | [FireCannonPattern.cs](Assets/Scripts/Patterns/FireCannonPattern.cs) |
| 화염방사기 | [FlamethrowerPattern.cs](Assets/Scripts/Patterns/FlamethrowerPattern.cs) |
| 냉기 레이저 | [FrostLaserPattern.cs](Assets/Scripts/Patterns/FrostLaserPattern.cs) |
| 유도 미사일 | [GuidedMissilePattern.cs](Assets/Scripts/Patterns/GuidedMissilePattern.cs) |
| 빙결 장판 | [IceAreaPattern.cs](Assets/Scripts/Patterns/IceAreaPattern.cs) |
| 레이저 폭격 | [LaserBombingPattern.cs](Assets/Scripts/Patterns/LaserBombingPattern.cs) |
| 레이저 | [LaserPattern.cs](Assets/Scripts/Patterns/LaserPattern.cs) |
| 오브젝트 소환 | [ObjectSpawnPattern.cs](Assets/Scripts/Patterns/ObjectSpawnPattern.cs) |
| 광역 효과 | [PatternAreaOfEffect.cs](Assets/Scripts/Patterns/PatternAreaOfEffect.cs) |
| 조준 사격 | [PrecisionStrikePattern.cs](Assets/Scripts/Patterns/PrecisionStrikePattern.cs) |
| 슬로우 장판 | [SlowAreaPattern.cs](Assets/Scripts/Patterns/SlowAreaPattern.cs) |

---

### Generic Object Pooling

```csharp
// 사용 예시
PoolManager.Instance.Spawn<PlayerBullet>(bulletPrefab, spawnPos, rotation);
PoolManager.Instance.Despawn(bullet);
```

GC 스파이크 방지를 위해 탄환·파티클·레이저·장판 등 모든 동적 오브젝트에 적용.

---

## 프로젝트 구조

```
Assets/Scripts/
├── Boss/           # BossController, PhaseManager, PhaseData
├── Bullets/        # 탄환 로직 + ObjectPool 시스템
├── ObjectControllers/  # 레이저·장판·데칼 등 오브젝트
├── Option/         # 설정(해상도, 음량) 시스템
├── Patterns/       # 보스 패턴 (PatternBase 상속 구조)
├── Perks/          # Stat → Node → Tree 특전 시스템
├── Player/         # MVC 구조 플레이어
├── Sounds/         # SoundManager, BGM/SFX
├── System/         # GameManager, UIManager, EquipManager
├── UI/             # 로비·스테이지·특전 선택 UI
└── Weapon/         # MVP 구조 무기 (Rifle / Shotgun / Sniper)
```

---

## Links

- **YouTube Demo** : https://youtu.be/Es8QreV_6Ow
- **GitHub** : https://github.com/Ellug/Project_SAD
