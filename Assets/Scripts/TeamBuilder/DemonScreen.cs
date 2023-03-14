using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameManager;

public class DemonScreen : MonoBehaviour
{
    public GameObject mainScreen;
    public GameObject demonDisplay;
    public GameObject demonDex;
    public GameObject textInfo;
    public GameObject savedDemons;
    public List<SavedData> actorBaseStats;

    private DemonDex demonList;
    private TeamBuilderControl teamBuilder;
    private int demIndex;
    private ActorStats savedDemon;
    private bool firstRun;
    private int statValue;
    private Dropdown skillDropdown;
    private int skillTypeVal;
    private List<Skill> skillDex;

    [System.Serializable]
    public class SavedData
    {
        public string name = "";
        public int demID = -1;
        public int level = 0;
        public int hp = 0;
        public int mp = 0;
        public int strength = 0;
        public int vitality = 0;
        public int magic = 0;
        public int agility = 0;
        public int luck = 0;
        public int hpMpPnts = 0;
        public int statPnts = 0;
        public List<Skill> skills = new List<Skill>();
    }

    // Start is called before the first frame update
    void Start()
    {
        demonList = demonDex.GetComponent<DemonDex>();
        teamBuilder = mainScreen.GetComponent<TeamBuilderControl>();
        statValue = 1;
        skillDropdown = transform.GetChild(2).Find("SkillDropdown").GetComponent<Dropdown>();

        foreach (Skill skill in teamBuilder.gameManager.skillCompendium)
        {
            var opData = new Dropdown.OptionData(skill.name);
            skillDropdown.options.Add(opData);
        }
        skillTypeVal = 0;
    }

    // Update is called once per frame
    void Update()
    {
        var allyDemon = demonList.MatchIndexWithDemon(transform.GetChild(0).Find("DemonType").GetComponent<Dropdown>().value);
        if (firstRun || demonDisplay.GetComponent<ActorStats>() == null || demonDisplay.GetComponent<ActorStats>().statSheet != allyDemon.GetComponent<ActorStats>().statSheet)
        {
            Destroy(demonDisplay);
            demonDisplay = Instantiate(allyDemon, new Vector3(3f, -1f, -5f), Quaternion.identity);

            savedDemon = savedDemons.transform.GetChild(demIndex).GetComponent<ActorStats>();
            var currDemon = demonDisplay.GetComponent<ActorStats>();
            if (currDemon.statSheet != savedDemon.statSheet)
            {
                savedDemon.statSheet = currDemon.statSheet;
                savedDemon.LoadCharacter();
                SetStatDefaults(savedDemon, demIndex);
                actorBaseStats[demIndex].hpMpPnts = 0;
                actorBaseStats[demIndex].statPnts = 0;

                foreach (int skillID in savedDemon.stats.baseSkills)
                {
                    savedDemon.stats.skills.Add(teamBuilder.gameManager.skillCompendium[skillID]);
                }
            }

            demonDisplay.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX |RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            demonDisplay.transform.SetParent(transform);
            demonDisplay.transform.rotation = Quaternion.Euler(0, 180, 0);
            firstRun = false;
        }

        textInfo.transform.GetChild(0).GetComponent<TMP_Text>().text = "Level - " + savedDemon.stats.level 
            + " (" + (savedDemon.stats.level - actorBaseStats[demIndex].level) + ")";
        textInfo.transform.GetChild(1).GetComponent<TMP_Text>().text = "HP - " + savedDemon.stats.baseStats.hp 
            + " (" + (savedDemon.stats.baseStats.hp - actorBaseStats[demIndex].hp) + ")";
        textInfo.transform.GetChild(2).GetComponent<TMP_Text>().text = "MP - " + savedDemon.stats.baseStats.mp
            + " (" + (savedDemon.stats.baseStats.mp - actorBaseStats[demIndex].mp) + ")";
        textInfo.transform.GetChild(3).GetComponent<TMP_Text>().text = "Strength - " + savedDemon.stats.baseStats.strength
            + " (" + (savedDemon.stats.baseStats.strength - actorBaseStats[demIndex].strength) + ")";
        textInfo.transform.GetChild(4).GetComponent<TMP_Text>().text = "Vitality - " + savedDemon.stats.baseStats.vitality
            + " (" + (savedDemon.stats.baseStats.vitality - actorBaseStats[demIndex].vitality) + ")";
        textInfo.transform.GetChild(5).GetComponent<TMP_Text>().text = "Magic - " + savedDemon.stats.baseStats.magic
            + " (" + (savedDemon.stats.baseStats.magic - actorBaseStats[demIndex].magic) + ")";
        textInfo.transform.GetChild(6).GetComponent<TMP_Text>().text = "Agility - " + savedDemon.stats.baseStats.agility
            + " (" + (savedDemon.stats.baseStats.agility - actorBaseStats[demIndex].agility) + ")";
        textInfo.transform.GetChild(7).GetComponent<TMP_Text>().text = "Luck - " + savedDemon.stats.baseStats.luck
            + " (" + (savedDemon.stats.baseStats.luck - actorBaseStats[demIndex].luck) + ")";
        textInfo.transform.GetChild(8).GetComponent<TMP_Text>().text = "HP/MP Points - " + actorBaseStats[demIndex].hpMpPnts;
        textInfo.transform.GetChild(9).GetComponent<TMP_Text>().text = "Stat Points - " + actorBaseStats[demIndex].statPnts;
        transform.GetChild(1).Find("ChangeAmntButton").GetChild(0).GetComponent<Text>().text = "+" + statValue;

        for(int i = 0; i < 8; ++i)
        {
            if (i < savedDemon.stats.skills.Count)
                transform.GetChild(2).GetChild(i + 1).GetComponent<TMP_Text>().text = savedDemon.stats.skills[i].name;
            else
                transform.GetChild(2).GetChild(i + 1).GetComponent<TMP_Text>().text = "---";
        }

        UpdateSkillDropDown();

        demonDisplay.transform.Rotate(0, -30 * Time.deltaTime, 0);
    }

