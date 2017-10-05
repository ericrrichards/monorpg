namespace MonoRpg.States {
    using Microsoft.Xna.Framework.Input;
    using MonoRpg.Engine;
    using MonoRpg.Engine.RenderEngine;

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

        

        public override void Enter(EnterArgs enterParams) {
            FrameCount = 0;
        }

        public override void Render(Renderer renderer) {  }
        public override void Exit() {  }

        public override bool Update(float dt) {
            if (FrameCount >= 0) {
                FrameCount = FrameCount + dt;
                if (FrameCount > FrameResetSpeed) {
                    FrameCount = -1;
                    Entity.SetFrame(Entity.StartFrame);
                    Character.Facing = Facing.Down;
                }
            }



            
            if (System.Keys.IsDown(Keys.Left)) {
                Controller.Change("move", new MoveParams(-1, 0));
            } else if (System.Keys.IsDown(Keys.Right)) {
                Controller.Change("move", new MoveParams(1, 0));
            } else if (System.Keys.IsDown(Keys.Up)) {
                Controller.Change("move", new MoveParams(0, -1));
            } else if (System.Keys.IsDown(Keys.Down)) {
                Controller.Change("move", new MoveParams(0, 1));
            }
            return false;
        }
    }
}