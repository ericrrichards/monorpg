namespace MonoRpg.Engine {
    public abstract class Action {
        public Map Map { get; private set; }
        public int TileX { get; set; }

        public int TileY { get; set; }
        public Action(Map map, int tileX, int tileY) {
            Map = map;
            TileX = tileX;
            TileY = tileY;
        }

        public abstract void Execute(Trigger trigger, Entity entity);
    }

    public class EmptyAction : Action {
        public EmptyAction() : base(null, 0, 0) { }
        public override void Execute(Trigger trigger, Entity entity) {  }
    }

    

    public class TeleportAction : Action {
        public TeleportAction(Map map, int tileX, int tileY) : base(map, tileX, tileY) { }

        public override void Execute(Trigger trigger, Entity entity) {
            entity.TileX = TileX;
            entity.TileY = TileY;
            entity.Teleport(Map);
        }
    }
}