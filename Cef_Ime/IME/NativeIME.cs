using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Cef_Ime.IME
{
    public static class NativeIME
    {
        public const int WM_INPUTLANGCHANGE = 0x51;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x101;
        public const int WM_CHAR = 0x102;
        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WM_SYSKEYUP = 0x105;
        public const int WM_IME_STARTCOMPOSITION = 0x10D;
        public const int WM_IME_ENDCOMPOSITION = 0x10E;
        public const int WM_IME_COMPOSITION = 0x10F;
        public const int WM_IME_SETCONTEXT = 0x281;
        public const int WM_IME_NOTIFY = 0x282;
        public const int WM_IME_CONTROL = 0x283;
        public const int WM_IME_COMPOSITIONFULL = 0x284;
        public const int WM_IME_SELECT = 0x285;
        public const int WM_IME_CHAR = 0x286;
        public const int WM_IME_REQUEST = 0x0288;
        public const int WM_IME_KEYDOWN = 0x290;
        public const int WM_IME_KEYUP = 0x291;
        public const int WM_SYSCHAR = 0x0106;

        internal const uint GCS_RESULTSTR = 0x0800;
        internal const uint GCS_COMPSTR = 0x0008;
        internal const uint GCS_COMPATTR = 0x0010;
        internal const uint GCS_CURSORPOS = 0x0080;
        internal const uint GCS_COMPCLAUSE = 0x0020;
        internal const uint CS_NOMOVECARET = 0x4000;

        internal const uint ATTR_TARGET_CONVERTED = 0x01;
        internal const uint ATTR_TARGET_NOTCONVERTED = 0x03;

        internal const uint ISC_SHOWUICOMPOSITIONWINDOW = 0x80000000;

        internal const uint CFS_DEFAULT = 0x0000;
        internal const uint CFS_RECT = 0x0001;
        internal const uint CFS_POINT = 0x0002;
        internal const uint CFS_FORCE_POSITION = 0x0020;

        internal const uint LANG_JAPANESE = 0x11;
        internal const uint LANG_CHINESE = 0x04;
        internal const uint LANG_KOREAN = 0x12;

        [StructLayout(LayoutKind.Sequential)]
        internal struct TagPoint
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TagRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TagCompositionForm
        {
            public uint DwStyle;
            public TagPoint PtCurrentPos;
            public TagRect RcArea;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TagCandidateForm
        {
            public uint DwIndex;
            public uint DwStyle;
            public TagPoint PtCurrentPos;
            public TagRect RcArea;
        }

        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("imm32.dll")]
        internal static extern IntPtr ImmCreateContext();

        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("imm32.dll")]
        internal static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);

        [DllImport("imm32.dll")]
        internal static extern bool ImmDestroyContext(IntPtr hIMC);

        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("imm32.dll")]
        internal static extern IntPtr ImmGetContext(IntPtr hWnd);

        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("Imm32.dll")]
        internal static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("imm32.dll", CharSet = CharSet.Unicode)]
        internal static extern uint ImmGetCompositionString(IntPtr hIMC, uint dwIndex, byte[] lpBuf, uint dwBufLen);

        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("imm32.dll")]
        internal static extern int ImmSetCompositionWindow(IntPtr hIMC, ref TagCompositionForm lpCompForm);

        [DllImport("user32.dll")]
        internal static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

        [DllImport("user32.dll")]
        internal static extern bool DestroyCaret();

        [DllImport("user32.dll")]
        internal static extern bool SetCaretPos(int x, int y);

        [DllImport("user32.dll")]
        internal static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
    }
}
