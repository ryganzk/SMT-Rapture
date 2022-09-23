using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillScreen : MonoBehaviour
{
    public GameManager gameManager;
    public Button skillButton;
    public Canvas mainBattleScreen;

    public void UpdateSkillButtons()
    {
        if (transform.childCount == 0)
        {
            int skillCount = gameManager.active.GetComponent<ActorStats>().stats.skills.Count;

            for (int i = 0; i < skillCount; ++i)
            {
                var newSkill = Instantiate(skillButton, Vector3.zero, Quaternion.identity) as Button;
                newSkill.transform.SetParent(this.transform);
                var rectTransform = newSkill.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(1, 0);
                rectTransform.position = new Vector3(1690, 60 + (60 * i), 0);

                var skill = gameManager.active.GetComponent<ActorStats>().stats.skills[i];
                newSkill.GetComponentInChildren<Text>().text = skill.name;
            }

            CreateBackButton(skillCount);
        }
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
}
