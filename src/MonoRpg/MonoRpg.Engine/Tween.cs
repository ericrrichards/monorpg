namespace MonoRpg.Engine {
    using global::System;

    public delegate float TweenFunc(float timePassed, float start, float distance, float duration);
    public class Tween {
        public bool Finished { get; set; }

        public float TimePassed { get; set; }

        public float TotalDuration { get; set; }

        public float Current { get; set; }

        public float StartValue { get; set; }

        public float Distance { get; set; }

        public TweenFunc TweenFunc { get; }
        public float FinishValue => StartValue + Distance;
        public float Value => Current;

        public Tween(float start, float finish, float totalDuration, TweenFunc tweenFunc = null) {
            TweenFunc = tweenFunc ?? Linear;
            Distance = finish - start;
            StartValue = start;
            Current = start;
            TotalDuration = totalDuration;
            TimePassed = 0;
            Finished = false;
        }


        public void Update(float elapsedTime) {
            TimePassed += elapsedTime;
            Current = TweenFunc(TimePassed, StartValue, Distance, TotalDuration);

            if (TimePassed > TotalDuration) {
                Current = StartValue + Distance;
                Finished = true;
            }
        }


        public static float Linear(float timePassed, float start, float distance, float duration) {
            return distance * timePassed / duration + start;
        }

        public static float EaseOutCirc(float timePassed, float start, float distance, float duration) {
            timePassed = timePassed / duration - 1;
            return distance * (float)Math.Sqrt(1 - timePassed * timePassed) + start;
        }

        public static float EaseInCirc(float timePassed, float start, float distance, float duration) {
            timePassed /= duration;
            return -distance * ((float)Math.Sqrt(1 - timePassed * timePassed) - 1) + start;
        }

        public static float EaseOutQuad(float timepassed, float start, float distance, float duration) {
            timepassed = timepassed / duration;
            return -distance * timepassed * (timepassed - 2) + start;
        }
    }
}