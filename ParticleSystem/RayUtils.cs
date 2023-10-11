using Raylib_cs;
using System.Text;
using System.Diagnostics;
using System.Numerics;

namespace Engine
{



    /// <summary>
	/// Ultilities for <see cref="Raylib"/>
	/// </summary>
    public static  class RayUtils
    {
		/// <summary>
		/// Get bound/rectange of texture
		/// </summary>
		/// <param name="texture">texture</param>
		/// <returns></returns>
        public static Rectangle Source(this Texture2D texture)
            => new Rectangle(0,0,texture.width,texture.height);

		/// <summary>
		/// Get resolution of the texture.
		/// </summary>
		/// <param name="texture">Texture</param>
		/// <returns>a vector2D of texture's resolution</returns>
        public static Vector2 Scale(this Texture2D texture)
            => new Vector2(texture.width,texture.height);

		/// <summary>
		/// Create rectangle utility
		/// </summary>
		/// <param name="position">Top left origin</param>
		/// <param name="scale">scale start from top-left origin</param>
		/// <returns>Ractangle</returns>
        public static Rectangle CreateRectanglePoint(Vector2 min, Vector2 max)
		{
			return CreateRectangle(min,max-min);
		}
        public static Rectangle CreateRectangle(Vector2 position, Vector2 scale)
            => new Rectangle(position.X,position.Y,scale.X,scale.Y);

        
        public static Rectangle MoveX(this Rectangle rec, float deltaX)
            => rec.Move(new Vector2(deltaX,0));
        public static Rectangle MoveY(this Rectangle rec, float deltaY)
            => rec.Move(new Vector2(0,deltaY));
        public static Rectangle Move(this Rectangle rec,float deltaX,float deltaY)
        {
            rec.x += deltaX;
            rec.y += deltaY;
            return rec;
        }
        public static Rectangle Move(this Rectangle rec,Vector2 offset)
        {
            rec.x += offset.X;
            rec.y += offset.Y;
            return rec;
        }
        public static Rectangle MoveTo(this Rectangle rec,Vector2 position)
        {
            rec.x = position.X;
            rec.y = position.Y;
            return rec;
        }
        public static Vector2 Scale(in this Rectangle rec)
            => new Vector2(rec.width, rec.height);
        public static Rectangle AddScale(this Rectangle rec,Vector2 scale)
        {
			rec.x += scale.X;
			rec.y += scale.Y;
			rec.width -= scale.X * 2;
			rec.height -= scale.Y * 2;
			return rec;
		}
        public static Rectangle AddScale(this Rectangle rec,float value)
        {
            rec.x += value;
            rec.y += value;
            rec.width -= value * 2;
            rec.height -= value * 2;
            return rec;
        }

		/// <summary>
		/// calculates the union of the two Rectangles. The result will be a rectangle that encompasses the other two.
		/// </summary>
		public static void Union(ref Rectangle value1, ref Rectangle value2, out Rectangle result)
		{
			result.x = Math.Min(value1.x, value2.x);
			result.y = Math.Min(value1.y, value2.y);

			var br1 = value1.BotRight();
			var br2 = value1.BotRight();

			result.width = Math.Max(br1.X, br2.X) - result.x;
			result.height = Math.Max(br1.Y, br2.Y) - result.y;
		}

		/// <summary>
		/// Update first to be the union of first and point
		/// </summary>
		/// <param name="first">First.</param>
		/// <param name="point">Point.</param>
		/// <param name="result">Result.</param>
		public static void Union(ref Rectangle first, ref Vector2 point, out Rectangle result)
		{
			var rect = new Rectangle(point.X, point.Y, 0, 0);
			Union(ref first, ref rect, out result);
		}

		#region Rectangle Vertex