    public void SetStatDefaults(ActorStats dem, int index)
    {
        actorBaseStats[index].name = dem.stats.name;
        //actorBaseStats[screenIndex].demID = 
        actorBaseStats[index].level = dem.stats.level;
        actorBaseStats[index].hp = dem.stats.baseStats.hp;
        actorBaseStats[index].mp = dem.stats.baseStats.mp;
        actorBaseStats[index].strength = dem.stats.baseStats.strength;
        actorBaseStats[index].vitality = dem.stats.baseStats.vitality;
        actorBaseStats[index].magic = dem.stats.baseStats.magic;
        actorBaseStats[index].agility = dem.stats.baseStats.agility;
        actorBaseStats[index].luck = dem.stats.baseStats.luck;

        foreach(Skill skill in dem.stats.skills)
        {
            actorBaseStats[index].skills.Add(skill);
        }
    }

    public void LoadBuiltDemon(int index)
    {
        demIndex = index;
        firstRun = true;
    }

    public void ChangeStatVal()
    {
        switch(statValue)
        {
            case 1:
                statValue = 5;
                return;
            case 5:
                statValue = 10;
                return;
            case 10:
                statValue = 1;
                return;
        }
    }

    public void AddSkill()
    {

        if (savedDemon.stats.skills.Count < 8)
        {
            foreach (Skill skill in savedDemon.stats.skills)
            {
                if (skill == skillDex[skillDropdown.value])
                    return;
            }

            savedDemon.stats.skills.Add(skillDex[skillDropdown.value]);
        }
    }

    public void RemoveSkill(int index)
    {
        if (savedDemon.stats.skills.Count > index)
            savedDemon.stats.skills.RemoveAt(index);
    }

