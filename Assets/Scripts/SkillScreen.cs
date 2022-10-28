using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class SkillScreen : MonoBehaviour
{
    public GameManager gameManager;
    public Button skillButton;
    public Canvas mainBattleScreen;
    public Canvas entityScreen;

    private GameObject selected;

    public void UpdateSkillButtons()
    {
        if (selected == gameManager.GetComponent<GameManager>().active)
            return;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        CreateAttackButton(gameManager.active.GetComponent<ActorStats>().stats.skills[0]);
        int skillCount = gameManager.active.GetComponent<ActorStats>().stats.skills.Count;
        
        for (int i = 0; i < skillCount; ++i)
        {
            var newSkill = Instantiate(skillButton, Vector3.zero, Quaternion.identity) as Button;
            newSkill.transform.SetParent(this.transform);
            var rectTransform = newSkill.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(1, 0);
            rectTransform.position = new Vector3(1690, 60 + (60 * (i + 1)), 0);

            NonPassiveSkill skill = (NonPassiveSkill) gameManager.active.GetComponent<ActorStats>().stats.skills[i];
            newSkill.GetComponentInChildren<Text>().text = skill.name + " - " + skill.cost + "MP";
            newSkill.onClick.AddListener(delegate { OnSkillButtonPress(skill); });
            newSkill.onClick.AddListener(entityScreen.GetComponent<EntityScreen>().UpdateSelectButtons);
        }

        CreateBackButton(skillCount + 1);
        selected = gameManager.GetComponent<GameManager>().active;
    }

    void CreateAttackButton(Skill skill)
    {
        var attack = Instantiate(skillButton, Vector3.zero, Quaternion.identity) as Button;
        attack.transform.SetParent(this.transform);
        var rectTransform = attack.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(1, 0);
        rectTransform.position = new Vector3(1690, 60, 0);
        
        attack.GetComponentInChildren<Text>().text = "Attack";
        attack.onClick.AddListener(delegate { OnSkillButtonPress(skill); });
        attack.onClick.AddListener(entityScreen.GetComponent<EntityScreen>().UpdateSelectButtons);
    }

    void CreateBackButton(int offset)
    {
        var back = Instantiate(skillButton, Vector3.zero, Quaternion.identity) as Button;
        back.transform.SetParent(transform);
        var rectTransform = back.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(1, 0);
        rectTransform.position = new Vector3(1690, 60 + (60 * offset), 0);

        back.GetComponentInChildren<Text>().text = "Back";
        back.onClick.AddListener(OnBackButtonPress);
    }

    void OnBackButtonPress()
    {
        mainBattleScreen.enabled = true;
        GetComponent<Canvas>().enabled = false;
        //gameManager.cameraAnimator.Play("SideToBackView");
    }

    void OnSkillButtonPress(Skill skill)
    {
        entityScreen.enabled = true;
        entityScreen.GetComponent<EntityScreen>().selectedSkill = (NonPassiveSkill) skill;
        GetComponent<Canvas>().enabled = false;

        var gameManagerScript = gameManager.GetComponent<GameManager>();
        //if ((gameManagerScript.DetermineSkillType(skill) < 2
        //        && gameManagerScript.playerTeam.GetComponent<Team>().homeTeam)
        //        || (gameManagerScript.DetermineSkillType(skill) >= 2
        //        && !gameManagerScript.playerTeam.GetComponent<Team>().homeTeam))
        //    gameManager.cameraAnimator.Play("SideToEnemyView");
        //else
        //    gameManager.cameraAnimator.Play("SideToAllyView");
    }
}
