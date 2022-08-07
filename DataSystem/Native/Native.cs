using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using OpenGL;


namespace CMDR.Native
{
	
	internal static partial class Win
	{
		#region PUBLIC_MEMBERS

		public static Window CurrentWindow = null;

		public static WNDCLASSEXW wndClass = default;

		public static WNDPROC WndProc = new WNDPROC(WindowProcedure);

		#endregion

		#region PRIVATE_MEMBER

		private static MSG _message;

		#endregion

		#region PUBLIC_METHODS

		public static bool CreateWindow(Window window)
		{
			if(CurrentWindow == null)
			{
				CurrentWindow = window;
			}
			else
			{
				CheckError("Window already existed. Cannot create more than one window.", true);
			}

			wndClass.cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEXW));
			wndClass.style = window.ClassStyle | (uint)CS.OWNDC;
			wndClass.lpfnWndProc = new WNDPROC(WindowProcedure);
            wndClass.cbClsExtra = 0;
            wndClass.cbWndExtra = 0;
            wndClass.hInstance = Process.GetCurrentProcess().Handle;
			wndClass.hIcon = LoadIconW(IntPtr.Zero, (ushort)IDI.APPLICATION);
			wndClass.hCursor = LoadCursorW(IntPtr.Zero, (ushort)IDC.ARROW);
			wndClass.hbrBackground = IntPtr.Zero; // TODO ... Make Black
			wndClass.lpszMenuName = "MENU_NAME";
			wndClass.lpszClassName = "CMDRWCLASS";
			wndClass.hIconSm = IntPtr.Zero;

			if (RegisterClassExW(ref wndClass) == 0)
			{
				CheckError("RegisterWindow", true);
			}

			window.HWND = CreateWindowExW(
				window.WindowStyleEX,
				"CMDRWCLASS",
				window.Title,
				window.WindowStyle,
				window.StartingPosX,
				window.StartingPosY,
				window.Width,
				window.Height,
				IntPtr.Zero,
				IntPtr.Zero,
				wndClass.hInstance,
				IntPtr.Zero
				);
			
			if (window.HWND == IntPtr.Zero)
				CheckError("CreateWindow returned null", true);
			
			return true;
		}

		public static void PrepareContext(Window window)
        {
			PIXELFORMATDESCRIPTOR pfd = new PIXELFORMATDESCRIPTOR()
			{
				nSize = (ushort)Marshal.SizeOf(typeof(PIXELFORMATDESCRIPTOR)),
				nVersion = 1,
				swFlags = (uint)(PFD.DRAW_TO_WINDOW | PFD.SUPPORT_OPENGL | PFD.DOUBLEBUFFER),
				iPixelType = (byte)PFD.TYPE_RGBA,
				cColorBits = 32,
				cRedBits = 0,
				cRedShift = 0,
				cGreenBits = 0,
				cGreenShift = 0,
				cBluebits = 0,
				cBlueShift = 0,
				cAlphaBits = 0,
				cAlphaShift = 0,
				cAccumBits = 0,
				cAccumRedBits = 0,
				cAccumGreenBits = 0,
				cAccumBlueBits = 0,
				cAccumAlphaBits = 0,
				cDepthBits = 24,
				cStencilBits = 8,
				cAuxBuffers = 0,
				iLayerType = 0,
				bReserved = 0,
				dwLayerMask = 0,
				dwVisibleMask = 0,
				dwDamageMask = 0
			};

			window.DC = GetDC(window.HWND);

			if (window.DC == IntPtr.Zero)
			{
				CheckError("Device Context is null", true);
			}

			window.PixelFormatNumber = ChoosePixelFormat(window.DC, pfd);

			if (window.PixelFormatNumber == 0)
			{	
				CheckError("PixelFormaNumber is zero", true);
			}

			if (SetPixelFormat(window.DC, window.PixelFormatNumber, pfd) == false)
			{
				CheckError("Set Pixel Format", true);
			}

			window.HGLRC = wglCreateContext(window.DC);

			wglMakeCurrent(window.DC, window.HGLRC);

			GL.Build();
		}

		public static IntPtr WindowProcedure(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
		{
			switch((WM)uMsg)
            {
				case WM.CREATE:
					PrepareContext(CurrentWindow);
					ShowWindow(CurrentWindow.HWND, (int)SW.SHOW);
					return IntPtr.Zero;

				case WM.QUIT:
					return IntPtr.Zero;

				case WM.CLOSE:
					CurrentWindow.OnClose();
					return IntPtr.Zero;

				default:
					return DefWindowProcW(hWnd, uMsg, wParam, lParam);
            }
		}

		public static void DestroyWindow(Window window)
		{
			PostQuitMessage(0);
			
			if (DestroyWindow(window.HWND))
            {
				CheckError("DestroyWindow", true);
            }
		}

		/// <summary>
		/// Handle Windows message queue.
		/// </summary>
		/// <returns> Returns true if the GameLoop should continue, otherwise returns false. </returns>
		public static bool HandleMessages()
        {
			bool continueGameLoop = true;

			while(PeekMessageW(ref _message, CurrentWindow.HWND, 0,0, PM.REMOVE))
            {
				TranslateMessage(ref _message);
				
				DispatchMessage(ref _message);
				
				// If WM.CLOSE message is received, the return value will stop the GameLoop.
				if (_message.message == WM.CLOSE)
				{
					continueGameLoop = false;
				}
            }

			return continueGameLoop;
        }

		/// <summary>
		/// Check for Windows errors. If an error is detected it will be logged.
		/// </summary>
		/// <param name="name"> Name provided for the log. Typically something to idicate the body of code CheckError was called from.
		/// This makes finding the point where the error was generated easier to find. </param>
		/// <param name="exit"> Specify true for hard throw if error is detected, otherwise error will simply be logged. </param>
		/// <returns> Returns Win32 error code. </returns>
		public static int CheckError(string name, bool exit)
        {
			int error = Marshal.GetLastWin32Error();
			
			if (error != 0)
            {
				Log.LogWin32Error(error, name);
				
				if (exit)
                {
					if(CurrentWindow != null)
                    {
						if(DestroyWindow(CurrentWindow.HWND) == false)
                        {
							Log.LogWin32Error(Marshal.GetLastWin32Error(), "Window Destroy Failed!");
                        }
                    }
					throw new Win32Exception(error);
                }
            }
			
			SetLastError(0);
			
			return error;
        }

        #endregion
	}
	
	
	
}