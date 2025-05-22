using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartAlgorithm : MonoBehaviour
{
    public static RestartAlgorithm Instance { get; private set; }

    [SerializeField] private Button _restartButton; // 改为序列化字段，可选手动拖拽

    private void Awake()
    {
        // 单例初始化
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


        // 安全性检查
        if (_restartButton == null)
        {
            Debug.LogError("RestartButton not found!");
            return;
        }

        // 强制初始化按钮状态
        _restartButton.gameObject.SetActive(false);
    }

    public void ShowButton() => _restartButton?.gameObject.SetActive(true);
    public void HideButton() => _restartButton?.gameObject.SetActive(false);

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}