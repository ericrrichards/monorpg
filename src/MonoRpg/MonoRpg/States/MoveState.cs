namespace MonoRpg.States {
    using System.Collections.Generic;

    using Microsoft.Xna.Framework;

    using MonoRpg.Engine;

    public class MoveState : State {
        public float MoveSpeed { get; set; }
        public Animation Animation { get; }
        public Tween Tween { get; set; }

        public int MoveY { get; set; }

        public int MoveX { get; set; }

        public StateMachine Controller { get; set; }

        public int TileWidth { get; set; }
        public Entity Entity { get; }
        public Character Character { get; }
        public Map Map { get; }
        public int PixelY { get; set; }

        public int PixelX { get; set; }
        public MoveState(Character character, Map map) : base("move") {
            Character = character;
            Map = map;
            TileWidth = map.TileWidth;
            Entity = character.Entity;
            Controller = character.Controller;
            MoveX = 0;
            MoveY = 0;
            Tween = new Tween(0, 0, 1);
            MoveSpeed = 0.3f;
            Animation = new Animation(new List<int>{Entity.StartFrame});
        }

        public override void Enter(EnterParameters enterParams) {
            Enter(enterParams as MoveParams);
        }

        private void Enter(MoveParams data) {
            var frames = new List<int>{Entity.StartFrame};

            if (data.X == -1) {
                frames = Character.Animations[Animations.Left];
                Character.Facing = Facing.Left;
            } else if (data.X == 1) {
                frames = Character.Animations[Animations.Right];
                Character.Facing = Facing.Right;
            } else if (data.Y == -1) {
                frames = Character.Animations[Animations.Up];
                Character.Facing = Facing.Up;
            } else if (data.Y == 1) {
                frames = Character.Animations[Animations.Down];
                Character.Facing = Facing.Down;
            }
            Animation.SetFrames(frames);

            MoveX = data.X;
            MoveY = data.Y;
            var pixelPos = Entity.Sprite.Position;
            PixelX = (int)pixelPos.X;
            PixelY = (int)pixelPos.Y;
            Tween = new Tween(0, TileWidth, MoveSpeed);

            var targetX = Entity.TileX + data.X;
            var targetY = Entity.TileY + data.Y;
            if (Map.IsBlocked(0, targetX, targetY)) {
                MoveX = 0;
                MoveY = 0;
                Entity.SetFrame(Animation.CurrentFrame);
                Controller.Change(Character.DefaultState);
            }
        }

        public override void Exit() {
            Trigger trigger;
            if (MoveX != 0 || MoveY != 0) {
                trigger = Map.GetTrigger(Entity.Layer, Entity.TileX, Entity.TileY);
                trigger?.OnExit.Execute(trigger, Entity);
            }
            Entity.SetTilePosition(Entity.TileX + MoveX, Entity.TileY + MoveY, Entity.Layer, Map);
            

            trigger = Map.GetTrigger(Entity.Layer, Entity.TileX, Entity.TileY);
            trigger?.OnEnter.Execute(trigger, Entity);
        }

        public override void Render(Renderer renderer) { }

        public override void Update(float dt) {
            Animation.Update(dt);
            Entity.SetFrame(Animation.CurrentFrame);
            Tween.Update(dt);
            var value = Tween.Value;
            var x = PixelX + value * MoveX;
            var y = PixelY - value * MoveY;
            Entity.X = x;
            Entity.Y = y;
            Entity.Sprite.Position = new Vector2(x,y);

            if (Tween.Finished) {
                Controller.Change(Character.DefaultState);
            }
        }
    }
}