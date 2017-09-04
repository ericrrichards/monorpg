namespace MonoRpg.Engine.GameStates {
    using global::System;
    using global::System.Collections.Generic;

    using MonoRpg.Engine.UI;

    public class InGameMenu : IStateObject {
        public StateStack Stack { get; set; }
        public StateMachine StateMachine { get; private set; }

        public InGameMenu(StateStack stack) {
            Stack = stack;
            StateMachine = new StateMachine(new Dictionary<string, Func<State>>() {
                {"frontmenu", () => new FrontMenu(this)},
                {"items", () => new ItemMenuState(this)},
                {"magic", () => new State("magic") },
                {"equip", () => new State("equip") },
                {"status", () => new State("status") }
            });
            StateMachine.Change("frontmenu");
        }

        public void Enter(EnterArgs arg) { }
        public void Exit() { }
        public bool Update(float dt) {
            if (Stack.Top == this) {
                StateMachine.Update(dt);
            }
            return false;
        }

        public void Render(Renderer renderer) {
            StateMachine.Render(renderer);
        }
        public void HandleInput(float dt) { }
    }
}