using Engine;
using Engine.External;
using Engine.TiledSharp;
using Engine.TiledSharp.Extension;
using System.Numerics;

public class Grid : Component
{
    string _gridLayerName;
    public TmxLayer _gridLayer;

    public Grid(string gridLayerName)
    {
        _gridLayerName = gridLayerName;
    }
    public override void OnAddedToEntity()
    {
        if(_gridLayer == null && Entity.TryGetComponent<TileMap>(out var tm))
        {
            
            tm.Map.TryFindLayer(_gridLayerName,out _gridLayer);
        }
    }

    public VectorInt2 GetLocation(Vector2 worldPoint)
    {
        int x = (int)MathF.Floor(worldPoint.X / _gridLayer.Map.TileWidth);
        int y = (int)MathF.Floor(worldPoint.Y / _gridLayer.Map.TileHeight);
        return new VectorInt2(x,y);
    }
    public TmxLayerTile GetTile(VectorInt2 point)
    {
        TmxLayerTile foundtile = null;

        try
        {
            foundtile = _gridLayer.GetTile(point.X, point.Y);
        }
        catch (Exception)
        {
        }
        return foundtile;
    }
    public TmxLayerTile? GetTileFromWorld(Vector2 worldPoint)
    {
        int x = (int)MathF.Floor( worldPoint.X / _gridLayer.Map.TileWidth);
        int y = (int)MathF.Floor(worldPoint.Y / _gridLayer.Map.TileHeight);
        return GetTile(new VectorInt2(x,y));
    }


}

