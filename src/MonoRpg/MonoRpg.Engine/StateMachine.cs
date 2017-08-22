namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;

    public class StateMachine {
        public State Current { get; set; }

        private static readonly State Empty  = new State("empty");

        public Dictionary<string, Func<State>> States { get; set; }
        public StateMachine(Dictionary<string, Func<State>> states) {
            States = states ?? new Dictionary<string, Func<State>>();
            Current = Empty;
        }

        public void Change(string stateName, EnterParameters enterParams=null) {
            Debug.Assert(States.ContainsKey(stateName));
            Current.Exit();
            Current = States[stateName]();
            Current.Enter(enterParams);
        }

        public void Update(float dt) {
            Current.Update(dt);
        }

        public void Render(Renderer renderer) {
            Current.Render(renderer);
        }
        
    }

    

    
}