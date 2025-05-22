using UnityEngine;

public class PageManager : MonoBehaviour
{
    #region Enums and Inspector References
    // ҳ�����Ͷ���
    public enum PageType { Main, Next }

    [Header("Page References")]
    [SerializeField] private RectTransform mainPage; // ��ҳ��
    [SerializeField] private RectTransform nextPage; // Nextҳ��

    [Header("Next Panel References")]
    [SerializeField] private RectTransform nextPatternContainer;   // Nextҳ���ģʽ������
    [SerializeField] private RectTransform nextNextArrayContainer; // Nextҳ���Next��������

    private PageType currentPage;
    private readonly System.Collections.Generic.Stack<PageType> history = new();

    public static PageManager PM;
    #endregion

    #region Properties
    /// <summary>
    /// ��ǰҳ������
    /// </summary>
    public PageType CurrentPage => currentPage;
    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        PM = this;
    }

    void Start()
    {
        //InitializePages();
    }
    #endregion

    #region Core Functionality
    /// <summary>
    /// ��ʼ��ҳ��״̬
    /// </summary>
    public void InitializePages()
    {
        if (mainPage == null || nextPage == null)
        {
            Debug.LogError("ҳ������δ��ȷ���ã�����Inspector�еİ󶨣�");
            return;
        }

        mainPage.gameObject.SetActive(true);
        nextPage.gameObject.SetActive(false);
        currentPage = PageType.Main;
    }

    /// <summary>
    /// �л�ҳ��
    /// </summary>
    /// <param name="target">Ŀ��ҳ��</param>
    /// <param name="recordHistory">�Ƿ��¼��ʷ</param>
    public void SwitchPage(PageType target, bool recordHistory = true)
    {
        if (currentPage == target) return;

        // ��ʷ��¼����
        if (recordHistory && currentPage != target)
        {
            // ���������ظ���¼
            if (history.Count == 0 || history.Peek() != currentPage)
            {
                history.Push(currentPage);
            }
        }

        // ����ҳ��״̬
        UpdatePageActivity(currentPage, false);
        UpdatePageActivity(target, true);

        // ���µ�ǰҳ��
        currentPage = target;
    }

    /// <summary>
    /// ������һҳ
    /// </summary>
    public void GoBack()
    {
        if (history.Count == 0) return;

        // ��ȡ��һҳ���л�
        PageType previousPage = history.Pop();
        SwitchPage(previousPage, false);
    }

    /// <summary>
    /// ��ȡָ��ҳ���ģʽ������
    /// </summary>
    /// <param name="pageType">ҳ������</param>
    /// <returns>��Ӧ��ģʽ������</returns>
    public RectTransform GetPatternContainer(PageType pageType)
    {
        return pageType == PageType.Main ? mainPage.Find("PatternContainer") as RectTransform : nextPatternContainer;
    }

    /// <summary>
    /// ��ȡָ��ҳ���Next��������
    /// </summary>
    /// <param name="pageType">ҳ������</param>
    /// <returns>��Ӧ��Next��������</returns>
    public RectTransform GetNextArrayContainer(PageType pageType)
    {
        if (pageType == PageType.Main)
        {
            return FindDeepChild(mainPage, "NextArrayContainer");
        }
        else
        {
            return nextNextArrayContainer;
        }
    }
    /// <summary>
    /// �������������������ΪtargetName������
    /// </summary>
    /// <param name="parent">�����</param>
    /// <param name="targetName"></param>
    /// <returns></returns>
    public RectTransform FindDeepChild(RectTransform parent, string targetName)
    {
        foreach (RectTransform child in parent)
        {
            if (child.name == targetName)
                return child;

            RectTransform result = FindDeepChild(child, targetName);
            if (result != null)
                return result;
        }
        return null;
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// ����ҳ��ļ���״̬
    /// </summary>
    /// <param name="type">ҳ������</param>
    /// <param name="active">�Ƿ񼤻�</param>
    private void UpdatePageActivity(PageType type, bool active)
    {
        RectTransform pageTransform = GetPageTransform(type);
        if (pageTransform != null)
        {
            pageTransform.gameObject.SetActive(active);
        }
        else
        {
            Debug.LogError($"ҳ������ {type} ������Ϊ�գ�����Inspector�еİ󶨣�");
        }
    }

    /// <summary>
    /// ��ȡָ��ҳ���RectTransform
    /// </summary>
    /// <param name="type">ҳ������</param>
    /// <returns>��Ӧ��RectTransform</returns>
    private RectTransform GetPageTransform(PageType type)
    {
        return type == PageType.Main ? mainPage : nextPage;
    }
    #endregion
}