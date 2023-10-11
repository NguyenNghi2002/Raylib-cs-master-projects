using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Keys = Raylib_cs.KeyboardKey;
using System.Threading.Tasks;

namespace Engine
{
    public static class User32
    {
        [Flags]
        public enum uType
        {
            MB_OK = 0,
            MB_OKCANCLE = 1,
            MB_ABORTRETRYIGNORE = 2,
            MB_YESNOCANCLE = 3,
            MB_YESNO = 4,
            MB_RETRYCANCLE  =5,
            MB_CANCLETRYCONTINUE = 6,

            MB_ICONEXCLMATION = 30,
            MB_ICONWARNING = 30,
            MB_ICONINFORMATION = 40,
            MB_ICONASTERISK = 40,
            MB_ICONQUESTION = 20,
            MB_ICONSTOP = 10,
            MB_ICONERROR = 10,
            MB_ICONHAND = 10,

            MB_DEFBUTTON1 = 0,
            MB_DEFBUTTON2 = 100,
            MB_DEFBUTTON3 = 200,
            MB_DEFBUTTON4 = 300,
        }


        public const string DllFileName = "user32.dll";

        [DllImport(DllFileName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern int MessageBox(IntPtr hWnd, string lpText, string lpCapion, uint utype);

        public static int MessageBox( string lpText, string lpCapion, uType utype)
            => MessageBox(default,lpText,lpCapion,(uint)utype);


        [DllImport(DllFileName, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);

        [DllImport(DllFileName)]
        public static extern short GetAsyncKeyState(int ArrowKeys);



        public static char ToAscii(Keys key, Keys modifiers)
        {
            var outputBuilder = new StringBuilder(2);
            int result = ToAscii((uint)key, 0, GetKeyState(modifiers),
                outputBuilder, 0);
            if (result == 1)
                return outputBuilder[0];
            else
                throw new Exception("Invalid key");
        }

        private const byte HighBit = 0x80;
        private static byte[] GetKeyState(Keys modifiers)
        {
            var keyState = new byte[256];
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if ((modifiers & key) == key)
                {
                    try
                    {
                        keyState[(int)key] = HighBit;
                    }
                    catch { }
                }
            }
            return keyState;
        }

        [DllImport("user32.dll")]
        private static extern int ToAscii(uint uVirtKey, uint uScanCode,
            byte[] lpKeyState,
            [Out] StringBuilder lpChar,
            uint uFlags);
    }

}
