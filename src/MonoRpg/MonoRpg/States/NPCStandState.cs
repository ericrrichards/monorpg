using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoRpg.States {
    using MonoRpg.Engine;
    public class NPCStandState : State {

        public Character Character { get; private set; }
        public Map Map { get; private set; }
        public Entity Entity { get; private set; }
        public StateMachine Controller { get; set; }
        public NPCStandState(Character character, Map map) : base("npc_stand") {
            Character = character;
            Map = map;
            Entity = character.Entity;
            Controller = character.Controller;
        }


    }

    public class PlanStrollState : State {
        public Character Character { get; private set; }
        public Map Map { get; private set; }
        public Entity Entity { get; private set; }
        public StateMachine Controller { get; set; }
        public float FrameResetSpeed { get; set; }
        public float FrameCount { get; set; }
        public Random Random { get; private set; }
        public float CountDown { get; set; }
        public PlanStrollState(Character character, Map map) : base("plan_stroll") {
            Character = character;
            Map = map;
            Entity = character.Entity;
            Controller = character.Controller;

            FrameResetSpeed = 0.05f;
            FrameCount = 0;

            Random = new Random();
            CountDown = Random.Next(0, 3);
        }

        public override void Enter(EnterArgs enterParams) {
            FrameCount = 0;
            CountDown = Random.Next(0, 3);
        }

        public override bool Update(float dt) {
            CountDown -= dt;

            if (CountDown <= 0) {
                var choice = Random.Next(4);
                if (choice == 0) {
                    Controller.Change("move", new MoveParams(-1, 0));
                } else if (choice == 1) {
                    Controller.Change("move", new MoveParams(1, 0));
                } else if (choice == 2) {
                    Controller.Change("move", new MoveParams(0, -1));
                } else if (choice == 3) {
                    Controller.Change("move", new MoveParams(0, 1));
                }
            }

            if (FrameCount >= 0) {
                FrameCount = FrameCount + dt;
                if (FrameCount > FrameResetSpeed) {
                    FrameCount = -1;
                    Entity.SetFrame(Entity.StartFrame);
                    Character.Facing = Facing.Down;
                }
            }
            return false;
        }
    }
}
