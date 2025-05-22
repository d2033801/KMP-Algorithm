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
    /// ��ʼ���ַ���
    /// </summary>
    /// <param name="content">�ַ�����</param>
    public void Initialize(string content)
    {
        text.text = content;
        defaultColor = background.color;
        ResetColor();
    }

    /// <summary>
    /// ���ñ�����ɫ
    /// </summary>
    /// <param name="color">Ŀ����ɫ</param>
    public void SetColor(Color color)
    {
        background.color = color;
    }

    /// <summary>
    /// ����ΪĬ����ɫ
    /// </summary>
    public void ResetColor()
    {
        background.color = defaultColor;
    }

    /// <summary>
    /// �����ַ�ֵ
    /// </summary>
    /// <param name="value">��ֵ</param>
    public void SetValue(string value)
    {
        text.text = value;
    }
}