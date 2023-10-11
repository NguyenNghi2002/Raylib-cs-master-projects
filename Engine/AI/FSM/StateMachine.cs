using System;
using System.Collections.Generic;


namespace Engine.AI.FSM
{
	public class StateMachine<TContext>
	{
		public event Action OnStateChanged;

		public State<TContext> CurrentState => _currentState;

		public State<TContext> PreviousState;
		public float ElapsedTimeInState = 0f;

		protected State<TContext> _currentState;
		protected TContext _context;
		Dictionary<Type, State<TContext>> _states = new Dictionary<Type, State<TContext>>();


		public StateMachine(TContext context, State<TContext> initialState)
		{
			_context = context;

			// setup our initial state
			AddState(initialState);
			_currentState = initialState;
			_currentState.Begin();
		}


		/// <summary>
		/// adds the state to the machine
		/// </summary>
		public void AddState(State<TContext> state)
		{
			state.SetMachineAndContext(this, _context);
			_states[state.GetType()] = state;
		}


		/// <summary>
		/// ticks the state machine with the provided delta time
		/// </summary>
		public virtual void Update(float deltaTime)
		{
			ElapsedTimeInState += deltaTime;
			_currentState.Reason();
			_currentState.Update(deltaTime);
		}

		/// <summary>
		/// Gets a specific state from the machine without having to
		/// change to it.
		/// </summary>
		public virtual R GetState<R>() where R : State<TContext>
		{
			var type = typeof(R);
			var errorMsg = string.Format("{0}: state {1} does not exist. Did you forget to add it by calling addState?", GetType(), type);
			Insist.IsTrue(_states.ContainsKey(type),errorMsg);

			return (R)_states[type];
		}


		/// <summary>
		/// changes the current state
		/// </summary>
		public R ChangeState<R>() where R : State<TContext>
		{
			// avoid changing to the same state
			var newType = typeof(R);
			if (_currentState.GetType() == newType)
				return _currentState as R;

			// only call end if we have a currentState
			if (_currentState != null)
				_currentState.End();


			var errorMsg = string.Format("{0}: state {1} does not exist. Did you forget to add it by calling addState?", GetType(), newType);
			Insist.IsTrue(_states.ContainsKey(newType), errorMsg);

			// swap states and call begin
			ElapsedTimeInState = 0f;
			PreviousState = _currentState;
			_currentState = _states[newType];
			_currentState.Begin();

			// fire the changed event if we have a listener
			if (OnStateChanged != null)
				OnStateChanged();

			return _currentState as R;
		}
	}
}