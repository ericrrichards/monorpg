namespace MonoRpg.Engine {
    using MonoRpg.Engine.GameStates;
    using MonoRpg.Engine.RenderEngine;

    public class State: IStateObject {
        public string Name { get; protected set; }
        public virtual void Render(Renderer renderer) { }
        public virtual void HandleInput(float dt) {
            
        }

        public virtual void Enter(EnterArgs enterParams) { }
        public virtual void Exit() { }

        public virtual bool Update(float dt) {
            return false;
        }

        protected internal State(string name) {
            Name = name;
        }
    }
    public abstract class EnterArgs { }
}