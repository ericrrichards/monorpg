namespace MonoRpg.Engine.UI {
    using global::System;
    using global::System.Collections.Generic;

    public class Selection {
        public Selection(Renderer renderer, SelectionArgs args) {
            X = 0;
            Y = 0;
            DataSource = args.DataSource;
            Columns = args.Columns;
            FocusX = 0;
            FocusY = 0;
            SpacingX = args.SpacingX;
            SpacingY = args.SpacingY;
            Cursor = new Sprite();
            var cursorTex = System.Content.FindTexture(args.Cursor);
            Cursor.Texture = cursorTex;
            CursorWidth = cursorTex.Width;
            ShowCursor = true;
            MaxRows = args.Rows;
            DisplayStart = 1;
            Scale = 1f;
            OnSelection = args.OnSelection;
            DisplayRows = args.DisplayRows;
            RenderItem = args.RenderItem ?? RenderItemFunc;

            Width = CalcWidth(renderer);
            Height = CalcHeight();
        }

        

        public int Height { get; set; }

        public int Width { get; set; }

        public Action<Renderer, int, int, string> RenderItem { get; set; }

        public int DisplayRows { get; set; }

        public Action<int> OnSelection { get; set; }

        public float Scale { get; set; }

        public int DisplayStart { get; set; }

        public int MaxRows { get; set; }

        public bool ShowCursor { get; set; }

        public int CursorWidth { get; set; }

        public Sprite Cursor { get; set; }

        public int SpacingY { get; set; }

        public int SpacingX { get; set; }

        public int FocusY { get; set; }

        public int FocusX { get; set; }

        public int Columns { get; set; }

        public List<string> DataSource { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public void RenderItemFunc(Renderer renderer, int x, int y, string item) { }
        private int CalcHeight() { return 0; }

        private int CalcWidth(Renderer renderer) { return 0; }
    }

    public class SelectionArgs {
        public SelectionArgs() {
            Columns = 1;
            SpacingX = 128;
            SpacingY = 24;
            Cursor = "cursor.png";
            _rows = -1;
            OnSelection = selectedIndex => { };
            DisplayRows = Rows;
        }

        public Action<int> OnSelection { get; set; }

        public string Cursor { get; set; }

        public int SpacingY { get; set; }

        public int SpacingX { get; set; }

        public List<string> DataSource { get; set; }
        public int Columns { get; set; }
        private int _rows;
        public int Rows {
            get => (_rows > 0 ? _rows : DataSource.Count);
            set => _rows = value;
        }
        public int DisplayRows { get; set; }
        public Action<Renderer, int, int, string> RenderItem { get; set; }
    }
}