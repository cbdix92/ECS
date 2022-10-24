using System;
using System.Runtime.InteropServices;


namespace CMDR.Native
{
    /// <summary>
    /// Contains the raw input from a device.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawinput"/> Microsoft Docs </see>
    [StructLayout(LayoutKind.Explicit)]
    internal struct RAWINPUT
    {
        [FieldOffset(0)]
        public RAWINPUTHEADER Header;

        [FieldOffset(24)]
        public RAWMOUSE Mouse;

        [FieldOffset(24)]
        public RAWKEYBOARD Keyboard;

        [FieldOffset(24)]
        public RAWHID HID;

    }

    /// <summary>
    /// Contains information about the state of the mouse.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawmouse"> Microsoft Docs </see>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RAWMOUSE
    {
        public ushort usFlags;

        public ulong ulButtons;

        public ushort usButtonFlags;

        public ushort usButtonData;

        public ulong ulRawButton;

        public long lLastX;

        public long lLastY;

        public ulong ulExtraInformation;
    }

    /// <summary>
    /// Contains information about the state of the keyboard.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawkeyboard"/> Microsoft Docs </see>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RAWKEYBOARD
    {
        public ushort MakeCode;
        
        public ushort Flags;
        
        public ushort Reserved;
        
        public ushort VKey;

        public uint Message;

        public ulong ExtraInformation;
    }

    /// <summary>
    /// Describes the format of the raw input from a Human Interface Device (HID).
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawhid"/> Microsoft Docs </see>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RAWHID
    {
        public uint dwSizeHid;

        public uint dwCount;

        public byte[] bRawData;
    }
}
