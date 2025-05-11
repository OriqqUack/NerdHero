using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Google;
using System.Threading.Tasks;
using UnityEngine.UI;
using BackEnd;
using TMPro;

public class GoogleBackendAutoLoginManager : MonoBehaviour
{
    [Header("UI")]
    public Progress loadingPanel;
    public Button googleLoginButton;
    public Button sceneLoadButton;
    public TextMeshProUGUI sceneLoadText;

    private GoogleSignInConfiguration configuration;

    private const string SavedEmailKey = "SavedGoogleEmail"; // 이메일 저장 키

    private const string EditorCustomId = "EditorTestAccount";
    private const string EditorCustomPw = "EditorTestPassword";
    
    public void Init()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = "192984349131-sa8ctkm54sjm2lu74h1008ktm40858er.apps.googleusercontent.com",
            RequestEmail = true,
            RequestIdToken = true
        };

        var bro = Backend.Initialize();
        if (bro.IsSuccess())
        {
            Debug.Log("✅ 뒤끝 초기화 성공");
        }
        else
        {
            Debug.LogError("❌ 뒤끝 초기화 실패");
        }

        loadingPanel.gameObject.SetActive(false);
        googleLoginButton.onClick.AddListener(OnGoogleLoginClicked);
        googleLoginButton.gameObject.SetActive(false);
        sceneLoadText.gameObject.SetActive(false);
        sceneLoadButton.interactable = false;

        AutoLoginIfPossible();
    }

    private void AutoLoginIfPossible()
    {
#if UNITY_EDITOR
        // ✅ 1순위: 에디터는 무조건 커스텀 계정
        Debug.Log("🖥️ [Editor] 커스텀 계정으로 자동 로그인 시도");
        LoginWithEditorCustomAccount();

#elif UNITY_ANDROID
    // ✅ 2순위: 안드로이드는 구글 로그인 사용
    if (PlayerPrefs.GetInt("GoogleLoginSuccess", 0) == 1 && PlayerPrefs.HasKey("SavedGoogleEmail"))
    {
        Debug.Log("📱 [Android] 저장된 이메일로 자동 로그인 시도");
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.DefaultInstance.SignIn()
            .ContinueWith(OnGoogleAuthenticationFinished, TaskScheduler.FromCurrentSynchronizationContext());
    }
    else
    {
        Debug.Log("📱 [Android] 수동 로그인 대기");
        googleLoginButton.gameObject.SetActive(true);
    }

#else
    // ✅ 3순위: PC, Mac, 기타 플랫폼 → 커스텀 계정 로그인
    Debug.Log("🖥️ [Standalone] 커스텀 계정으로 자동 로그인 시도");
    LoginWithEditorCustomAccount();

#endif
    }
    
    private void LoginWithEditorCustomAccount()
    {
        var bro = Backend.BMember.CustomLogin(EditorCustomId, EditorCustomPw);

        if (bro.IsSuccess())
        {
            Debug.Log("✅ 에디터 커스텀 계정 로그인 성공!");
            googleLoginButton.gameObject.SetActive(false);
            LoadingProgress();
        }
        else
        {
            Debug.LogWarning($"⚠️ 에디터 커스텀 계정 로그인 실패: {bro.GetErrorCode()}, {bro.GetMessage()}");

            // 실패 코드가 "회원정보 없음"이면 회원가입
            if (int.Parse(bro.GetStatusCode()) == 401 || bro.GetErrorCode() == "BadUnauthorizedException" || bro.GetErrorCode() == "bad customId")
            {
                Debug.Log("🆕 테스트 계정이 없으니 회원가입 시도");
                var signUpBro = Backend.BMember.CustomSignUp(EditorCustomId, EditorCustomPw);

                if (signUpBro.IsSuccess())
                {
                    Debug.Log("✅ 에디터 테스트 계정 회원가입 성공!");
                    // 가입 후 다시 로그인
                    LoginWithEditorCustomAccount();
                }
                else
                {
                    Debug.LogError($"❌ 에디터 테스트 계정 회원가입 실패: {signUpBro.GetErrorCode()}, {signUpBro.GetMessage()}");
                }
            }
            else
            {
                Debug.LogError($"❌ 에디터 테스트 계정 로그인 실패(알 수 없는 에러): {bro.GetErrorCode()}, {bro.GetMessage()}");
            }
        }
    }

    
    private void OnGoogleLoginClicked()
    {
#if UNITY_ANDROID
        Debug.Log("▶️ 구글 로그인 버튼 클릭");
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.DefaultInstance.SignIn()
            .ContinueWith(OnGoogleAuthenticationFinished, TaskScheduler.FromCurrentSynchronizationContext());
        googleLoginButton.interactable = false;
#endif
    }

    private void OnGoogleAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("❌ 구글 로그인 실패: " + task.Exception);
            googleLoginButton.interactable = true;
        }
        else if (task.IsCanceled)
        {
            Debug.LogWarning("⚠️ 구글 로그인 취소됨");
            googleLoginButton.interactable = true;
        }
        else
        {
            Debug.Log("✅ 구글 로그인 성공");
            InGameLogger.Log("✅ 구글 로그인 성공");

            string idToken = task.Result.IdToken;
            string email = task.Result.Email;

            PlayerPrefs.SetInt("GoogleLoginSuccess", 1);
            PlayerPrefs.SetString("SavedGoogleEmail", email);
            PlayerPrefs.Save();
            
            BackendLoginWithIdToken(idToken, email);
        }
    }

    private void BackendLoginWithIdToken(string idToken, string email)
    {
        Debug.Log("▶️ 뒤끝에 ID Token 로그인 요청");

        var bro = Backend.BMember.AuthorizeFederation(idToken, FederationType.Google, email);

        if (bro.IsSuccess())
        {
            Debug.Log("✅ 뒤끝 ID Token 로그인 성공!");

            googleLoginButton.gameObject.SetActive(false);
            LoadingProgress();
        }
        else
        {
            Debug.LogError($"❌ 뒤끝 로그인 실패: {bro.GetErrorCode()}, {bro.GetMessage()}");

            if (bro.GetErrorCode() == "403")
            {
                Debug.LogWarning("⚠️ ID Token 만료. 다시 로그인 필요");
                PlayerPrefs.DeleteKey(SavedEmailKey);
            }

            googleLoginButton.interactable = true; // 실패하면 버튼 다시 활성화
        }
    }

    private void LoadingProgress()
    {
        loadingPanel.gameObject.SetActive(true);
        Managers.BackendManager.LoadUserData();
        loadingPanel.Play(OnAfterProgress);
    }

    private void OnAfterProgress()
    {
        loadingPanel.gameObject.SetActive(false);
        sceneLoadButton.interactable = true;
        sceneLoadText.gameObject.SetActive(true);
    }
}