    public void UpdateSkillDropDown()
    {
        int typeVal = transform.GetChild(2).Find("SkillTypeDropdown").GetComponent<Dropdown>().value;
        if (typeVal == skillTypeVal)
            return;
        
        skillDropdown.options = new List<Dropdown.OptionData>();
        skillDex = new List<Skill>();
        var startIndex = 0;
        var endIndex = 0;

        switch(typeVal)
        {
            case 0:
                startIndex = 0;
                endIndex = teamBuilder.gameManager.skillCompendium.Count;
                break;
            case 1:
                startIndex = 0;
                endIndex = 73;
                break;
            case 2:
                startIndex = 73;
                endIndex = 91;
                break;
            case 3:
                startIndex = 91;
                endIndex = 110;
                break;
            case 4:
                startIndex = 110;
                endIndex = 126;
                break;
            case 5:
                startIndex = 126;
                endIndex = 141;
                break;
            case 6:
                startIndex = 141;
                endIndex = 150;
                break;
            case 7:
                startIndex = 150;
                endIndex = 163;
                break;
            case 8:
                startIndex = 163;
                endIndex = 183;
                break;
            case 9:
                startIndex = 183;
                endIndex = 201;
                break;
            case 10:
                startIndex = 201;
                endIndex = 221;
                break;
            case 11:
                startIndex = 221;
                endIndex = 259;
                break;
            case 12:
                startIndex = 259;
                endIndex = teamBuilder.gameManager.skillCompendium.Count;
                break;
        }

        for (int i = startIndex; i < endIndex; ++i)
        {
            if (teamBuilder.gameManager.skillCompendium[i].unique == 0 || teamBuilder.gameManager.skillCompendium[i].unique == savedDemon.stats.demID)
            {
                var opData = new Dropdown.OptionData(teamBuilder.gameManager.skillCompendium[i].name);
                skillDropdown.options.Add(opData);
                skillDex.Add(teamBuilder.gameManager.skillCompendium[i]);
            }
        }

        skillTypeVal = typeVal;
        transform.GetChild(2).Find("SkillDropdown").GetComponent<Dropdown>().value = 1;
        transform.GetChild(2).Find("SkillDropdown").GetComponent<Dropdown>().value = 0;
    }

    public void DemonIncreaseStat(int stat)
    {
        switch (stat)
        {
            case 0:
                if (savedDemon.stats.level + statValue < 100)
                {
                    savedDemon.stats.level += statValue;
                    actorBaseStats[demIndex].hpMpPnts += statValue * 5;
                    actorBaseStats[demIndex].statPnts += statValue * 3;
                }
                return;
            case 1:
                if (actorBaseStats[demIndex].hpMpPnts >= statValue)
                {
                    savedDemon.stats.baseStats.hp += statValue;
                    actorBaseStats[demIndex].hpMpPnts -= statValue;
                }
                return;
            case 2:
                if (actorBaseStats[demIndex].hpMpPnts >= statValue)
                {
                    savedDemon.stats.baseStats.mp += statValue;
                    actorBaseStats[demIndex].hpMpPnts -= statValue;
                }
                return;
            case 3:
                if (actorBaseStats[demIndex].statPnts >= statValue)
                {
                    savedDemon.stats.baseStats.strength += statValue;
                    actorBaseStats[demIndex].statPnts -= statValue;
                }
                return;
            case 4:
                if (actorBaseStats[demIndex].statPnts >= statValue)
                {
                    savedDemon.stats.baseStats.vitality += statValue;
                    actorBaseStats[demIndex].statPnts -= statValue;
                }
                return;
            case 5:
                if (actorBaseStats[demIndex].statPnts >= statValue)
                {
                    savedDemon.stats.baseStats.magic += statValue;
                    actorBaseStats[demIndex].statPnts -= statValue;
                }
                return;
            case 6:
                if (actorBaseStats[demIndex].statPnts >= statValue)
                {
                    savedDemon.stats.baseStats.agility += statValue;
                    actorBaseStats[demIndex].statPnts -= statValue;
                }
                return;
            case 7:
                if (actorBaseStats[demIndex].statPnts >= statValue)
                {
                    savedDemon.stats.baseStats.luck += statValue;
                    actorBaseStats[demIndex].statPnts -= statValue;
                }
                return;
        }
    }

