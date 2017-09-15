using System;
using System.Text;
using System.Threading.Tasks;

namespace MonoRpg.Engine {
    using global::System.Diagnostics;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    // type alias, because this is getting really tedious...
    using StoryBoardFunc = Func<Storyboard, IStoryboardEvent>;

    public interface IStoryboardEvent {
        void Update(float dt, Storyboard storyboard);
        bool IsBlocking { get; set; }
        bool IsFinished { get; }
    }

    public static class Events {
        public static StoryBoardFunc EmptyEvent => Wait(0);

        public static StoryBoardFunc Wait(float seconds) {
            return (storyboard) => new WaitEvent(seconds);
        }

        public static StoryBoardFunc BlackScreen(string id=null, float alpha=1.0f) {
            if (string.IsNullOrEmpty(id)) {
                id = "blackscreen";
            }
            var black = new Color(0,0,0,alpha);
            return storyboard => {
                var screen = new ScreenState(black);
                storyboard.PushState(id, screen);
                return EmptyEvent(storyboard);
            };
        }

        public static StoryBoardFunc FadeScreen(float start, float finish, float duration = 3, string id = null) {
            return storyboard => {
                ScreenState target = storyboard.SubStack.Top as ScreenState;
                if (!string.IsNullOrEmpty(id)) {
                    target = storyboard.States[id] as ScreenState;
                }
                Debug.Assert(target != null);
                var tween = new Tween(start, finish, duration);
                return new TweenEvent<ScreenState>(
                    tween, 
                    target,
                    (state, value) => state.Color = new Color(state.Color, value)
                );
            };
        }

        public static StoryBoardFunc FadeInScreen(string id=null, float duration=3) {
            return FadeScreen(0, 1, duration, id);
        }

        public static StoryBoardFunc FadeOutScreen(string id=null, float duration=3) {
            return FadeScreen(1, 0, duration, id);
        }

        public static StoryBoardFunc Caption(string id, string style, string text) {
            return storyboard => {
                var styleCopy = new CaptionStyle(CaptionStyle.Styles[style]);
                var caption = new CaptionState(styleCopy, text);
                storyboard.PushState(id, caption);
                return new TweenEvent<CaptionStyle>(new Tween(0, 1, styleCopy.Duration), styleCopy, styleCopy.ApplyFunc);
            };
        }
        public static StoryBoardFunc FadeOutCaption(string id = null, float duration = -1) {
            return storyboard => {
                var target = storyboard.SubStack.Top as CaptionState;
                if (id != null) {
                    target = storyboard.States[id] as CaptionState;
                }
                Debug.Assert(target != null);
                var styleCopy = target.Style;
                if (duration < 0) {
                    duration = styleCopy.Duration;
                }
                return new TweenEvent<CaptionStyle>(new Tween(1, 0, duration), styleCopy, styleCopy.ApplyFunc);
            };
        }

        public static StoryBoardFunc NoBlock(StoryBoardFunc f) {
            return storyboard => {
                var evt = f(storyboard);
                evt.IsBlocking = false;
                return evt;
            };
        }

        public static StoryBoardFunc KillState(string id) {
            return storyboard => {
                storyboard.RemoveState(id);
                return EmptyEvent(storyboard);
            };
        }

        public static StoryBoardFunc Play(string soundName, string name=null, float volume=1f) {
            name = name ?? soundName;
            return storyboard => {
                var sound = System.Content.GetSound(soundName);
                var instance = sound.CreateInstance();
                instance.Volume = volume;
                instance.Play();
                storyboard.AddSound(name, instance);
                return EmptyEvent(storyboard);
            };
        }

        public static StoryBoardFunc Stop(string name) {
            return storyboard => {
                storyboard.StopSound(name);
                return EmptyEvent(storyboard);
            };
        }

        public static StoryBoardFunc FadeSound(string name, float start, float finish, float duration) {
            return storyboard => {
                var sound = storyboard.PlayingSounds[name];
                return new TweenEvent<SoundEffectInstance>(new Tween(start, finish, duration),sound, (instance, value) => instance.Volume = value );
            };
        }
    }

    public class WaitEvent : IStoryboardEvent{
        public float Seconds { get; set; }
        public bool IsBlocking {
            get;
            set; 
        }
        public bool IsFinished => Seconds <= 0;

        internal WaitEvent(float seconds) {
            Seconds = seconds;
            IsBlocking = true;
        }

        public void Update(float dt, Storyboard storyboard) {
            Seconds -= dt;
        }
    }

    public class TweenEvent<T> : IStoryboardEvent {
        public Tween Tween { get; set; }
        public T Target { get; set; }
        public Action<T, float> ApplyFunc { get; set; }

        public bool IsBlocking { get; set; }
        public bool IsFinished => Tween.Finished;

        public TweenEvent(Tween tween, T target, Action<T, float> applyFunc) {
            Tween = tween;
            Target = target;
            ApplyFunc = applyFunc;
            IsBlocking = true;

        }

        public void Update(float dt, Storyboard storyboard) {
            Tween.Update(dt);
            ApplyFunc(Target, Tween.Value);
        }
        public void Render() { }
    }
}
