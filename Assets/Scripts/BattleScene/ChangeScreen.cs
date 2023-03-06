using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeScreen : MonoBehaviour
{
    private GameObject active;

    public GameManager gameManager;
    public Canvas mainScreen;
    public Button button;
    public Sprite inactiveImg;

    public void ShowSwitcheable()
    {
        if (active == gameManager.active)
            return;

        DeleteFaceButtons();

        active = gameManager.active;

        if (gameManager.active == gameManager.playerTeam.GetComponent<Team>().player)
            PlayerChangeScreen();
        else
            DemonChangeScreen(gameManager.active);
    }

    public void PlayerChangeScreen()
    {
        int offset = 0;
        var team = gameManager.GetComponent<GameManager>().playerTeam;

        for (int i = 0; i < 3; ++i)
        {
            var faceImage = Instantiate(button, new Vector2(360f + 300f * ((offset % 4) + 1), 660f), Quaternion.identity);

            // Defeated demons will show as blank
            if (team.GetComponent<Team>().activeDemons[i] != null && team.GetComponent<Team>().activeDemons[i].GetComponent<ActorStats>().stats.battleStats.hp > 0)
                faceImage.GetComponent<Image>().sprite = team.GetComponent<Team>().activeDemons[i].GetComponent<ActorStats>().faceSprite;
            else
                faceImage.GetComponent<Image>().sprite = inactiveImg;
            faceImage.transform.SetParent(transform.Find("FaceImages").transform);

            GameObject demon = team.GetComponent<Team>().activeDemons[i];
            faceImage.onClick.AddListener(delegate
            {
                DeleteFaceButtons();
                DemonChangeScreen(demon);
                transform.Find("Back").gameObject.SetActive(false);
                transform.Find("DoubleBack").gameObject.SetActive(true);
            });
            ++offset;
        }
    }

    private void DemonChangeScreen(GameObject selected)
    {
        int offset = 0;
        var team = gameManager.GetComponent<GameManager>().playerTeam;
        foreach (Transform child in team.transform)
        {
            if (child.gameObject == selected || child == team.GetComponent<Team>().player.transform
                || child.GetComponent<ActorStats>().stats.battleStats.hp == 0)
                continue;

            var faceImage = Instantiate(button, new Vector2(210f + 300f * ((offset % 4) + 1), 660f - 200f * (offset / 4)), Quaternion.identity);
            faceImage.GetComponent<Image>().sprite = child.GetComponent<ActorStats>().faceSprite;
            faceImage.transform.SetParent(transform.Find("FaceImages").transform);
            faceImage.onClick.AddListener(delegate
            {
                gameManager.GetComponent<GameManager>().ChangeDemons(selected, child.gameObject);
                DisableChangeScreen();
            });
            ++offset;
        }

        active = gameManager.active;
    }

    public void DeleteFaceButtons()
    {
        foreach (Transform child in transform.Find("FaceImages").transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void DisableChangeScreen()
    {
        transform.GetComponent<Canvas>().enabled = false;
    }
}
