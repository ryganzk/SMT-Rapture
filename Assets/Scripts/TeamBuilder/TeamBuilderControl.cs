using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DemonScreen;

[System.Serializable]
public class TeamList {
    public List<ActorStats> teamData;
}

public class TeamBuilderControl : MonoBehaviour
{
    public GameManager gameManager;
    public Transform allyDemonEditor;
    public Transform enemyDemonEditor;
    public int screenIndex;
    public GameObject playerDemons;
    public GameObject enemyDemons;

    public void Start()
    {
        Directory.CreateDirectory(Application.persistentDataPath + "/teams/");
        gameManager.ReadCompendiums();
        
        allyDemonEditor.GetComponent<DemonScreen>().actorBaseStats = new List<SavedData>(){ new SavedData(), new SavedData(), new SavedData(), new SavedData() };
        for (int i = 0; i < 4; ++i)
        {
            GameObject currDemon = playerDemons.transform.GetChild(i).gameObject;
            currDemon.GetComponent<ActorStats>().LoadCharacter();

            if (i == 0)
                allyDemonEditor.GetComponent<DemonScreen>().actorBaseStats[i].demID = 31;
            else
                allyDemonEditor.GetComponent<DemonScreen>().actorBaseStats[i].demID = 41;
        
            foreach (int skillID in currDemon.GetComponent<ActorStats>().stats.baseSkills)
            {
                currDemon.GetComponent<ActorStats>().stats.skills.Add(gameManager.skillCompendium[skillID]);
            }

            allyDemonEditor.GetComponent<DemonScreen>().SetStatDefaults(currDemon.GetComponent<ActorStats>(), i);
        }

        enemyDemonEditor.GetComponent<DemonScreen>().actorBaseStats = new List<SavedData>(){ new SavedData(), new SavedData(), new SavedData(), new SavedData() };
        for (int i = 0; i < 4; ++i)
        {
            GameObject currDemon = enemyDemons.transform.GetChild(i).gameObject;
            currDemon.GetComponent<ActorStats>().LoadCharacter();

            foreach (int skillID in currDemon.GetComponent<ActorStats>().stats.baseSkills)
            {
                currDemon.GetComponent<ActorStats>().stats.skills.Add(gameManager.skillCompendium[skillID]);
            }

            enemyDemonEditor.GetComponent<DemonScreen>().SetStatDefaults(currDemon.GetComponent<ActorStats>(), i);
        }
    }

