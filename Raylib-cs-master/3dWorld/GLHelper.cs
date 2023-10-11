using System.Runtime.InteropServices;

namespace OpenGL
{
    public static unsafe partial class GLHelper
    {
        private const string dllName = "opengl32";

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void glTexPeramteri(uint id, int param, int value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void glCullFace(int mode);

        /// <summary>
        /// The glDepthFunc function specifies the function used to compare each incoming pixel z value with the z value present in the depth buffer. The comparison is performed only if depth testing is enabled. (See glEnable with the argument GL_DEPTH_TEST.)
        /// </summary>
        /// <param name="func"></param>
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void glDepthFunc(int func);
        

    }
}