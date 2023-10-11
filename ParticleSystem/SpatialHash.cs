using Engine;
using Raylib_cs;
using Engine;
using System.Collections;
using System.Net;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace Test
{
    public interface ISpatialHashable
    {
        public Rectangle Bounds {get;}
        
    }
    public class SpatialHash <THashObject> where THashObject : ISpatialHashable
    {
        public Rectangle GridBounds = default;
        CellStorage<THashObject> _cellDatas = new CellStorage<THashObject>();
        List<Rectangle> recs = new List<Rectangle>();
        public HashSet<THashObject> Objects => _cellDatas.Values;

        


        private float _cellSize;
        public SpatialHash (float cellSize = 10f)
        {
            _cellSize = cellSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="createIfEmpty"></param>
        /// <returns>referenced collection of data from cell</returns>
        public List<THashObject>? GetCellAtPoint(Vector2 vector2, bool createIfEmpty = false)
            =>GetCellAtPoint(vector2.X, vector2.Y, createIfEmpty);  
        public List<THashObject>? GetCellAtPoint(float x, float y, bool createIfEmpty = false)
        {
            List<THashObject> cell = null;
            if (Contain(x, y))
            {
                int locX =  (int)(x / _cellSize);
                int locY = (int)(y / _cellSize);
                if (!_cellDatas.TryGetValue(locX, locY, out cell)
                    && createIfEmpty)
                {
                    cell = new List<THashObject>();
                    _cellDatas.Add(locX,locY,cell);
                }
            }
            return cell;
        }
        public List<THashObject> GetCellAtLocation(int x, int y, bool createIfEmpty = false)
        {
            List<THashObject> cellDatas = null;
            if (!_cellDatas.TryGetValue(x, y, out cellDatas)
                    && createIfEmpty)
            {
                cellDatas = new List<THashObject>();
                _cellDatas.Add(x, y, cellDatas);
            }
            return cellDatas;
        }

        public HashSet<THashObject> GetOverlapObjects(Rectangle bounds)
        {
            var objectListInCells = GetCellObject(bounds);

            var objs = from obj in objectListInCells
                       where Raylib.CheckCollisionRecs(bounds,obj.Bounds)
                       select obj;

            return objs.ToHashSet();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns>Objects in cell that colide with given bounds</returns>
        public HashSet<THashObject> GetCellObject(Rectangle bounds)
        {
            var topLeft = bounds.TopLeft();
            var botRight = bounds.BotRight();

            var TLLocation = ToLocation(topLeft);
            var BRLocation = ToLocation(botRight);
            HashSet<THashObject> _temp = new HashSet<THashObject>();
            for (int y = TLLocation.Y; y <= BRLocation.Y; y++)
            {
                for (int x = TLLocation.X; x <= BRLocation.X; x++)
                {
                    List<THashObject> list = GetCellAtLocation(x, y, false);
                    if (list is null) continue;
                    _temp.UnionWith(list);
                }
            }
            return _temp;
        }

        VectorInt2 ToLocation(Vector2 point)
            => new VectorInt2((int)(point.X/_cellSize),(int)(point.Y/_cellSize) ) ;

        public void Register(THashObject value)
        {
            var selectCell = value.Bounds;
            var topLeft = selectCell.TopLeft();
            var botRight = selectCell.BotRight();


            if (_cellDatas.Count == 0)
                GridBounds = selectCell;
            else
            {
                if (!Contain(topLeft))
                    Union(GridBounds, topLeft, out GridBounds);

                if (!Contain(botRight))
                    Union(GridBounds, botRight, out GridBounds);
            }

            var TLLocation = ToLocation(topLeft);
            var BRLocation = ToLocation(botRight);

            for (int y = TLLocation.Y; y <= BRLocation.Y ; y++)
                for (int x = TLLocation.X; x <= BRLocation.X ; x++)
                {
                    List<THashObject> c = GetCellAtLocation(x,y,true);
                    c.Add(value);

                }

        }
        public bool DeList(THashObject value)
        {
            bool removed = false;
            var selectCell = value.Bounds;

            var topLeft = selectCell.TopLeft();
            var botRight = selectCell.BotRight();

            var TLLocation = ToLocation(topLeft);
            var BRLocation = ToLocation(botRight);

            HashSet<THashObject> valuesList = _cellDatas.Values;
            for (int y = TLLocation.Y; y <= BRLocation.Y; y++)
            {
                for (int x = TLLocation.X; x <= BRLocation.X; x++)
                {
                    List<THashObject> list = GetCellAtLocation(x,y,false);
                    if (list is null) continue;

                    if (list.Remove(value))
                    {
                        if (list.Count == 0) _cellDatas.RemoveCellAt(x,y);
                        removed = true;
                    }
                }
            }
            return removed;
        }
        public bool Contain(float x, float y)
            => Contain(new Vector2(x,y));
        public bool Contain(Vector2 point)
        {
            return Raylib.CheckCollisionPointRec(point,GridBounds);
        }

        internal void Draw()
        {
            Raylib.DrawRectangleLinesEx(GridBounds, 2, Color.RED);
            foreach (var item in recs)
            {
                Raylib.DrawRectangleRec(item, Color.YELLOW);
            }
            foreach (var cell in _cellDatas)
            {
                var absPos = new Vector2(cell.Key.X, cell.Key.Y) * _cellSize;
                var rec = new Rectangle(absPos.X, absPos.Y, _cellSize, _cellSize);

                Raylib.DrawRectangleLinesEx(rec, 2, Color.GRAY);
                Raylib.DrawTextEx(Raylib.GetFontDefault(), cell.Key.ToString(), absPos, 10, 2, Color.WHITE);
                Raylib.DrawTextEx(Raylib.GetFontDefault(), $"Count: {cell.Value.Count}", absPos.MoveY(10), 10, 2, Color.WHITE);
                for (int i = 0; i < cell.Value.Count; i++)
                {
                    Raylib.DrawTextEx(Raylib.GetFontDefault(), $"{cell.Value[i]}", absPos.Move(_cellSize / 2f).MoveY(i * 10), 10, 2, Color.WHITE);
                }
            }
#if false
#endif
        }



        /// <summary>
        /// calculates the union of the two Rectangles. The result will be a rectangle that encompasses the other two.
        /// </summary>
        public static void Union(Rectangle rec, Rectangle other, out Rectangle result)
        {
            result.x = Math.Min(rec.x, other.x);
            result.y = Math.Min(rec.y, other.y);

            var br1 = rec.BotRight();
            var br2 = other.BotRight();

            result.width = Math.Max(br1.X, br2.X) - result.x;
            result.height = Math.Max(br1.Y, br2.Y) - result.y;
        }

        /// <summary>
        /// Update first to be the union of first and point
        /// </summary>
        /// <param name="rec">First.</param>
        /// <param name="point">Point.</param>
        /// <param name="result">Result.</param>
        public static void Union(Rectangle rec, Vector2 point, out Rectangle result)
        {
            var rect = new Rectangle(point.X, point.Y, 0, 0);
            Union(rec, rect, out result);
        }
    }

    /// <summary>
    /// Storage hold reference to Value
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class CellStorage<TValue> :IEnumerable<KeyValuePair<CellStorage<TValue>.Location, List<TValue>>>
    {
        public struct Location : IEquatable<Location>
        {
            public int X, Y;

            public Location(float x, float y) : this((int)x, (int)y) { }
            public Location(int x, int y)
            {
                X = x;
                Y = y;
            }

            public bool Equals(CellStorage<TValue>.Location other)
            {
                return X == other.X && Y == other.Y;
            }
            public void Set(int x, int y)
            {
                X = x;
                Y = y;
            }

            public override string ToString()
            {
                return X.ToString() + " " + Y.ToString();
            }
        }

        private Location _location = new Location();

        public Dictionary<Location, List<TValue>> _storage = new Dictionary<Location, List<TValue>>();
        public Dictionary< List<TValue>,Location> _storageReverse = new Dictionary< List<TValue>,Location>();

        public int Count => _storage.Count;
        public HashSet<TValue> Values
        {
            get
            {
                HashSet<TValue> set = new HashSet<TValue>();

                foreach (var list in _storage.Values)
                {
                    set.UnionWith(list);
                }
                return set;
            }
        }
        public Dictionary<Location, List<TValue>>.KeyCollection Keys => _storage.Keys;


        public bool TryGetValue(int x, int y, out List<TValue> value)
        {
            _location.Set(x, y);
            return _storage.TryGetValue(_location, out value);
        }
        public void Add(int x, int y, List<TValue> value)
        {
            _location.Set(x, y);
            _storage.Add(_location, value);
            _storageReverse.Add(value, _location);
        }
        public bool TryAdd(int x, int y, List<TValue> value)
        {
            _location.Set(x, y);

            var added = _storage.TryAdd(_location, value);
            if (added)
            {
                _storageReverse.Add(value, _location) ;
            }
            return added;
        }

        public bool RemoveCellAt(int x, int y)
        {
            _location.Set(x, y);
            var removed = _storage.Remove(_location, out var valueList);
            if (removed)
            {
                _storageReverse.Remove(valueList);
            }
            return removed;
        }

        public Location GetLocation(List<TValue> value)
        {
            _storageReverse.TryGetValue(value,out _location);
            return _location;
        }

        public void Clear()
        {
            _storageReverse.Clear();
            _storage.Clear();
        }

        public IEnumerator<KeyValuePair<Location, List<TValue>>> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

    }
}
