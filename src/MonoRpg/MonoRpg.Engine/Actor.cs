namespace MonoRpg.Engine {
    using global::System.Collections.Generic;
    using global::System.Diagnostics;

    using Microsoft.Xna.Framework.Graphics;

    public class LevelUp {
        public int XP { get; set; }
        public int Level { get; set; }
        public Dictionary<string, Stat> Stats { get; set; }

        public LevelUp(int nextLevelXP) {
            XP = -nextLevelXP;
            Level = 1;
            Stats = new Dictionary<string, Stat>();
        }
    }

    public class Equipment {
        public Item Weapon { get; set; }
        public Item Armor { get; set; }
        public Item Accessory1 { get; set; }
        public Item Accessory2 { get; set; }
    }

    public class ActorDef {
        public string Id { get; set; }
        public Stat[] Stats { get; set; }
        public Dictionary<string, Dice> StatGrowth { get; set; }
        public string Portrait { get; set; }
        public string Name { get; set; }
        public List<string> Actions { get; set; }
        public int XP { get; set; }
        public int Level { get; set; }
        public Item Weapon { get; set; }
        public Item Armor { get; set; }
        public Item Accessory1 { get; set; }
        public Item Accessory2 { get; set; }



        public ActorDef() {
            Level = 1;
            StatGrowth = new Dictionary<string, Dice>();
        }
    }

    public class Actor {
        public ActorDef Def { get; set; }
        public Stats Stats { get; set; }
        public Dictionary<string, Dice> StatGrowth { get; set; }
        public int Level { get; set; }
        public int XP { get; set; }
        public int NextLevelXP { get; set; }
        public string Id { get; set; }
        public List<string> Actions { get; private set; }
        public string Name { get; set; }
        public Sprite Portrait { get; set; }
        public Equipment Equipment { get; set; }

        public Texture2D PortraitTexture { get; set; }

        public Actor(ActorDef def) {
            Def = def;
            Name = def.Name;
            Id = def.Id;
            Actions = def.Actions;
            Portrait = new Sprite();
            if (!string.IsNullOrEmpty(def.Portrait)) {
                PortraitTexture = System.Content.FindTexture(def.Portrait);
                Portrait.Texture = PortraitTexture;
            }
            Stats = new Stats(def.Stats);
            StatGrowth = def.StatGrowth?? new Dictionary<string, Dice>();
            XP = def.XP;
            Level = def.Level;
            Equipment = new Equipment {
                Weapon = def.Weapon,
                Armor = def.Armor,
                Accessory1 = def.Accessory1,
                Accessory2 = def.Accessory2
            };
            NextLevelXP = Levels.NextLevelXP(Level);
        }

        public bool ReadyToLevelUp => XP >= NextLevelXP;

        public bool AddXP(int xp) {
            XP += xp;
            return ReadyToLevelUp;
        }

        public LevelUp CreateLevelUp() {
            var levelUp = new LevelUp(NextLevelXP);
            foreach (var dice in StatGrowth) {
                levelUp.Stats[dice.Key] = new Stat(dice.Key, dice.Value.Roll());
            }
            return levelUp;
        }

        public void ApplyLevel(LevelUp levelUp) {
            XP += levelUp.XP;
            Level += levelUp.Level;
            NextLevelXP = Levels.NextLevelXP(Level);

            Debug.Assert(XP >= 0);

            foreach (var levelUpStat in levelUp.Stats) {
                Stats.Base[levelUpStat.Key] += levelUpStat.Value;
            }
        }
    }
}