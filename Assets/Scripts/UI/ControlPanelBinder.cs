using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlPanelBinder : MonoBehaviour
{
    #region UI References
    [Header("UI References")]
    [SerializeField] private KMPVisualizer kmpVisualizer;         // 核心算法可视化器
    [SerializeField] private Button playPauseBtn;                 // 播放/暂停按钮
    [SerializeField] private Button stepForwardBtn;               // 单步前进按钮
    [SerializeField] private Button stepBackBtn;                  // 单步后退按钮
    [SerializeField] private Slider speedSlider;                  // 动画速度滑动条
    [SerializeField] private TMP_Text speedValueText;             // 速度显示文本
    [SerializeField] private Slider progressSlider;               // 进度滑动条
    [SerializeField] private StatusControl statusController;      // 状态文本控制
    #endregion

    #region Unity Callbacks
    void Start()
    {
        InitializeUI();
        BindEvents();
        
    }
    #endregion

    #region Initialization
    private void InitializeUI()
    {
        // 初始化速度滑动条
        speedSlider.minValue = 0.5f;
        speedSlider.maxValue = 5f;
        speedSlider.value = kmpVisualizer.AnimationSpeed;
        UpdateSpeedDisplay(kmpVisualizer.AnimationSpeed);

        // 初始化进度滑动条
        progressSlider.interactable = false; // 默认不可交互

        // 初始化状态文本
        UpdateStatusText("Ready");
    }

    private void BindEvents()
    {
        // 绑定按钮点击事件
        playPauseBtn.onClick.AddListener(OnPlayPauseClicked);
        stepForwardBtn.onClick.AddListener(OnStepForwardClicked);
        stepBackBtn.onClick.AddListener(OnStepBackClicked);

        // 绑定速度滑动条事件
        speedSlider.onValueChanged.AddListener(OnSpeedSliderChanged);
    }
    #endregion

    #region Event HandlersOnPlayPauseClicked
    public void OnPlayPauseClicked()
    {
        kmpVisualizer.TogglePlayPause();
        UpdateButtonStates(); // 更新按钮状态
        Debug.Log("Clicked");

        // 更新状态文本
        if (kmpVisualizer.IsPlaying && !kmpVisualizer.IsPaused)
        {
            UpdateStatusText("Playing");
        }
        else
        {
            UpdateStatusText("Paused");
        }
    }
    public void OnStepForwardClicked()
    {
        if (kmpVisualizer.IsPaused)
        {
            kmpVisualizer.StepForward();
            UpdateButtonStates();
            UpdateStatusText("Stepped Forward");
        }
    }

    public void OnStepBackClicked()
    {
        if (kmpVisualizer.HistoryCount > 0)
        {
            kmpVisualizer.StepBack();
            UpdateButtonStates();
            UpdateStatusText("Stepped Back");
        }
    }

    public void OnSpeedSliderChanged(float value)
    {
        Debug.Log(value);
        kmpVisualizer.SetAnimationSpeed(value);
        UpdateSpeedDisplay(value);
    }

    public void OnResetClicked()
    {
        kmpVisualizer.ResetAlgorithm();
        UpdateButtonStates();
        UpdateStatusText("Reset");
    }
    #endregion

    #region UI Updates
    private void UpdateButtonStates()
    {
        stepBackBtn.interactable = false;//kmpVisualizer.HistoryCount > 0;
        stepForwardBtn.interactable = kmpVisualizer.IsPaused;

        // 更新播放/暂停按钮文本
        if (kmpVisualizer.IsPlaying && !kmpVisualizer.IsPaused)
        {
            playPauseBtn.GetComponentInChildren<TMP_Text>().text = "Pause"; // 正在运行且未暂停
        }
        else
        {
            playPauseBtn.GetComponentInChildren<TMP_Text>().text = "Play"; // 未运行或已暂停
        }
    }

    private void UpdateSpeedDisplay(float value)
    {
        speedValueText.text = $"Speed: {value:F1}x";
    }

    private void UpdateStatusText(string status)
    {
        statusController.UpdateStatusText(status);
    }
    #endregion
}