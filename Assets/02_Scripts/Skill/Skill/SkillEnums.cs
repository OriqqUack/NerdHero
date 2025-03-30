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

// TargetSearcher�� ���� Target�� Search �ϴ°�?
// TargetSelectionCompleted�� Target ����(Selection)�� �Ǿ��� �� �ٷ� Search�� ������
// (Skill�� Target�� ������ �ʰ� ������ �� ���)
// Apply�� Skill�� ����� �� Search�� ������
// (Skill�� ����� ������ Target�� �޶��� �� ���� ��� ���)
public enum TargetSearchTimingOption
{
    // Target ����(Selection)�� �Ǿ��� ��
    // (TargetSearcher�� SelectTarget�� �Ϸ��ϸ� �ٷ� Search ����)
    TargetSelectionCompleted,
    // Skill�� ����� ��
    Apply
}

// ���� ����� CustomAction�ΰ�?
public enum SkillCustomActionType
{
    Cast,
    Charge,
    PrecedingAction,
    Action,
}

// Skill�� ������� �� � ������ ����� ���ΰ�?
// Auto�� Skill�� ApplyCount��ŭ �ڵ� �����.
// Input�� Ư�� Key�� ������ Skill�� �����.
// (ex. �� ������ Q ��ų - Q Button�� ������ ��ų�� #�� ���� �ִ� 3�� ����� �� ����)
public enum SkillExecutionType
{
    Auto,
    Input
}

// TargetSearcher�� SelectTarget�� ����Ǵ� Ÿ�̹��� �����ΰ�?
// Use�� Skill�� ����� �� ������
// UseInAction�� ���� ExecutionType�� Input�� �� Skill�� �����ų ������ ������.
public enum TargetSelectionTimingOption
{
    Use,
    UseInAction,
    Both
}

// Skill�� ���� ����
// Instant�� Skill�� ����Ǹ� �ٷ� ������
// Animation�� Skill�� ���� ������ Animation���� �����Ͽ� ������.
// �̸� ���� ĳ���Ͱ� �ָ��� �������� ����, ���� ���� �� �� �� Skill�� ���� ������ �����ϰ� ���� �� ����.
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