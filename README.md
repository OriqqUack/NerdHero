
# 🎮 '히어로는 출동이 무서워' 스크립트 소개

이 저장소는 Unity 기반 프로젝트인 **NerdHero**의 주요 게임 로직과 UI, 백엔드 시스템을 포함한 스크립트 모음입니다.  
전투, 퀘스트, 스킬, 강화, 인벤토리 등 다양한 시스템을 구성한 게임 아키텍처를 담고 있습니다.

---

## 🧩 시스템 개요

### 🗂 스크립트 카테고리

| 분류             | 설명 |
|------------------|------|
| `Backend/`       | 백엔드 SDK 기반 로그인, 유저 데이터 연동, 클라우드 저장 관리 |
| `Contents/`      | 카메라 제어, 캐릭터 동작, 애프터이미지, 슬롯머신, 퀘스트, 스킬 등 게임 기능 |
| `UI/`            | 인게임, 인벤토리, 장비, 상점, 퀘스트, 설정 등 UI 시스템 |
| `Utils/`         | 유틸리티 클래스 및 공통 기능 처리 |
| `Managers/`      | 게임 흐름과 전역 데이터를 관리하는 싱글톤 매니저 클래스 |

---

## ⚙️ 주요 시스템 소개

### 🎯 퀘스트 시스템
- `UI_Quest.cs`와 관련 매니저를 통해 퀘스트 수락, 진행, 완료까지의 전 과정을 처리
- 퀘스트 조건 및 보상은 데이터 기반으로 관리
- UI에서 퀘스트 트래킹 가능

### 🧠 스킬 시스템
- `SkillEditor.cs` 등에서 캐릭터의 스킬 목록을 시각화하고, 효과 설명 제공
- 'StateMachine.cs' 을 통해 Skill State 관리
  
### 📦 인벤토리 시스템
- `UI_Inventory.cs`는 무기, 방어구 필터링 기능 포함
- 동적 슬롯 생성 및 아이템 등록 처리
- 백엔드에서 받아온 아이템 정보를 기반으로 UI 구성

### 🎮 인게임 전투 및 연출
- 웨이브 기반 전투 시스템, 적 등장 패턴 관리
- `AfterImageGenerator.cs` 등 시각적 피드백 요소 포함
- 카메라 제어 및 HUD 시스템 분리 관리
- Behavior Designer를 통한 Monster AI 구현
  
---

## 🖥️ UI 아키텍처

UI는 다음과 같은 구조로 명확히 분리되어 있으며, `UiWindow` 상속 및 `Initialize()` 방식으로 일관된 초기화 구조를 갖습니다.

- `MainScene/` – 로비, 상점, 우편함, 설정
- `InGameScene/` – 일시정지, 부활, 알림
- `Equipment/` – 인벤토리, 장비 상세, 강화
- `Quest/`, `Reward/` – 퀘스트 알림 및 보상
- `HUD_Indicator/` – 게임 내 실시간 정보 UI

---

## ☁️ 백엔드 연동

- `GoogleBackendAutoLoginManager`를 통해 자동 로그인 처리
- `UserInfo.cs`, `BackendDataManager.cs`에서 유저 데이터를 서버와 동기화
- 클라우드 세이브/로드 기반 유저 정보 보존

---

## 🔧 기술 및 설계 패턴

- **싱글톤 매니저**: 게임, 인벤토리, UI 등 전역 처리
- **ScriptableObject**: 아이템 및 데이터 정의에 활용 가능성 높음
- **DOTween**: UI 애니메이션 및 연출 전반에 사용
- **이벤트 기반 처리**: 버튼, 강화 애니메이션, 퀘스트 완료 등

---

## 🔍 탐색 추천

| 탐색 대상 | 설명 |
|-----------|------|
| `UI_Inventory.cs` | 필터링, 슬롯 생성, 아이템 UI 처리 |
| `UI_Forge.cs`     | 강화 애니메이션 및 흐름 구성 |
| `Quest.cs`     | 퀘스트 수락 → 진행 → 완료 처리 흐름 |
| `SKill.cs` | 스킬 설정 |
| `BackendDataManager.cs` | 서버 저장, 로딩, 유저 데이터 관리 |

---

## 📁 예시 폴더 구조

```
Scripts/
├── Backend/
├── Contents/
│   ├── Camera/
│   ├── SlotMachine/
│   ├── Quest/
│   ├── Skill/
├── Managers/
├── UI/
│   ├── Equipment/
│   ├── MainScene/
│   ├── InGameScene/
│   ├── Quest/
│   ├── Reward/
└── Utils/
```

---
