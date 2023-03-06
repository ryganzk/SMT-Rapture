using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeamBuilderControl : MonoBehaviour
{
    public Transform allyDemonEditor;
    public Transform enemyDemonEditor;
    public List<int> demonIDs;
    public int screenIndex;

    public void Start()
    {
        demonIDs = new List<int>(){ 31, 41, 41, 41, 31, 41, 41, 41 };
    }

    public void StartGame()
    {
        
        PlayerPrefs.SetInt("allyPlayer", demonIDs[0]);
        for (int i = 0; i < 3; ++i)
        {
            PlayerPrefs.SetInt("allyTeammate" + i, demonIDs[i + 1]);
        }

        PlayerPrefs.SetInt("enemyPlayer", demonIDs[4]);
        for (int i = 0; i < 3; ++i)
        {
            PlayerPrefs.SetInt("enemyTeammate" + i, demonIDs[i + 5]);
        }

        SceneManager.LoadScene("BattleScene");
    }

    public void SetAllyIndex(int index)
    {
        screenIndex = index;
        allyDemonEditor.Find("DemonType").GetComponent<Dropdown>().value = demonIDs[screenIndex];
    }

    public void SetEnemyIndex(int index)
    {
        screenIndex = index;
        enemyDemonEditor.Find("DemonType").GetComponent<Dropdown>().value = demonIDs[screenIndex + 4];
    }

    public void CloseAllyChangeScreen()
    {
        demonIDs[screenIndex] = allyDemonEditor.Find("DemonType").GetComponent<Dropdown>().value;
    }

    public void CloseEnemyChangeScreen()
    {
        demonIDs[screenIndex + 4] = enemyDemonEditor.Find("DemonType").GetComponent<Dropdown>().value;
    }
}
