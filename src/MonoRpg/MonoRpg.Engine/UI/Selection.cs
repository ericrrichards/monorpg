namespace MonoRpg.Engine.UI {
    using global::System;
    using global::System.Collections.Generic;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public class Selection {
        public int Height { get; set; }
        public int Width { get; set; }
        public Action<Renderer, int, int, string> RenderItem { get; set; }
        public int DisplayRows { get; set; }
        public Action<int, string> OnSelection { get; set; }
        public float Scale { get; set; }
        public int DisplayStart { get; set; }
        public int MaxRows { get; set; }
        public bool DisplayCursor { get; set; }
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
            DisplayCursor = true;
            MaxRows = args.Rows;
            DisplayStart = 0;
            Scale = 1f;
            TextScale = 1f;
            OnSelection = args.OnSelection;
            DisplayRows = args.DisplayRows;
            RenderItem = args.RenderItem ?? RenderItemFunc;

            Width = CalcWidth(renderer);
            Height = CalcHeight();
        }

        private void RenderItemFunc(Renderer renderer, int x, int y, string item) {
            if (string.IsNullOrEmpty(item)) {
                renderer.DrawText2D(x, y, "--", Color.White, Scale*TextScale);
            } else {
                renderer.DrawText2D(x, y, item, Color.White, Scale*TextScale);
            }

        }

        public void HandleInput() {
            var ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Up)) {
                MoveUp();
            } else if (ks.IsKeyDown(Keys.Down)) {
                MoveDown();
            } else if (ks.IsKeyDown(Keys.Left)) {
                MoveLeft();
            } else if (ks.IsKeyDown(Keys.Right)) {
                MoveRight();
            } else if (ks.IsKeyDown(Keys.Space)) {
                OnClick();
            }
        }

        private void MoveUp() {
            FocusY = Math.Max(FocusY - 1, 0);
            if (FocusY < DisplayStart) {
                MoveDisplayUp();
            }
        }

        private void MoveDown() {
            FocusY = Math.Min(FocusY + 1, MaxRows - 1);
            if (FocusY >= DisplayStart + DisplayRows) {
                MoveDisplayDown();
            }
        }

        private void MoveLeft() {
            FocusX = Math.Max(FocusX - 1, 0);
        }

        private void MoveRight() {
            FocusX = Math.Min(FocusX + 1, Columns - 1);
        }

        private void OnClick() {
            var index = GetIndex();
            OnSelection(index, DataSource[index]);
        }

        private void MoveDisplayUp() { DisplayStart -= 1; }
        private void MoveDisplayDown() { DisplayStart += 1; }
        private int GetIndex() { return FocusX + FocusY * Columns; }

        public void Render(Renderer renderer) {
            var displayStart = DisplayStart;
            var displayEnd = displayStart + DisplayRows-1;

            var x = X;
            var y = Y;

            var cursorWidth = CursorWidth * Scale;

            var spacingX = SpacingX * Scale;
            var rowHeight = SpacingY * Scale;

            Cursor.Scale = new Vector2(Scale);

            var itemIndex = displayStart * Columns;
            for (var i = displayStart; i < displayEnd; i++) {
                for (var j = 0; j < Columns; j++) {
                    if (i == FocusY && j == FocusX && DisplayCursor) {
                        Cursor.Position = new Vector2(x, y);
                        renderer.DrawSprite(Cursor);
                    }
                    var item = DataSource[itemIndex];
                    RenderItem(renderer, (int)(x + cursorWidth), y, item);
                    x += (int)spacingX;
                    itemIndex++;
                }
                y -= (int)rowHeight;
                x = X;
            }
        }

        public int GetWidth() { return (int)(Width * Scale); }
        public int GetHeight() { return (int)(Height * Scale); }

        private int CalcWidth(Renderer renderer) {
            if (Columns == 1) {
                var maxEntryWidth = 0;
                foreach (var item in DataSource) {
                    var width = renderer.MeasureText(item, -1, TextScale).X;
                    maxEntryWidth = (int)Math.Max(width, maxEntryWidth);
                }
                return maxEntryWidth;
            }
            return SpacingX * Columns;
        }

        private int CalcHeight() {
            var height = DisplayRows * SpacingY;
            return height - SpacingY / 2;
        }

        public void ShowCursor() { DisplayCursor = true; }
        public void HideCursor() { DisplayCursor = false; }
        public Vector2 Position {
            get => new Vector2(X, Y);
            set {
                X = (int)value.X;
                Y = (int)value.Y;
            }
        }
        public float PercentageShown => DisplayRows / (float)MaxRows;
        public float PercentageScrolled {
            get {
                var onePercent = 1.0f / MaxRows;
                var currentPercent = FocusY / (float)MaxRows;
                if (currentPercent <= onePercent) {
                    return 0;
                }
                return currentPercent;
            }
        }
        public string SelectedItem => DataSource[GetIndex()];
        public float TextScale { get; set; }
    }

    public class SelectionArgs {
        public SelectionArgs(List<string> items) {
            DataSource = items ?? new List<string>();
            Columns = 1;
            SpacingX = 128;
            SpacingY = 24;
            Cursor = "cursor.png";
            _rows = -1;
            OnSelection = (index, selectedItem) => { };
            DisplayRows = Rows;
        }

        public Action<int, string> OnSelection { get; set; }

        public string Cursor { get; set; }

        public int SpacingY { get; set; }

        public int SpacingX { get; set; }

        public List<string> DataSource { get; set; }
        public int Columns { get; set; }
        private int _rows;
        public int Rows {
            get => _rows > 0 ? _rows : DataSource.Count/Columns;
            set => _rows = value;
        }
        public int DisplayRows { get; set; }
        public Action<Renderer, int, int, string> RenderItem { get; set; }
    }
}