using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillStats : MonoBehaviour
{
    [System.Serializable]
    public class Skill
    {
        public string name;
        public string desc;
        public int skillID;
        public int cost;
        public int level;
        public int targets;
        public int power;
        public int type;
        public int accuracy;
        public int[] hits;
        public bool physical;
        public bool pierce;
    }

    public Skill skill = new Skill();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
