using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
    protected List<State> states;
    protected State currentState;
    protected State defaultState;

    protected class State
    {
        protected bool hasExitTime;
        protected float exitTime;
        protected float exitTimer;

        public State()
        {
            hasExitTime = false;
            exitTime = 0;
        }

        public State(float exitTime)
        {
            if (exitTime >= 0)
            {
                hasExitTime = true;
                this.exitTime = exitTime;
            }
            else
            {
                hasExitTime = false;
                this.exitTime = 0;
            }
        }

        public virtual void StateEnter() {}

        public virtual void StateUpdate() {}

        public virtual void StateExit() {}
    }

    protected class StateTransition
    {
        private StateMachine stateMachine;
        private State toState;

        public StateTransition (StateMachine stateMachine, State toState)
        {
            this.stateMachine = stateMachine;
            this.toState = toState;
        }

        public void Execute()
        {
            stateMachine.currentState.StateExit();
            stateMachine.currentState = toState;
            stateMachine.currentState.StateEnter();
        }
    }
}
