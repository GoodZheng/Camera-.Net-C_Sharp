C#标准库本身不带有能调用电脑摄像头的库，如果利用windows自身的API来实现的话，着实有些麻烦。Aforge这个第三方库能很好的实现调用、处理的功能。

<span style="color:red;background:;font-size:40px;font-family:微软雅黑;">PS:文末给大家分享了完整的项目源码，各位最好先下载下来，对照着来看</span>

# 1、先看一下效果

![](F:\boke图片\发表\C#调用摄像头\102.gif)

# 2、Aforge介绍

AForge.NET是一个专门为开发者和研究者基于C#框架设计的，这个框架提供了不同的类库和关于类库的资源，还有很多应用程序例子，包括计算机视觉与人工智能，图像处理，神经网络，遗传算法，机器学习，机器人等领域。

# 3、我使用的开发环境

操作系统：       		 win10专业版

.Net framework:  	3.5

IDE:							vs 2019  WinForm

------

<span style="color:blue;background:;font-size:40px;font-family:微软雅黑;">正式开始啦</span>





# 1、新建winform项目

我命名为Camera_001，随意

<img src="F:\boke图片\发表\C#调用摄像头\1.png" style="zoom:50%;" />

# 2、安装AForge

## 1）右击项目名：

![](F:\boke图片\发表\C#调用摄像头\2.png)

## 2）打开“管理NuGet程序包”:

![](F:\boke图片\发表\C#调用摄像头\3.png)

## 3)按下面的操作进行安装：

![](F:\boke图片\发表\C#调用摄像头\4.png)

同理，依次安装下面的几个包：

![](F:\boke图片\发表\C#调用摄像头\5.png)

<font color='orange'>稍微注意一下：这几个包的作者都是AForge.Net</font>

# 3、检查

安装好包之后，会在工具箱发现多了一些控件。vs不愧为宇宙最强IDE！

![](F:\boke图片\发表\C#调用摄像头\6.png)

# 4、开始编程！

#### 1）首先，

在新添加进来的控件中找到<font color='orange'>VideoSourcePlayer控件</font>，拖进窗体中，调整好尺寸。命名我就按默认的吧（videoSourcePlayer1），你们随意！

这个控件的作用是：显示从摄像头中获取的图像

![](F:\boke图片\发表\C#调用摄像头\7.png)

#### 2）添加下面的控件

在comboBox控件中添加两个项：

​					摄像头1

​					摄像头2

两个button控件。把它们的Enabled属性都设置为false，因为一开始没有选择摄像头，不能拍摄更不能保存。

pictureBox用于显示拍摄得到的图片

![](F:\boke图片\发表\C#调用摄像头\8.png)

#### 3）添加using要使用的库

```c#
using AForge;
using AForge.Controls;
using AForge.Video;
using AForge.Video.DirectShow;
```

<font color='cornflowerblue'>PS：下面采用代码+图片的方式</font>

#### 4）声明下面的变量，先不赋值。

```c#
FilterInfoCollection videoDevices;//摄像头设备集合
VideoCaptureDevice videoSource;//捕获设备源
Bitmap img;//处理图片
```

![](F:\boke图片\发表\C#调用摄像头\10.png)

#### 5）在窗体的Load事件中添加下面的代码

viderDevices变量用于保存电脑中所有的摄像设备

```c#
        private void Form1_Load(object sender, EventArgs e)
        {
            //先检测电脑所有的摄像头
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            MessageBox.Show("检测到了" + videoDevices.Count.ToString() + "个摄像头！");
        }
```

![](F:\boke图片\发表\C#调用摄像头\11.png)

#### 6）选择摄像头

双击comboBox控件，在生成的事件中写下面的代码

```c#
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "摄像头1" && videoDevices.Count > 0)
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            else if (comboBox1.Text == "摄像头2" && videoDevices.Count > 1)
                videoSource = new VideoCaptureDevice(videoDevices[1].MonikerString);
            else
            {
                MessageBox.Show("选择的摄像头不存在！！！");
                return;
            }
            videoSourcePlayer1.VideoSource = videoSource;
            videoSourcePlayer1.Start();

            button1.Enabled = true;//开启“拍摄功能”
        }
```

<font color='orange'>这个时候可以运行一下噢！！！！</font>看一下效果。

#### 7）关闭并释放摄像头

到这，会有两个问题：

1. 关闭窗口时程序不会停止。这是因为你选择的摄像头并未关闭释放
2. 切换到一个不存在的摄像头时，之前的摄像头依然在使用。

所以，在代码的下面编写一个方法（函数）：

```c#
		// 关闭并释放摄像头
        public void ShutCamera()
        {
            if (videoSourcePlayer1.VideoSource != null)
            {
                videoSourcePlayer1.SignalToStop();
                videoSourcePlayer1.WaitForStop();
                videoSourcePlayer1.VideoSource = null;
            }
        }
```

在窗体的Formclosing事件中调用一次，在comboBox的事件中调用一次

![](F:\boke图片\发表\C#调用摄像头\13.png)

![](F:\boke图片\发表\C#调用摄像头\14.png)

#### 8）开始拍摄

双击button1拍摄按钮，加入下面的代码：

```c#
        private void button1_Click(object sender, EventArgs e)
        {
            img = videoSourcePlayer1.GetCurrentVideoFrame();//拍摄
            pictureBox1.Image = img;
            button2.Enabled = true;//开启“保存”功能
        }
```

运行一下，发现拍摄之后，图片显示的是什么鬼！

不要慌，问题不大。把pictureBox控件的SizeMode属性设置为Zoom，<font color='red'>很重要！！！</font>再运行一下看看。

#### 9）保存

双击button2保存按钮，加入下面的代码：

```c#
//"保存"按钮click事件
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //以当前时间为文件名，保存为jpg格式
                //图片路径在程序bin目录下的Debug下
                TimeSpan tss = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                long a = Convert.ToInt64(tss.TotalMilliseconds) / 1000;  //以秒为单位
                img.Save(string.Format("{0}.jpg", a.ToString()));
                MessageBox.Show("保存成功！");
                button2.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
```

# 大功告成！！！

其实远没有结束，AForge是一个很强大的库，能实现很多功能。比如选择不同的分辨率，实现录像功能等等等等。剩下的就交给各位慢慢探索吧！！！

[附：完整的项目源码](链接：https://pan.baidu.com/s/1mMsdiFNs6HbvPkFgHyQEog 
提取码：kss2)

