namespace MonoRpg.Engine.GameStates {
    using global::System;
    using global::System.Collections.Generic;

    using MonoRpg.Engine.RenderEngine;
    using MonoRpg.Engine.UI;

    public class InGameMenu : BaseStateObject {
        public StateMachine StateMachine { get; private set; }
        public float TextSize { get; set; }
        public float LabelSize { get; set; }
        public float TitleSize { get; set; }

        public InGameMenu(StateStack stack) : base(stack) {
            TitleSize = 1.2f;
            LabelSize = 0.88f;
            TextSize = 1f;


            StateMachine = new StateMachine(new Dictionary<string, Func<IStateObject>>() {
                {"frontmenu", () => new FrontMenu(this)},
                {"items", () => new ItemMenuState(this)},
                //{"magic", () => new State("magic") },
                //{"equip", () => new State("equip") },
                //{"status", () => new State("status") }
            });
            StateMachine.Change("frontmenu");
        }
        
        public override bool Update(float dt) {
            if (Stack.Top == this) {
                StateMachine.Update(dt);
            }
            return false;
        }

        public override void Render(Renderer renderer) {
            StateMachine.Render(renderer);
        }
    }
}