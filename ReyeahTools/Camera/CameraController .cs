using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReyeahTools.Camera
{
    public class CameraController:IDisposable
    {
        // EDSDK 常量定义
        private const uint kEdsPropID_Evf_OutputDevice = 0x503;
        private const uint kEdsPropID_Evf_Mode = 0x506;
        private const uint kEdsPropID_Evf_AFMode = 0x507;
        private const uint kEdsEvfOutputDevice_PC = 0x02;
        private const uint kEdsEvfOutputDevice_TFT = 0x01;
        private const uint kEdsCameraCommand_TakePicture = 0x00;
        private const uint kEdsCameraCommand_PressShutterButton = 0x04;
        private const uint kEdsCameraCommand_EvfAfOn = 0x511;
        private const uint kEdsShutterButton_Completely = 0x03;
        private const uint kEdsShutterButton_OFF = 0x00;

        // 相机对象
        private IntPtr _cameraRef;
        private bool _isLiveViewActive = false;
        private bool _isDisposed = false;

        /// <summary>
        /// 析构方法
        /// </summary>
        ~CameraController()
        {
            Dispose();
        }

        /// <summary>
        /// 初始化相机连接
        /// </summary>
        public void Initialize()
        {
            // 初始化SDK
            var err = EDSDK.EdsInitializeSDK();
            if (err != EDSDK.EDS_ERR_OK)
            {
                throw new Exception($"Failed to initialize EDSDK. Error code: {err}");
            }

            // 获取相机列表
            err = EDSDK.EdsGetCameraList(out IntPtr cameraList);
            if (err != EDSDK.EDS_ERR_OK)
            {
                throw new Exception($"Failed to get camera list. Error code: {err}");
            }

            try
            {
                // 获取相机数量
                err = EDSDK.EdsGetChildCount(cameraList, out int count);
                if (err != EDSDK.EDS_ERR_OK || count == 0)
                {
                    throw new Exception("No camera detected.");
                }

                // 获取第一台相机
                err = EDSDK.EdsGetChildAtIndex(cameraList, 0, out _cameraRef);
                if (err != EDSDK.EDS_ERR_OK)
                {
                    throw new Exception($"Failed to get camera. Error code: {err}");
                }

                // 打开相机会话
                err = EDSDK.EdsOpenSession(_cameraRef);
                if (err != EDSDK.EDS_ERR_OK)
                {
                    throw new Exception($"Failed to open camera session. Error code: {err}");
                }
            }
            finally
            {
                // 释放相机列表
                if (cameraList != IntPtr.Zero)
                {
                    EDSDK.EdsRelease(cameraList);
                }
            }
        }

        /// <summary>
        /// 启动实时取景
        /// </summary>
        public void StartLiveView()
        {
            if (_isLiveViewActive) return;

            // 设置输出设备为PC
            uint device = kEdsEvfOutputDevice_PC;
            var err = EDSDK.EdsSetPropertyData(_cameraRef, kEdsPropID_Evf_OutputDevice, 0, sizeof(uint), ref device);
            if (err != EDSDK.EDS_ERR_OK)
            {
                throw new Exception($"Failed to set live view output device. Error code: {err}");
            }

            _isLiveViewActive = true;
            Console.WriteLine("Live view started.");
        }

        /// <summary>
        /// 停止实时取景
        /// </summary>
        public void StopLiveView()
        {
            if (!_isLiveViewActive) return;

            // 关闭输出设备
            uint device = 0;
            var err = EDSDK.EdsSetPropertyData(_cameraRef, kEdsPropID_Evf_OutputDevice, 0, sizeof(uint), ref device);
            if (err != EDSDK.EDS_ERR_OK)
            {
                throw new Exception($"Failed to stop live view. Error code: {err}");
            }

            _isLiveViewActive = false;
            Console.WriteLine("Live view stopped.");
        }

        /// <summary>
        /// 执行自动对焦
        /// </summary>
        public void AutoFocus()
        {
            if (!_isLiveViewActive)
            {
                throw new InvalidOperationException("Live view must be active for auto focus.");
            }

            // 发送自动对焦命令
            var err = EDSDK.EdsSendCommand(_cameraRef, kEdsCameraCommand_EvfAfOn, 0);
            if (err != EDSDK.EDS_ERR_OK)
            {
                throw new Exception($"Failed to perform auto focus. Error code: {err}");
            }

            Console.WriteLine("Auto focus performed.");
        }

        /// <summary>
        /// 拍摄照片
        /// </summary>
        public void TakePicture()
        {
            // 半按快门对焦
            uint shutterButton = kEdsShutterButton_Completely;
            var err = EDSDK.EdsSendCommand(_cameraRef, kEdsCameraCommand_PressShutterButton, (int)shutterButton);
            if (err != EDSDK.EDS_ERR_OK)
            {
                throw new Exception($"Failed to press shutter button. Error code: {err}");
            }

            // 等待对焦完成
            Thread.Sleep(500);

            // 完全按下快门拍照
            err = EDSDK.EdsSendCommand(_cameraRef, kEdsCameraCommand_TakePicture, 0);
            if (err != EDSDK.EDS_ERR_OK)
            {
                throw new Exception($"Failed to take picture. Error code: {err}");
            }

            // 释放快门按钮
            shutterButton = kEdsShutterButton_OFF;
            err = EDSDK.EdsSendCommand(_cameraRef, kEdsCameraCommand_PressShutterButton, (int)shutterButton);
            if (err != EDSDK.EDS_ERR_OK)
            {
                throw new Exception($"Failed to release shutter button. Error code: {err}");
            }

            Console.WriteLine("Picture taken.");
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            try
            {
                // 停止实时取景
                if (_isLiveViewActive)
                {
                    StopLiveView();
                }

                // 关闭相机会话
                if (_cameraRef != IntPtr.Zero)
                {
                    EDSDK.EdsCloseSession(_cameraRef);
                    EDSDK.EdsRelease(_cameraRef);
                    _cameraRef = IntPtr.Zero;
                }

                // 终止SDK
                EDSDK.EdsTerminateSDK();
            }
            finally
            {
                _isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

       
    }
}
