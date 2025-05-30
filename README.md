
# 📁 `02_Scripts` – Unity 게임 스크립트 구조 (NerdHero 프로젝트)

이 폴더는 Unity 기반 프로젝트인 **NerdHero**의 주요 게임 로직 및 UI 시스템을 구성하는 C# 스크립트로 구성되어 있습니다. 액션 중심의 전투, 장비 강화, 인벤토리 시스템, 그리고 백엔드 연동이 특징입니다.

---

## 🧩 시스템 개요

### 🗂 스크립트 카테고리

| 분류             | 설명 |
|------------------|------|
| `Backend/`       | 백엔드 SDK 기반 로그인, 유저 데이터 연동 및 클라우드 저장 관리 |
| `Contents/`      | 카메라, 캐릭터, 애프터이미지, 슬롯 머신 등 주요 게임 기능 |
| `UI/`            | 인게임, 인벤토리, 장비, 상점, 설정 등 전체 UI 시스템 |
| `Utils/`         | 보조 유틸리티, 데이터 클래스 및 헬퍼 기능 |
| `Managers/`      | GameManager, UIManager, InventoryManager 등 싱글톤 매니저들 |

---

## 🕹️ 주요 기능

### 🎮 인게임 시스템
- **웨이브 기반 전투 시스템**: 패턴 기반 적 스폰 및 보상
- **슬롯 머신 메커니즘**: 가챠 또는 보상 시스템에 활용
- **애프터이미지**: 빠른 움직임을 시각적으로 강조하는 효과

### ⚔️ 장비 및 스탯
- 무기/방어구 필터링이 가능한 장비 시스템
- 장비 상세 설명 및 능력치 팝업 UI
- 강화 시스템 (`UI_Forge.cs`)

### 📦 인벤토리
- 실시간 인벤토리 슬롯 생성 및 필터링
- Backend와 연동된 아이템 정보 처리

---

## 🖥️ UI 아키텍처

UI는 다음과 같이 모듈화되어 있으며, 각 UI는 `UiWindow`를 상속하고 `Initialize()` 방식으로 초기화됩니다.

- `UI/MainScene/` – 로비, 프로필, 상점, 우편함
- `UI/InGameScene/` – 일시정지, 부활, 종료 알림
- `UI/Equipment/` – 인벤토리, 장비 강화, 상세 정보
- `UI/HUD_Indicator/` – HUD 및 화면 상단 정보

---

## ☁️ 백엔드 연동

- `GoogleBackendAutoLoginManager`를 통한 자동 로그인
- `UserInfo.cs`, `BackendDataManager.cs`가 사용자 정보 관리
- 클라우드 세이브/로드 기능 포함

---

## 🔧 기술 및 패턴

- **싱글톤 패턴**: 주요 매니저 클래스에서 사용
- **ScriptableObject** 기반 아이템 정의 추정
- **이벤트 기반 UI 구성**: 버튼 클릭, 애니메이션 이벤트 등
- **DOTween 애니메이션 활용**: 슬라이더, 팝업, 트윈 처리

---

## 🧪 탐색 추천

코드 리뷰나 포트폴리오 확인 시 다음 순서를 추천합니다:

1. `UI/Equipment/UI_Inventory.cs` – 인벤토리 필터링 및 동적 생성
2. `UI_Forge.cs` – 강화 애니메이션 및 슬롯 설정 흐름
3. `Backend/BackendDataManager.cs` – 유저 정보 동기화 및 저장

---

## 📁 폴더 구조 예시

```
02_Scripts/
├── Backend/
├── Contents/
│   ├── AfterImage/
│   ├── Camera/
│   ├── SlotMachine/
├── Managers/
├── UI/
│   ├── Equipment/
│   ├── InGameScene/
│   ├── MainScene/
│   ├── HUD_Indicator/
└── Utils/
```

---

**NerdHero**는 UI와 게임 로직이 잘 분리되어 있어 구조적으로 이해하기 쉽고, 확장성도 고려된 Unity 프로젝트입니다.
