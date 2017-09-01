namespace MonoRpg.Engine.GameStates {
    public interface IStateObject {
        void Enter(EnterArgs enterParams=null);
        void Exit();
        bool Update(float dt);
        void Render(Renderer renderer);
        void HandleInput(float dt);
    }
}