    public void StartGame()
    {
        // PLAYER TEAM
        var playerStats = playerDemons.transform.GetChild(0).GetComponent<ActorStats>();
        PlayerPrefs.SetInt("allyPlayer", allyDemonEditor.GetComponent<DemonScreen>().actorBaseStats[0].demID);
        PlayerPrefs.SetInt("allyPlayerLevel", playerStats.stats.level);
        PlayerPrefs.SetInt("allyPlayerHp", playerStats.stats.baseStats.hp);
        PlayerPrefs.SetInt("allyPlayerMp", playerStats.stats.baseStats.mp);
        PlayerPrefs.SetInt("allyPlayerStrength", playerStats.stats.baseStats.strength);
        PlayerPrefs.SetInt("allyPlayerVitality", playerStats.stats.baseStats.vitality);
        PlayerPrefs.SetInt("allyPlayerMagic", playerStats.stats.baseStats.magic);
        PlayerPrefs.SetInt("allyPlayerAgility", playerStats.stats.baseStats.agility);
        PlayerPrefs.SetInt("allyPlayerLuck", playerStats.stats.baseStats.luck);

        for(int i = 0; i < 8; ++i)
        { 
            if (i < playerStats.stats.skills.Count)
                PlayerPrefs.SetInt("allyPlayerSkill" + i, playerStats.stats.skills[i].skillID);
            else
                PlayerPrefs.SetInt("allyPlayerSkill" + i, -1);

        }

        for (int i = 0; i < 3; ++i)
        {
            var teammateStats = playerDemons.transform.GetChild(i + 1).GetComponent<ActorStats>();
            PlayerPrefs.SetInt("allyTeammate" + i, allyDemonEditor.GetComponent<DemonScreen>().actorBaseStats[i + 1].demID);
            PlayerPrefs.SetInt("allyTeammate" + i + "Level", teammateStats.stats.level);
            PlayerPrefs.SetInt("allyTeammate" + i + "Hp", teammateStats.stats.baseStats.hp);
            PlayerPrefs.SetInt("allyTeammate" + i + "Mp", teammateStats.stats.baseStats.mp);
            PlayerPrefs.SetInt("allyTeammate" + i + "Strength", teammateStats.stats.baseStats.strength);
            PlayerPrefs.SetInt("allyTeammate" + i + "Vitality", teammateStats.stats.baseStats.vitality);
            PlayerPrefs.SetInt("allyTeammate" + i + "Magic", teammateStats.stats.baseStats.magic);
            PlayerPrefs.SetInt("allyTeammate" + i + "Agility", teammateStats.stats.baseStats.agility);
            PlayerPrefs.SetInt("allyTeammate" + i + "Luck", teammateStats.stats.baseStats.luck);

            for(int j = 0; j < 8; ++i)
            { 
                if (i < playerStats.stats.skills.Count)
                    PlayerPrefs.SetInt("allyTeammate" + i + "Skill" + j, playerStats.stats.skills[j].skillID);
                else
                    PlayerPrefs.SetInt("allyTeammate" + i + "Skill" + j, -1);
            }
        }

        // ENEMY TEAM
        var enemyStats = enemyDemons.transform.GetChild(0).GetComponent<ActorStats>();
        PlayerPrefs.SetInt("enemyPlayer", enemyDemonEditor.GetComponent<DemonScreen>().actorBaseStats[0].demID);
        PlayerPrefs.SetInt("enemyPlayerLevel", enemyStats.stats.level);
        PlayerPrefs.SetInt("enemyPlayerHp", enemyStats.stats.baseStats.hp);
        PlayerPrefs.SetInt("enemyPlayerMp", enemyStats.stats.baseStats.mp);
        PlayerPrefs.SetInt("enemyPlayerStrength", enemyStats.stats.baseStats.strength);
        PlayerPrefs.SetInt("enemyPlayerVitality", enemyStats.stats.baseStats.vitality);
        PlayerPrefs.SetInt("enemyPlayerMagic", enemyStats.stats.baseStats.magic);
        PlayerPrefs.SetInt("enemyPlayerAgility", enemyStats.stats.baseStats.agility);
        PlayerPrefs.SetInt("enemyPlayerLuck", enemyStats.stats.baseStats.luck);

        for(int i = 0; i < 8; ++i)
        { 
            if (i < enemyStats.stats.skills.Count)
                PlayerPrefs.SetInt("enemyPlayerSkill" + i, enemyStats.stats.skills[i].skillID);
            else
                PlayerPrefs.SetInt("enemyPlayerSkill" + i, -1);

            for(int j = 0; j < 8; ++i)
            { 
                if (i < enemyStats.stats.skills.Count)
                    PlayerPrefs.SetInt("allyTeammate" + i + "Skill" + j, enemyStats.stats.skills[j].skillID);
                else
                    PlayerPrefs.SetInt("allyTeammate" + i + "Skill" + j, -1);
            }
        }

        for (int i = 0; i < 3; ++i)
        {
            var teammateStats = enemyDemons.transform.GetChild(i + 1).GetComponent<ActorStats>();
            PlayerPrefs.SetInt("enemyTeammate" + i, allyDemonEditor.GetComponent<DemonScreen>().actorBaseStats[i + 1].demID);
            PlayerPrefs.SetInt("enemyTeammate" + i + "Level", teammateStats.stats.level);
            PlayerPrefs.SetInt("enemyTeammate" + i + "Hp", teammateStats.stats.baseStats.hp);
            PlayerPrefs.SetInt("enemyTeammate" + i + "Mp", teammateStats.stats.baseStats.mp);
            PlayerPrefs.SetInt("enemyTeammate" + i + "Strength", teammateStats.stats.baseStats.strength);
            PlayerPrefs.SetInt("enemyTeammate" + i + "Vitality", teammateStats.stats.baseStats.vitality);
            PlayerPrefs.SetInt("enemyTeammate" + i + "Magic", teammateStats.stats.baseStats.magic);
            PlayerPrefs.SetInt("enemyTeammate" + i + "Agility", teammateStats.stats.baseStats.agility);
            PlayerPrefs.SetInt("enemyTeammate" + i + "Luck", teammateStats.stats.baseStats.luck);
        }

        SceneManager.LoadScene("BattleScene");
    }

