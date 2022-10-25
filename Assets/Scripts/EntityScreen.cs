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
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Team team;
        if (gameManager.GetComponent<GameManager>().DetermineSkillType(selectedSkill) < 2)
            team = gameManager.opponentTeam.GetComponent<Team>();
        else
            team = gameManager.playerTeam.GetComponent<Team>();


        CreateSelectButton(team.player);
        foreach (GameObject obj in team.activeDemons)
        {
            if (obj != null)
                CreateSelectButton(obj);
        }

        CreateBackButton();
    }

    void CreateSelectButton(GameObject obj)
    {
        var button = Instantiate(selectButton, obj.transform.position, Quaternion.identity);
        button.transform.SetParent(this.transform);
        button.GetComponent<EntityHUD>().lookAt = obj.transform;
        button.onClick.AddListener(delegate { OnSelectButtonPress(obj); });
    }

    void OnSelectButtonPress(GameObject obj)
    {
        GetComponent<Canvas>().enabled = false;
        mainBattleScreen.enabled = true;
        gameManager.cameraAnimator.Play("BackView");

        switch (gameManager.DetermineSkillType(GetComponent<EntityScreen>().selectedSkill))
        {
            case 0:
            case 1:
                gameManager.active.GetComponent<Animator>().SetTrigger("skillAtk");
                gameManager.GetComponent<GameManager>().Damage((AttackSkill) selectedSkill, obj);
                break;
            case 2:
            case 3:
                gameManager.active.GetComponent<Animator>().SetTrigger("skillRcv");
                break;

        }

        gameManager.GetComponent<GameManager>().NextUp(1);
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
        
        if (gameManager.GetComponent<GameManager>().DetermineSkillType(selectedSkill) < 2)
            gameManager.cameraAnimator.Play("EnemyToSideView");
        else
            gameManager.cameraAnimator.Play("AllyToSideView");
    }
}
