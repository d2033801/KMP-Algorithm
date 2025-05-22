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
    /// 移动指针到指定索引位置
    /// </summary>
    /// <param name="index">字符索引</param>
    /// <param name="container">父容器</param>
    /// <param name="charWidth">字符宽度</param>
    /// <param name="spacing">字符间距</param>
    public void MoveTo(int index, RectTransform container, float charWidth, float spacing)
    {
        float xPos = CalculatePosition(index, container, charWidth, spacing);
        rectTransform.anchoredPosition = new Vector2(xPos, rectTransform.anchoredPosition.y);
    }

    /// <summary>
    /// 计算指针位置
    /// </summary>
    /// <param name="index">字符索引</param>
    /// <param name="container">父容器</param>
    /// <param name="charWidth">字符宽度</param>
    /// <param name="spacing">字符间距</param>
    /// <returns>计算后的X坐标</returns>
    private float CalculatePosition(int index, RectTransform container, float charWidth, float spacing)
    {
        
        float startOffset = -container.rect.width / 2 + charWidth / 2;
        return (startOffset + index * (charWidth + spacing)) ;
    }
}