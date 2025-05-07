using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Profile : UiWindow
{
    [Space(10)][Header("Profile Buttons")] 
    [SerializeField] private Button profileBtn;
    [SerializeField] private Image profileBg;
    [SerializeField] private Image profileInnerBorder;
    [SerializeField] private Image profileDeco;
    [SerializeField] private GameObject profileGlow;
    [SerializeField] private Button frameBtn;
    [SerializeField] private Image frameBg;
    [SerializeField] private Image frameInnerBorder;
    [SerializeField] private Image frameDeco;
    [SerializeField] private GameObject frameGlow;
    [SerializeField] private Button playerIconBtn;
    [SerializeField] private Image iconBg;
    [SerializeField] private Image iconInnerBorder;
    [SerializeField] private Image iconDeco;
    [SerializeField] private GameObject iconGlow;
    [SerializeField] private TextMeshProUGUI playerNickName;
    [SerializeField] private Button nickChangeBtn;

    [Space(10)] [Header("Profile Info")] 
    [SerializeField] private GameObject profileMiddle;
    [SerializeField] private GameObject profileBottom;
    [SerializeField] private Image frameImage;
    [SerializeField] private Image playerIconImage;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerIdText;
    [SerializeField] private TextMeshProUGUI playerLevelText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private TextMeshProUGUI artifactsCountText;
    
    [Space(10)] [Header("Profile Bottom")]
    [SerializeField] private TextMeshProUGUI stageClearRecordText;
    [SerializeField] private TextMeshProUGUI rankingRecordText;
    [SerializeField] private TextMeshProUGUI killMonsterRecordText;
    [SerializeField] private TextMeshProUGUI victoriesRecordText;

    [Space(10)] [Header("Frame Select")] 
    [SerializeField] private GameObject frameMiddle;
    [SerializeField] private GameObject frameBottom;
    [SerializeField] private Transform frameContents;
    
    [Space(10)] [Header("Frame Bottom")]
    [SerializeField] private Image selectedFrameImage;
    [SerializeField] private TextMeshProUGUI frameDescriptionText;
    [SerializeField] private Button frameChooseBtn;
    
    [Space(10)] [Header("Icon Select")] 
    [SerializeField] private GameObject iconMiddle;
    [SerializeField] private GameObject iconBottom;
    [SerializeField] private Transform iconContents;
    
    [Space(10)] [Header("Icon Bottom")]
    [SerializeField] private Image selectedIconImage;
    [SerializeField] private TextMeshProUGUI iconDescriptionText;
    [SerializeField] private Button iconChooseBtn;

    [Space(10)] [Header("Other Settings")]
    [SerializeField] private FrameData[] frameLists;
    [SerializeField] private FrameData[] iconLists;
    [SerializeField] private Button framePrefab;

    private GameObject _currentMiddle;
    private GameObject _currentBottom;
    
    private Color _baseProfileBgColor;
    private Color _baseProfileInnerBorderColor;
    private Color _baseProfileDecoColor;
    
    private Image _baseProfileImage;
    private Image _baseProfileInnerBorderImage;
    private Image _baseProfileDecoImage;
    
    protected override void Start()
    {
        base.Start();
        
        _baseProfileImage = profileBtn.GetComponent<Image>();
        _baseProfileBgColor = _baseProfileImage.color;
        _baseProfileInnerBorderImage = profileInnerBorder.GetComponent<Image>();
        _baseProfileInnerBorderColor = _baseProfileInnerBorderImage.color;
        _baseProfileDecoImage = profileDeco.GetComponent<Image>();
        _baseProfileDecoColor = _baseProfileDecoImage.color;
        
        profileBtn.onClick.AddListener(() => UpdateProfile());
        frameBtn.onClick.AddListener(() => UpdatePlayerFrame());
        playerIconBtn.onClick.AddListener(() => UpdatePlayerIcon());
        nickChangeBtn.onClick.AddListener(() => UI_MainScene.Instance.OpenNickNameChanger());
        UserInfo.Instance.OnUserInfoUpdated.AddListener(() => UpdateNickName());
        foreach (var data in frameLists)
        {
            Button btn = Instantiate(framePrefab, frameContents);
            btn.transform.GetChild(0).GetComponent<Image>().sprite = data.Sprite;
            btn.onClick.AddListener(() => SelectFrame(data));
        }

        foreach (var data in iconLists)
        {
            Button btn = Instantiate(framePrefab, iconContents);
            btn.transform.GetChild(0).GetComponent<Image>().sprite = data.Sprite;
            btn.onClick.AddListener(() => SelectIcon(data));
        }
        
        frameImage.sprite = 
            Resources.Load<Sprite>($"Sprites/ProfileFrame/{GameManager.Instance.PlayerProfile.profileFrameName}");
        playerIconImage.sprite =
            Resources.Load<Sprite>($"Sprites/ProfileIcon/{GameManager.Instance.PlayerProfile.profileIconName}");
        
        UpdateProfile();
        UpdateNickName();
    }

    public void UpdateNickName()
    {
        playerNickName.text = string.IsNullOrEmpty(UserInfo.Data.nickName) ? UserInfo.Data.gamerId : UserInfo.Data.nickName;
    }

    public void SelectFrame(FrameData frameData)
    {
        selectedFrameImage.sprite = frameData.Sprite;
        frameDescriptionText.text = frameData.Description;
        GameManager.Instance.PlayerProfile.profileFrameName = frameData.Sprite.name;
        frameChooseBtn.onClick.AddListener(() => ChooseFrame());
    }

    public void SelectIcon(FrameData sprite)
    {
        selectedIconImage.sprite = sprite.Sprite;
        iconDescriptionText.text = sprite.Description;
        GameManager.Instance.PlayerProfile.profileIconName = sprite.Sprite.name;
        iconChooseBtn.onClick.AddListener(() => ChooseIcon());
    }

    private void ChooseFrame()
    {
        frameImage.sprite = selectedFrameImage.sprite;
    }
    private void ChooseIcon()
    {
        playerIconImage.sprite = selectedIconImage.sprite;
    }
    
    private void UpdateProfile()
    {
        GameManager gm = GameManager.Instance;

        BtnControl(true, false, false);
        
        CurrentControl(profileMiddle, profileBottom);
        
        
        playerNameText.text = gm.PlayerProfile.playerName;
        playerIdText.text = gm.PlayerProfile.playerID;
        playerLevelText.text = gm.PlayerProfile.playerLevel.ToString();
        expText.text = $"{gm.PlayerProfile.playerExp} / 100";
        expSlider.value = gm.PlayerProfile.playerExp / 100f;
        playerScoreText.text = gm.PlayerProfile.maxScore.ToString();
        artifactsCountText.text = $"{gm.PlayerProfile.currentArtifact.ToString()} / 64";
        
        stageClearRecordText.text = gm.PlayerProfile.totalStageClears.ToString();
        rankingRecordText.text = gm.PlayerProfile.ranking.ToString();
        killMonsterRecordText.text = gm.PlayerProfile.totalKills.ToString();
        victoriesRecordText.text = gm.PlayerProfile.victories.ToString();
    }

    private void UpdatePlayerFrame()
    {
        CurrentControl(frameMiddle, frameBottom);
        BtnControl(false, true, false);
    }

    private void UpdatePlayerIcon()
    {
        CurrentControl(iconMiddle, iconBottom);
        BtnControl(false, false, true);
    }

    private void CurrentControl(GameObject middle, GameObject bottom)
    {
        if(_currentMiddle)
        {
            _currentMiddle.SetActive(false);
            _currentBottom.SetActive(false);
        }
        
        middle.SetActive(true);
        bottom.SetActive(true);
        
        _currentMiddle = middle;
        _currentBottom = bottom;
    }

    private Image _currentBg;
    private Image _currentInnerBorder;
    private Image _currentDeco;
    private GameObject _currentGlow;
    
    private void BtnControl(bool profile, bool frame, bool icon)
    {
        if (_currentBg)
        {
            _currentBg.color = _baseProfileBgColor;
            _currentInnerBorder.color = _baseProfileInnerBorderColor;
            _currentDeco.color = _baseProfileDecoColor;
            _currentGlow.SetActive(false);
        }
        
        if (profile)
        {
            profileBg.color = GameColors.SelectedProfileBackgroundColor;
            profileInnerBorder.color = GameColors.SelectedProfileInnerBorderColor;
            profileDeco.color = GameColors.SelectedProfileDecoColor;
            profileGlow.SetActive(true);
            
            _currentBg = profileBg;
            _currentInnerBorder = profileInnerBorder;
            _currentDeco = profileDeco;
            _currentGlow = profileGlow;
        }

        if (frame)
        {
            frameBg.color = GameColors.SelectedProfileBackgroundColor;
            frameInnerBorder.color = GameColors.SelectedProfileInnerBorderColor;
            frameDeco.color = GameColors.SelectedProfileDecoColor;
            frameGlow.SetActive(true);
            
            _currentBg = frameBg;
            _currentInnerBorder = frameInnerBorder;
            _currentDeco = frameDeco;
            _currentGlow = frameGlow;
        }

        if (icon)
        {
            iconBg.color = GameColors.SelectedProfileBackgroundColor;
            iconInnerBorder.color = GameColors.SelectedProfileInnerBorderColor;
            iconDeco.color = GameColors.SelectedProfileDecoColor;
            iconGlow.SetActive(true);
            
            _currentBg = iconBg;
            _currentInnerBorder = iconInnerBorder;
            _currentDeco = iconDeco;
            _currentGlow = iconGlow;
        }
    }
}
