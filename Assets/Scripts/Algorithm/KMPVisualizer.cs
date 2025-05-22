using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public class KMPVisualizer : MonoBehaviour
{
    #region UI References
    [Header("UI References")]
    [SerializeField] private RectTransform mainStringContainer; // 主串容器
    [SerializeField] private RectTransform patternContainer;    // 模式串容器
    [SerializeField] private RectTransform nextArrayContainer;  // Next数组容器
    [SerializeField] private TMP_Text mainCodeText;            // 主页面代码文本
    [SerializeField] private TMP_Text nextCodeText;            // Next页面代码文本
    [SerializeField] private GameObject charBlockPrefab;       // 字符块预制体
    [SerializeField] private GameObject pointerPrefab;         // 指针预制体
    [SerializeField] private PageManager pageManager;          // 页面管理器
    [SerializeField] private StatusControl statusController;   // 状态文本控制

    [Header("Next Panel References")]
    [SerializeField] private RectTransform nextPatternContainer;   // Next页面的模式串容器
    [SerializeField] private RectTransform nextNextArrayContainer; // Next页面的Next数组容器
    [SerializeField] private RectTransform nextPrefixSuffixPanel;  // Next页面的前缀数组容器
    #endregion

    #region Algorithm Parameters
    [Header("Algorithm Settings")]
    [SerializeField] private string mainString = "ABABACD";             //主串
    [SerializeField] private string pattern = "ABAC";                   //模式串
    [SerializeField][Range(0.5f, 5f)] private float animationSpeed = 1f;

    [Header("匹配用颜色")]
    [SerializeField] private Color compareColor = Color.yellow;
    [SerializeField] private Color matchColor = Color.green;
    [SerializeField] private Color mismatchColor = Color.red;

    [Header("Next数组专用颜色")]
    [SerializeField] private Color nextDefaultColor = Color.white;
    [SerializeField] private Color nextHighlightColor = new Color(0.2f, 0.8f, 1f);
    #endregion

    #region Runtime Data
    private List<CharBlock> mainChars = new();
    private List<CharBlock> patternChars = new();
    private List<CharBlock> nextPatternChars = new();
    private List<CharBlock> nextSuffixChars = new();            //后缀字符数组, 即next生成时的主串
    private List<CharBlock> nextArrayBlocks = new();
    private PointerController mainPointer;
    private PointerController patternPointer;   
    private PointerController nextSuffixPointer;
    private PointerController nextPatternPointer;
    private int[] nextArray;
    private Coroutine algorithmCoroutine;
    private readonly Stack<AlgorithmState> history = new();
    private bool isPlaying = false;
    private bool isStepping = false;
    private bool isSucceed = false;               //是否成功匹配
    private bool isPaused = false;
    private bool triggerStepForward = false;
    private int currentMainIndex = 0;
    private int currentPatternIndex = 0;

    public float AnimationSpeed { get => animationSpeed; set => animationSpeed = value; }
    public bool IsPlaying { get => isPlaying; set => isPlaying = value; }
    public int HistoryCount => history.Count;

    public bool IsPaused { get => isPaused; }
    public string MainString { get => mainString; set => mainString = value; }
    public string Pattern { get => pattern; set => pattern = value; }
    #endregion

    #region Unity Callbacks
    void Start()
    {
        //StartCoroutine(InitializeAfterFrame());
        
    }
    public void Initialize(string main, string pattern)
    {
        this.mainString = main;
        this.pattern = pattern;

        // 调用你的主算法逻辑
        StartCoroutine(InitializeAfterFrame());
    }

    private IEnumerator InitializeAfterFrame()
    {
        yield return null; // 等待一帧再初始化，防止ui元素未刷新
        InitializeUIElements();
        CreateMainPagePointers();
    }
    #endregion

    #region Core Functionality
    private void InitializeUIElements()
    {
        nextArrayContainer = pageManager.GetNextArrayContainer(PageManager.PageType.Main);
        // 初始化主算法代码
        mainCodeText.text = @"
int i = pos, j = 1;
while (i <= S[0] && j <= T[0]) {
    if (j == 0 || S[i] == T[j]) {
        ++i; ++j;
    } else {
        j = next[j];
    }
}
if (j > T[0]) return i - T[0];
else return 0;
";

        //        // 初始化Next数组生成代码
        //        nextCodeText.text = @"
        //next[1] = 0;
        //int i = 1, j = 0;
        //while (i < T[0]) {
        //    if (j == 0 || T[i] == T[j]) {
        //        ++i; ++j; next[i] = j;
        //    } else {
        //        j = next[j];
        //    }
        //}
        //";

        // 创建字符块
        CreateCharBlocks(mainString, mainStringContainer, mainChars);
        CreateCharBlocks(pattern, patternContainer, patternChars);

        // 初始化Next数组显示
        InitializeNextArrayDisplay(nextArrayContainer, nextArrayBlocks);
    }

    /// <summary>
    /// 创建字符块
    /// </summary>
    /// <param name="text">字符串</param>
    /// <param name="container">字符串容器</param>
    /// <param name="targetList">要生成到哪个字符列表</param>

    private void CreateCharBlocks(string text, RectTransform container, List<CharBlock> targetList)
    {
        foreach (char c in text)
        {
            GameObject obj = Instantiate(charBlockPrefab, container);
            CharBlock block = obj.GetComponent<CharBlock>();
            block.Initialize(c.ToString());
            targetList.Add(block);
        }
    }

    /// <summary>
    /// 初始化Next数组显示
    /// </summary>
    private void InitializeNextArrayDisplay(RectTransform container, List<CharBlock> targetList)
    {
        for (int i = 0; i < pattern.Length; i++)
        {
            GameObject obj = Instantiate(charBlockPrefab, container);
            CharBlock block = obj.GetComponent<CharBlock>();
            block.Initialize("0"); // 初始化为默认值 "0"
            targetList.Add(block);
        }
    }
    
    /// <summary>
    /// 将next数组显示出来
    /// </summary>
    /// <param name="container">捆绑的父结点</param>
    /// <param name="targetList">要展示的next数组</param>
    private void ShowNextArrayDisplay(RectTransform container, List<CharBlock> targetList)
    {
        for (int i = 0; i < pattern.Length; i++)
        {
            GameObject obj = Instantiate(targetList[i].gameObject, container);

        }
    }
    /// <summary>
    /// 创建指针
    /// </summary>
    private void CreateMainPagePointers()
    {
        mainPointer = CreatePointer(mainStringContainer, pointerPrefab);
        patternPointer = CreatePointer(patternContainer, pointerPrefab);
    }

    /// <summary>
    /// 创建并初始化指针
    /// </summary>
    /// <param name="container">目标容器</param>
    /// <param name="prefab">指针预制体</param>
    /// <param name="initialIndex">初始索引</param>
    /// <returns>创建好的指针控制器</returns>
    private PointerController CreatePointer(RectTransform container, GameObject prefab, int initialIndex = 0)
    {
        // 强制更新目标容器布局
        LayoutRebuilder.ForceRebuildLayoutImmediate(container);

        // 实例化指针并获取控制器
        PointerController pointer = Instantiate(prefab, container).GetComponent<PointerController>();

        // 初始化指针位置
        UpdatePointerPositions(initialIndex, pointer, container);

        return pointer;
    }

    /// <summary>
    /// 清理容器内容
    /// </summary>
    private void ClearContainer(RectTransform container, List<CharBlock> targetList)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
        targetList.Clear();
    }
    #endregion

    #region Algorithm Implementation
    public void StartVisualization()
    {
        if (algorithmCoroutine != null) StopCoroutine(algorithmCoroutine);
        algorithmCoroutine = StartCoroutine(FullKMPProcess());
    }

    public void SetAnimationSpeed(float speed)
    {
        animationSpeed = Mathf.Clamp(speed, 0.5f, 5f);
    }

    public void ResetAlgorithm()
    {
        StopAllCoroutines();
        history.Clear();

        // 清理主页面的内容
        ClearContainer(pageManager.GetPatternContainer(PageManager.PageType.Main), patternChars);
        ClearContainer(pageManager.GetNextArrayContainer(PageManager.PageType.Main), nextArrayBlocks);

        // 清理NextPanel的内容
        ClearContainer(pageManager.GetPatternContainer(PageManager.PageType.Next), patternChars);
        ClearContainer(pageManager.GetNextArrayContainer(PageManager.PageType.Next), nextArrayBlocks);

        InitializeUIElements();
        UpdatePointerPositions(0, mainPointer, mainStringContainer);
        UpdatePointerPositions(0, patternPointer, patternContainer);
    }

    private IEnumerator FullKMPProcess()
    {
        ClearContainer(pageManager.GetNextArrayContainer(PageManager.PageType.Main), nextArrayBlocks);
        pageManager.SwitchPage(PageManager.PageType.Next, true);

        //等待一帧, 使得canvas组件加载完成
        yield return null;
        // 动态生成 NextPanel 的内容, 分别为"next主串"、"next模式串"、next值数组、生成两个指针
        CreateCharBlocks(pattern, nextPrefixSuffixPanel, nextSuffixChars);      
        CreateCharBlocks(pattern, pageManager.GetPatternContainer(PageManager.PageType.Next), nextPatternChars);
        InitializeNextArrayDisplay(pageManager.GetNextArrayContainer(PageManager.PageType.Next), nextArrayBlocks);

        nextPatternPointer = CreatePointer(nextPatternContainer, pointerPrefab);
        nextSuffixPointer = CreatePointer(nextPrefixSuffixPanel, pointerPrefab);
        yield return StartCoroutine(GenerateNextArray());


        pageManager.SwitchPage(PageManager.PageType.Main, true);

        // 动态生成主页面的内容
        //  CreateCharBlocks(mainString, pageManager.GetPatternContainer(PageManager.PageType.Main), mainChars);
        ShowNextArrayDisplay(pageManager.GetNextArrayContainer(PageManager.PageType.Main), nextArrayBlocks);

        yield return StartCoroutine(RunMainAlgorithm());
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        if (isSucceed)
        {
            statusController.UpdateStatusText("<color=green>"+"Success!"+"</color>");
        }
        else
        {
            statusController.UpdateStatusText("<color=red>Fail!</color>");
        }
        RestartAlgorithm.Instance.ShowButton();
    }
    private IEnumerator GenerateNextArray()
    {

        nextArray = new int[pattern.Length + 1]; // 从1开始索引
        nextArray[1] = 0;
        UpdateCodeHighlight(0, true);           // next[1] = 0;
        yield return AnimateStep();
        int i = 1, j = 0;
        UpdatePointerPositions(i - 1, nextSuffixPointer, nextPrefixSuffixPanel);
        UpdatePointerPositions(j - 1, nextPatternPointer, nextPatternContainer);
        UpdateCodeHighlight(1, true);           // int i = 1, j = 0;
        yield return AnimateStep();
        UpdateCodeHighlight(2, true);           // while (i < T[0]) {
        yield return AnimateStep();
        while (i < pattern.Length)
        {
          
            if (j != 0)
            {
                CompareNextCharacters(j - 1, i - 1);
                HighlightPrefixSuffix(i - 1, j - 1); // 高亮前后缀
            }
            else
            {
                ResetNextColors();
            }


            SaveState();

            UpdateCodeHighlight(3, true);       // if (j == 0 || T[i] == T[j]) 
            //  SetMatchState(i, j);
            yield return AnimateStep();
            if (j == 0 || pattern[i - 1] == pattern[j - 1])
            {
                UpdateCodeHighlight(4, true);   // ++i; ++j; next[i] = j;
                i++;
                j++;
                nextArray[i] = j; // 更新 next 数组
                ResetNextColors();
                HighlightPrefixSuffix(i - 1, j - 1); // 高亮前后缀
                UpdateNextArrayDisplay(i, nextArray[i], PageManager.PageType.Next);     //更新next数组
            }
            else
            {
                UpdateCodeHighlight(6, true);   // j = next[j];
                j = nextArray[j]; // 回溯
                ResetNextColors();
                HighlightPrefixSuffix(i - 1, j - 1); // 高亮前后缀
            }
            UpdatePointerPositions(i - 1 , nextSuffixPointer, nextPrefixSuffixPanel);
            UpdatePointerPositions(j - 1 , nextPatternPointer, nextPatternContainer);
            Debug.Log($"Step {i}: next[{i}] = {nextArray[i]}");

            yield return AnimateStep();
            UpdateCodeHighlight(2, true);
            yield return AnimateStep();
        }
        // 重置next数组的颜色，防止继续出现compare颜色
        ResetNextColors();
        HighlightPrefixSuffix(i - 1, j - 1); // 高亮前后缀
        yield return AnimateStep();
    }

    /// <summary>
    /// 更新next数组显示
    /// </summary>
    /// <param name="index">正在匹配第几个数字</param>
    /// <param name="value">next数值</param>
    /// <param name="pageType">主匹配页面或是next页面</param>
    private void UpdateNextArrayDisplay(int index, int value, PageManager.PageType pageType)
    {
        RectTransform nextArrayContainer = pageManager.GetNextArrayContainer(pageType);

        if (index < 1 || index > nextArrayBlocks.Count || nextArrayContainer == null) return;

        nextArrayBlocks[index - 1].SetValue(value.ToString());
        StartCoroutine(AnimateNextIndexHighlight(index - 1));
    }

    /// <summary>
    /// 闪两下高亮提示next数组更新
    /// </summary>
    /// <param name="index">更新的next值是第几个</param>
    /// <returns></returns>
    private IEnumerator AnimateNextIndexHighlight(int index)
    {
        Color originalColor = nextArrayBlocks[index].CurrentColor;

        for (int i = 0; i < 2; i++)
        {
            nextArrayBlocks[index].SetColor(nextHighlightColor);
            yield return new WaitForSeconds(0.2f / animationSpeed);
            nextArrayBlocks[index].SetColor(originalColor);
            yield return new WaitForSeconds(0.2f / animationSpeed);
        }
    }

    private IEnumerator RunMainAlgorithm()
    {
        ResetColors();
        UpdateCodeHighlight(0, false);
        int i = 1, j = 1;
        SaveState();
        yield return AnimateStep();
       
        UpdateCodeHighlight(1, false);
        SaveState();
        yield return AnimateStep();

        while (i <= mainString.Length && j <= pattern.Length)
        {

            SaveState();

            UpdateCodeHighlight(2, false);          //if (j == 0 || S[i] == T[j]) {
            if (j == 0)             //j==0时, j和i都指向下一个元素
            {
                ResetColors();
                i++;
                j++;
                UpdatePointerPositions(i - 1, mainPointer, mainStringContainer);
                UpdatePointerPositions(j - 1, patternPointer, patternContainer);
                UpdateCodeHighlight(3, false);      // ++i; ++j;
                SaveState();
                yield return AnimateStep();
                if (i > mainString.Length || j > pattern.Length)
                {
                    break;
                }
            }
            UpdateCodeHighlight(2, false);
            CompareCharacters(i, j);
            SaveState();
            yield return AnimateStep();
            if (mainString[i - 1] == pattern[j - 1])
            {
                UpdateCodeHighlight(3, false);        // ++i; ++j;
                SetMatchState(i, j);
                i++;
                j++;
            }
            else
            {
                UpdateCodeHighlight(5, false);         // j = next[j];
                SetMismatchState(i, j);
                j = nextArray[j];
            }

            UpdatePointerPositions(i - 1, mainPointer, mainStringContainer);
            UpdatePointerPositions(j - 1, patternPointer, patternContainer);
            SaveState();
            yield return AnimateStep();
        }
        if (j > pattern.Length)
        {
            UpdateCodeHighlight(8, false);              //if (j > T[0]) return i - T[0];
            isSucceed = true;
        }
        else
        {
            UpdateCodeHighlight(9, false);             //else return 0;
            isSucceed = false;
        }
    }
    #endregion

    #region State Management
    private struct AlgorithmState
    {
        public PageManager.PageType pageType;
        public int mainIndex;
        public int patternIndex;
        public int[] nextArraySnapshot;
        public List<Color> mainColors;
        public List<Color> patternColors;
        public string mainCode;
        public string nextCode;
    }

    private void SaveState()
    {
        AlgorithmState state = new()
        {
            pageType = pageManager.CurrentPage,
            mainIndex = currentMainIndex,
            patternIndex = currentPatternIndex,
            nextArraySnapshot = (int[])nextArray.Clone(),
            mainColors = mainChars.ConvertAll(c => c.CurrentColor),
            patternColors = patternChars.ConvertAll(c => c.CurrentColor),
            mainCode = mainCodeText.text,
            nextCode = nextCodeText.text
        };
        history.Push(state);
    }

    private IEnumerator RestoreState(AlgorithmState state)
    {
        if (pageManager.CurrentPage != state.pageType)
        {
            pageManager.SwitchPage(state.pageType, false);
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < mainChars.Count; i++)
            mainChars[i].SetColor(state.mainColors[i]);

        for (int i = 0; i < patternChars.Count; i++)
            patternChars[i].SetColor(state.patternColors[i]);

        nextArray = state.nextArraySnapshot;
        for (int i = 0; i < nextArrayBlocks.Count; i++)
            nextArrayBlocks[i].SetValue(nextArray[i + 1].ToString());

        currentMainIndex = state.mainIndex;
        currentPatternIndex = state.patternIndex;
        UpdatePointerPositions(state.mainIndex, mainPointer,mainStringContainer);
        UpdatePointerPositions(state.patternIndex, patternPointer, patternContainer);

        mainCodeText.text = state.mainCode;
        nextCodeText.text = state.nextCode;

        IsPlaying = false;
    }

    public void StepForward()
    {
        triggerStepForward = true;
        //if (!isPlaying)
        //{
        //    StartCoroutine(SingleStepRoutine());
        //}
    }

    private IEnumerator SingleStepRoutine()
    {
        isPlaying = true;
        yield return AnimateStep();
        isPlaying = false;
    }

    public void StepBack()
    {
        if (history.Count > 0)
        {
            StartCoroutine(RestoreState(history.Pop()));
        }
    }
    #endregion

    #region UI Interaction
    /// <summary>
    /// 更新指针位置
    /// </summary>
    /// <param name="index">该指针的位置索引</param>
    /// <param name="pointerController">指针控制器</param>
    /// <param name="container">该指针的容器</param>
    private void UpdatePointerPositions(int index, PointerController pointerController, RectTransform container)
    {
        if (pointerController != null)
        {

                float charWidth = charBlockPrefab.GetComponent<RectTransform>().rect.width;
                pointerController.MoveTo(index, container, charWidth, 0); // 5为间距补偿值
        }
    }

    /// <summary>
    /// 高亮前后缀
    /// </summary>
    /// <param name="currentIndex">后缀的最后一个字符+1的索引, 即i的值</param>
    /// <param name="nextValue">此时有多少前后缀相同, 即j的值</param>
    private void HighlightPrefixSuffix(int currentIndex, int nextValue)
    {
        // 高亮前缀
        for (int i = 0; i < nextValue; i++)
        {
            nextPatternChars[i].SetColor(nextHighlightColor);
        }

        // 高亮后缀（如果有）
        int suffixStart = currentIndex - nextValue;
        for (int i = suffixStart; i < currentIndex; i++)
        {
             nextSuffixChars[i].SetColor(nextHighlightColor);
        }
    }

    private void UpdateCodeHighlight(int codeLine, bool isNextCode)
    {
        string[] codeLines = isNextCode
            ? new[]
            {
                "next[1] = 0;",
                "int i = 1, j = 0;",
                "while (i < T[0]) {",
                "    if (j == 0 || T[i] == T[j]) {",
                "        ++i; ++j; next[i] = j;",
                "    } else {",
                "        j = next[j];",
                "    }",
                "}"
            }
            : new[]
            {
                "int i = pos, j = 1;",
                "while (i <= S[0] && j <= T[0]) {",
                "    if (j == 0 || S[i] == T[j]) {",
                "        ++i; ++j;",
                "    } else {",
                "        j = next[j];",
                "    }",
                "}",
                "if (j > T[0]) return i - T[0];",
                "else return 0;"
            };

        System.Text.StringBuilder sb = new();
        for (int i = 0; i < codeLines.Length; i++)
        {
            string line = codeLines[i];
            if (i == codeLine)
                sb.AppendLine($"<mark><color=orange>{line}</color></mark>");
            else
                sb.AppendLine(line);
        }

        if (isNextCode)
            nextCodeText.text = sb.ToString();
        else
            mainCodeText.text = sb.ToString();
    }

    private void CompareCharacters(int mainIndex, int patternIndex)
    {
        ResetColors();
        mainChars[mainIndex - 1].SetColor(compareColor);
        patternChars[patternIndex - 1].SetColor(compareColor);
    }  
    private void CompareNextCharacters(int patternIndex, int suffixIndex)
    {
        ResetNextColors();
        nextSuffixChars[suffixIndex].SetColor(compareColor);
        nextPatternChars[patternIndex].SetColor(compareColor);
    }

    private void SetMatchState(int mainIndex, int patternIndex)
    {
        mainChars[mainIndex - 1].SetColor(matchColor);
        patternChars[patternIndex - 1].SetColor(matchColor);
    }

    private void SetMismatchState(int mainIndex, int patternIndex)
    {
        mainChars[mainIndex - 1].SetColor(mismatchColor);
        patternChars[patternIndex - 1].SetColor(mismatchColor);
    }

    private void ResetColors()
    {
        mainChars.ForEach(c => c.ResetColor());
        patternChars.ForEach(c => c.ResetColor());
    }

    private void ResetNextColors()
    {
        nextSuffixChars.ForEach(c => c.ResetColor());
        nextPatternChars.ForEach(c => c.ResetColor());
    }

    private IEnumerator AnimateStep()
    {
        float timer = 0;
        while (timer < 1 / animationSpeed)
        {
            Debug.Log(triggerStepForward);
            // 实现单步逻辑
            if (triggerStepForward == true)
            {
                triggerStepForward = !triggerStepForward;
                break;
            }
               
            //   if (Input.GetMouseButtonDown(0)) break;
            // 实现暂停逻辑
            if (isPaused)
            {
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }
            yield return null;
        }
    }
    #endregion

    #region Public Controls
    public void TogglePlayPause()
    {
        if (isPlaying)
        {
            isPaused = !isPaused; // 切换暂停状态
        }
        else
        {
            isPlaying = true;
            isPaused = false; // 确保从头开始时不处于暂停状态
            algorithmCoroutine = StartCoroutine(FullKMPProcess());
        }
    }

    public void OnSpeedChanged(float value) => animationSpeed = value;
    #endregion
}