		//---------------------------------------------------------------------------
		//			Vertice position setter
		//---------------------------------------------------------------------------
		/// <summary>
		/// Move vertex topleft from rectangle
		/// </summary>
		/// <param name="rectangle">source rectangle</param>
		/// <param name="point">point to expand top left</param>
		/// <returns></returns>
		public static Rectangle SetTopRight(this Rectangle rec, Vector2 point)
			=> rec.SetRight(point.X).SetTop(point.Y);
		public static Rectangle SetTopLeft(this Rectangle rec,Vector2 point)
			=> rec.SetLeft(point.X).SetTop(point.Y);
		public static Rectangle SetBotRight(this Rectangle rec,Vector2 point)
			=> rec.SetRight(point.X).SetBot(point.Y);
		public static Rectangle SetBotLeft(this Rectangle rec,Vector2 point)
			=> rec.SetLeft(point.X).SetBot(point.Y);


		public static Rectangle SetTop(this Rectangle rec, float value)
		{
			var moveError = value - rec.y;
			rec.y = value;
			rec.height -= moveError;
			return rec;
		}
		public static Rectangle SetLeft(this Rectangle rec,float value)
        {
			var moveError = value - rec.x;
			rec.x = value;
			rec.width -= moveError;
			return rec;
		}
		public static Rectangle SetBot(this Rectangle rec,float value)
        {
			var moveError = value - (rec.y + rec.height);
			rec.height += moveError;
			return rec;
        }
		public static Rectangle SetRight(this Rectangle rec, float value)
		{
			var moveError = value - (rec.x + rec.width);
			rec.width += moveError;
			return rec;
		}


		//---------------------------------------------------------------------------
		//			Vertice position getter
		//---------------------------------------------------------------------------
		public static Vector2 TopLeft(this Rectangle rec)
            => new Vector2(rec.x, rec.y);
        public static Vector2 TopRight(this Rectangle rec)
            => new Vector2(rec.x + rec.width, rec.y);
        public static Vector2 BotLeft(this Rectangle rec)
            => new Vector2(rec.x, rec.y + rec.height);
        public static Vector2 BotRight(this Rectangle rec)
            => new Vector2(rec.x + rec.width, rec.y + rec.height);
		public static Vector2 Center(this Rectangle rec)
			=>new Vector2(rec.x + rec.width / 2f, rec.y + rec.height/2f);

		public static bool IsQualify(Rectangle rec) => rec.width != 0 && rec.height != 0;
		public static bool IsQualify(in this Rectangle rec) => IsQualify(rec);
		public static Vector2 GetCoord01(Rectangle rec,Vector2 point)
		{
			Debug.Assert(rec.IsQualify(), " invalid rectangle");
			var min = rec.TopLeft();
			var max = rec.BotRight();
			return RaymathF.InverseLerp(min,max,point);
		}
		#endregion

        public static NPatchInfo CreateNPatchInfoPadded(Texture2D texture,int padding,NPatchLayout layout)
            => new NPatchInfo()
            {
                source = texture.Source(),
                top = padding,
                bottom = padding,
                left = padding,
                right = padding,
                layout = layout
            };
        public static Texture2D LoadScaledTextureFromImage(Image image,int scale)
        {
            var cloneImage = Raylib.ImageCopy(image);
            Raylib.ImageResizeNN(ref cloneImage, image.width * scale, image.height * scale);

            var texture = Raylib.LoadTextureFromImage(cloneImage);
            Raylib.UnloadImage(cloneImage);
            return texture;
        }
        public static Texture2D LoadScaledTexture(string fileName,int scale)
        {
            var image = Raylib.LoadImage(fileName);
            Raylib.ImageResizeNN(ref image,image.width * scale,image.height * scale);
            var texture = Raylib.LoadTextureFromImage(image);
            Raylib.UnloadImage(image);
            return texture;
        }
		public static Texture2D LoadTextureCropped(string fileName, Rectangle crop)
		{
			var image = Raylib.LoadImage(fileName);
			Raylib.ImageCrop(ref image, crop);
			var texture = Raylib.LoadTextureFromImage(image);
			Raylib.UnloadImage(image);
			return texture;
		}
		public static Texture2D Crop(Texture2D texture2D,Rectangle crop)
		{
			var image = Raylib.LoadImageFromTexture(texture2D);
			Raylib.ImageCrop(ref image,crop);
			var result = Raylib.LoadTextureFromImage(image);
			Raylib.UnloadImage(image);
			return result;
		}

