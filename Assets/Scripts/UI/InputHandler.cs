using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField mainStringInput;
    public TMP_InputField patternInput;
    public Button confirmButton;

    [Header("KMP Controller")]
    public KMPVisualizer kmpController;

    [Header("初始化应该显示的东西")]
    public GameObject ControlPanel;
    public GameObject OverlayElements;

    void Start()
    {
        // 自动绑定按钮点击事件
        confirmButton.onClick.AddListener(OnConfirmInput);
    }

    private void OnConfirmInput()
    {
        // 获取输入内容
        string main = mainStringInput.text;
        string pattern = patternInput.text;

        // 验证输入
        if (string.IsNullOrEmpty(main))
        {
            Debug.LogError("Main string cannot be empty!");
            return;
        }

        if (string.IsNullOrEmpty(pattern))
        {
            Debug.LogError("Pattern cannot be empty!");
            return;
        }
        ControlPanel.SetActive(true);
        OverlayElements.SetActive(true);

        // 传递参数到KMP控制器
        kmpController.Initialize(main, pattern);
        PageManager.PM.InitializePages();

        // 禁用输入界面（可选）
        gameObject.SetActive(false);
    }
}