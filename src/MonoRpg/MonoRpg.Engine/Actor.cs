namespace MonoRpg.Engine {
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Collections.ObjectModel;
    using global::System.Diagnostics;

    using Microsoft.Xna.Framework.Graphics;

    using MonoRpg.Engine.RenderEngine;

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
        public int Weapon { get; set; }
        public int Armor { get; set; }
        public int Accessory1 { get; set; }
        public int Accessory2 { get; set; }
        public int this[string slotId] => SlotToItem[slotId]();
        private Dictionary<string, Func<int>> SlotToItem { get; }

        public Equipment() {
            SlotToItem = new Dictionary<string, Func<int>> {
                [Actor.EquipmentSlotId[0]] = ()=>Weapon,
                [Actor.EquipmentSlotId[1]] = ()=>Armor,
                [Actor.EquipmentSlotId[2]] = ()=>Accessory1,
                [Actor.EquipmentSlotId[3]] = () => Accessory2,
            };
        }
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
        public int Weapon { get; set; }
        public int Armor { get; set; }
        public int Accessory1 { get; set; }
        public int Accessory2 { get; set; }
        public int[] EquipSlots { get; set; }


        public ActorDef() {
            Level = 1;
            StatGrowth = new Dictionary<string, Dice>();
            EquipSlots = new[]{0,1,2};
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
        public int[] ActiveEquipSlots { get; set; }

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
            StatGrowth = def.StatGrowth ?? new Dictionary<string, Dice>();
            XP = def.XP;
            Level = def.Level;
            Equipment = new Equipment { Weapon = def.Weapon, Armor = def.Armor, Accessory1 = def.Accessory1, Accessory2 = def.Accessory2 };
            NextLevelXP = Levels.NextLevelXP(Level);
            ActiveEquipSlots = def.EquipSlots;
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

        public void RenderEquipment(Renderer renderer, int x, int y, int index) {
            x += 100;
            var label = EquipmentSlotLabels[index];
            renderer.AlignText(TextAlignment.Right, TextAlignment.Center);
            renderer.DrawText2D(x, y, label);

            var slotId = EquipmentSlotId[index];
            var text = "none";
            if (Equipment[slotId] > 0) {
                var itemId = Equipment[slotId];
                var item = ItemDB.Items[itemId];
                text = item.Name;
            }
            renderer.AlignText(TextAlignment.Left, TextAlignment.Center);
            renderer.DrawText2D(x + 10, y, text);
        }

        public static ReadOnlyCollection<string> EquipmentSlotLabels { get; } = new ReadOnlyCollection<string>(new[] {
            "Weapon:",
            "Armor:",
            "Accessory:",
            "Accessory:"
        });
        public static ReadOnlyCollection<string> EquipmentSlotId { get; } = new ReadOnlyCollection<string>(new[] {
            "weapon",
            "armor",
            "acces1",
            "acces2"
        });
        public static ReadOnlyCollection<string> ActorStats { get; }= new ReadOnlyCollection<string>(new[] {
            Stats.Strength,
            Stats.Speed,
            Stats.Intelligence
        });
        public static ReadOnlyCollection<string> ActorStatLabels { get; } = new ReadOnlyCollection<string>(new[] {
            "Strength",
            "Speed",
            "Intelligence"
        });
        public static ReadOnlyCollection<string> ItemStats { get; } = new ReadOnlyCollection<string>(new[] {
            "attack",
            "defense",
            "magic",
            "resist"
        });
        public static ReadOnlyCollection<string> ItemStatLabels { get; } = new ReadOnlyCollection<string>(new[] {
            "Attack",
            "Defense",
            "Magic",
            "Resist"
        });
        public ReadOnlyDictionary<string,string> ActionLabels { get; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> {
            ["attack"] = "Attack",
            ["item"] = "Item"
        });
    }
}