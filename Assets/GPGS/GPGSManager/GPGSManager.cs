using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class GPGSManager : MonoBehaviour
{
    private int coin;
    public void GPGSLogin()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            string displayName = PlayGamesPlatform.Instance.GetUserDisplayName();
            string userID = PlayGamesPlatform.Instance.GetUserId();
            //로그인 성공
        }
        else
        {
            //로그인 실패
        }
    }

    public void ShowLeaderboardUI()
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_coin);
    }

    public void AddCoin()
    {
        coin += 1;
        PlayGamesPlatform.Instance.ReportScore(coin, GPGSIds.leaderboard_coin, (bool success) => { });
    }

    public void IncrementGPGSAchievement()
    {
        //단계별 업적 증가
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement, 1, (bool success) => { });
    }

    public void UnLockAchievement()
    {
        PlayGamesPlatform.Instance.UnlockAchievement(GPGSIds.achievement);
    }
}
