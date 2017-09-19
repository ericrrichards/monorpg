using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoRpg.Engine.GameStates {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    using MonoRpg.Engine.Tiled;
    using MonoRpg.Engine.UI;

    public class ExploreState : IStateObject {
        public StateStack Stack { get; set; }
        public TiledMap MapDef { get; set; }
        public Map Map { get; set; }
        public Character Hero { get; set; }
        public ExploreState(StateStack stack, TiledMap mapDef, Vector3 startPos) {
            Stack = stack;
            MapDef = mapDef;
            Map = new Map(mapDef);
            Hero = new Character(EntityDefs.Instance.Characters["hero"], Map);
            Hero.Entity.SetTilePosition((int)startPos.X, (int)startPos.Y, (int)startPos.Z, Map);
            Map.GotoTile((int)startPos.X, (int)startPos.Y);
        }

        public void Enter( EnterArgs args) {  }
        public void Exit() {  }

        public bool Update(float dt) {
            var playerPos = Hero.Entity.Sprite.Position;
            Map.CamX = (int)Math.Floor(playerPos.X);
            Map.CamY = (int)Math.Floor(playerPos.Y);

            
            foreach (var npc in Map.NPCs) {
                npc.Controller.Update(dt);
            }
            return false;
        }

        public void Render(Renderer renderer) {
            renderer.Translate(-Map.CamX, -Map.CamY);
            var layerCount = Map.LayerCount;
            for (var i = 0; i < layerCount; i++) {
                Entity heroEntity = null;
                if (i == Hero.Entity.Layer) {
                    heroEntity = Hero.Entity;
                }
                Map.RenderLayer(renderer, i, heroEntity);
            }
            renderer.Translate(0,0);
        }

        public void HandleInput(float dt) {
            Hero.Controller.Update(dt);
            if (System.Keys.WasPressed(Keys.Space)) {
                var (x, y) = Hero.GetFacedTileCoords();
                var layer = Hero.Entity.Layer;
                var trigger = Map.GetTrigger(layer, x, y);
                trigger?.OnUse(Hero.Entity);
            }
            if (System.Keys.WasPressed(Keys.LeftAlt)) {
                var menu = new InGameMenu(Stack);
                Stack.Push(menu);
                return;
            }
        }

        public void HideHero() {
            Hero.Entity.SetTilePosition(Hero.Entity.TileX, Hero.Entity.TileY, -1, Map);
        }

        public void ShowHero(int layer=1) {
            Hero.Entity.SetTilePosition(Hero.Entity.TileX, Hero.Entity.TileY, layer, Map);
        }
        
    }
}
