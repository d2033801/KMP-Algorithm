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

    [Header("��ʼ��Ӧ����ʾ�Ķ���")]
    public GameObject ControlPanel;
    public GameObject OverlayElements;

    void Start()
    {
        // �Զ��󶨰�ť����¼�
        confirmButton.onClick.AddListener(OnConfirmInput);
    }

    private void OnConfirmInput()
    {
        // ��ȡ��������
        string main = mainStringInput.text;
        string pattern = patternInput.text;

        // ��֤����
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

        // ���ݲ�����KMP������
        kmpController.Initialize(main, pattern);
        PageManager.PM.InitializePages();

        // ����������棨��ѡ��
        gameObject.SetActive(false);
    }
}