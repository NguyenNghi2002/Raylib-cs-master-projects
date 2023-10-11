using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace _3dWorld
{
    public class Camera
    {
        public Vector3 Position;
        public Quaternion Rotation = Quaternion.Identity;
        public Vector3 Scale;
        public Matrix4x4 World = Matrix4x4.Identity;

        public float FOV;
        public float NearClip;
        public float FarClip;
        public double Aspect;
        CameraProjection Mode;

        public Matrix4x4 projection = Matrix4x4.Identity;
        public Matrix4x4 View = Matrix4x4.Identity;

        public Camera(Vector3 position, Vector3 rotation, float fov, float near, float far, double aspect)
        {
            this.Position = position;
            this.Rotation = Raymath.QuaternionFromEuler(rotation.X * Raylib.DEG2RAD, rotation.Y * Raylib.DEG2RAD, rotation.Z * Raylib.DEG2RAD);
            Scale = Vector3.One;
            FOV = fov;
            NearClip = near;
            FarClip = far;
            Aspect = aspect;
            Mode = CameraProjection.CAMERA_PERSPECTIVE;
            Update();
        }

        public void Update()
        {
            Matrix4x4 pos = Matrix4x4.CreateTranslation(Position);
            Matrix4x4 rot = Raymath.QuaternionToMatrix(Rotation);
            Matrix4x4 sca = Matrix4x4.CreateScale(Scale);
            World = Matrix4x4.Multiply(Matrix4x4.Multiply(sca, rot), pos);
            View = Raymath.MatrixInvert(World);

            if (Mode == CameraProjection.CAMERA_ORTHOGRAPHIC)
            {
                float top = FOV / 2f;
                float right = (float)(top * Aspect);
                projection = Matrix4x4.CreateOrthographicOffCenter(-right, right, -top, top, NearClip, FarClip);
            }
            else
            {
                // Setup perspective projection
                float top = NearClip * MathF.Tan(FOV * 0.5f * Raylib.DEG2RAD);
                float right = (float)(top * Aspect);
                projection = Raymath.MatrixFrustum(-right, right, -top, top, NearClip, FarClip);
                //projection = Matrix4x4.CreatePerspectiveFieldOfView(FOV * Raylib.DEG2RAD, (float)Aspect, NearClip, FarClip);
            }
        }

        public void Begin()
        {
            Rlgl.rlDrawRenderBatchActive();      // Update and draw internal render batch

            Rlgl.rlMatrixMode(MatrixMode.PROJECTION);    // Switch to projection matrix
            Rlgl.rlPushMatrix();                 // Save previous matrix, which contains the settings for the 2d ortho projection
            Rlgl.rlLoadIdentity();               // Reset current matrix (projection)
            Rlgl.rlViewport(0, 0, GetScreenWidth(), GetScreenHeight());
            float aspect = (float)Aspect;

            // NOTE: zNear and zFar values are important when computing depth buffer values
            if (Mode == CameraProjection.CAMERA_PERSPECTIVE)
            {
                // Setup perspective projection
                double top = NearClip * MathF.Tan((float)(FOV * 0.5f * Raylib.DEG2RAD));
                double right = top * aspect;

                Rlgl.rlFrustum(-right, right, -top, top, NearClip, FarClip);
            }
            else if (Mode == CameraProjection.CAMERA_ORTHOGRAPHIC)
            {
                // Setup orthographic projection
                double top = FOV / 2.0;
                double right = top * aspect;

                Rlgl.rlOrtho(-right, right, -top, top, NearClip, FarClip);
            }

            Rlgl.rlMatrixMode(MatrixMode.MODELVIEW);     // Switch back to modelview matrix
            Rlgl.rlLoadIdentity();               // Reset current matrix (modelview)

            // Setup Camera view
            Rlgl.rlMultMatrixf(View);      // Multiply modelview matrix by view matrix (camera)

            Rlgl.rlEnableDepthTest();            // Enable DEPTH_TEST for 3D
        }

        public void End()
        {
            Raylib.EndMode3D();
        }
    }
}