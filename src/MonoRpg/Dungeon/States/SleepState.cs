namespace Dungeon.States {
    using System;
    using System.Collections.Generic;

    using MonoRpg.Engine;

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