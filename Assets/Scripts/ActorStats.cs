using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class ActorStats : MonoBehaviour
{
    public TextAsset statSheet;
    public Stats stats;
    public Sprite faceSprite;
    public bool guard;

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
        public int demID;
        public int[] religion;
        public int[] alignment;
        public int level;
        public BaseStats baseStats;
        public int[] baseSkills;
        public int[] levelSkills;
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
