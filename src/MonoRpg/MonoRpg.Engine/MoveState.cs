namespace MonoRpg {
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
                frames = Character.LeftAnimation;
            } else if (data.X == 1) {
                frames = Character.RightAnimation;
            } else if (data.Y == -1) {
                frames = Character.UpAnimation;
            } else if (data.Y == 1) {
                frames = Character.DownAnimation;
            }
            Animation.SetFrames(frames);

            MoveX = data.X;
            MoveY = data.Y;
            var pixelPos = Entity.Sprite.Position;
            PixelX = (int)pixelPos.X;
            PixelY = (int)pixelPos.Y;
            Tween = new Tween(0, TileWidth, MoveSpeed);
        }

        public override void Exit() {
            Entity.TileX += MoveX;
            Entity.TileY += MoveY;
            Entity.Teleport(Map);
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
                Controller.Change("wait");
            }
        }
    }
}