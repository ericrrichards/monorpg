namespace MonoRpg.Engine.UI {
    using global::System.Collections.Generic;
    using global::System.Diagnostics;

    public class Layout {
        public Dictionary<string, LayoutPanel> Panels { get; set; }
        public PanelParams PanelDef { get; set; }

        public Layout() {
            Panels = new Dictionary<string, LayoutPanel>();
            PanelDef = new PanelParams {
                Size = 3,
                Texture = System.Content.FindTexture("simple_panel.png")
            };
            Panels["screen"] = new LayoutPanel {
                X = 0,
                Y = 0,
                Width = System.ScreenWidth,
                Height = System.ScreenHeight
            };
        }

        public int Top(string name) {
            Debug.Assert(Panels.ContainsKey(name));
            var panel = Panels[name];
            return panel.Y + panel.Height / 2;
        }

        public int Bottom(string name) {
            Debug.Assert(Panels.ContainsKey(name));
            var panel = Panels[name];
            return panel.Y - panel.Height / 2;
        }
        public int Left(string name) {
            Debug.Assert(Panels.ContainsKey(name));
            var panel = Panels[name];
            return panel.X - panel.Width / 2;
        }
        public int Right(string name) {
            Debug.Assert(Panels.ContainsKey(name));
            var panel = Panels[name];
            return panel.X + panel.Width / 2;
        }
        public int CenterY(string name) {
            Debug.Assert(Panels.ContainsKey(name));
            var panel = Panels[name];
            return panel.Y;
        }
        public int CenterX(string name) {
            Debug.Assert(Panels.ContainsKey(name));
            var panel = Panels[name];
            return panel.X;
        }

        public Panel CreatePanel(string name) {
            Debug.Assert(Panels.ContainsKey(name));
            var layout = Panels[name];
            var panel = new Panel(PanelDef);
            panel.CenterPosition(layout.X, layout.Y, layout.Width, layout.Height);
            return panel;
        }

        public void DebugRender(Renderer renderer) {
            foreach (var layoutPanel in Panels) {
                var panel = CreatePanel(layoutPanel.Key);
                panel.Render(renderer);
            }
        }

        public Layout Contract(string name, int horizontalMargin=0, int verticalMargin=0) {
            Debug.Assert(Panels.ContainsKey(name));
            var panel = Panels[name];
            panel.Width -= horizontalMargin;
            panel.Height -= verticalMargin;
            return this;
        }

        public Layout SplitHorizontal(string name, string topName, string bottomName, float prc, int splitSize) {
            Debug.Assert(Panels.ContainsKey(name));
            Debug.Assert(!Panels.ContainsKey(topName));
            Debug.Assert(!Panels.ContainsKey(bottomName));

            var parent = Panels[name];
            Panels.Remove(name);

            var p1Height = parent.Height * prc;
            var p2Height = parent.Height * (1 - prc);

            Panels[topName] = new LayoutPanel {
                X = parent.X,
                Y = (int)(parent.Y + parent.Height/2 - p1Height/2 + splitSize/2f),
                Width = parent.Width,
                Height = (int)(p1Height - splitSize)
            };
            Panels[bottomName] = new LayoutPanel {
                X = parent.X,
                Y = (int)(parent.Y - parent.Height / 2 + p2Height / 2 - splitSize / 2f),
                Width = parent.Width,
                Height = (int)(p2Height - splitSize)
            };
            return this;
        }

        public Layout SplitVertical(string name, string leftName, string rightName, float prc, int splitSize) {
            Debug.Assert(Panels.ContainsKey(name));
            Debug.Assert(!Panels.ContainsKey(leftName));
            Debug.Assert(!Panels.ContainsKey(rightName));

            var parent = Panels[name];
            Panels.Remove(name);

            var p1Width = parent.Width * prc;
            var p2Width = parent.Width * (1 - prc);

            Panels[rightName] = new LayoutPanel {
                X = (int)(parent.X + parent.Width / 2 - p1Width / 2 + splitSize / 2f),
                Y = parent.Y,
                Width = (int)(p1Width - splitSize),
                Height = parent.Height
            };
            Panels[leftName] = new LayoutPanel {
                X = (int)(parent.X - parent.Width / 2 + p2Width / 2 - splitSize / 2f),
                Y = parent.Y,
                Width = (int)(p2Width - splitSize),
                Height = parent.Height
            };


            return this;
        }

    }

    public class LayoutPanel {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set;}
        public int Height { get; set; }
    }
}