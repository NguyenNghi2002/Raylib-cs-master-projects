using Engine;
using Raylib_cs;

namespace Engine
{
    public interface ITimer
    {
        void Reset();
        void Stop();
        object Context { get; }

    }

    public class Timer : IPoolable, ITimer
    {
        public object Context { get; set; }
        Action<Timer> _timeoutAction;
        bool _isDone;
        bool _isRepeat;
        float _duration;
        float _elapseTime;

        void IPoolable.Reset()
        {
            _timeoutAction = null;
            _isDone = true;
            _duration = 0;
            _elapseTime = 0;
        }
        public void Reset()
        {
            _elapseTime = 0;
        }

        internal bool Tick()
        {
            if (!_isDone && _elapseTime > _duration)
            {
                _elapseTime %= _duration;
                _timeoutAction.Invoke(this);

                if (!_isDone && !_isRepeat)
                    _isDone = true;
            }

            _elapseTime += Raylib.GetFrameTime();

            return _isDone;
        }

        internal void Setup(float duration, bool repeat, object context, Action<Timer> timeOutAction)
        {
            _duration = duration;
            _isRepeat = repeat;
            Context = context;
            _timeoutAction = timeOutAction;

            _isDone = false;
        }

        public void Stop()
        {
            _isDone = true;
        }

        public T GetContext<T>() => (T)Context;
    }

    public class TimerManager 
    {
        List<Timer> _timers = new List<Timer>();

        public int UpdateOrder { get; set; } = 0;

        public void Update()
        {
            for (int i = _timers.Count - 1; i >= 0; i--)
            {
                Timer timer = _timers[i];
                if (timer.Tick())
                {
                    Pool<Timer>.Free(timer);
                    _timers.RemoveAt(i);
                }
            }
        }
        public ITimer Schedule(float duration, bool repeat, object context, Action<ITimer> onTimeout)
        {
          var timer = Pool<Timer>.Obtain();
            timer.Setup(duration, repeat, context, onTimeout);
            _timers.Add(timer);
            return timer;
        }

    }
}