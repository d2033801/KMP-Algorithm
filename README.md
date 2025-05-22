# KMP-Algorithm
使用Unity3D与生成式ai辅助制作的半成品KMP算法演示程序 / A work-in-progress KMP algorithm demonstration program developed using Unity3D with generative AI-assisted development

可以用来辅助学习，或是检验自己求得的next值是否正确。

安卓端的ui可能比较小，这是我最开始未考虑周全导致的。

**注意**：没有设置退出逻辑，请直接点右上角关闭键/移动端直接用home键来关闭
项目所用Unity版本: 6000.0.33f1
# 下载
请点击[这里](https://github.com/d2033801/KMP-Algorithm/releases)进行下载，包含安卓和Windows两个版本。
# 效果图
*求next数组*
![图片](https://github.com/user-attachments/assets/034f0ce6-fa07-4b0e-8764-c15d3eb7bdbd)

*主匹配演示*
![图片](https://github.com/user-attachments/assets/91212d26-204d-44a8-afae-2f865a828865)
# 使用方法
在**主界面**输入主串与模式串
![图片](https://github.com/user-attachments/assets/60b0dd0f-2e0c-4b58-a5e6-8f9347639b60)

进入如下界面后，点击*Play*键开始匹配。注意: *Back*功能未实现，是假按钮
![图片](https://github.com/user-attachments/assets/ccc5fad2-3f5e-4c44-8887-ffd6872bac69)

首先会进入**求next数组**的界面, 在暂停时可以直接点击*Next Step*按钮来单步执行, *Speed*滑条可拖动调整演示速度
**next数组**求完后会返回到**主匹配界面**进行匹配
![图片](https://github.com/user-attachments/assets/8f2f131c-9cec-4e8b-9896-2efa4f9c3ae3)

匹配成功后会显示**Success!**, 失败则会显示**Fail!**。点击*Reset*按钮可以重置。匹配结束后左下角的按钮正常应该被禁止使用，但未按照预期做出，可以点但是不会有反应。
![图片](https://github.com/user-attachments/assets/e80e7477-b288-4660-8dfc-1a3f8874c3d4)
