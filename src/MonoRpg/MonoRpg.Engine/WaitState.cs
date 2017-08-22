namespace MonoRpg.Engine {
    using Microsoft.Xna.Framework.Input;

    public class WaitState : State {
        public StateMachine Controller { get; }
        public Character Character { get; }
        public Map Map { get; }
        public Entity Entity { get; }

        public WaitState(Character character, Map map) : base("wait") {
            Character = character;
            Map = map;
            Entity = character.Entity;
            Controller = character.Controller;
        }

        public override void Enter(EnterParameters enterParams) {
            Entity.SetFrame(Entity.StartFrame);
        }

        public override void Render(Renderer renderer) {  }
        public override void Exit() {  }

        public override void Update(float dt) {
            var ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Left)) {
                Controller.Change("move", new MoveParams(-1, 0));
            } else if (ks.IsKeyDown(Keys.Right)) {
                Controller.Change("move", new MoveParams(1, 0));
            } else if (ks.IsKeyDown(Keys.Up)) {
                Controller.Change("move", new MoveParams(0, -1));
            } else if (ks.IsKeyDown(Keys.Down)) {
                Controller.Change("move", new MoveParams(0, 1));
            }
        }
    }
}