		/// <summary>
		/// Set To Directory, If directory doesn't exist, then it will create 
		/// </summary>
		/// <param name="directoryName">folder name</param>
		public static void EnsureDirectory(string directoryName)
        {
            if (!Directory.Exists(@$"./{directoryName}")) Directory.CreateDirectory(directoryName);
            Directory.SetCurrentDirectory(directoryName);
        }

		/// <summary>
		/// Get elapsed time in seconds since <see cref="Raylib.InitWindow(int, int, string)"/>
		/// </summary>
        public static float GetTime()
            => (float)Raylib.GetTime() ;
		/// <summary>
		/// Open URL with default system browser (if available)
		/// </summary>
        public static void OpenURL(string url)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(url);

            unsafe
            {
                fixed (byte* p = bytes)
                {
                    sbyte* sp = (sbyte*)p;
                    Raylib_cs.Raylib.OpenURL(sp);
                }
            }
        }


		/// <summary>
		/// Draw strech-able texture
		/// </summary>
		/// <param name="texture">Texture2D</param>
		/// <param name="src1">Source top left</param>
		/// <param name="src2">Source top right</param>
		/// <param name="src3">Source bottom right</param>
		/// <param name="src4">Source bottom left</param>
		/// <param name="dest1"></param>
		/// <param name="dest2"></param>
		/// <param name="dest3"></param>
		/// <param name="dest4"></param>
		/// <param name="tintColor">tint Color</param>
		/// <param name="quadCount">quad subdivide count</param>
		public static void DrawTextureDynamicPro(Texture2D texture,
			Vector2 src1, Vector2 src2, Vector2 src3, Vector2 src4,
			Vector2 dest1, Vector2 dest2, Vector2 dest3, Vector2 dest4,
			Color tintColor, int quadCount)
		{
			Rlgl.rlCheckRenderBatchLimit(quadCount * 4);
			Rlgl.rlSetTexture(texture.id);

			Rlgl.rlBegin(DrawMode.QUADS);
			{
				//Set quad transform facing along z coord
				Rlgl.rlNormal3f(0f, 0f, 1f);
				Rlgl.rlColor4ub(tintColor.r, tintColor.g, tintColor.b, tintColor.a);

				
				float quadLerp = 1f / quadCount;
				for (int y = 0; y < quadCount; y++)
				{
					var lerpY = y * quadLerp;
					var nextlerpY = (y + 1) * quadLerp;

					//**************
					// Texture coords
					// ys = y coord source
					//**************
					Vector2 ysLeft = Vector2.Lerp(src4, src1, lerpY);
					Vector2 ysRight = Vector2.Lerp(src3, src2, lerpY);

					var ysNextLeft = Vector2.Lerp(src4, src1, nextlerpY);
					var ysNextRight = Vector2.Lerp(src3, src2, nextlerpY);

					//**************
					// Destination coords
					// yd = y coord destination
					//**************
					#region Destination

					Vector2 ydLeft = Vector2.Lerp(dest4, dest1, lerpY);
					Vector2 ydRight = Vector2.Lerp(dest3, dest2, lerpY);

					Vector2 ydNextLeft = Vector2.Lerp(dest4, dest1, nextlerpY);
					Vector2 ydNextRight = Vector2.Lerp(dest3, dest2, nextlerpY);
					#endregion

					for (int x = 0; x < quadCount; x++)
					{
						var lerpX = x * quadLerp;
						var nextLerpX = (x + 1) * quadLerp;

						var ps4 = Vector2.Lerp(ysLeft, ysRight, lerpX);
						var ps3 = Vector2.Lerp(ysLeft, ysRight, nextLerpX);
						var ps2 = Vector2.Lerp(ysNextLeft, ysNextRight, nextLerpX);
						var ps1 = Vector2.Lerp(ysNextLeft, ysNextRight, lerpX);

						var pd4 = Vector2.Lerp(ydLeft, ydRight, lerpX);
						var pd3 = Vector2.Lerp(ydLeft, ydRight, nextLerpX);
						var pd2 = Vector2.Lerp(ydNextLeft, ydNextRight, nextLerpX);
						var pd1 = Vector2.Lerp(ydNextLeft, ydNextRight, lerpX);


						///    source coord
						///   1 *-------------* 2
						///		| \           |
						///		|   \         |
						///		|     \.      |
						///		|        \    |
						///		|			\ |	
						///   4 *-------------* 3

						///    destination coord
						///   4 *-------------* 3
						///		|             |
						///		|             |
						///		|             |
						///		|             |
						///		|			  |	
						///   1 *-------------* 2


						//Bottom left( point 1)
						Rlgl.rlTexCoord2f(ps1.X, ps1.Y);
						Rlgl.rlVertex2f(pd1.X, pd1.Y);

						//Bottom right( point 2)
						Rlgl.rlTexCoord2f(ps2.X, ps2.Y);
						Rlgl.rlVertex2f(pd2.X, pd2.Y);

						//Top right( point 3)
						Rlgl.rlTexCoord2f(ps3.X, ps3.Y);
						Rlgl.rlVertex2f(pd3.X, pd3.Y);

						//Top left( point 4)
						Rlgl.rlTexCoord2f(ps4.X, ps4.Y);
						Rlgl.rlVertex2f(pd4.X, pd4.Y);


						Rlgl.rlSetTexture(0);

					}//Ent of loop x

				}//end of loop y

			}Rlgl.rlEnd();
		}

