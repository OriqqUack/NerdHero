using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UI_Quest : UiWindow
{
    private static readonly int rewardHour = 10; // 오전 10시 기준
    [SerializeField] private List<Button> rewardButtons; // 활성화할 버튼 리스트
    [SerializeField] private List<Button> buttons;
    [SerializeField] private List<GameObject> popups;
    
    private GameObject currentPopup;
    protected override void Start()
    {
        base.Start();
        /*StartCoroutine(CheckDailyReward());

        currentPopup = popups[0];
        for (int i = 0; i < buttons.Count; i++)
        {
            int i1 = i;
            buttons[i].onClick.AddListener(() => OpenPopup(i1));
        }*/
    }

    public override void Initialize(WindowHolder holder, string name = "")
    {
        base.Initialize(holder, name);
        
    }

    private void OpenPopup(int i)
    {
        if(currentPopup) currentPopup.SetActive(false);
        currentPopup = popups[i];
        popups[i].SetActive(true);
    }
    
    IEnumerator CheckDailyReward()
    {
        Task<DateTime> timeTask = GetNetworkTime();
        yield return new WaitUntil(() => timeTask.IsCompleted);

        DateTime utcTime = timeTask.Result; // 서버에서 받은 현재 시간
        DateTime kstTime = utcTime.AddHours(9); // 한국 시간 변환
        DateTime serverTime = kstTime; // 정확한 시간 비교를 위해 변경

        long lastClaimTicks = Convert.ToInt64(PlayerPrefs.GetString("LastClaimTicks", "0"));
        DateTime lastClaimTime = new DateTime(lastClaimTicks); // 마지막 보상 시간 불러오기

        if (!PlayerPrefs.HasKey("ConsecutiveLoginDays"))
        {
            PlayerPrefs.SetInt("ConsecutiveLoginDays", 0);
        }

        int consecutiveLoginDays = PlayerPrefs.GetInt("ConsecutiveLoginDays", 0);

        // 보상 지급 가능 여부 확인
        DateTime nextRewardTime = GetNextRewardTime(lastClaimTime);
        bool canClaim = serverTime >= nextRewardTime;

        if (canClaim)
        {
            consecutiveLoginDays++;
            PlayerPrefs.SetInt("ConsecutiveLoginDays", consecutiveLoginDays);
            PlayerPrefs.SetString("LastClaimTicks", serverTime.Ticks.ToString());
            PlayerPrefs.Save();

            Debug.Log($"보상을 받을 수 있습니다! (접속 횟수: {consecutiveLoginDays})");
            GiveReward();
        }
        else
        {
            TimeSpan timeLeft = nextRewardTime - serverTime;
            Debug.Log($"다음 보상까지 남은 시간: {timeLeft.Hours}시간 {timeLeft.Minutes}분 {timeLeft.Seconds}초");
        }

        // 버튼 업데이트
        UpdateButtons(consecutiveLoginDays);
    }

    /// <summary>
    /// Google NTP 서버에서 현재 시간 가져오기
    /// </summary>
    private async Task<DateTime> GetNetworkTime()
    {
        const string ntpServer = "time.google.com";
        try
        {
            var ntpData = new byte[48];
            ntpData[0] = 0x1B; // NTP 요청 패킷

            using (var socket = new UdpClient())
            {
                socket.Connect(ntpServer, 123);
                await socket.SendAsync(ntpData, ntpData.Length);

                var response = await socket.ReceiveAsync();
                ntpData = response.Buffer;
            }

            ulong intPart = BitConverter.ToUInt32(ntpData, 40);
            ulong fractPart = BitConverter.ToUInt32(ntpData, 44);

            intPart = (uint)IPAddress.NetworkToHostOrder((int)intPart);
            fractPart = (uint)IPAddress.NetworkToHostOrder((int)fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000);
            var utcTime = new DateTime(1900, 1, 1).AddMilliseconds(milliseconds);

            Debug.Log($"[NTP] 서버 시간 (UTC): {utcTime}");
            Debug.Log($"[NTP] 한국 시간 (KST): {utcTime.AddHours(9)}");
            
            return utcTime;
        }
        catch (Exception ex)
        {
            Debug.LogError($"NTP 시간 가져오기 실패: {ex.Message}");
            return DateTime.UtcNow; // 인터넷 연결이 없을 경우 현재 UTC 시간 사용
        }
    }

    /// <summary>
    /// 마지막 보상을 받은 시간을 기준으로, 다음 보상 가능 시간을 계산
    /// </summary>
    private DateTime GetNextRewardTime(DateTime lastClaimTime)
    {
        DateTime nextRewardDay = new DateTime(lastClaimTime.Year, lastClaimTime.Month, lastClaimTime.Day, rewardHour, 0, 0);
        
        // 마지막 보상 시간이 오전 10시 이후였다면, 다음 날 오전 10시로 설정
        if (lastClaimTime >= nextRewardDay)
        {
            nextRewardDay = nextRewardDay.AddDays(1);
        }

        return nextRewardDay;
    }
    
    /// <summary>
    /// 접속 횟수에 따라 버튼을 활성화
    /// </summary>
    private void UpdateButtons(int consecutiveLoginDays)
    {
        for (int i = 0; i < rewardButtons.Count; i++)
        {
            rewardButtons[i].interactable = (i < consecutiveLoginDays); // 실제 접속한 횟수만큼 활성화
        }

        Debug.Log($"총 {consecutiveLoginDays}번 접속하여 {Mathf.Min(consecutiveLoginDays, rewardButtons.Count)}개의 버튼이 활성화됨");
    }

    void GiveReward()
    {
        Debug.Log("보상을 지급했습니다!");
    }
    
    /// <summary>
    /// [테스트용] 하루가 지난 것처럼 설정 (접속한 날만 증가)
    /// </summary>
    [ContextMenu("하루가 지난 것처럼 설정")]
    public void SimulateNextDay()
    {
        long lastClaimTicks = Convert.ToInt64(PlayerPrefs.GetString("LastClaimTicks", "0"));
        DateTime lastClaimTime = new DateTime(lastClaimTicks).Date;
        DateTime simulatedTime = lastClaimTime.AddDays(1); // 하루 추가

        int consecutiveLoginDays = PlayerPrefs.GetInt("ConsecutiveLoginDays", 0) + 1;
        PlayerPrefs.SetInt("ConsecutiveLoginDays", consecutiveLoginDays);
        PlayerPrefs.SetString("LastClaimTicks", simulatedTime.Ticks.ToString());
        PlayerPrefs.Save();

        Debug.Log($"[테스트] 하루를 강제로 추가했습니다. 현재 접속 횟수: {consecutiveLoginDays}");
        StartCoroutine(CheckDailyReward()); // 버튼 업데이트
    }

    /// <summary>
    /// [초기화 기능] 모든 보상 기록 및 접속 횟수를 초기화
    /// </summary>
    [ContextMenu("초기화 (보상 & 접속 기록)")]
    public void ResetRewardData()
    {
        PlayerPrefs.DeleteKey("LastClaimTicks");
        PlayerPrefs.DeleteKey("ConsecutiveLoginDays");
        PlayerPrefs.Save();

        // 모든 버튼 비활성화
        foreach (Button btn in rewardButtons)
        {
            btn.interactable = false;
        }

        Debug.Log("[초기화 완료] 보상 기록과 접속 횟수가 초기화되었습니다.");
    }
}
