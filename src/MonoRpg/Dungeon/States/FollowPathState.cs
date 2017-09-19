namespace Dungeon.States {
    using MonoRpg.Engine;

    public class FollowPathState : State {
        public StateMachine Controller { get; }
        public Character Character { get; }
        public Map Map { get; }
        public Entity Entity { get; }
        public FollowPathState(Character character, Map map) : base("follow_path") {
            Character = character;
            Map = map;
            Entity = character.Entity;
            Controller = character.Controller;
        }

        public override void Enter(EnterArgs enterParams) {
            if (Character.PathIndex == -1 || Character.Path == null || Character.PathIndex >= Character.Path.Count) {
                Character.DefaultState = Character.PreviousDefaultState ?? Character.DefaultState;
                Controller.Change(Character.DefaultState);
                return;
            }
            Facing direction = Character.Path[Character.PathIndex];
            switch (direction) {
                case Facing.Left:
                    Controller.Change("move", new MoveParams(-1, 0));
                    break;
                case Facing.Right:
                    Controller.Change("move", new MoveParams(1, 0));
                    break;
                case Facing.Up:
                    Controller.Change("move", new MoveParams(0, -1));
                    break;
                case Facing.Down:
                    Controller.Change("move", new MoveParams(0, 1));
                    break;
            } 
        }

        public override void Exit() {
            Character.PathIndex++;
        }
    }
}