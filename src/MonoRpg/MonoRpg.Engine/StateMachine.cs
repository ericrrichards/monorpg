namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;

    using MonoRpg.Engine.GameStates;
    using MonoRpg.Engine.RenderEngine;

    public class StateMachine {
        public IStateObject Current { get; set; }

        private static readonly State Empty  = new State("empty");

        public Dictionary<string, Func<IStateObject>> States { get; set; }
        public StateMachine(Dictionary<string, Func<IStateObject>> states) {
            States = states ?? new Dictionary<string, Func<IStateObject>>();
            Current = Empty;
        }

        public void Change(string stateName, EnterArgs enterParams=null) {
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