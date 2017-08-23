namespace MonoRpg.Engine {
    using MonoRpg.Engine.Tiled;

    public class TeleportAction : Action {
        public TeleportAction(Map map, int tileX, int tileY, int layer=0) : base(map, tileX, tileY, layer) { }

        public override void Execute(Trigger trigger, Entity entity) {
            entity.SetTilePosition(TileX, TileY, Layer, Map);
        }
    }
}