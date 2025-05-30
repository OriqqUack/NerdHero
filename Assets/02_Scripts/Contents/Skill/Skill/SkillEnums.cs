// Skill�� ���� ���� ���ΰ�
// FinishWhenApplyCompleted�� applyCount��ŭ ��� �����̵Ǹ� ����
// DurationEnded�� applyCount��ŭ �����ߵ�, ���ߵ� �ð��� ������ ����
public enum SkillRunningFinishOption
{
    FinishWhenApplyCompleted,
    FinishWhenDurationEnded,
}

// Skill�� Charge�� ������(���� �ð��� ������) � �ൿ�� ���� ���ΰ�?
// Use�� Skill�� �ڵ����� �����
// Cancel�� Skill�� ����� ��ҵ�
public enum SkillChargeFinishActionOption
{
    Use,
    Cancel,
}

// ���� ����ؾ��ϴ� ��ų�ΰ�?(Active)
// �ڵ����� ���Ǵ� ��ų�ΰ�?(Passive)
public enum SkillType
{
    Active,
    Passive
}

// �ʿ��� TargetSearcher �˻� ��� Type�� �����ΰ�?
public enum NeedSelectionResultType
{
    Target,
    Position
}

// �ܹ߼� ��ų�ΰ�?(Instant)
// ����, �״� �� �� �ִ� Toggle�� ��ų�ΰ�?
public enum SkillUseType
{
    Instant,
    Toggle
}

public enum MovingSkillType
{
    Stop,
    Move
}

// TargetSearcher가 타겟을 Search 하는 경우
// TargetSelectionCompleted는 Target 선택(Selection)이 완료된 후 바로 Search하는 경우
// (Skill이 Target을 선택하지 않고 발동하는 경우에 사용)
// Apply는 Skill이 발동될 때 Search하는 경우
// (Skill이 발동될 때 Target을 맞추거나, 타격 범위 안의 적을 탐색하는 경우)
public enum TargetSearchTimingOption
{
    // Target 선택(Selection)이 완료된 후
    // (TargetSearcher가 SelectTarget을 완료하면 바로 Search 실행)
    TargetSelectionCompleted,
    // Skill이 발동될 때
    Apply
}

// 스킬 발동의 CustomAction인가?
public enum SkillCustomActionType
{
    Cast,
    Charge,
    PrecedingAction,
    Action,
}

// Skill을 발동할 때 어떤 방식으로 발동할 것인가?
// Auto는 Skill의 ApplyCount만큼 자동으로 발동.
// Input은 특정 Key 입력으로 Skill을 발동.
// (ex. Q 키를 눌러서 발동하는 Q 스킬 - Q 버튼을 누를 때 스킬이 최대 3번까지 발동할 수 있다)
public enum SkillExecutionType
{
    Auto,
    Input
}

// TargetSearcher가 SelectTarget을 호출하는 타이밍을 설정하는 것인가?
// Use는 Skill을 사용할 때 호출
// UseInAction은 ExecutionType이 Input일 때 Skill을 사용하거나 입력받았을 때 호출
public enum TargetSelectionTimingOption
{
    Use,
    UseInAction,
    Both
}

// Skill의 적용 방식
// Instant는 Skill이 발동되면 바로 적용
// Animation은 Skill 발동 애니메이션이 끝난 후 적용
// (예를 들어 캐릭터가 손을 뻗는 모션을 끝내고 나서 스킬이 적용되는 경우)
public enum SkillApplyType
{
    Instant,
    Animation
}


// Skill�� ����ϴ� Entity�� InSkillActionState�� �������� ���ΰ�?
// FinishOnceApplied�� Skill�� �ѹ� ������ڸ��� �ٷ� ĳ���͸� ������ �� ����.
// FinishWhenFullyApplied�� Skill�� ApplyCount��ŭ ��� ����Ǿ� ĳ���͸� ������ �� ����.
// FinishWhenAnimationEnded�� Skill�� ���� ���ο� ������� ���� �������� Animation�� ������ ĳ���͸� ������ �� ����
// ���� ���, ������� ���̾�� ������ �ִϸ��̼��� ��,
// ���̾�� ������ �ִϸ��̼��� ������ ������ �⺻ �������� ���ƿ��� ĳ������ ��� ��������.
public enum InSkillActionFinishOption
{
    FinishOnceApplied,
    FinishWhenFullyApplied,
    FinishWhenAnimationEnded,
}