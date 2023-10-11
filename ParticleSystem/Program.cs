using System.Numerics;
using Raylib_cs;
using Engine;
using Engine;
using Test;
using QuickGraph;
using System.Diagnostics;
namespace Test
{

	public class GridCell
	{
		public Grid Grid;
		public VectorInt2 Location;
		public Vector2 Scale;
		public Rectangle Bound;

		public Vector2 FlowDirection;
		public int Cost;
		public int BestCost;

		public Vector2 LocalPosition => new Vector2(Location.X * Scale.X, Location.Y * Scale.Y);
		public Vector2 TopLeft => Grid.Position + LocalPosition;
		public Vector2 Center => Grid.Position + LocalPosition + Scale / 2f;
		internal GridCell(Grid grid, VectorInt2 location, Vector2 cellScale)
		{
			this.Grid = grid;
			this.Location = location;
			this.Scale = cellScale;

			Cost = 1;
			BestCost = grid.CostLimit;
		}

	}

	/// <summary>
	/// This class belong to <see cref="Grid"/>,ovrride this class
	/// </summary>
	public class Grid
	{
		public int CostLimit = byte.MaxValue;
		private readonly int _cols, _rows;
		private float _gidWidth, _gidHeight;

		internal GridCell[] Cells;

		private Vector2 _position;
		private Rectangle _bound;

		public GridCell Destination { get; private set; }

		public Vector2 Position
		{
			get => _position;
			set => SetPosition(value);
		}
		public int CellCount => _cols * _rows;

		#region Getters
		public float Width => _cols * _gidWidth;
		public float Height => _rows * _gidHeight;
		public int Rows => _rows;
		public int Columns => _cols;
		public GridCell[] GidNodes => Cells;
		public event Action<Grid> OnGridLoaded;
		#endregion

		public void LoadGrid()
		{
			Cells = new GridCell[CellCount];
			for (int i = 0; i < CellCount; i++)
			{
				var locX = Index2LocX(i);
				var locY = Index2LocY(i);
				Cells[i] = new GridCell(this, new VectorInt2(locX, locY), new Vector2(_gidWidth, _gidHeight));
			}
			OnGridLoaded?.Invoke(this);
		}
		public Grid(int cols, int rows, float gidWidth, float gidHeight)
		{
			_cols = cols;
			_rows = rows;
			_gidWidth = gidWidth;
			_gidHeight = gidHeight;

			LoadGrid();

			UpdateGridTransform();
		}

		public Grid SetPosition(Vector2 position)
		{
			_position = position;
			UpdateGridTransform();
			return this;
		}
		public void DrawLine(float lineWidth, Color color)
		{
			for (int i = 0; i < Cells.Length; i++)
			{
				var cell = Cells[i];
				var rec = Cells[i].Bound;
				var center = rec.Center();

				Raylib.DrawRectangleLinesEx(rec, lineWidth, color);
				//Raylib.DrawRectangleRec(rec,Color.RED.Alpha((byte)(0.7f*RaymathSharp.Remap(cell.BestCost,0,CostLimit,255,0))) );
				Raylib.DrawLineV(rec.Center(), rec.Center() + cell.FlowDirection.Normalize() * 20f, Color.GREEN);
				Raylib.DrawCircleV(center, 2f, Color.WHITE);
				Raylib.DrawTextEx(Raylib.GetFontDefault(), Cells[i].Cost.ToString(), rec.TopLeft(), 10, 2, Color.GREEN);
				Raylib.DrawTextEx(Raylib.GetFontDefault(), Cells[i].BestCost.ToString(), rec.TopLeft().MoveY(10), 10, 2, Color.GREEN);
			}
		}

		public enum NeighborDirection
		{
			NEAR = 0,
			ADJACENT,
		}
		public List<GridCell> GetNeighbors(GridCell cell, NeighborDirection cellDirection)
		{
			List<GridCell> founds = new List<GridCell>();
			if (!Cells.Contains(cell)) return founds;
			var loc = cell.Location;
			for (int y = -1; y <= 1; y++)
			{
				for (int x = -1; x <= 1; x++)
				{
					switch (cellDirection)
					{
						case NeighborDirection.NEAR:
							if (x == 0 && y == 0) continue;
							break;
						case NeighborDirection.ADJACENT:
							if (Math.Abs(x) == Math.Abs(y)) continue;
							break;
						default:
							break;
					}
					var locX = loc.X + x;
					var locY = loc.Y + y;
					if (locX >= 0 && locX < Columns && locY >= 0 && locY < Rows)
					{
						int index = LocToIndex(locX, locY);
						if (index >= 0 && index < Cells.Length)
						{
							founds.Add(Cells[index]);
						}
					}
				}
			}
			return founds;
		}

		/// <summary>
		/// Get single gid from point that inside a gid bound
		/// </summary>
		/// <param name="point">point reflect on grithe grid.</param>
		/// <returns><see cref="GridCell"/> of <see cref="Grid"/></returns>
		public GridCell? GetCellAt(Vector2 point)
			=> TryCellGidAt(point, out GridCell node) ?
				node : null;

