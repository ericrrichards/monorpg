namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Linq;

    public class Character {
        public string DefaultState { get; set; }
        public string PreviousDefaultState { get; set; }

        public StateMachine Controller { get; set; }

        public Entity Entity { get; private set; }
        public Dictionary<Animations, List<int>> Animations { get; private set; }
        public Facing Facing { get; set; }
        public int PathIndex { get; set; }
        public List<Facing> Path { get; set; }
        public string Id { get; set; }
        public int TalkIndex { get; set; }

        public Character(CharacterDef def, Map map) {
            Debug.Assert(EntityDefs.Instance.Entities.ContainsKey(def.Entity));
            var entityDef = EntityDefs.Instance.Entities[def.Entity];

            Entity = new Entity(entityDef);
            Animations = def.Animations;
            Facing = def.Facing;
            var states = new Dictionary<string, Func<State>>();
            Controller = new StateMachine(states);

            foreach (var stateName in def.Controller) {
                Debug.Assert(EntityDefs.Instance.CharacterStates.ContainsKey(stateName));
                var state = EntityDefs.Instance.CharacterStates[stateName];
                Debug.Assert(!states.ContainsKey(stateName));
                var instance = (State)Activator.CreateInstance(state, this, map);
                states[instance.Name] = () => instance;
            }
            Controller.States = states;

            Controller.Change(def.State);
            DefaultState = def.State;
            PathIndex = -1;
        }

        

        public (int x, int y) GetFacedTileCoords() {
            int xInc = 0, yInc = 0;
            switch (Facing) {
                case Facing.Up:
                    yInc = -1;
                    break;
                case Facing.Right:
                    xInc = 1;
                    break;
                case Facing.Down:
                    yInc = 1;
                    break;
                case Facing.Left:
                    xInc = -1;
                    break;
            }
            return (Entity.TileX + xInc, Entity.TileY + yInc);
        }

        public void FollowPath(List<Facing> path) {
            PathIndex = 0;
            Path = path;
            PreviousDefaultState = DefaultState;
            DefaultState = "follow_path";
            Controller.Change("follow_path");
        }

        public void FollowPath(params Facing[] path) {
            FollowPath(path.ToList());
        }

        
    }
}