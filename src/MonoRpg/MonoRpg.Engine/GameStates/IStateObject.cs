namespace MonoRpg.Engine.GameStates {
    public interface IStateObject {
        void Enter(EnterArgs enterParams=null);
        void Exit();
        void HandleInput(float dt);
        bool Update(float dt);
        void Render(Renderer renderer);
        
    }
}