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
    public GameObject demonDex;
    public GameObject exportErrorText;

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
                currDemon.GetComponent<ActorStats>().stats.dexID = 71;
            else
                currDemon.GetComponent<ActorStats>().stats.dexID = 92;
        
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

            if (i == 0)
                currDemon.GetComponent<ActorStats>().stats.dexID = 71;
            else
                currDemon.GetComponent<ActorStats>().stats.dexID = 92;

            foreach (int skillID in currDemon.GetComponent<ActorStats>().stats.baseSkills)
            {
                currDemon.GetComponent<ActorStats>().stats.skills.Add(gameManager.skillCompendium[skillID]);
            }

            enemyDemonEditor.GetComponent<DemonScreen>().SetStatDefaults(currDemon.GetComponent<ActorStats>(), i);
        }

        allyDemonEditor.transform.GetChild(0).Find("DemonType").GetComponent<DemonDropdown>().PopulateDropDown();
        enemyDemonEditor.transform.GetChild(0).Find("DemonType").GetComponent<DemonDropdown>().PopulateDropDown();
    }

    public void StartGame()
    {
        // PLAYER TEAM
        var playerStats = playerDemons.transform.GetChild(0).GetComponent<ActorStats>();
        PlayerPrefs.SetInt("allyPlayer", playerStats.stats.dexID);
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

        PlayerPrefs.SetFloat("allyPlayerAIAttackFactor", 1.0f);
        PlayerPrefs.SetFloat("allyPlayerAIAilmentFactor", 1.0f);
        PlayerPrefs.SetFloat("allyPlayerAIHealFactor", 1.0f);
        PlayerPrefs.SetFloat("allyPlayerAISupportFactor", 1.0f);
        PlayerPrefs.SetFloat("allyPlayerAISelfFactor", 1.0f);
        PlayerPrefs.SetFloat("allyPlayerAIAllyPlayerFactor", 1.0f);
        PlayerPrefs.SetFloat("allyPlayerAIEnemyPlayerFactor", 1.0f);
        PlayerPrefs.SetFloat("allyPlayerAIAllyTeamFactor", 1.0f);
        PlayerPrefs.SetFloat("allyPlayerAIEnemyTeamFactor", 1.0f);

        for (int i = 0; i < 3; ++i)
        {
            var teammateStats = playerDemons.transform.GetChild(i + 1).GetComponent<ActorStats>();
            PlayerPrefs.SetInt("allyTeammate" + i, teammateStats.stats.dexID);
            PlayerPrefs.SetInt("allyTeammate" + i + "Level", teammateStats.stats.level);
            PlayerPrefs.SetInt("allyTeammate" + i + "Hp", teammateStats.stats.baseStats.hp);
            PlayerPrefs.SetInt("allyTeammate" + i + "Mp", teammateStats.stats.baseStats.mp);
            PlayerPrefs.SetInt("allyTeammate" + i + "Strength", teammateStats.stats.baseStats.strength);
            PlayerPrefs.SetInt("allyTeammate" + i + "Vitality", teammateStats.stats.baseStats.vitality);
            PlayerPrefs.SetInt("allyTeammate" + i + "Magic", teammateStats.stats.baseStats.magic);
            PlayerPrefs.SetInt("allyTeammate" + i + "Agility", teammateStats.stats.baseStats.agility);
            PlayerPrefs.SetInt("allyTeammate" + i + "Luck", teammateStats.stats.baseStats.luck);

            for(int j = 0; j < 8; ++j)
            { 
                if (j < teammateStats.stats.skills.Count)
                    PlayerPrefs.SetInt("allyTeammate" + i + "Skill" + j, teammateStats.stats.skills[j].skillID);
                else
                    PlayerPrefs.SetInt("allyTeammate" + i + "Skill" + j, -1);
            }

            PlayerPrefs.SetFloat("allyTeammate" + i + "AIAttackFactor", 1.0f);
            PlayerPrefs.SetFloat("allyTeammate" + i + "AIAilmentFactor", 1.0f);
            PlayerPrefs.SetFloat("allyTeammate" + i + "AIHealFactor", 1.0f);
            PlayerPrefs.SetFloat("allyTeammate" + i + "AISupportFactor", 1.0f);
            PlayerPrefs.SetFloat("allyTeammate" + i + "AISelfFactor", 1.0f);
            PlayerPrefs.SetFloat("allyTeammate" + i + "AIAllyPlayerFactor", 1.0f);
            PlayerPrefs.SetFloat("allyTeammate" + i + "AIEnemyPlayerFactor", 1.0f);
            PlayerPrefs.SetFloat("allyTeammate" + i + "AIAllyTeamFactor", 1.0f);
            PlayerPrefs.SetFloat("allyTeammate" + i + "AIEnemyTeamFactor", 1.0f);
        }

        // ENEMY TEAM
        var enemyStats = enemyDemons.transform.GetChild(0).GetComponent<ActorStats>();
        PlayerPrefs.SetInt("enemyPlayer", enemyStats.stats.dexID);
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
        }

        PlayerPrefs.SetFloat("enemyPlayerAIAttackFactor", 1.0f);
        PlayerPrefs.SetFloat("enemyPlayerAIAilmentFactor", 1.0f);
        PlayerPrefs.SetFloat("enemyPlayerAIHealFactor", 1.0f);
        PlayerPrefs.SetFloat("enemyPlayerAISupportFactor", 1.0f);
        PlayerPrefs.SetFloat("enemyPlayerAISelfFactor", 1.0f);
        PlayerPrefs.SetFloat("enemyPlayerAIAllyPlayerFactor", 1.0f);
        PlayerPrefs.SetFloat("enemyPlayerAIEnemyPlayerFactor", 1.0f);
        PlayerPrefs.SetFloat("enemyPlayerAIAllyTeamFactor", 1.0f);
        PlayerPrefs.SetFloat("enemyPlayerAIEnemyTeamFactor", 1.0f);

        for (int i = 0; i < 3; ++i)
        {
            var teammateStats = enemyDemons.transform.GetChild(i + 1).GetComponent<ActorStats>();
            PlayerPrefs.SetInt("enemyTeammate" + i, teammateStats.stats.dexID);
            PlayerPrefs.SetInt("enemyTeammate" + i + "Level", teammateStats.stats.level);
            PlayerPrefs.SetInt("enemyTeammate" + i + "Hp", teammateStats.stats.baseStats.hp);
            PlayerPrefs.SetInt("enemyTeammate" + i + "Mp", teammateStats.stats.baseStats.mp);
            PlayerPrefs.SetInt("enemyTeammate" + i + "Strength", teammateStats.stats.baseStats.strength);
            PlayerPrefs.SetInt("enemyTeammate" + i + "Vitality", teammateStats.stats.baseStats.vitality);
            PlayerPrefs.SetInt("enemyTeammate" + i + "Magic", teammateStats.stats.baseStats.magic);
            PlayerPrefs.SetInt("enemyTeammate" + i + "Agility", teammateStats.stats.baseStats.agility);
            PlayerPrefs.SetInt("enemyTeammate" + i + "Luck", teammateStats.stats.baseStats.luck);

            for(int j = 0; j < 8; ++j)
            { 
                if (j < teammateStats.stats.skills.Count)
                    PlayerPrefs.SetInt("enemyTeammate" + i + "Skill" + j, teammateStats.stats.skills[j].skillID);
                else
                    PlayerPrefs.SetInt("enemyTeammate" + i + "Skill" + j, -1);
            }

            PlayerPrefs.SetFloat("enemyTeammate" + i + "AIAttackFactor", 1.0f);
            PlayerPrefs.SetFloat("enemyTeammate" + i + "AIAilmentFactor", 1.0f);
            PlayerPrefs.SetFloat("enemyTeammate" + i + "AIHealFactor", 1.0f);
            PlayerPrefs.SetFloat("enemyTeammate" + i + "AISupportFactor", 1.0f);
            PlayerPrefs.SetFloat("enemyTeammate" + i + "AISelfFactor", 1.0f);
            PlayerPrefs.SetFloat("enemyTeammate" + i + "AIAllyPlayerFactor", 1.0f);
            PlayerPrefs.SetFloat("enemyTeammate" + i + "AIEnemyPlayerFactor", 1.0f);
            PlayerPrefs.SetFloat("enemyTeammate" + i + "AIAllyTeamFactor", 1.0f);
            PlayerPrefs.SetFloat("enemyTeammate" + i + "AIEnemyTeamFactor", 1.0f);
        }

        SceneManager.LoadScene("BattleScene");
    }

    public void SetAllyIndex(int index)
    {
        screenIndex = index;
        allyDemonEditor.GetChild(0).Find("DemonType").GetComponent<Dropdown>().value = playerDemons.transform.GetChild(screenIndex).GetComponent<ActorStats>().stats.dexID;
    }

    public void SetEnemyIndex(int index)
    {
        screenIndex = index;
        enemyDemonEditor.GetChild(0).Find("DemonType").GetComponent<Dropdown>().value = enemyDemons.transform.GetChild(screenIndex).GetComponent<ActorStats>().stats.dexID;
    }

    public void CloseAllyChangeScreen()
    {
        playerDemons.transform.GetChild(screenIndex).GetComponent<ActorStats>().stats.dexID = allyDemonEditor.GetChild(0).Find("DemonType").GetComponent<Dropdown>().value;
    }

    public void CloseEnemyChangeScreen()
    {
        enemyDemons.transform.GetChild(screenIndex).GetComponent<ActorStats>().stats.dexID = enemyDemonEditor.GetChild(0).Find("DemonType").GetComponent<Dropdown>().value;
    }

    public void PopulateImportDropDown(Dropdown importDrop)
    {
        importDrop.options = new List<Dropdown.OptionData>();
        string[] subdirectories = Directory.GetDirectories(Application.persistentDataPath + "/teams/");

        // Loop through each subdirectory and add its name to the list
        foreach (string subdirectory in subdirectories)
        {
            var dir = new DirectoryInfo(subdirectory);
            var opData = new Dropdown.OptionData(dir.Name);
            importDrop.options.Add(opData);

            if (subdirectory == subdirectories[0])
                importDrop.transform.GetChild(0).GetComponent<Text>().text = dir.Name;
        }
        importDrop.value = 0;
    }

    public void ExportPlayerJson(InputField input)
    {
        string name = input.text;
        if (name == "")
        {
            exportErrorText.SetActive(true);
            return;
        }

        exportErrorText.SetActive(false);
        Directory.CreateDirectory(Application.persistentDataPath + "/teams/" + name);
        var jsonData = JsonUtility.ToJson(playerDemons.transform.GetChild(0).GetComponent<ActorStats>());
        File.WriteAllText(Application.persistentDataPath + "/teams/" + name + "/player.json", jsonData);

        for (int i = 0; i < 3; ++i)
        {
            jsonData = JsonUtility.ToJson(playerDemons.transform.GetChild(i + 1).GetComponent<ActorStats>());
            File.WriteAllText(Application.persistentDataPath + "/teams/" + name + "/teammate" + i + ".json", jsonData);
        }
    }

    public void ExportEnemyJson(InputField input)
    {
        string name = input.text;
        if (name == "")
        {
            exportErrorText.SetActive(true);
            return;
        }

        exportErrorText.SetActive(false);
        Directory.CreateDirectory(Application.persistentDataPath + "/teams/" + name);
        var jsonData = JsonUtility.ToJson(enemyDemons.transform.GetChild(0).GetComponent<ActorStats>());
        File.WriteAllText(Application.persistentDataPath + "/teams/" + name + "/player.json", jsonData);

        for (int i = 0; i < 3; ++i)
        {
            jsonData = JsonUtility.ToJson(enemyDemons.transform.GetChild(i + 1).GetComponent<ActorStats>());
            File.WriteAllText(Application.persistentDataPath + "/teams/" + name + "/teammate" + i + ".json", jsonData);
        }
    }

    public void ImportPlayerJson(Dropdown importDrop)
    {
        string name = importDrop.options[importDrop.value].text;
        var jsonData = File.ReadAllText(Application.persistentDataPath + "/teams/" + name + "/player.json");
        var changedDemon = playerDemons.transform.GetChild(0).GetComponent<ActorStats>();
        JsonUtility.FromJsonOverwrite(jsonData, changedDemon);
        var baseDemon = demonDex.GetComponent<DemonDex>().demonDex[changedDemon.stats.dexID].GetComponent<ActorStats>();
        baseDemon.LoadCharacter();
        allyDemonEditor.GetComponent<DemonScreen>().SetStatDefaults(baseDemon, 0);

        for (int i = 0; i < 3; ++i)
        {
            jsonData = File.ReadAllText(Application.persistentDataPath + "/teams/" + name + "/teammate" + i + ".json");
            JsonUtility.FromJsonOverwrite(jsonData, playerDemons.transform.GetChild(i + 1).GetComponent<ActorStats>());
            allyDemonEditor.GetComponent<DemonScreen>().SetStatDefaults(playerDemons.transform.GetChild(i + 1).GetComponent<ActorStats>(), i + 1);
        }
    }

    public void ImportEnemyJson(Dropdown importDrop)
    {
        string name = importDrop.options[importDrop.value].text;
        var jsonData = File.ReadAllText(Application.persistentDataPath + "/teams/" + name + "/player.json");
        var changedDemon = enemyDemons.transform.GetChild(0).GetComponent<ActorStats>();
        JsonUtility.FromJsonOverwrite(jsonData, changedDemon);
        var baseDemon = demonDex.GetComponent<DemonDex>().demonDex[changedDemon.stats.dexID].GetComponent<ActorStats>();
        baseDemon.LoadCharacter();
        enemyDemonEditor.GetComponent<DemonScreen>().SetStatDefaults(baseDemon, 0);

        for (int i = 0; i < 3; ++i)
        {
            jsonData = File.ReadAllText(Application.persistentDataPath + "/teams/" + name + "/teammate" + i + ".json");
            JsonUtility.FromJsonOverwrite(jsonData, enemyDemons.transform.GetChild(i + 1).GetComponent<ActorStats>());
            enemyDemonEditor.GetComponent<DemonScreen>().SetStatDefaults(enemyDemons.transform.GetChild(i + 1).GetComponent<ActorStats>(), i + 1);
        }
    }
}
