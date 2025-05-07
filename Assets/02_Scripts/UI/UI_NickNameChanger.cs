using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using BackEnd;
using Unity.VisualScripting;

public class UI_NickNameChanger : UiWindow
{
    [System.Serializable]
    public class NickNameChangeEvent : UnityEvent { }
    public NickNameChangeEvent nickNameChange = new NickNameChangeEvent();
    
    [SerializeField] private TextMeshProUGUI alertText;
    [SerializeField] private TMP_InputField inputFieldNickName;
    [SerializeField] private Button btnUpdateNickName;

    protected override void Start()
    {
        base.Start();
        btnUpdateNickName.onClick.AddListener(() => UpdateNickName());
        nickNameChange.AddListener(() => UserInfo.Instance.GetUserInfoFromBackend());
    }

    private void UpdateNickName()
    {
        Backend.BMember.UpdateNickname(inputFieldNickName.text, callback =>
        {
            btnUpdateNickName.interactable = true;

            if (callback.IsSuccess())
            {
                nickNameChange?.Invoke();
                Close();
            }
            else
            {
                string message = string.Empty;

                switch (int.Parse(callback.GetStatusCode()))
                {
                    case 400: // 빈 닉네임
                        message = "닉네임이 비어있거나 | 20자 이상이거나 | 공백이 있습니다.";
                        break;
                    case 409:
                        message = "이미 존재하는 닉네임입니다.";
                        break;
                    default:
                        message = callback.GetMessage();
                        break;
                }
                
                alertText.text = message;
            }
        });
    }
}