    public void SetAllyIndex(int index)
    {
        screenIndex = index;
        allyDemonEditor.GetChild(screenIndex).Find("DemonType").GetComponent<Dropdown>().value = allyDemonEditor.GetComponent<DemonScreen>().actorBaseStats[screenIndex].demID;
    }

    public void SetEnemyIndex(int index)
    {
        screenIndex = index;
        enemyDemonEditor.GetChild(screenIndex).Find("DemonType").GetComponent<Dropdown>().value = enemyDemonEditor.GetComponent<DemonScreen>().actorBaseStats[screenIndex].demID;
    }

    public void CloseAllyChangeScreen()
    {
        allyDemonEditor.GetComponent<DemonScreen>().actorBaseStats[screenIndex].demID = allyDemonEditor.GetChild(0).Find("DemonType").GetComponent<Dropdown>().value;
    }

    public void CloseEnemyChangeScreen()
    {
        enemyDemonEditor.GetComponent<DemonScreen>().actorBaseStats[screenIndex].demID = enemyDemonEditor.GetChild(0).Find("DemonType").GetComponent<Dropdown>().value;
    }

    public void PopulateImportDropDown(Dropdown importDrop)
    {
        // var importDrop = allyFileTools.GetChild(1).Find("DirectorySelect").GetComponent<Dropdown>();
        importDrop.options = new List<Dropdown.OptionData>();
        string[] subdirectories = Directory.GetDirectories(Application.persistentDataPath + "/teams/");

        // Loop through each subdirectory and add its name to the list
        foreach (string subdirectory in subdirectories)
        {
            var dir = new DirectoryInfo(subdirectory);
            var opData = new Dropdown.OptionData(dir.Name);
            importDrop.options.Add(opData);
        }
    }

    public void ExportJson(string name)
    {
        Directory.CreateDirectory(Application.persistentDataPath + "/teams/" + name);
        var jsonData = JsonUtility.ToJson(playerDemons.transform.GetChild(0).GetComponent<ActorStats>());
        File.WriteAllText(Application.persistentDataPath + "/teams/" + name + "/player.json", jsonData);

        for (int i = 0; i < 3; ++i)
        {
            jsonData = JsonUtility.ToJson(playerDemons.transform.GetChild(i + 1).GetComponent<ActorStats>());
            File.WriteAllText(Application.persistentDataPath + "/teams/" + name + "/teammate" + i + ".json", jsonData);
        }
    }

    public void ImportJson(Dropdown importDrop)
    {
        // Remember to reset allocatable points
        allyDemonEditor.GetComponent<DemonScreen>().SetStatDefaults(playerDemons.transform.GetChild(0).gameObject.GetComponent<ActorStats>(), 0);

        string name = importDrop.options[importDrop.value].text;
        var jsonData = File.ReadAllText(Application.persistentDataPath + "/teams/" + name + "/player.json");
        JsonUtility.FromJsonOverwrite(jsonData, playerDemons.transform.GetChild(0).GetComponent<ActorStats>());
        allyDemonEditor.GetComponent<DemonScreen>().actorBaseStats[screenIndex].demID = 31;
    }
}