		/// <summary>
		/// Get single gid from point that inside a gid bound
		/// </summary>
		/// <param name="point"></param>
		/// <param name="gidNode"></param>
		/// <returns></returns>
		public bool TryCellGidAt(Vector2 point, out GridCell gidNode)
		{
			gidNode = null;
			try
			{
				if (Contain(point))
				{
					var localPoint = point - _position;
					int locX = (int)(localPoint.X / _gidWidth);
					int locY = (int)(localPoint.Y / _gidHeight);
					int index = LocToIndex(locX, locY);
					gidNode = Cells[index];
					if (Raylib.CheckCollisionPointRec(point, gidNode.Bound))
						return true;
				}
			}
			catch (Exception)
			{

			}

			return false;
		}

		public bool Contain(Vector2 point)
			=> Raylib.CheckCollisionPointRec(point, _bound);

		private void UpdateGridTransform()
		{
			for (int i = 0; i < Cells.Length; i++)
			{
				var worldPos = Cells[i].LocalPosition + _position;
				Cells[i].Bound = new Rectangle(worldPos.X, worldPos.Y, _gidWidth, _gidHeight);

			}
			_bound = new Rectangle(Position.X, Position.Y, Width, Height);
		}

        #region Untils
        public int Index2LocX(int index)
            => index % Columns;
        public int Index2LocY(int index)
            => index / Columns;
        public int LocToIndex(VectorInt2 location) => LocToIndex(location.X, location.Y);
        public int LocToIndex(int x, int y)
        {
            var xQualified = x >= 0 && x < Width;
            var yQualified = y >= 0 && y < Height;
            if (xQualified && yQualified)
                return x + y * _cols;
            else
                return -1;
        } 
        #endregion


        private void GenerateCostField(GridCell destinationCell, NeighborDirection direction, Action<GridCell, GridCell> action)
		{
			if (!Cells.Contains(destinationCell)) return;



			destinationCell.BestCost = 0;

			Queue<GridCell> frontier = new Queue<GridCell>();
			frontier.Enqueue(destinationCell);

			while (frontier.Count() > 0)
			{
				GridCell curr = frontier.Dequeue();
				List<GridCell> neighbors = GetNeighbors(curr, direction);

				foreach (var neighCell in neighbors)
				{

					if (neighCell.Cost == CostLimit) continue;
					int moveCost = neighCell.Cost + curr.BestCost;
					if (moveCost < neighCell.BestCost)
					{
						action?.Invoke(curr, neighCell);
						neighCell.BestCost = moveCost;
						frontier.Enqueue(neighCell);
					}
				}
			}
		}
		public void GenerateFlowField(GridCell target, NeighborDirection direction)
		{
			foreach (var cell in Cells)
			{
				cell.BestCost = CostLimit;
				cell.FlowDirection = Vector2.Zero;
			}
			try
			{
				Destination = target;
				target.FlowDirection = Vector2.Zero;
				///Low to high
				GenerateCostField(target, direction,
					(low, high) =>
					{

						high.FlowDirection = Vector2.Normalize(low.Bound.Center() - high.Bound.Center());
					});
			}
			catch (Exception)
			{

			}
		}

	}



    public sealed class Program
    {


        public class Collid : ISpatialHashable
        {
			private Rectangle rectangle;
			public Rectangle Bounds => rectangle;

			public Collid()
            {
				rectangle = 	RayUtils.CreateRectangle(Raylib.GetMousePosition(),new Vector2(Random.Shared.Next(20,150), Random.Shared.Next(20, 150)));
            }

            public override string ToString()
            {
				return rectangle.ToString();
            }
        }
        public static void Main()
        {
			var spatialHash = new SpatialHash<Collid>(100);
			var track = new List<Vector2>();
			Raylib.InitWindow(1280,720,"Raylib");

			
            while (!Raylib.WindowShouldClose())
            {
				var mouse = Raylib.GetMousePosition();
				var rec = RayUtils.CreateRectangle(mouse, new Vector2(20));

				if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
                {
					track.Add(mouse);
					spatialHash.Register(new Collid());
                }
				if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT))
                {
					var collider = RayUtils.CreateRectangle(mouse, new Vector2(20));
                    Console.WriteLine(spatialHash.Objects.Count);
                    foreach (var item in spatialHash.Objects)
                    {
						Console.WriteLine("      --" +item);

						if (Raylib.CheckCollisionRecs(collider,item.Bounds))
							spatialHash.DeList(item);

                    }

                }

				Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.BLACK);
                {
                    spatialHash.Draw();
                    rec = RayUtils.CreateRectangle(mouse, new Vector2(20));
                    Raylib.DrawRectangleRec(rec, Color.YELLOW);

                    foreach (Collid item in spatialHash.Objects)
                    {
                        Raylib.DrawRectangleRec(item.Bounds, Color.GRAY);
                    }
					var a = spatialHash.GetOverlapObjects(rec);
                    if (a.Count != 0)
                    {
						var count = 0;
                        foreach (Collid item in a)
                        {
							count++;
							Raylib.DrawRectangleRec(item.Bounds, Color.GREEN);
                        }

							Console.WriteLine(count);

                    }
					
					Raylib.DrawFPS(10,10);
				}
				Raylib.EndDrawing();

            }
			Raylib.CloseWindow();
        }
    }
}