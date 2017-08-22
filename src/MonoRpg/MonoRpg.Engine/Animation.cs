namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;

    public class Animation {
        private List<int> Frames { get; set; }

        public int Index { get; set; }

        public float SPF { get; set; }

        public float Time { get; set; }

        public bool Loop { get; set; }

        public int CurrentFrame => Frames[Index];
        public bool Finished => Loop == false && Index >= Frames.Count;

        public Animation(List<int> frames, bool loop = true, float spf = 0.12f) {
            Frames = frames ?? new List<int> { 0 };
            Index = 0;
            SPF = spf;
            Time = 0.0f;
            Loop = loop;
        }

        public void Update(float dt) {
            Time += dt;
            if (Time > SPF) {
                Index++;
                Time = 0;

                if (Index >= Frames.Count) {
                    if (Loop) {
                        Index = 0;
                    } else {
                        Index = Frames.Count - 1;
                    }
                    
                }
            }
        }

        public void SetFrames(List<int> frames) {
            Frames = frames;
            Index = Math.Min(Index, frames.Count - 1);
        }

        
    }
}