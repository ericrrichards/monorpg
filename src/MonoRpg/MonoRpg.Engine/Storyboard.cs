namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;

    using MonoRpg.Engine.GameStates;
    using MonoRpg.Engine.UI;

    public class Storyboard : IStateObject{
        public StateStack Stack { get; set; }
        public List<Func<IStoryboardEvent>> EventFactories { get; set; }
        public List<IStoryboardEvent> InstantiatedEvents { get; set; }

        public Storyboard(StateStack stack, params Func<IStoryboardEvent>[] events) {
            Stack = stack;
            EventFactories = events.ToList();
            InstantiatedEvents = new List<IStoryboardEvent>();
            for (var index = 0; index < EventFactories.Count; index++) {
                InstantiatedEvents.Add(null);
            }
        }

        public void Enter(EnterArgs enterParams = null) {
        }
        public void Exit() {
        }

        public void HandleInput(float dt) {
        }

        public bool Update(float dt) {
            if (EventFactories.Count == 0) {
                Stack.Pop();
            }
            var deleteIndex = -1;
            for (var index = 0; index < EventFactories.Count; index++) {
                var evt = InstantiatedEvents[index];
                if (evt == null) {
                    InstantiatedEvents[index] = EventFactories[index]();
                    evt = InstantiatedEvents[index];
                }
                evt.Update(dt);
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
            var debugText = $"Events Stack: {InstantiatedEvents.Count}";
            renderer.DrawText2D(0,0, debugText);
        }
    }
}