    public void DemonDecreaseStat(int stat)
    {
        switch (stat)
        {
            case 0:
                if (savedDemon.stats.level > actorBaseStats[demIndex].level && actorBaseStats[demIndex].hpMpPnts >= statValue * 5
                    && actorBaseStats[demIndex].statPnts >= statValue * 3)
                {
                    savedDemon.stats.level -= statValue;
                    actorBaseStats[demIndex].hpMpPnts -= statValue * 5;
                    actorBaseStats[demIndex].statPnts -= statValue * 3;
                }
                return;
            case 1:
                if (savedDemon.stats.baseStats.hp > actorBaseStats[demIndex].hp)
                {
                    int dec = statValue;
                    if (savedDemon.stats.baseStats.hp - actorBaseStats[demIndex].hp < statValue)
                        dec = savedDemon.stats.baseStats.hp - actorBaseStats[demIndex].hp;
                    savedDemon.stats.baseStats.hp -= dec;
                    actorBaseStats[demIndex].hpMpPnts += dec;
                }
                return;
            case 2:
                if (savedDemon.stats.baseStats.mp > actorBaseStats[demIndex].mp)
                {
                    int dec = statValue;
                    if (savedDemon.stats.baseStats.mp - actorBaseStats[demIndex].mp < statValue)
                        dec = savedDemon.stats.baseStats.mp - actorBaseStats[demIndex].mp;
                    savedDemon.stats.baseStats.mp -= dec;
                    actorBaseStats[demIndex].hpMpPnts += dec;
                }
                return;
            case 3:
                if (savedDemon.stats.baseStats.strength > actorBaseStats[demIndex].strength)
                {
                    int dec = statValue;
                    if (savedDemon.stats.baseStats.strength - actorBaseStats[demIndex].strength < statValue)
                        dec = savedDemon.stats.baseStats.strength- actorBaseStats[demIndex].strength;
                    savedDemon.stats.baseStats.strength -= dec;
                    actorBaseStats[demIndex].statPnts += dec;
                }
                return;
            case 4:
                if (savedDemon.stats.baseStats.vitality > actorBaseStats[demIndex].vitality)
                {
                    int dec = statValue;
                    if (savedDemon.stats.baseStats.vitality - actorBaseStats[demIndex].vitality < statValue)
                        dec = savedDemon.stats.baseStats.vitality - actorBaseStats[demIndex].vitality;
                    savedDemon.stats.baseStats.vitality -= dec;
                    actorBaseStats[demIndex].statPnts += dec;
                }
                return;
            case 5:
                if (savedDemon.stats.baseStats.magic > actorBaseStats[demIndex].magic)
                {
                    int dec = statValue;
                    if (savedDemon.stats.baseStats.magic - actorBaseStats[demIndex].magic < statValue)
                        dec = savedDemon.stats.baseStats.magic - actorBaseStats[demIndex].magic;
                    savedDemon.stats.baseStats.magic -= dec;
                    actorBaseStats[demIndex].statPnts += dec;
                }
                return;
            case 6:
                if (savedDemon.stats.baseStats.agility > actorBaseStats[demIndex].agility)
                {
                    int dec = statValue;
                    if (savedDemon.stats.baseStats.agility - actorBaseStats[demIndex].agility < statValue)
                        dec = savedDemon.stats.baseStats.agility - actorBaseStats[demIndex].agility;
                    savedDemon.stats.baseStats.agility -= dec;
                    actorBaseStats[demIndex].statPnts += dec;
                }
                return;
            case 7:
                if (savedDemon.stats.baseStats.luck > actorBaseStats[demIndex].luck)
                {
                    int dec = statValue;
                    if (savedDemon.stats.baseStats.luck - actorBaseStats[demIndex].luck < statValue)
                        dec = savedDemon.stats.baseStats.luck - actorBaseStats[demIndex].luck;
                    savedDemon.stats.baseStats.luck -= dec;
                    actorBaseStats[demIndex].statPnts += dec;
                }
                return;
        }
    }
}
