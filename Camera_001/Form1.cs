/* ==============================================================================
 * 功能描述：    调用电脑摄像头，并进行拍摄、保存。
 * 创 建 者：    泰勒Peano
 * 交流邮箱：    goodzheng@88.com
 * 交流QQ：      656029714
 * 创建日期：    2020.09.04
 *.Net Version  3.5
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AForge;
using AForge.Controls;
using AForge.Video;
using AForge.Video.DirectShow;

namespace Camera_001
{
    public partial class Form1 : Form
    {
        #region 变量

        FilterInfoCollection videoDevices;//摄像头设备集合
        VideoCaptureDevice videoSource;//捕获设备源
        Bitmap img;//处理图片

        #endregion

        #region 构造函数
        public Form1()
        {
            InitializeComponent();
        }
        #endregion

        #region 事件
        private void Form1_Load(object sender, EventArgs e)
        {
            //先检测电脑所有的摄像头
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            MessageBox.Show("检测到了" + videoDevices.Count.ToString() + "个摄像头！");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShutCamera();//保证释放摄像头
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ShutCamera();//保证释放摄像头

        }

        private void button1_Click(object sender, EventArgs e)
        {
            img = videoSourcePlayer1.GetCurrentVideoFrame();//拍摄
            pictureBox1.Image = img;
            button2.Enabled = true;//开启“保存”功能
        }

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
        #endregion

        #region 方法
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
        #endregion
    }
}
