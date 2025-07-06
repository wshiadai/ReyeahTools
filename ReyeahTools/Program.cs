using ReyeahTools.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReyeahTools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("睿烨相机Demo程序启动...");
            using (var camera = new CameraController())
            {
                try
                {
                    Console.WriteLine("开始拍照...");

                    // 初始化相机
                    camera.Initialize();

                    // 启动实时取景
                    camera.StartLiveView();

                    // 执行自动对焦
                    camera.AutoFocus();

                    // 等待2秒
                    Thread.Sleep(2000);

                    // 拍摄照片
                    camera.TakePicture();

                    // 停止实时取景
                    camera.StopLiveView();
                    Console.WriteLine("拍照完成！");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"拍照出现错误: {ex.Message}");
                }
            }
        }
    }
}
