namespace MonoRpg.Engine.GameStates {
    using MonoRpg.Engine.UI;

    public interface IStateObject {
        void Enter(EnterArgs enterParams=null);
        void Exit();
        void HandleInput(float dt);
        bool Update(float dt);
        void Render(Renderer renderer);
        
    }

    public class BaseStateObject : IStateObject {
        public StateStack Stack { get; private set; }

        protected BaseStateObject(StateStack stack) {
            Stack = stack;
        }

        public void SetStack(StateStack stack) {
            Stack = stack;
        }


        public virtual void Enter(EnterArgs enterParams = null) {
        }

        public virtual void Exit() {
        }

        public virtual void HandleInput(float dt) {
        }

        public virtual bool Update(float dt) {
            return false;
        }

        public virtual void Render(Renderer renderer) {

        }
    }
}