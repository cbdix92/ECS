using System;
using System.Runtime.InteropServices;

namespace CMDR.Native
{
    /// <summary>
    /// The MONITORINFOEX structure contains information about a display monitor.
    /// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct MONITORINFOEXW
	{
		public uint cbSize;
		public Rect rcMonitor;
		public Rect rcWork;
		public uint dwFlags;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string szDevice;

        public static MONITORINFOEXW Create()
        {
            MONITORINFOEXW monitorInfo = new MONITORINFOEXW();
            monitorInfo.cbSize = (uint)Marshal.SizeOf(monitorInfo);
            monitorInfo.szDevice = String.Empty;
            return monitorInfo;
        }
	}
	
}