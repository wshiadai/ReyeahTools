using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ReyeahTools.Camera
{
    public class EDSDK
    {
        public const uint EDS_ERR_OK = 0x00000000;

        [DllImport("EDSDK.dll")]
        public static extern uint EdsInitializeSDK();

        [DllImport("EDSDK.dll")]
        public static extern uint EdsTerminateSDK();

        [DllImport("EDSDK.dll")]
        public static extern uint EdsGetCameraList(out IntPtr outCameraListRef);

        [DllImport("EDSDK.dll")]
        public static extern uint EdsGetChildCount(IntPtr inRef, out int outCount);

        [DllImport("EDSDK.dll")]
        public static extern uint EdsGetChildAtIndex(IntPtr inRef, int inIndex, out IntPtr outCameraRef);

        [DllImport("EDSDK.dll")]
        public static extern uint EdsRelease(IntPtr inRef);

        [DllImport("EDSDK.dll")]
        public static extern uint EdsOpenSession(IntPtr inCameraRef);

        [DllImport("EDSDK.dll")]
        public static extern uint EdsCloseSession(IntPtr inCameraRef);

        [DllImport("EDSDK.dll")]
        public static extern uint EdsSetPropertyData(IntPtr inRef, uint inPropertyID, int inParam, int inPropertySize, ref uint inPropertyData);

        [DllImport("EDSDK.dll")]
        public static extern uint EdsSendCommand(IntPtr inCameraRef, uint inCommand, int inParam);
    }
}
