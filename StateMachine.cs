using System;
using System.Collections.Generic;
using System.Text;

namespace StateMachine
{
  public class StateMachine<T> where T : struct
  {
    class StateInternal
    {
      public T State;
      public HashSet<T> Accept;

      public event Action Enter;
      public event Action Exit;

      public bool Can(T state)
      {
        return Accept.Contains(state);
      }

      public void OnEnter()
      {
        if(Enter != null)
          Enter();
      }

      public void OnExit()
      {
        if(Exit != null)
          Exit();
      }

      public StateInternal(T state)
      {
        this.State = state;
        this.Accept = new HashSet<T>();
      }
    }

    class TransitionTable : Dictionary<T, StateInternal>
    {
      public StateInternal Add(T from, T to)
      {
        StateInternal state;
        if(!this.TryGetValue(from, out state))
        {
          state = new StateInternal(from);
          this.Add(from, state);
        }

        if (!state.Can(to))
          state.Accept.Add(to);

        Add(to);

        return state;
      }

      public StateInternal Add(T to)
      {
          StateInternal state;
          if (!this.TryGetValue(to, out state))
          {
              state = new StateInternal(to);
              this.Add(to, state);
          }
          return state;
      }
    }

    private StateInternal state;
    private TransitionTable transitionTable;

    public T State {
      get
      {
        return state.State;
      }
      set
      {
        if (state.State.Equals(value))
            return;

        if(!state.Can(value))
        {
          throw new ArgumentException(String.Format("Cannot transition from {0} to {1}", state.State, value));
        }
        else
        {
          state.OnExit();
          state = transitionTable[value];
          state.OnEnter();
        }
      }
    }

    public void Valid(T from, T to)
    {
      if (from.Equals(to))
        return;

      transitionTable.Add(from, to);
    }

    public void Valid(T[] from, T to)
    {
      for(int index = 0; index < from.Length; index++) {
        Valid(from[index], to);
      }
    }

    public void Valid(T from, T[] to)
    {
      for(int index = 0; index < to.Length; index++) {
        Valid(from, to[index]);
      }
    }

    public void ValidTwoWay(T state1, T state2)
    {
      Valid(state1, state2);
      Valid(state2, state1);
    }

    public void ValidAny()
    {
      Type typeT = typeof(T);
      if (!typeT.IsEnum)
        throw new InvalidOperationException("T must be an enum.");

      foreach (T state1 in Enum.GetValues(typeT))
        foreach (T state2 in Enum.GetValues(typeT))
          ValidTwoWay(state1, state2);
    }

    public void DoWhen(T state, Action action)
    {
      transitionTable[state].Enter += action;
    }

    public void DoWhenAny(Action<T> action)
    {
      foreach (var pair in transitionTable)
      {
          T myval = pair.Key;
          pair.Value.Enter += delegate
          {
              action(myval);
          };
      }
    }

    public void DoFollowing(T state, Action action)
    {
      transitionTable[state].Exit += action;
    }

    public void DoFollowingAny(Action<T> action)
    {
        foreach (var pair in transitionTable)
        {
            T myval = pair.Key;
            pair.Value.Exit += delegate
            {
                action(myval);
            };
        }
    }

    public StateMachine(T initial)
    {
      this.transitionTable = new TransitionTable();
      this.state = this.transitionTable.Add(initial);
    }
  }
}

