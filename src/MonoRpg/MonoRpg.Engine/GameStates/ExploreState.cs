using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoRpg.Engine.GameStates {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    using MonoRpg.Engine.RenderEngine;
    using MonoRpg.Engine.Tiled;
    using MonoRpg.Engine.UI;

    public class ExploreState : BaseStateObject {
        public TiledMap MapDef { get; set; }
        public Map Map { get; set; }
        public Character Hero { get; set; }
        public bool FollowCam { get; set; }
        public Character FollowChar { get; set; }
        public int ManualCamX { get; set; }
        public int ManualCamY { get; set; }

        public ExploreState(StateStack stack, TiledMap mapDef, Vector3 startPos) : base(stack) {
            MapDef = mapDef;
            Map = new Map(mapDef);

            FollowCam = true;
            FollowChar = Hero;
            ManualCamX = 0;
            ManualCamY = 0;

            Hero = new Character(EntityDefs.Instance.Characters["hero"], Map);
            Hero.Entity.SetTilePosition((int)startPos.X, (int)startPos.Y, (int)startPos.Z, Map);
            Map.GotoTile((int)startPos.X, (int)startPos.Y);
        }
        

        public override bool Update(float dt) {

            UpdateCamera(Map);
            
            foreach (var npc in Map.NPCs) {
                npc.Controller.Update(dt);
            }
            return false;
        }

        public override void Render(Renderer renderer) {
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

        public override void HandleInput(float dt) {
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

        public void ShowHero(int layer=0) {
            Hero.Entity.SetTilePosition(Hero.Entity.TileX, Hero.Entity.TileY, layer, Map);
        }

        public void UpdateCamera(Map map) {
            if (FollowCam) {
                var pos = Hero.Entity.Sprite.Position.ToPoint();
                map.CamX = pos.X;
                map.CamY = pos.Y;
            } else {
                map.CamX = ManualCamX;
                map.CamY = ManualCamY;
            }
        }

        public void SetFollowCam(bool follow, Character character = null) {
            FollowChar = character ?? FollowChar;
            FollowCam = follow;
            if (!FollowCam) {
                var pos = FollowChar.Entity.Sprite.Position.ToPoint();
                ManualCamX = pos.X;
                ManualCamY = pos.Y;
            }
        }

    }
}
