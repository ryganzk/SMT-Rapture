using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class EntityScreen : MonoBehaviour
{
    public GameManager gameManager;
    public Button selectButton;
    public Button backButton;
    public Canvas mainBattleScreen;
    public Canvas skillScreen;

    [SerializeReference]
    public Skill selectedSkill;
    
    public void UpdateSelectButtons()
    {
        foreach (Transform obj in gameManager.opponentTeam.transform)
        {
            var button = Instantiate(selectButton, obj.transform.position, Quaternion.identity);
            button.transform.SetParent(this.transform);
            button.GetComponent<EntityHUD>().lookAt = obj;
            button.onClick.AddListener(OnSelectButtonPress);
        }

        CreateBackButton();
    }

    void OnSelectButtonPress()
    {
        GetComponent<Canvas>().enabled = false;
        mainBattleScreen.enabled = true;
        gameManager.cameraAnimator.Play("BackView");

        switch (gameManager.DetermineSkillType(GetComponent<EntityScreen>().selectedSkill))
        {
            case 0:
            case 1:
                gameManager.active.GetComponent<Animator>().Play("NahobinoSkillAtk");
                break;
            case 2:
            case 3:
                gameManager.active.transform.Rotate(0, 90, 0);
                gameManager.active.GetComponent<Animator>().Play("NahobinoSkillRcv");
                break;

        }
    }

    void CreateBackButton()
    {
        var back = Instantiate(backButton, Vector3.zero, Quaternion.identity) as Button;
        back.transform.SetParent(transform);
        var rectTransform = back.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(1, 0);
        rectTransform.position = new Vector3(1690, 60, 0);

        back.GetComponentInChildren<Text>().text = "Back";
        back.onClick.AddListener(OnBackButtonPress);
    }

    void OnBackButtonPress()
    {
        skillScreen.enabled = true;
        GetComponent<Canvas>().enabled = false;
        gameManager.cameraAnimator.Play("EnemyToSideView");
    }
}
