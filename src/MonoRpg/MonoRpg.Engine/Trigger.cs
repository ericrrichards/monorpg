namespace MonoRpg.Engine {
    public class Trigger {
        public Action OnEnter { get; set; }

        public Action OnExit { get; set; }

        public Action OnUse { get; set; }

        public Trigger(Action onEnter = null, Action onExit = null, Action onUse = null) {
            OnEnter = onEnter ?? new EmptyAction();
            OnExit = onExit ?? new EmptyAction();
            OnUse = onUse ?? new EmptyAction();
        }

        
    }
}