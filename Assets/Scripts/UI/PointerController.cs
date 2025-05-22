using Unity.VisualScripting;
using UnityEngine;

public class PointerController : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private void Awake()
    {
        canvas = transform.root.GetComponentInChildren<Canvas>();
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// �ƶ�ָ�뵽ָ������λ��
    /// </summary>
    /// <param name="index">�ַ�����</param>
    /// <param name="container">������</param>
    /// <param name="charWidth">�ַ����</param>
    /// <param name="spacing">�ַ����</param>
    public void MoveTo(int index, RectTransform container, float charWidth, float spacing)
    {
        float xPos = CalculatePosition(index, container, charWidth, spacing);
        rectTransform.anchoredPosition = new Vector2(xPos, rectTransform.anchoredPosition.y);
    }

    /// <summary>
    /// ����ָ��λ��
    /// </summary>
    /// <param name="index">�ַ�����</param>
    /// <param name="container">������</param>
    /// <param name="charWidth">�ַ����</param>
    /// <param name="spacing">�ַ����</param>
    /// <returns>������X����</returns>
    private float CalculatePosition(int index, RectTransform container, float charWidth, float spacing)
    {
        
        float startOffset = -container.rect.width / 2 + charWidth / 2;
        return (startOffset + index * (charWidth + spacing)) ;
    }
}