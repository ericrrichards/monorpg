namespace MonoRpg.Engine {
    using global::System;

    public class Trigger {
        public Action<Entity> OnEnter { get; }

        public Action<Entity> OnExit { get; }

        public Action<Entity> OnUse { get; }

        public Trigger(Action<Entity> onEnter = null, Action<Entity> onExit = null, Action<Entity> onUse = null) {
            OnEnter = onEnter ?? (entity => {}) ;
            OnExit = onExit ?? (entity => {});
            OnUse = onUse ?? (entity => {});
        }

        
    }


}