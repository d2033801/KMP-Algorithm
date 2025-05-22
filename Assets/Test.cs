using Unity.VisualScripting;
using UnityEngine;



public class Test : MonoBehaviour
{
    [SerializeField, Tooltip("CharBlock预制体")] 
    private CharBlock charBlockPrefab;
    [SerializeField, Tooltip("作为父对象的动态容器")]
    private RectTransform dynamicContainer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 测试代码
        // 实例化预制体
        CharBlock block = Instantiate(charBlockPrefab, dynamicContainer);
        block.Initialize("A");
        block.SetColor(Color.red);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSliderChanged(float value)
    {
        Debug.Log(value);
    }
}
