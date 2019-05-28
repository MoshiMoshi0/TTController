using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Plugin.RazerConnectEffect
{
    public enum RzResult
    {
        SUCCESS = 0
    }

    public static class RzChromaBroadcastNative
    {
        private static IntPtr _dllHandle;

        public static void Load()
        {
            if (_dllHandle != IntPtr.Zero)
                return;

            _dllHandle = LoadLibrary("RzChromaBroadcastAPI.dll");
            _initPointer = (InitPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "Init"), typeof(InitPointer));
            _unInitPointer = (UnInitPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "UnInit"), typeof(UnInitPointer));
            _registerEventNotificationPointer = (RegisterEventNotificationPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "RegisterEventNotification"), typeof(RegisterEventNotificationPointer));
            _unRegisterEventNotificationPointer = (UnRegisterEventNotificationPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "UnRegisterEventNotification"), typeof(UnRegisterEventNotificationPointer));
        }

        public static void UnLoad()
        {
            while (FreeLibrary(_dllHandle)) ;
            _dllHandle = IntPtr.Zero;
        }

        private static InitPointer _initPointer;
        private static UnInitPointer _unInitPointer;
        private static RegisterEventNotificationPointer _registerEventNotificationPointer;
        private static UnRegisterEventNotificationPointer _unRegisterEventNotificationPointer;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RzResult InitPointer(uint a, uint b, uint c, uint d);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RzResult UnInitPointer();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RzResult RegisterEventNotificationPointer(RzBroadcastCallback callback);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RzResult UnRegisterEventNotificationPointer();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int RzBroadcastCallback(int message, IntPtr data);

        internal static RzResult Init(Guid guid)
        {
            var i = new BigInteger(guid.ToByteArray());

            var a = (uint)((i >> 96) & 0xFFFFFFFF);
            var b = (uint)((i >> 64) & 0xFFFFFFFF);
            var c = (uint)((i >> 32) & 0xFFFFFFFF);
            var d = (uint)((i >> 0) & 0xFFFFFFFF);

            return _initPointer(a, b, c, d);
        }
        internal static RzResult UnInit() => _unInitPointer();

        internal static RzResult RegisterEventNotification(RzBroadcastCallback callback) => _registerEventNotificationPointer(callback);
        internal static RzResult UnregisterEventNotification() => _unRegisterEventNotificationPointer();

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr dllHandle);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr dllHandle, string name);
    }
}
