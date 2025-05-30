using UnityEngine;

//Scene에 미리 만들어둔 Holder의 정보를 담고 있다
public class WindowHolder : MonoBehaviour
{
    public enum HolderType
    {
        Shop,
        Profile,
        Quest,
        Setting,
        MailBox,
        RewardBox,
        EnergyCharge,
        Equipment,
        EquipmentDetail,
        Pause,
        ExitAlert,
        Revive,
        GameEnd,
        CardSelec,
        ChangeNickName,
        Congratue
    }

    public string Name = "Window";
    public HolderType Type;

    public UiWindow OpenWindow()
    {
        UiWindow newWindow = null;
        switch (Type)
        {
            case HolderType.Shop:
                newWindow = WindowManager.GetWindow("Shop", this);
                break;
            case HolderType.Profile:
                newWindow = WindowManager.GetWindow("Profile", this);
                break;
            case HolderType.Setting:
                newWindow = WindowManager.GetWindow("Setting", this);
                break;
            case HolderType.MailBox:
                newWindow = WindowManager.GetWindow("MailBox", this);
                break;
            case HolderType.RewardBox:
                newWindow = WindowManager.GetWindow("RewardBox", this);
                break;
            case HolderType.EnergyCharge:
                newWindow = WindowManager.GetWindow("EnergyCharge", this);
                break;
            case HolderType.Quest:
                newWindow = WindowManager.GetWindow("Quest", this);
                break;
            case HolderType.Equipment:
                newWindow = WindowManager.GetWindow("Equipment", this);
                break;
            case HolderType.EquipmentDetail:
                newWindow = WindowManager.GetWindow("EquipmentDetail", this);
                break;
            case HolderType.Pause:
                newWindow = WindowManager.GetWindow("Pause", this);
                break;
            case HolderType.ExitAlert:
                newWindow = WindowManager.GetWindow("ExitAlert", this);
                break;
            case HolderType.Revive:
                newWindow = WindowManager.GetWindow("Revive", this);
                break;
            case HolderType.GameEnd:
                newWindow = WindowManager.GetWindow("GameEnd", this);
                break;
            case HolderType.CardSelec:
                newWindow = WindowManager.GetWindow("CardSelec", this);
                break;
            case HolderType.ChangeNickName:
                newWindow = WindowManager.GetWindow("ChangeNickName", this);
                break;
            case HolderType.Congratue:
                newWindow = WindowManager.GetWindow("Congratue", this);
                break;
        }
        if(newWindow != null) newWindow.Initialize(this, Name);

        return newWindow;
    }
}
