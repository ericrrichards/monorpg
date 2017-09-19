namespace Dungeon.States {
    using System.Collections.Generic;

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

    public class SleepState : State {
        public StateMachine Controller { get; }
        public Character Character { get; }
        public Map Map { get; }
        public Entity Entity { get; }
        public Animation Animation { get; }
        public Entity SleepEntity { get; }

        public SleepState(Character character, Map map) : base("sleep") {
            Character = character;
            Map = map;
            Entity = character.Entity;
            Controller = character.Controller;
            Animation = new Animation(new List<int>{0,1,2,3}, true, 0.6f);

            SleepEntity = new Entity(EntityDefs.Instance.Entities["sleep"]);
            Entity.SetFrame(character.Animations[(Animations)character.Facing][0]);
        }

        public override void Enter(EnterArgs enterParams) {
            Entity.AddEntity("snore", SleepEntity);
        }

        public override void Exit() {
            Entity.RemoveChild("snore");
        }

        public override bool Update(float dt) {
            Animation.Update(dt);
            SleepEntity.SetFrame(Animation.CurrentFrame);
            return false;
        }
    }
}