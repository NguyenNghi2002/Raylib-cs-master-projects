﻿
using Engine;
using Raylib_cs;
using Raylib_cs.Extension;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Engine.TiledSharp
{
    public static class RayTiled
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawMap(TmxMap map,Vector2 origin,Vector2 position,Vector2 scale)
        {
            foreach (ITmxLayer layer in map.Layers)
            {
                if (layer is TmxLayer tmxLayer)
                    DrawLayer(tmxLayer, position, scale);
                else if (layer is TmxObjectGroup tmxObjectGroup)
                    DrawObjectGroup(tmxObjectGroup, position, scale);
                else if (layer is TmxImageLayer tmxImageLayer)
                    DrawImageLayer(tmxImageLayer, position, scale);
                else if (layer is TmxGroup tmxGroup)
                    DrawGroup(tmxGroup, origin, position, scale,Color.WHITE);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawGroup(TmxGroup group,Vector2 origin,Vector2 position,Vector2 scale,Color? color = null)
        {
            var offset = new Vector2((float)group.OffsetX, (float)group.OffsetY); 
            foreach (ITmxLayer layer in group.Layers)
            {
                if (layer is TmxLayer tmxLayer)
                    DrawLayer(tmxLayer, position, scale);
                else if (layer is TmxObjectGroup tmxObjectGroup)
                    DrawObjectGroup(tmxObjectGroup, position, scale);
                else if (layer is TmxImageLayer tmxImageLayer)
                    DrawImageLayer(tmxImageLayer, position, scale);
                else if (layer is TmxGroup tmxGroup)
                {
                    DrawGroup(tmxGroup, 
                        origin,
                        position,
                         scale,
                        Raylib.Fade(group.Color.RayColor, (float)group.Opacity) 
                        );
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawImageLayer(TmxImageLayer imageLayer,Vector2 position,Vector2 scale,Color? color = null)
        {
            if (!imageLayer.Visible) return;

            position += new Vector2((float)imageLayer.OffsetX, (float)imageLayer.OffsetY);

            var tint = color ?? new Color((byte)imageLayer.Color.R, (byte)imageLayer.Color.G, (byte)imageLayer.Color.B);
            tint = Raylib.Fade(tint, (float)imageLayer.Opacity);

            var texture = imageLayer.Image.Texture;
            var srcRec = imageLayer.Image.Texture.RawData.Source();
            var desRec = srcRec.MuliplyScale(scale).Move(position);
            Raylib.DrawTexturePro(texture,srcRec,desRec,Vector2.Zero,0f,tint);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawObjectGroup(TmxObjectGroup objectGroup, Vector2 position, Vector2 scale, Color? outlineColor = null, Color? fillColor = null, float lineWidth = 4f)
        {
            if (!objectGroup.Visible) return;

            position += new Vector2((float)objectGroup.OffsetX, (float)objectGroup.OffsetY);

            var outlineTint = outlineColor ?? new Color((byte)objectGroup.Color.R, (byte)objectGroup.Color.G, (byte)objectGroup.Color.B, (byte)1);
            outlineTint = Raylib.Fade(outlineTint, (float)objectGroup.Opacity);
            var fillTint = fillColor ?? Raylib.Fade(outlineTint, 0.3f);


            Vector2 size;
            foreach (var obj in objectGroup.Objects)
            {
                var pos = new Vector2((float)obj.X, (float)obj.Y) + position;
                var rot = (float)obj.Rotation ;
                if (!obj.Visible) continue;
                switch (obj.ObjectType)
                {
                    case TmxObjectType.Basic:

                        size = new Vector2((float)obj.Width, (float)obj.Height) * scale;

                        var rec = new Rectangle(pos.X, pos.Y, size.X, size.Y);

                        Raylib.DrawRectanglePro(rec,Vector2.Zero, rot, fillTint );
                        RayUtils.DrawRectangleLines(rec,Vector2.Zero, rot, lineWidth, outlineTint);

                        break;
                    case TmxObjectType.Tile:
                        pos = new Vector2((float)obj.X, (float)obj.Y) + position;
                        var tileSize = new Vector2((float)obj.Width, (float)obj.Height) ;
                        rot = (float)obj.Rotation;

                        DrawObjTile(obj,pos,tileSize,rot,Color.WHITE);


                        break;
                    case TmxObjectType.Ellipse:

                        pos = new Vector2((float)obj.X, (float)obj.Y) + position;
                        size = new Vector2((float)obj.Width, (float)obj.Height) * scale;
                        rot = (float)obj.Rotation;

                        Raylib.DrawCircleV(pos, 2, Color.RED);
                        RayUtils.DrawEllipse(pos,size/2f,size/2f,rot,fillTint);
                        RayUtils.DrawEllipseLines(pos,size/2f,size/2f,rot,lineWidth,outlineTint);
                        break;
                    case TmxObjectType.Polygon:

                        RayUtils.DrawLineStrip(obj.Points.Select(p => new Vector2((float)p.X, (float)p.Y)),
                            new Vector2((float)obj.X, (float)obj.Y) + position,rot,
                            lineWidth, outlineTint);

                        break;
                    case TmxObjectType.Polyline:

                        RayUtils.DrawLineStrip(obj.Points.Select(p => new Vector2((float)p.X, (float)p.Y) ), 
                            new Vector2((float)obj.X, (float)obj.Y) + position,rot,
                            lineWidth,outlineTint);
                        break;
                    default:
                        break;
                }
            }


        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawLayer(TmxLayer layer,Vector2 position,Vector2 scale,Color? color = null)
        {
            if (!layer.Visible) return;

            position += new Vector2((float)layer.OffsetX, (float)layer.OffsetY);

            var tint = color ?? new Color((byte)layer.Color.R, (byte)layer.Color.G, (byte)layer.Color.B, (byte)1);
            tint = Raylib.Fade(tint, (float)layer.Opacity);

            var tileWidth = layer.Map.TileWidth * scale.X;
            var tileHeight = layer.Map.TileHeight * scale.Y;

            var WidthCount = layer.Map.Width;
            var HeightCount = layer.Map.Height;

            for (int y = 0; y < HeightCount; y++)
            {
                for (int x = 0; x < WidthCount; x++)
                {
                    var tile = layer.GetTile(x,y);
                    if(tile != null)
                    {
                        DrawLayerTile(tile, position,scale,tileWidth,tileHeight,0f,
                            tint ,layer.Map.Orientation);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawObjTile(TmxObject objTile,Vector2 position,Vector2 tileScale,float angle,Color color)
        {
            if (objTile.ObjectType is not TmxObjectType.Tile) return;

            var tile = objTile.Tile;
            var tileset = objTile.Tile.Tileset;

            int srcTileID = tile.Gid - tileset.FirstGid; // Gid ID to Index
            var tilesetTile = tile.GetTilesetTile();
            if (tilesetTile != null && tilesetTile.AnimationFrames.Count > 0)
            {
                srcTileID = tilesetTile.CurrentAnimationFrameGid - tileset.FirstGid;
            }


            int srcLocX = (int)(srcTileID % tileset.Columns);
            int srcLocY = (int)(srcTileID / tileset.Columns);


            var tileWidth = 64;
            var tileHeight =  64;

            Vector2 srcLocation = new Vector2(srcLocX, srcLocY);
            Vector2 srcScale = new Vector2(tileWidth, tileHeight);
            Vector2 srcPos = srcScale * srcLocation;

            Rectangle srcRec = new Rectangle(srcPos.X, srcPos.Y, srcScale.X, srcScale.Y);
            Rectangle desRec = new Rectangle(position.X, position.Y, tileScale.X, tileScale.Y);

            //Handle flipping
            if (tile.HorizontalFlip)
                srcRec.width *= -1;
            if (tile.VerticalFlip)
                srcRec.height *= -1;

            Raylib.DrawCircleV(position,10,Color.YELLOW);
            Raylib.DrawTexturePro(tileset.Image.Texture, srcRec, desRec, Vector2.UnitY * tileScale.Y, angle, color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawLayerTile(TmxLayerTile tile,
            Vector2 origin,Vector2 scale,
            float tileWidth,float tileHeight,float angle,
            Color color, OrientationType orientation
            )
        {
            if (tile.Gid == 0) return;
            var tileset = tile.Tileset;

            //Destination Info
            Vector2 tileLocation = new Vector2(tile.X, tile.Y);
            Vector2 tileScale = new Vector2(tileWidth,tileHeight) * scale;
            Vector2 tilePos = tileLocation * tileScale + origin;

            //Source Info

            int srcTileID = tile.Gid - tileset.FirstGid; // Gid ID to Index
            var tilesetTile = tile.GetTilesetTile();
            if (tilesetTile != null && tilesetTile.AnimationFrames.Count > 0 )
            {
                srcTileID = tilesetTile.CurrentAnimationFrameGid - tileset.FirstGid;

            }
            // index to 2d Location
            int srcLocX = (int)(srcTileID % tileset.Columns); 
            int srcLocY = (int)(srcTileID / tileset.Columns);

            

            Vector2 srcLocation = new Vector2(srcLocX,srcLocY);
            Vector2 srcScale = new Vector2(tileWidth, tileHeight);
            Vector2 srcPos = srcScale * srcLocation;

            Rectangle srcRec = new Rectangle(srcPos.X , srcPos.Y , srcScale.X , srcScale.Y);
            Rectangle desRec = new Rectangle(tilePos.X, tilePos.Y, tileScale.X, tileScale.Y);



            //Handle Orientation type
            switch (orientation)
            {
                case OrientationType.Isometric:
                    break;
                case OrientationType.Hexagonal:
                    break;
                case OrientationType.Staggered: throw new NotImplementedException("Staggered Tiled maps are not yet supported.");

                case OrientationType.Unknown:
                case OrientationType.Orthogonal:
                default: break;
            }

            //Handle flipping
            if (tile.HorizontalFlip) 
                srcRec.width *= -1;
            if (tile.VerticalFlip) 
                srcRec.height *= -1;

            Raylib.DrawTexturePro(tileset.Image.Texture, srcRec, desRec, Vector2.Zero, angle,color);

        }

    }
}