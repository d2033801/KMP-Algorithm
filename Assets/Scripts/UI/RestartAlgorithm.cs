using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartAlgorithm : MonoBehaviour
{
    public static RestartAlgorithm Instance { get; private set; }

    [SerializeField] private Button _restartButton; // ��Ϊ���л��ֶΣ���ѡ�ֶ���ק

    private void Awake()
    {
        // ������ʼ��
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


        // ��ȫ�Լ��
        if (_restartButton == null)
        {
            Debug.LogError("RestartButton not found!");
            return;
        }

        // ǿ�Ƴ�ʼ����ť״̬
        _restartButton.gameObject.SetActive(false);
    }

    public void ShowButton() => _restartButton?.gameObject.SetActive(true);
    public void HideButton() => _restartButton?.gameObject.SetActive(false);

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}