namespace MonoRpg.States {
    using Microsoft.Xna.Framework.Input;
    using MonoRpg.Engine;

    public class WaitState : State {
        public StateMachine Controller { get; }
        public Character Character { get; }
        public Map Map { get; }
        public Entity Entity { get; }
        public float FrameResetSpeed { get; set; }
        public float FrameCount { get; set; }

        public WaitState(Character character, Map map) : base("wait") {
            Character = character;
            Map = map;
            Entity = character.Entity;
            Controller = character.Controller;
            FrameResetSpeed = 0.05f;
            FrameCount = 0;
        }

        

        public override void Enter(EnterParameters enterParams) {
            FrameCount = 0;
        }

        public override void Render(Renderer renderer) {  }
        public override void Exit() {  }

        public override void Update(float dt) {
            if (FrameCount >= 0) {
                FrameCount = FrameCount + dt;
                if (FrameCount > FrameResetSpeed) {
                    FrameCount = -1;
                    Entity.SetFrame(Entity.StartFrame);
                    Character.Facing = Facing.Down;
                }
            }




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