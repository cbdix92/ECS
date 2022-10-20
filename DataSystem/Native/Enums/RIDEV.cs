using System;

namespace CMDR.Native
{
    /// <summary>
    /// For RAWINPUTDEVICE DWORD field
    /// https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawinputdevice
    /// </summary>
    internal enum RIDEV : uint
    {
        /// <summary>
        /// If set, this removes the top level collection from the inclusion list. This tells the operating system to stop reading from a device which matches the top level collection.
        /// </summary>
        REMOVE = 0x00000001,
        
        /// <summary>
        /// If set, this specifies the top level collections to exclude when reading a complete usage page. This flag only affects a TLC whose usage page is already specified with RIDEV_PAGEONLY.
        /// </summary>
        EXCLUDE = 0x00000010,
        
        /// <summary>
        /// If set, this specifies all devices whose top level collection is from the specified usUsagePage. Note that usUsage must be zero. To exclude a particular top level collection, use RIDEV_EXCLUDE.
        /// </summary>
        PAGEONLY = 0x00000020,
        
        /// <summary>
        /// If set, this prevents any devices specified by usUsagePage or usUsage from generating legacy messages. This is only for the mouse and keyboard. See Remarks.
        /// </summary>
        NOLEGACY = 0x00000030,
        
        /// <summary>
        /// If set, this enables the caller to receive the input even when the caller is not in the foreground. Note that hwndTarget must be specified.
        /// </summary>
        INPUTSINK = 0x00000100,
        
        /// <summary>
        /// If set, the mouse button click does not activate the other window. RIDEV_CAPTUREMOUSE can be specified only if RIDEV_NOLEGACY is specified for a mouse device.
        /// </summary>
        CAPTUREMOUSE = 0x00000200,
        
        /// <summary>
        /// If set, the application-defined keyboard device hotkeys are not handled. However, the system hotkeys; for example, ALT+TAB and CTRL+ALT+DEL, are still handled. By default, all keyboard hotkeys are handled. RIDEV_NOHOTKEYS can be specified even if RIDEV_NOLEGACY is not specified and hwndTarget is NULL.
        /// </summary>
        NOHOTKEYS = 0x00000200,
        
        /// <summary>
        /// If set, the application command keys are handled. RIDEV_APPKEYS can be specified only if RIDEV_NOLEGACY is specified for a keyboard device.
        /// </summary>
        APPKEYS = 0x00000400,
        
        /// <summary>
        /// If set, this enables the caller to receive input in the background only if the foreground application does not process it. In other words, if the foreground application is not registered for raw input, then the background application that is registered will receive the input.
        /// Windows XP: This flag is not supported until Windows Vista
        /// </summary>
        EXINPUTSINK = 0x00001000,
        
        /// <summary>
        /// If set, this enables the caller to receive WM_INPUT_DEVICE_CHANGE notifications for device arrival and device removal.
        /// Windows XP: This flag is not supported until Windows Vista
        /// </summary>
        DEVNOTIFY = 0x00002000
    }
}