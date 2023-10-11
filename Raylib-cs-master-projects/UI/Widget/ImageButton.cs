using Raylib_cs;
using System.Diagnostics;

namespace Engine.UI
{
	/// <summary>
	/// A button with a child {@link Image} to display an image. This is useful when the button must be larger than the image and the
	/// image centered on the button. If the image is the size of the button, a {@link Button} without any children can be used, where
	/// the {@link Button.ButtonStyle#up}, {@link Button.ButtonStyle#down}, and {@link Button.ButtonStyle#checked} nine patches define
	/// the image.
	/// </summary>
	public class ImageButton : Button
	{
		Image image;
		ImageButtonStyle style;


		public ImageButton(ImageButtonStyle style) : base(style)
		{
			image = new Image();
			image.SetScaling(Scaling.Fit);
			Add(image);
			SetStyle(style);
			SetSize(PreferredWidth, PreferredHeight);
		}

#if false
        public ImageButton(Skin skin, string styleName = null) : this(skin.Get<ImageButtonStyle>(styleName))
        { } 
#endif


        public ImageButton(IDrawable imageUp) : this(new ImageButtonStyle(null, null, null, imageUp, null, null))
		{ }


		public ImageButton(IDrawable imageUp, IDrawable imageDown) : this(new ImageButtonStyle(null, null, null,
			imageUp, imageDown, null))
		{ }


		public ImageButton(IDrawable imageUp, IDrawable imageDown, IDrawable imageOver) : this(
			new ImageButtonStyle(null, null, null, imageUp, imageDown, imageOver))
		{ }


		public override void SetStyle(ButtonStyle style)
		{
            Insist.IsTrue(style is ImageButtonStyle, "style must be a ImageButtonStyle");

			base.SetStyle(style);
			this.style = (ImageButtonStyle) style;
			if (image != null)
				UpdateImage();
		}


		public new ImageButtonStyle GetStyle()
		{
			return style;
		}


		public Image GetImage()
		{
			return image;
		}


		public Cell GetImageCell()
		{
			return GetCell(image);
		}


		private void UpdateImage()
		{
			IDrawable drawable = null;
			if (_isDisabled && style.ImageDisabled != null)
				drawable = style.ImageDisabled;
			else if (_mouseDown && style.ImageDown != null)
				drawable = style.ImageDown;
			else if (IsChecked && style.ImageChecked != null)
				drawable = (style.ImageCheckedOver != null && _mouseOver) ? style.ImageCheckedOver : style.ImageChecked;
			else if (_mouseOver && style.ImageOver != null)
				drawable = style.ImageOver;
			else if (style.ImageUp != null) //
				drawable = style.ImageUp;

			image.SetDrawable(drawable);
		}


		public override void Draw(float parentAlpha)
		{
			UpdateImage();
			base.Draw( parentAlpha);
		}
	}


	public class ImageButtonStyle : ButtonStyle
	{
		/** Optional. */
		public IDrawable ImageUp, ImageDown, ImageOver, ImageChecked, ImageCheckedOver, ImageDisabled;


		public ImageButtonStyle()
		{ }


		public ImageButtonStyle(IDrawable up, IDrawable down, IDrawable checkked, IDrawable imageUp,
		                        IDrawable imageDown, IDrawable imageChecked) : base(up, down, checkked)
		{
			ImageUp = imageUp;
			ImageDown = imageDown;
			ImageChecked = imageChecked;
		}


		public static ImageButtonStyle CreateRaylib(float minWidth,float minHeight,Texture2D texture)
		{
			var imgBtt = Create(
				Raylib.GetColor(0xc9c9c9ff), Raylib.GetColor(0x838383ff),
				Raylib.GetColor(0x97e8ffff), Raylib.GetColor(0x0492c7ff),
				Raylib.GetColor(0xc9effeff), Raylib.GetColor(0x5bb2d9ff),
				minWidth,minHeight,texture);
			return imgBtt;
		}
		public static ImageButtonStyle Create(
		Color upColor, Color outlineUpColor,
		Color downColor, Color outlineDownColor,
		Color overColor, Color outlineOverColor,
		float textureWidth,float textureHeight,Texture2D? texture)
		{
			var width = 100;
			var height = 20;
			var upDraw = new PrimitiveDrawable(width, height, upColor, outlineUpColor);
			var downDraw = new PrimitiveDrawable(width, height, downColor, outlineDownColor);
			var overDraw = new PrimitiveDrawable(width, height, overColor, outlineOverColor);
			var txtDraw = new TextureDrawable(textureWidth,textureHeight,texture);
			return new ImageButtonStyle
			{
				//Up = new PrimitiveDrawable(upColor),
				//Down = new PrimitiveDrawable(downColor),
				//Over = new PrimitiveDrawable(overColor)
				Up = upDraw,
				Down = downDraw,
				Over = overDraw,

				ImageUp = texture != null ? txtDraw: upDraw,
				ImageOver = texture != null ? txtDraw: overDraw,
				ImageDown = texture != null ? txtDraw : downDraw,
			};
		}


		public new ImageButtonStyle Clone()
		{
			return new ImageButtonStyle
			{
				Up = Up,
				Down = Down,
				Over = Over,
				Checked = Checked,
				CheckedOver = CheckedOver,
				Disabled = Disabled,

				ImageUp = ImageUp,
				ImageDown = ImageDown,
				ImageOver = ImageOver,
				ImageChecked = ImageChecked,
				ImageCheckedOver = ImageCheckedOver,
				ImageDisabled = ImageDisabled
			};
		}
	}
}