using System.Numerics;
using Raylib_cs;
using Engine;

namespace Engine
{
    public class Entity
    {
		public string name;
		public Transformation Transform { get; }
		public Entity(string name)
        {
			this.name = name;
			Transform = new Transformation(this);
        }

		public void Draw(Color color)
        {
			color =  Transform.Parent == null ? Raylib.Fade(color,0.3f) : color;
			var pos = Transform.Position.ToVec2();
			var localpos = Transform.LocalPosition;
			var rot = Transform.GetEulerRotation().Z;
			var localRot = ToEulerAngles(  Transform.LocalRotation).Z;
			var angle =  (rot * Raylib.RAD2DEG);

			if (angle < 0) angle = 360+ angle;

			var localAngle = localRot * Raylib.RAD2DEG;
			var rec = new Rectangle(Transform.Position.X, Transform.Position.Y, 20, 20);
			Raylib.DrawRectanglePro(rec,new Vector2(10),rot * Raylib.RAD2DEG,color);
			Raylib.DrawCircleV(Transform.GetPosition().ToVec2(), 1, Color.RED);

			Raylib.DrawTextEx(Raylib.GetFontDefault(), pos.ToString(), pos.ToVec2(), 20, 1, color);
			Raylib.DrawTextEx(Raylib.GetFontDefault(), angle.ToString(), pos.ToVec2() + new Vector2(0,40), 20, 1, color);

			Raylib.DrawTextEx(Raylib.GetFontDefault(), localpos.ToString(), pos.ToVec2() + new Vector2(100,40), 15, 1, color);
			Raylib.DrawTextEx(Raylib.GetFontDefault(), localAngle.ToString(), pos.ToVec2() + new Vector2(100,90), 15, 1, color);

			Raylib.DrawLineV(pos, Vector3.Transform(new Vector3(0,50,0),Transform.LocalToWorld).ToVec2(),Color.GREEN);
			Raylib.DrawLineV(pos, Vector3.Transform(new Vector3(50,0,0),Transform.LocalToWorld).ToVec2(),Color.RED);
		}

		/// <summary>
		/// Credit <see href="https://stackoverflow.com/questions/70462758/c-sharp-how-to-convert-quaternions-to-euler-angles-xyz"/> 
		/// </summary>
		/// <param name="q"></param>
		/// <returns></returns>
		public static Vector3 ToEulerAngles(Quaternion q)
		{
			Vector3 angles = new();

			// roll / x
			double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
			double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
			angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

			// pitch / y
			double sinp = 2 * (q.W * q.Y - q.Z * q.X);
			if (Math.Abs(sinp) >= 1)
			{
				angles.Y = (float)Math.CopySign(Math.PI / 2, sinp);
			}
			else
			{
				angles.Y = (float)Math.Asin(sinp);
			}

			// yaw / z
			double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
			double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
			angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

			return angles;
		}
	}
}