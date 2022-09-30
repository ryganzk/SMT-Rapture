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

    public void UpdateSkillButtons()
    {
        CreateAttackButton(gameManager.active.GetComponent<ActorStats>().stats.skills[0]);
        int skillCount = gameManager.active.GetComponent<ActorStats>().stats.skills.Count;

        if (transform.childCount == 1)
        {
            for (int i = 0; i < skillCount; ++i)
            {
                var newSkill = Instantiate(skillButton, Vector3.zero, Quaternion.identity) as Button;
                newSkill.transform.SetParent(this.transform);
                var rectTransform = newSkill.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(1, 0);
                rectTransform.position = new Vector3(1690, 60 + (60 * (i + 1)), 0);

                var skill = gameManager.active.GetComponent<ActorStats>().stats.skills[i];
                newSkill.GetComponentInChildren<Text>().text = skill.name;
                newSkill.onClick.AddListener(delegate { OnSkillButtonPress(skill); });
                newSkill.onClick.AddListener(entityScreen.GetComponent<EntityScreen>().UpdateSelectButtons);
            }
        }

        CreateBackButton(skillCount + 1);
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
        gameManager.cameraAnimator.Play("SideToBackView");
    }

    void OnSkillButtonPress(Skill skill)
    {
        entityScreen.enabled = true;
        entityScreen.GetComponent<EntityScreen>().selectedSkill = skill;
        GetComponent<Canvas>().enabled = false;
        gameManager.cameraAnimator.Play("SideToEnemyView");
    }
}
