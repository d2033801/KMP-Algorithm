using UnityEngine;

public class PageManager : MonoBehaviour
{
    #region Enums and Inspector References
    // 页面类型定义
    public enum PageType { Main, Next }

    [Header("Page References")]
    [SerializeField] private RectTransform mainPage; // 主页面
    [SerializeField] private RectTransform nextPage; // Next页面

    [Header("Next Panel References")]
    [SerializeField] private RectTransform nextPatternContainer;   // Next页面的模式串容器
    [SerializeField] private RectTransform nextNextArrayContainer; // Next页面的Next数组容器

    private PageType currentPage;
    private readonly System.Collections.Generic.Stack<PageType> history = new();

    public static PageManager PM;
    #endregion

    #region Properties
    /// <summary>
    /// 当前页面类型
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
    /// 初始化页面状态
    /// </summary>
    public void InitializePages()
    {
        if (mainPage == null || nextPage == null)
        {
            Debug.LogError("页面引用未正确设置，请检查Inspector中的绑定！");
            return;
        }

        mainPage.gameObject.SetActive(true);
        nextPage.gameObject.SetActive(false);
        currentPage = PageType.Main;
    }

    /// <summary>
    /// 切换页面
    /// </summary>
    /// <param name="target">目标页面</param>
    /// <param name="recordHistory">是否记录历史</param>
    public void SwitchPage(PageType target, bool recordHistory = true)
    {
        if (currentPage == target) return;

        // 历史记录管理
        if (recordHistory && currentPage != target)
        {
            // 避免连续重复记录
            if (history.Count == 0 || history.Peek() != currentPage)
            {
                history.Push(currentPage);
            }
        }

        // 更新页面状态
        UpdatePageActivity(currentPage, false);
        UpdatePageActivity(target, true);

        // 更新当前页面
        currentPage = target;
    }

    /// <summary>
    /// 返回上一页
    /// </summary>
    public void GoBack()
    {
        if (history.Count == 0) return;

        // 获取上一页并切换
        PageType previousPage = history.Pop();
        SwitchPage(previousPage, false);
    }

    /// <summary>
    /// 获取指定页面的模式串容器
    /// </summary>
    /// <param name="pageType">页面类型</param>
    /// <returns>对应的模式串容器</returns>
    public RectTransform GetPatternContainer(PageType pageType)
    {
        return pageType == PageType.Main ? mainPage.Find("PatternContainer") as RectTransform : nextPatternContainer;
    }

    /// <summary>
    /// 获取指定页面的Next数组容器
    /// </summary>
    /// <param name="pageType">页面类型</param>
    /// <returns>对应的Next数组容器</returns>
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
    /// 深度搜索所有子树中名为targetName的物体
    /// </summary>
    /// <param name="parent">父结点</param>
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
    /// 更新页面的激活状态
    /// </summary>
    /// <param name="type">页面类型</param>
    /// <param name="active">是否激活</param>
    private void UpdatePageActivity(PageType type, bool active)
    {
        RectTransform pageTransform = GetPageTransform(type);
        if (pageTransform != null)
        {
            pageTransform.gameObject.SetActive(active);
        }
        else
        {
            Debug.LogError($"页面类型 {type} 的引用为空，请检查Inspector中的绑定！");
        }
    }

    /// <summary>
    /// 获取指定页面的RectTransform
    /// </summary>
    /// <param name="type">页面类型</param>
    /// <returns>对应的RectTransform</returns>
    private RectTransform GetPageTransform(PageType type)
    {
        return type == PageType.Main ? mainPage : nextPage;
    }
    #endregion
}