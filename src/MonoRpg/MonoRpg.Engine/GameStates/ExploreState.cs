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

    public interface IGameState {
        void Enter();
        void Exit();
        void Update(float dt);
        void Render(Renderer renderer);
        void HandleInput();
    }

    public class ExploreState : IGameState {
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

        public void Enter() {  }
        public void Exit() {  }

        public void Update(float dt) {
            var playerPos = Hero.Entity.Sprite.Position;
            Map.CamX = (int)Math.Floor(playerPos.X);
            Map.CamY = (int)Math.Floor(playerPos.Y);

            Hero.Controller.Update(dt);
            foreach (var npc in Map.NPCs) {
                npc.Controller.Update(dt);
            }
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

        public void HandleInput() {
            var ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Space)) {
                var (x, y) = Hero.GetFacedTileCoords();
                var layer = Hero.Entity.Layer;
                var trigger = Map.GetTrigger(layer, x, y);
                trigger?.OnUse(Hero.Entity);
            }
        }
    }
}
