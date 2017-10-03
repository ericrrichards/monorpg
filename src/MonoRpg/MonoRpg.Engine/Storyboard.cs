namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Linq;

    using Microsoft.Xna.Framework.Audio;

    using MonoRpg.Engine.GameStates;
    using MonoRpg.Engine.UI;

    public class Storyboard : IStateObject{
        public StateStack Stack { get; set; }
        public List<Func<Storyboard, IStoryboardEvent>> EventFactories { get; set; }
        public List<IStoryboardEvent> InstantiatedEvents { get; set; }

        public Dictionary<string, IStateObject> States { get; set; }
        public StateStack SubStack { get; set; }
        public Dictionary<string, SoundEffectInstance> PlayingSounds { get; set; }


        public Storyboard(StateStack stack, bool handIn, params Func<Storyboard, IStoryboardEvent>[] events) {
            Stack = stack;
            EventFactories = events.ToList();
            InstantiatedEvents = new List<IStoryboardEvent>();
            for (var index = 0; index < EventFactories.Count; index++) {
                InstantiatedEvents.Add(null);
            }
            States = new Dictionary<string, IStateObject>();
            SubStack = new StateStack();
            PlayingSounds = new Dictionary<string, SoundEffectInstance>();

            if (handIn) {
                var state = Stack.Pop();
                PushState("handin", state);
            }
        }

        

        public void Enter(EnterArgs enterParams = null) {
        }
        public void Exit() {
            foreach (var sound in PlayingSounds) {
                sound.Value.Stop();
            }
        }

        public void HandleInput(float dt) {
        }

        public bool Update(float dt) {
            SubStack.Update(dt);

            if (EventFactories.Count == 0) {
                Stack.Pop();
            }
            var deleteIndex = -1;
            for (var index = 0; index < EventFactories.Count; index++) {
                var evt = InstantiatedEvents[index];
                if (evt == null) {
                    InstantiatedEvents[index] = EventFactories[index](this);
                    evt = InstantiatedEvents[index];
                }
                evt.Update(dt, this);
                if (evt.IsFinished) {
                    deleteIndex = index;
                    break;
                }
                if (evt.IsBlocking) {
                    break;
                }
            }
            if (deleteIndex >= 0) {
                EventFactories.RemoveAt(deleteIndex);
                InstantiatedEvents.RemoveAt(deleteIndex);
            }
            return false;
        }   

        public void Render(Renderer renderer) {
            SubStack.Render(renderer);

            //var debugText = $"Events Stack: {InstantiatedEvents.Count}";
            //renderer.DrawText2D(0,0, debugText);
        }

        public void PushState(string id, IStateObject state) {
            States[id] = state;
            SubStack.Push(state);
        }

        public void RemoveState(string id) {
            var state = States[id];
            States.Remove(id);
            SubStack.States.Remove(state);
        }

        public void AddSound(string name, SoundEffectInstance sound) {
            Debug.Assert(!PlayingSounds.ContainsKey(name));
            PlayingSounds.Add(name, sound);
        }

        public void StopSound(string name) {
            if (PlayingSounds.ContainsKey(name)) {
                PlayingSounds[name].Stop();
                PlayingSounds.Remove(name);
            }
        }
    }
}