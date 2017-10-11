namespace MonoRpg.Engine.GameStates {
    using global::System.Collections.Generic;
    using global::System.Diagnostics;

    using MonoRpg.Engine.UI;

    public class StatusMenuState : BaseStateObject {
        public InGameMenu Parent { get; set; }
        public StateMachine StateMachine { get; private set; }
        public Layout Layout { get; private set; }
        public List<Panel> Panels { get; set; }
        public Actor Actor { get; private set; }
        public ActorSummary ActorSummary { get; private set; }

        public StatusMenuState(InGameMenu parent): base(parent.Stack) {
            Parent = parent;
            StateMachine = parent.StateMachine;
            Layout = new Layout()
                .Contract("screen", 118, 40)
                .SplitHorizontal("screen", "title", "bottom", 0.12f, 2);
            Panels = new List<Panel> { Layout.CreatePanel("title"), Layout.CreatePanel("bottom") };
        }

        public override void Enter(EnterArgs enterParams = null) {
            var actorArgs = enterParams as StatusArgs;
            Debug.Assert(actorArgs != null);

            Actor = actorArgs.Actor;
            ActorSummary = new ActorSummary(Actor, new ActorSummaryArgs{ShowXP = true});

        }
    }
}