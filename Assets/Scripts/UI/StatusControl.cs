using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ¿ØÖÆStatusÎÄ±¾
/// </summary>
public class StatusControl : MonoBehaviour
{
    private TMP_Text statusText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        statusText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    public void UpdateStatusText(string status) 
    {
        statusText.text = $"Status: {status}";
    }
}
