using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Team : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject player;
    public List<GameObject> activeDemons;
    public GameObject[] demons;
    public List<OpposingData> opposingData;
    public bool homeTeam;
    public bool ai;

    [System.Serializable]
    public class OpposingData
    {
        public GameObject demon;
        public List<int> resistances;
        public List<int> ailmentResistances;
        public List<Skill> skills;
    }

    public void FillOpposingData(Team team) {
        opposingData.Add(CreateOpposingData(team.player));
        foreach (GameObject demon in team.demons)
        {
            opposingData.Add(CreateOpposingData(demon));
        }
    }

    OpposingData CreateOpposingData(GameObject demon)
    {
        OpposingData opposingData = new OpposingData();
        opposingData.demon = demon;
        opposingData.resistances = new List<int> { 7, 7, 7, 7, 7, 7, 7 };
        opposingData.ailmentResistances = new List<int> { 7, 7, 7, 7, 7, 7 };
        return opposingData;
    }

    public OpposingData FindOpposingData(GameObject demon)
    {
        foreach(OpposingData data in opposingData)
        {
            if (demon == data.demon)
                return data;
        }
        return null;
    }

    public void UpdateOpposingData(OpposingData data, NonPassiveSkill skill, ActorStats target)
    {
        if (skill.skillID <= ATTACK_ID)
        {
            data.resistances[((AttackSkill) skill).type] = gameManager.GetResistance((AttackSkill) skill, target);
        }
    }
}
