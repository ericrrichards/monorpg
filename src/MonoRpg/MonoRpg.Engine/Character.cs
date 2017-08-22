namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;

    public class Character {
        public Entity Entity { get; set; }
        public WaitState WaitState { get; }
        public MoveState MoveState { get; }
        public StateMachine Controller { get; set; }

        public Character(Entity entity, Map map) {
            Entity = entity;
            Controller = new StateMachine(new Dictionary<string, Func<State>> {
                { "wait", ()=>WaitState },
                { "move", ()=>MoveState }
            });
            WaitState = new WaitState(this, map);
            MoveState = new MoveState(this, map);
            
            Controller.Change(WaitState.Name);
        }
    }
}