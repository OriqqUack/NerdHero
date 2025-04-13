public enum EffectType
{
    None,
    Buff,
    Debuff
}

public enum EffectRarity
{
    Common,
    Uncommon,
    Rare
}

// Effect�� �ߺ� ���� ����� ���� ���� ��, ���� �ߺ� ������ �Ͼ�ٸ� �ߺ� ����� Effect �� �� ������ ������ ���ΰ�?
public enum EffectRemoveDuplicateTargetOption
{
    // �̹� �������� Effect�� ����
    Old,
    // ���� ����� Effect�� ����
    New,
}

// Effect�� �Ϸ� ������ �����ΰ�?
public enum EffectRunningFinishOption
{
    // Effect�� ������ ���� Ƚ����ŭ ����ȴٸ� �Ϸ�Ǵ� Option.
    // ��, �� Option�� ���� �ð�(=Duration)�� ������ �Ϸ��.
    // Ÿ���� �����ٴ���, ġ�Ḧ ���ִ� Effect�� ����Option
    FinishWhenApplyCompleted,
    // ���� �ð��� ������ �Ϸ�Ǵ� Option.
    // Effect�� ������ ���� Ƚ����ŭ ����ǵ�, ���� �ð��� ���Ҵٸ� �Ϸᰡ �ȵ�.
    // ó�� �ѹ� ����ǰ�, ���� �ð����� ���ӵǴ� Buff�� Debuff Effect�� ������ Option.
    FinishWhenDurationEnded,
}