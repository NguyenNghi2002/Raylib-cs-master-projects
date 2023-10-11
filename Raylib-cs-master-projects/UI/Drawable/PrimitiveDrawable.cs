﻿using Raylib_cs;
using Engine;
using System.Numerics;

namespace Engine.UI
{
    public class PrimitiveDrawable : IDrawable
	{

		#region IDrawable implementation

		public float LeftWidth { get; set; }
		public float RightWidth { get; set; }
		public float TopHeight { get; set; }
		public float BottomHeight { get; set; }
		public float MinWidth { get; set; } = 0;
		public float MinHeight { get; set; } = 0;
		public void SetPadding(float top, float bottom, float left, float right)
		{
			TopHeight = top;
			BottomHeight = bottom;
			LeftWidth = left;
			RightWidth = right;

		}
		public PrimitiveDrawable Pad(float top, float bottom, float left, float right)
		{
			TopHeight = top;
			BottomHeight = bottom;
			LeftWidth = left;
			RightWidth = right;
			return this;
		}

		#endregion

		public Color? OutlineColor;
		public Color? Color;
		public bool UseFilledRect = true;
        public bool UseOutline = true;
		public float LineWidth = 3f;
        public bool EnableAlphaBlend = false;

        #region Constructors
        public PrimitiveDrawable(Color? color = null, Color? outlineColor = null)
        {
            Color = color ?? Raylib_cs.Color.BLANK ;
            OutlineColor = outlineColor ?? Raylib_cs.Color.BLANK ;
			MinWidth = 10;
			MinHeight = 10;
        }
        public PrimitiveDrawable(float horizontalPadding,Color color) : this(color, color, horizontalPadding) { }
        public PrimitiveDrawable(Color color, Color outlineColor, float horizontalPadding) : this(color,outlineColor)
        {
            LeftWidth = RightWidth = horizontalPadding;
        }

        public PrimitiveDrawable(Color color, float horizontalPadding, float verticalPadding) : this(color, color,horizontalPadding,verticalPadding)
        {

        }
        public PrimitiveDrawable(Color color, Color outlineColor, float horizontalPadding, float verticalPadding) : this(color, outlineColor)
        {
            LeftWidth = RightWidth = horizontalPadding;
            TopHeight = BottomHeight = verticalPadding;
        }
        public PrimitiveDrawable(float minWidth, float minHeight, Color? color = null, Color? outlineColor = null) : this(color, outlineColor)
        {
            MinWidth = minWidth;
            MinHeight = minHeight;
        }
        public PrimitiveDrawable(float minSize) : this(minSize, minSize)
        {
        }
        public PrimitiveDrawable(float minSize, Color color, Color outlineColor) : this(minSize, minSize, color, outlineColor)
        {
        } 
        #endregion

        public virtual void Draw( float x, float y, float width, float height, Color color)
		{
			Color col = Color.HasValue ? Color.Value : color;
			var outlineCol = OutlineColor.HasValue  ? OutlineColor.Value : color;
			if (EnableAlphaBlend)
			{
				if (color.a != 255)
				{
					float c = (color.a / 255f);
					col.r = (byte)(col.r * c);
					col.g = (byte)(col.g * c);
					col.b = (byte)(col.b * c);
					col.a = (byte)(col.a * c);
					//col *= (color.a / 255f);
				}
				if (col.a != 255)
				{
					float c = (color.a / 255f);
					col.r = (byte)(col.r * c);
					col.g = (byte)(col.g * c);
					col.b = (byte)(col.b * c);
					col.a = (byte)(col.a * c);

					//col *= (col.a / 255f);
				}
				if (outlineCol.a != 255)
				{
					float c = (col.a / 255f);
					outlineCol.r = (byte)(outlineCol.r * c);
					outlineCol.g = (byte)(outlineCol.g * c);
					outlineCol.b = (byte)(outlineCol.b * c);
					outlineCol.a = (byte)(outlineCol.a * c);

				}
			}
			var rec = new Rectangle(x, y, width, height);

			rec = rec.SetBot(rec.BotRight().Y +  BottomHeight);
			rec = rec.SetTop(rec.TopLeft().Y -  TopHeight);
			rec = rec.SetLeft(rec.TopLeft().X -  LeftWidth);
			rec = rec.SetRight(rec.BotRight().X +  RightWidth);

			if (UseFilledRect && col.a != 0)
				Raylib.DrawRectanglePro(rec, Vector2.Zero, 0, col);
			if (!UseFilledRect || (UseOutline && UseFilledRect) && outlineCol.a != 0)
				Raylib.DrawRectangleLinesEx(rec, LineWidth, outlineCol);
		}

		public void Set(float top, float bottom, float left, float right)
		{
			SetPadding(top, bottom, left, right);
		}
	}

}