namespace MonoRpg.Engine {
    public class State {
        public string Name { get; protected set; }
        public virtual void Render(Renderer renderer) { }
        public virtual void Enter(EnterParameters enterParams) { }
        public virtual void Exit() { }
        public virtual void Update(float dt) { }

        internal protected State(string name) {
            Name = name;
        }
    }
    public abstract class EnterParameters { }
}