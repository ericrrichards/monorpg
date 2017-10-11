namespace Arena.States {
    using MonoRpg.Engine;

    public class NPCStandState : State {

        public Character Character { get; private set; }
        public Map Map { get; private set; }
        public Entity Entity { get; private set; }
        public StateMachine Controller { get; set; }
        public NPCStandState(Character character, Map map) : base("npc_stand") {
            Character = character;
            Map = map;
            Entity = character.Entity;
            Controller = character.Controller;
        }


    }
}

