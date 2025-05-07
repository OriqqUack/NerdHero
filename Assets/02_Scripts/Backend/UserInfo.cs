using System;
using BackEnd;
using LitJson;
using UnityEngine;

public class UserInfo : MonoSingleton<UserInfo>
{
    [System.Serializable]
    public class UserInfoEvent : UnityEngine.Events.UnityEvent { }
    public UserInfoEvent OnUserInfoUpdated = new UserInfoEvent();
    
    private static UserInfoData data = new UserInfoData();
    public static UserInfoData Data => data;

    public void GetUserInfoFromBackend()
    {
        Backend.BMember.GetUserInfo(callback =>
        {
            if (callback.IsSuccess())
            {
                try
                {
                    var returnJson = callback.GetReturnValuetoJSON();
                
                    // [💬 중요!] 받은 JSON 전체를 출력
                    Debug.Log($"[GetUserInfoFromBackend] 서버 응답 JSON: {returnJson?.ToJson() ?? "null"}");

                    if (returnJson != null && returnJson.ContainsKey("row"))
                    {
                        JsonData json = returnJson["row"];

                        data.gamerId = SafeGetString(json, "gamerId");
                        data.countryCode = SafeGetString(json, "countryCode");
                        data.nickName = SafeGetString(json, "nickname");
                        data.inDate = SafeGetString(json, "inDate");
                        data.emailForFindPassword = SafeGetString(json, "emailForFindPassword");
                        data.subscriptionType = SafeGetString(json, "subscriptionType");
                        data.federationId = SafeGetString(json, "federationId");
                    }
                    else
                    {
                        Debug.LogError("❌ [UserInfo] 서버 응답에 row가 없습니다");
                        data.Reset();
                    }
                }
                catch (Exception e)
                {
                    data.Reset();
                    Debug.LogError($"❌ [UserInfo] 파싱 에러: {e}");
                }
            }
            else
            {
                data.Reset();
                Debug.LogError($"❌ [UserInfo] 실패 응답: {callback.GetMessage()}");
            }

            OnUserInfoUpdated?.Invoke();
        });
    }
    
    private string SafeGetString(JsonData json, string key)
    {
        return json.ContainsKey(key) && json[key] != null ? json[key].ToString() : string.Empty;
    }

}
