using Raylib_cs;
using System.Numerics;
using Matrix2D = System.Numerics.Matrix3x2;
using Matrix = System.Numerics.Matrix4x4;
using Engine;

public partial class RayBatcher
{
}

public partial class Batcher
{
    public Matrix _transformedMatrix = Matrix.Identity;
    private int _spriteCount = 0;
    private BlendMode _blendMode  = BlendMode.BLEND_ALPHA;

    private SortedList<float,Texture2D> drawCalls = new SortedList<float,Texture2D>();
    public int SpriteCount { get; private set; }

    /// <summary>
    /// Default False
    /// </summary>
    bool _isBeginning ;


    public void Begin(BlendMode blendMode = BlendMode.BLEND_ALPHA,Matrix transformMatrix = default,float layerDepth = 1f)
    {
        if (_isBeginning)
            throw new Exception("Batcher has been beginned");

        this._blendMode = blendMode;
        this._transformedMatrix = transformMatrix ;
        this._isBeginning = true;
    }
    public void End()
    {
        Insist.IsTrue(_isBeginning, "Batcher should be call End() after called Begin().");

        Raylib.EndDrawing();

        _transformedMatrix = Matrix.Identity;
        _blendMode = BlendMode.BLEND_ALPHA;
        _isBeginning = false;
    }

    public void CheckBegin()
    {
        if (!_isBeginning)
            throw new Exception("Begin() has not been called. Begin() must be called before Draw()");

    }

    public void SettupRenderState()
    {
        Rlgl.rlSetBlendMode(_blendMode );
        Rlgl.rlPushMatrix();
        Rlgl.rlMultMatrixf(_transformedMatrix);
    }

    public void FlushBatch()
    {
        if (SpriteCount == 0) return;
        Rlgl.rlPopMatrix();
        var origin = 0;

        SpriteCount = 0;
    }

}
