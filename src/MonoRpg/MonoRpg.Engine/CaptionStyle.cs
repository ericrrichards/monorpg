namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;

    using Microsoft.Xna.Framework;

    using MonoRpg.Engine.RenderEngine;

    public class CaptionStyle {
        public float Duration { get; set; }
        public Action<CaptionStyle, float> ApplyFunc;
        public Action<CaptionStyle, Renderer, string> Render { get; set; }
        public int Width { get; set; }
        public int Y { get; set; }
        public Color Color { get; set; }
        public int X { get; set; }
        public TextAlignment AlignY { get; set; }
        public TextAlignment AlignX { get; set; }
        public float Scale { get; set; }
        public string Font { get; set; }
        public CaptionStyle() {
            Font = "default";
            Scale = 1.0f;
            AlignX = TextAlignment.Center;
            AlignY = TextAlignment.Center;
            X = 0;
            Y = 0;
            Color = Color.White;
            Width = -1;
            Render = DefaultRenderer;
            ApplyFunc = (target, value) => { };
            Duration = 3f;
        }
        public CaptionStyle(CaptionStyle copy) {
            Font = copy.Font;
            Scale = copy.Scale;
            AlignX = copy.AlignX;
            AlignY = copy.AlignY;
            X = copy.X;
            Y = copy.Y;
            Color = copy.Color;
            Width = copy.Width;
            Render = copy.Render;
            ApplyFunc = copy.ApplyFunc;
            Duration = copy.Duration;
        }



        public static void DefaultRenderer(CaptionStyle style, Renderer renderer, string text) {
            renderer.AlignText(style.AlignX, style.AlignY);
            renderer.DrawText2D(style.X, style.Y, text, style.Color, style.Scale, style.Width, style.Font);
        }

        public static void FadeApply(CaptionStyle target, float value) {
            target.Color = new Color(target.Color, value);
        }

        public static readonly Dictionary<string, CaptionStyle> Styles = new Dictionary<string, CaptionStyle> {
            ["default"] = new CaptionStyle(),
            ["title"] = new CaptionStyle {
                Font = "contra_italic",
                Scale = 3,
                Y=75,
                ApplyFunc = FadeApply
            },
            ["subtitle"] = new CaptionStyle {
                Scale = 1,
                Y = -5,
                Color = new Color(0.4f, 0.38f, 0.39f, 1),
                ApplyFunc = FadeApply,
                Duration = 1
            }
        };

        
    }

}