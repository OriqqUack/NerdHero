using UnityEngine;

public static class GameColors
{
    public static readonly Color CommonBG = new Color(0.1411765f, 0.7568628f, 1f);         // 파란색 계열
    public static readonly Color CommonGradient = new Color(0.31761f, 0.806812f, 1f);      // 주황색 계열
    public static readonly Color RareBG = new Color(0.8392158f, 0.2078432f, 0.9607844f);     // 어두운 배경색
    public static readonly Color RareGradient = new Color(0.5529412f, 0.4392157f, 0.7803922f);         // 보라 계열 포인트 색
    public static readonly Color LegendaryBG = new Color(1f, 0.7294118f, 0.07058824f);
    public static readonly Color LegendaryGradient = new Color(0.8431373f, 0.650980f, 0.364705f);
    
    public static readonly Color SelectedBottomTabColor = new Color(0.3685112f, 0.3051303f, 0.4779873f);
    public static readonly Color SelectedInventoryTabColor = new Color(0.254902f, 0.1764706f, 0.4392157f);
    public static readonly Color SelectedInventoryBorderColor = new Color(0.3333333f, 0.2509804f, 0.5215687f);
    
    public static readonly Color SelectedProfileBackgroundColor = new Color(0.427451f, 0.3529412f, 0.8039216f);
    public static readonly Color SelectedProfileInnerBorderColor = new Color(0.5529412f, 0.4705882f, 0.8823529f);
    public static readonly Color SelectedProfileDecoColor = new Color(0.5529412f, 0.4705882f, 0.8823529f);
}
