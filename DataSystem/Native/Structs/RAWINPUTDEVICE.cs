using System;
using System.Runtime.InteropServices;

namespace CMDR.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RAWINPUTDEVICE
    {
        ushort usUsagePage;
        ushort usUsage;
        uint dwFlags;
        IntPtr hwndTarget;

        public RAWINPUTDEVICE(HID_USAGE_PAGE page, HID_USAGE_GENERIC usage, RIDEV flags, IntPtr hwnd)
        {
            usUsagePage = (ushort)page;

            usUsage = (ushort)usage;

            dwFlags = (uint)flags;

            hwndTarget = hwnd;
        }
    }
}