		public static void DrawRectangleLinesPro(Rectangle rectangle, Vector2 org,float rotation,float lineWidth,Color color)
		{
			var pos = rectangle.TopLeft();
			rectangle = rectangle.Move(-org);
			rotation = rotation * Raylib.DEG2RAD;

			var v0 = rectangle.TopLeft();
			var v1 = rectangle.TopRight();
			var v2 = rectangle.BotRight();
			var v3 = rectangle.BotLeft();

			v0 = RaymathF.Vector2Rotate(pos ,v0,rotation);
			v1 = RaymathF.Vector2Rotate(pos ,v1,rotation);
			v2 = RaymathF.Vector2Rotate(pos ,v2,rotation);
			v3 = RaymathF.Vector2Rotate(pos ,v3,rotation);

			Raylib.DrawLineEx(v0,v1,lineWidth,color);
			Raylib.DrawLineEx(v1,v2,lineWidth,color);
			Raylib.DrawLineEx(v2,v3,lineWidth,color);
			Raylib.DrawLineEx(v3,v0,lineWidth,color);
		}

		public static Rectangle NegateWidth(in this Rectangle rec)
			=> new Rectangle(rec.x,rec.y,-rec.width,rec.height);
		public static Rectangle NegateHeight(in this Rectangle rec)
			=> new Rectangle(rec.x, rec.y, rec.width, -rec.height);

		public static char CharToShiftedUSSymbol(char c)
        {
			return c switch
            {
                '1' => '!',
                '2' => '@',
                '3' => '#',
                '4' => '$',
                '5' => '%',
                '6' => '^',
                '7' => '&',
                '8' => '*',
                '9' => '(',
                '0' => ')',
                '-' => '_',
                '=' => '+',
                '[' => '{',
                ']' => '}',
                '\\' => '|',
                ';' => ':',
                '\'' => '"',
                ',' => '<',
                '.' => '>',
                '/' => '?',
                _ => c,
            };
        }


	}
}
