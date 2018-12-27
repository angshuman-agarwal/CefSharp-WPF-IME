using CefSharp;
using System;
using System.Collections.Generic;
using System.Text;
using static Cef_Ime.IME.NativeIME;


namespace Cef_Ime.IME
{
    public class IMEHandler : IDisposable
    {
        internal const uint ColorUNDERLINE = 0xFFFFFFFF;   // Black SkColor value for underline,
        internal const uint ColorBKCOLOR = 0xFF000000;    // White SkColor value for background,

        internal IntPtr _hIMC;
        IntPtr _hWnd;

        internal static IMEHandler Create(IntPtr hWnd)
        {
            return new IMEHandler(hWnd);
        }

        private IMEHandler(IntPtr hWnd)
        {
            _hWnd = hWnd;
            _hIMC = ImmGetContext(_hWnd);
        }

        public void Dispose()
        {
            ImmReleaseContext(_hWnd, _hIMC);
        }

        private bool GetString(uint lParam, uint type, out string text)
        {
            text = string.Empty;

            if (!IsParam(lParam, type))
                return false;

            var strLen = ImmGetCompositionString(_hIMC, type, null, 0);
            if (strLen <= 0)
                return false;

            strLen += sizeof(char); // For trailing NULL - ImmGetCompositionString excludes that.

            // buffer contains char (2 bytes)
            byte[] buffer = new byte[strLen];
            ImmGetCompositionString(_hIMC, type, buffer, strLen);
            text = Encoding.Unicode.GetString(buffer);
            text = text.Remove(text.Length - 1); // Remove trailing null
            return true;
        }

        internal bool GetResult(uint lParam, out string text)
        {
            return GetString(lParam, GCS_RESULTSTR, out text);
        }

        internal bool GetComposition(uint lParam, List<CompositionUnderline> underlines, ref int compositionStart, out string text)
        {
            bool ret = GetString(lParam, GCS_COMPSTR, out text);
            if (ret)
                GetCompositionInfo(lParam, text, underlines, ref compositionStart);

            return ret;
        }

        private void GetCompositionInfo(uint lParam, string text, List<CompositionUnderline> underlines, ref int compositionStart)
        {
            underlines.Clear();

            int targetStart = text.Length;
            int targetEnd = text.Length;
            if (IsParam(lParam, GCS_COMPATTR))
                GetCompositionSelectionRange(ref targetStart, ref targetEnd);

            // Retrieve the selection range information. If CS_NOMOVECARET is specified
            // it means the cursor should not be moved and we therefore place the caret at
            // the beginning of the composition string. Otherwise we should honour the
            // GCS_CURSORPOS value if it's available.
            if (!IsParam(lParam, CS_NOMOVECARET) && IsParam(lParam, GCS_CURSORPOS))
            {
                // IMM32 does not support non-zero-width selection in a composition. So
                // always use the caret position as selection range.
                int cursor = (int)ImmGetCompositionString(_hIMC, GCS_CURSORPOS, null, 0);
                compositionStart = cursor;
            }
            else
                compositionStart = 0;

            if (IsParam(lParam, GCS_COMPCLAUSE))
                GetCompositionUnderlines(targetStart, targetEnd, underlines);

            if (underlines.Count < 1)
            {
                Range range = new Range();
                bool thick = false;
                if (targetStart > 0)
                    range = new Range(0, targetStart);
                if (targetEnd > targetStart)
                {
                    range = new Range(targetStart, targetEnd);
                    thick = true;
                }
                if (targetEnd < text.Length)
                    range = new Range(targetEnd, text.Length);

                underlines.Add(new CompositionUnderline(range, ColorUNDERLINE, ColorBKCOLOR, thick));
            }
        }

        private void GetCompositionUnderlines(int targetStart, int targetEnd, List<CompositionUnderline> underlines)
        {
            var clauseSize = ImmGetCompositionString(_hIMC, GCS_COMPCLAUSE, null, 0);
            int clauseLength = (int)clauseSize / sizeof(Int32);
            if (clauseLength > 0)
            {
                // buffer contains 32 bytes (4 bytes) array
                var clauseData = new byte[(int)clauseSize];
                ImmGetCompositionString(_hIMC, GCS_COMPCLAUSE, clauseData, clauseSize);

                for (int i = 0; i < clauseLength - 1; i++)
                {
                    int from = BitConverter.ToInt32(clauseData, i * sizeof(Int32));
                    int to = BitConverter.ToInt32(clauseData, (i + 1) * sizeof(Int32));

                    var range = new Range(from, to);
                    bool thick = (range.From >= targetStart && range.To <= targetEnd);

                    underlines.Add(new CompositionUnderline(range, ColorUNDERLINE, ColorBKCOLOR, thick));
                }
            }
        }

        private void GetCompositionSelectionRange(ref int targetStart, ref int targetEnd)
        {
            var attribute_size = ImmGetCompositionString(_hIMC, GCS_COMPATTR, null, 0);
            if (attribute_size > 0)
            {
                int start = 0;
                int end = 0;

                // Buffer contains 8bit array
                var attributeData = new byte[attribute_size];
                ImmGetCompositionString(_hIMC, GCS_COMPATTR, attributeData, attribute_size);

                for (start = 0; start < attribute_size; ++start)
                    if (IsSelectionAttribute(attributeData[start]))
                        break;

                for (end = start; end < attribute_size; ++end)
                    if (!IsSelectionAttribute(attributeData[end]))
                        break;

                targetStart = start;
                targetEnd = end;
            }
        }

        private bool IsSelectionAttribute(byte attribute)
        {
            return (attribute == ATTR_TARGET_CONVERTED ||
                attribute == ATTR_TARGET_NOTCONVERTED);
        }

        internal static bool IsParam(uint lParam, uint type)
        {
            return (lParam & type) == type;
        }
    }
}
