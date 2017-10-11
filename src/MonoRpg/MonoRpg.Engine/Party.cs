namespace MonoRpg.Engine {
    using global::System.Collections.Generic;

    public class Party {
        public Dictionary<string, Actor> Members { get; set; }

        public Party() {
            Members = new Dictionary<string, Actor>();
        }

        public void Add(Actor member) {
            Members.Add(member.Id, member);
        }

        public void RemoveById(string id) {
            Members.Remove(id);
        }

        public void Remove(Actor member) {
            RemoveById(member.Id);
        }
    }
}