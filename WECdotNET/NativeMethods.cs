using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace WECdotNET
{
    class NativeMethods
    {
        [DllImport("wecapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr EcOpenSubscription(string subscriptionName, uint accessMask, uint Flags);

        [DllImport("wecapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool EcClose(IntPtr handle);

        [DllImport("wecapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr EcOpenSubscriptionEnum(uint flags);

        [DllImport("wecapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool EcEnumNextSubscription(IntPtr subscriptionEnum, uint subscriptionNameBufferSize, StringBuilder subscriptionNameBuffer, out uint subscriptionNameBufferUsed);
    }

    enum AccessMask : uint
    {
        ReadAccess = 1,
        WriteAccess = 2
    }

    enum Flags : uint
    {
        OpenAlways = 0,
        CreateNew = 1,
        OpenExisting = 2
    }
}
