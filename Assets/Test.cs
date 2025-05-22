using Unity.VisualScripting;
using UnityEngine;



public class Test : MonoBehaviour
{
    [SerializeField, Tooltip("CharBlockԤ����")] 
    private CharBlock charBlockPrefab;
    [SerializeField, Tooltip("��Ϊ������Ķ�̬����")]
    private RectTransform dynamicContainer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ���Դ���
        // ʵ����Ԥ����
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
