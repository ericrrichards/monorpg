namespace MonoRpg {
    using MonoRpg.Engine;

    public class MoveParams : EnterParameters {
        public MoveParams(int x, int y) {
            X = x;
            Y = y;
        }

        public int Y { get; }

        public int X { get; }
    }
}