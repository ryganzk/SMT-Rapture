using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

[System.Serializable]
public class ActorStats : MonoBehaviour
{
    [SerializeField] public TextAsset statSheet;
    [SerializeField] public Stats stats;
    [SerializeField] public Sprite faceSprite;

    [SerializeField] public bool guard;
    [SerializeField] public List<List<int>> boosts;
    [SerializeField] public List<int> ailment = new List<int> { 0, 0, 0 };
    [SerializeField] public int charge = 0;
    [SerializeField] public int protective = 0;
    [SerializeField] public int damageDown = 0;
    [SerializeField] public List<int> taunt = new List<int> { 0, 0 };
    [SerializeField] public List<int> attack = new List<int> { 0, 0 };
    [SerializeField] public List<int> defense = new List<int> { 0, 0 };
    [SerializeField] public List<int> accEvas = new List<int> { 0, 0 };
    [SerializeField] public List<int> passives = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

    [System.Serializable]
    public class BaseStats
    {
        public int hp;
        public int mp;
        public int strength;
        public int vitality;
        public int magic;
        public int agility;
        public int luck;
    }

    [System.Serializable]
    public class BattleStats
    {
        public int hp;
        public int mp;
    }

    [System.Serializable]
    public class LevelSkills
    {
        public int physical;
        public int fire;
        public int ice;
        public int electric;
        public int force;
        public int light;
        public int dark;
    }

    [System.Serializable]
    public class Resistances
    {
        public int physical;
        public int fire;
        public int ice;
        public int electric;
        public int force;
        public int light;
        public int dark;
    }

    [System.Serializable]
    public class AilmentResistances
    {
        public int poison;
        public int confusion;
        public int charm;
        public int seal;
        public int sleep;
        public int mirage;
    }

    [System.Serializable]
    public class Potentials
    {
        public int physical;
        public int fire;
        public int ice;
        public int electric;
        public int force;
        public int light;
        public int dark;
        public int almighty;
        public int ailment;
        public int heal;
        public int support;
    }

    [System.Serializable]
    public class Stats
    {
        public string name;
        public string desc;
        public int race;
        public int level;
        public int demID;
        public List<int> religion;
        public List<int> alignment;
        public BaseStats baseStats;
        public BattleStats battleStats;
        public List<int> baseSkills;
        public List<int> levelSkills;
        [SerializeReference]
        public List<Skill> skills;
        public Resistances resistances;
        public AilmentResistances ailmentResistances;
        public Potentials potentials;
    }

    public void LoadCharacter()
    {
        stats = JsonUtility.FromJson<Stats>(statSheet.text);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
