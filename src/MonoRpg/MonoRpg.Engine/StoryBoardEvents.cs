using System;
using System.Text;
using System.Threading.Tasks;

namespace MonoRpg.Engine {
    public interface IStoryboardEvent {
        void Update(float dt);
        bool IsBlocking { get; }
        bool IsFinished { get; }
    }
    public class WaitEvent : IStoryboardEvent{
        public float Seconds { get; set; }
        public bool IsBlocking => true;
        public bool IsFinished => Seconds <= 0;

        private WaitEvent(float seconds) {
            Seconds = seconds;
        }

        public void Update(float dt) {
            Seconds -= dt;
        }

        public static Func<WaitEvent> Wait(float seconds) {
            return () => new WaitEvent(seconds);
        }
    }
}
