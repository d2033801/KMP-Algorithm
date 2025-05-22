using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharBlock : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text text;

    private Color defaultColor;

    public Color CurrentColor => background.color;

    /// <summary>
    /// 初始化字符块
    /// </summary>
    /// <param name="content">字符内容</param>
    public void Initialize(string content)
    {
        text.text = content;
        defaultColor = background.color;
        ResetColor();
    }

    /// <summary>
    /// 设置背景颜色
    /// </summary>
    /// <param name="color">目标颜色</param>
    public void SetColor(Color color)
    {
        background.color = color;
    }

    /// <summary>
    /// 重置为默认颜色
    /// </summary>
    public void ResetColor()
    {
        background.color = defaultColor;
    }

    /// <summary>
    /// 更新字符值
    /// </summary>
    /// <param name="value">新值</param>
    public void SetValue(string value)
    {
        text.text = value;
    }
}