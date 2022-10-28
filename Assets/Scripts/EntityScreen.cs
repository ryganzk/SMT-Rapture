using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class EntityScreen : MonoBehaviour
{
    private int skillNum;

    public GameManager gameManager;
    public Button selectButton;
    public Button backButton;
    public Canvas mainBattleScreen;
    public Canvas skillScreen;

    [SerializeReference]
    public NonPassiveSkill selectedSkill;
    
    public void UpdateSelectButtons()
    {
        skillNum = 0;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Team team;
        // Opposing team selectors if skill is attack/ailment
        if (gameManager.GetComponent<GameManager>().DetermineSkillType(selectedSkill) < 2)
            team = gameManager.opponentTeam.GetComponent<Team>();
        // Opposing team selectors if skill is support and directed towards enemies
        else if (gameManager.GetComponent<GameManager>().DetermineSkillType(selectedSkill) == 3)
        {
            skillNum = 3;
            SupportSkill selectedSkillSupp = (SupportSkill) selectedSkill;

            // Player team selectors if skill buffs, charges, or has veil effect
            if (selectedSkillSupp.buff || selectedSkillSupp.veil || selectedSkillSupp.chargeID.Count > 0)
                team = gameManager.playerTeam.GetComponent<Team>();
            // Opposing team selectors if the above isn't true
            else
                team = gameManager.opponentTeam.GetComponent<Team>();
        }
        // Player team selectors if otherwise
        else
            team = gameManager.playerTeam.GetComponent<Team>();

        // Create one selector for self if skill is self-only
        if (skillNum == 3 && ((SupportSkill) selectedSkill).selfOnly)
        {
            CreateSelectButton(gameManager.active);
        }
        else
        {
            CreateSelectButton(team.player);

            // Create selectors for all demons if you can only single target
            if (selectedSkill.targets == 0)
            {
                foreach (GameObject obj in team.activeDemons)
                {
                    if (obj != null)
                        CreateSelectButton(obj);
                }
            }
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
        //gameManager.cameraAnimator.Play("BackView");

        List<GameObject> team = gameManager.opponentTeam.GetComponent<Team>().activeDemons;

        switch (gameManager.DetermineSkillType(GetComponent<EntityScreen>().selectedSkill))
        {

            case 0:
            case 1:
                gameManager.active.GetComponent<Animator>().SetTrigger("skillAtk");
                switch (selectedSkill.targets)
                {
                    case 1:
                        gameManager.GetComponent<GameManager>().PartyDamage((AttackSkill) selectedSkill, team);
                        break;
                    case 2:
                        gameManager.GetComponent<GameManager>().RandDamage((AttackSkill) selectedSkill, team);
                        break;
                    default:
                        gameManager.GetComponent<GameManager>().Damage((AttackSkill) selectedSkill, obj);
                        break;
                }
                break;
            case 2:
            case 3:
                gameManager.active.GetComponent<Animator>().SetTrigger("skillRcv");
                switch (selectedSkill.targets)
                {
                    case 1:
                        gameManager.GetComponent<GameManager>().PartySupport((SupportSkill) selectedSkill, obj.transform.GetComponentInParent<Team>().activeDemons);
                        break;
                    default:
                        gameManager.GetComponent<GameManager>().Support((SupportSkill) selectedSkill, obj);
                        break;
                }
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
        
        //if (gameManager.GetComponent<GameManager>().DetermineSkillType(selectedSkill) < 2)
        //    gameManager.cameraAnimator.Play("EnemyToSideView");
        //else
        //    gameManager.cameraAnimator.Play("AllyToSideView");
    }
}
