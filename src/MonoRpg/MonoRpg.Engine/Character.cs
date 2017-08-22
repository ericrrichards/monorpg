namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;

    public class Character {
        public Entity Entity { get; set; }
        public WaitState WaitState { get; }
        public MoveState MoveState { get; }
        public StateMachine Controller { get; set; }

        public List<List<int>> Animations { get; set; }

        public List<int> UpAnimation => Animations[0];
        public List<int> RightAnimation => Animations[1];
        public List<int> DownAnimation => Animations[2];
        public List<int> LeftAnimation => Animations[3];
        public Facing Facing { get; set; }



        public Character(Entity entity, Map map, List<List<int>> animations, Facing facing=Facing.Down) {
            Animations = animations ??
                new List<List<int>> {
                    new List<int>{entity.StartFrame},
                    new List<int> { entity.StartFrame },
                    new List<int> { entity.StartFrame },
                    new List<int>{entity.StartFrame}
                };
            Entity = entity;
            Controller = new StateMachine(new Dictionary<string, Func<State>> {
                { "wait", ()=>WaitState },
                { "move", ()=>MoveState }
            });
            WaitState = new WaitState(this, map);
            MoveState = new MoveState(this, map);

            Controller.Change(WaitState.Name);
            Facing = facing;
        }

        public (int x, int y) GetFacedTileCoords() {
            int xInc=0, yInc=0;
            switch (Facing) {
                case Facing.Up:
                    yInc = -1;
                    break;
                case Facing.Right:
                    xInc = 1;
                    break;
                case Facing.Down:
                    yInc = 1;
                    break;
                case Facing.Left:
                    xInc = -1;
                    break;
            }
            return (Entity.TileX + xInc, Entity.TileY + yInc);
        }
    }

    public enum Facing {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }
}