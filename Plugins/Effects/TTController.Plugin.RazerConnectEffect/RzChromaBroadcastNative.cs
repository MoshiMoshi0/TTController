using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace TTController.Plugin.RazerConnectEffect
{
    public enum RzResult
    {
        SUCCESS = 0,
        ACCESS_DENIED = 5,
        INVALID_HANDLE = 6,
        INVALID_GUID = 12,
        NOT_SUPPORTED = 50,
        INVALID_PARAMETER = 87,
        SERVICE_NOT_ACTIVE = 1062,
        SINGLE_INSTANCE_APP = 1152,
        DEVICE_NOT_CONNECTED = 1167,
        NOT_FOUND = 1168,
        REQUEST_ABORTED = 1235,
        ALREADY_INITIALIZED = 1247,
        RESOURCE_DISABLED = 4309,
        DEVICE_NOT_AVAILABLE = 4319,
        NOT_VALID_STATE = 5023
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int RzBroadcastCallback(int message, IntPtr data);

    public static class RzChromaBroadcastNative
    {
        public static readonly int BroadcastColorCount = 5;

        private static IntPtr _dllHandle;

        public static bool Load()
        {
            if (_dllHandle != IntPtr.Zero)
                return true;

            _dllHandle = LoadLibrary(Environment.Is64BitProcess ? "RzChromaBroadcastAPI64.dll" : "RzChromaBroadcastAPI.dll");
            if (_dllHandle == IntPtr.Zero)
                return false;

            if (Environment.Is64BitProcess)
                _initPointer64 = (InitPointer64)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "Init"), typeof(InitPointer64));
            else
                _initPointer32 = (InitPointer32)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "Init"), typeof(InitPointer32));

            _unInitPointer = (UnInitPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "UnInit"), typeof(UnInitPointer));
            _registerEventNotificationPointer = (RegisterEventNotificationPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "RegisterEventNotification"), typeof(RegisterEventNotificationPointer));
            _unRegisterEventNotificationPointer = (UnRegisterEventNotificationPointer)Marshal.GetDelegateForFunctionPointer(GetProcAddress(_dllHandle, "UnRegisterEventNotification"), typeof(UnRegisterEventNotificationPointer));

            return true;
        }

        public static void UnLoad()
        {
            while (FreeLibrary(_dllHandle)) ;
            _dllHandle = IntPtr.Zero;
        }

        private static InitPointer32 _initPointer32;
        private static InitPointer64 _initPointer64;
        private static UnInitPointer _unInitPointer;
        private static RegisterEventNotificationPointer _registerEventNotificationPointer;
        private static UnRegisterEventNotificationPointer _unRegisterEventNotificationPointer;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int InitPointer32(uint a, uint b, uint c, uint d);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate long InitPointer64(IntPtr guid);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RzResult UnInitPointer();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RzResult RegisterEventNotificationPointer(RzBroadcastCallback callback);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RzResult UnRegisterEventNotificationPointer();

        internal static RzResult Init(Guid guid)
        {
            var i = new BigInteger(guid.ToByteArray());

            var a = ((i >> 96) & 0xFFFFFFFF);
            var b = ((i >> 64) & 0xFFFFFFFF);
            var c = ((i >> 32) & 0xFFFFFFFF);
            var d = ((i >> 0) & 0xFFFFFFFF);

            if (Environment.Is64BitProcess)
            {
                i = (a << 0) | (b << 32) | (c << 64) | (d << 96);

                var offset = 0;
                var memory = Marshal.AllocHGlobal(16);
                foreach (var bb in i.ToByteArray())
                    Marshal.WriteByte(memory, offset++, bb);

                var result = _initPointer64(memory);
                Marshal.FreeHGlobal(memory);
                return (RzResult)result;
            }
            else
            {
                return (RzResult)_initPointer32((uint)a, (uint)b, (uint)c, (uint)d);
            }
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
