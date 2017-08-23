namespace MonoRpg.Engine {
    public abstract class Action {
        public Map Map { get; private set; }
        public int TileX { get; set; }

        public int TileY { get; set; }
        public int Layer { get; set; }
        public Action(Map map, int tileX, int tileY, int layer) {
            Map = map;
            TileX = tileX;
            TileY = tileY;
            Layer = layer;
        }

        public abstract void Execute(Trigger trigger, Entity entity);
    }

    public class EmptyAction : Action {
        public EmptyAction() : base(null, 0, 0, 0) { }
        public override void Execute(Trigger trigger, Entity entity) {  }
    }
}