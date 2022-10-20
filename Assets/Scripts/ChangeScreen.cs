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

    public void ShowSwitcheable()
    {
        if (active == gameManager.active)
            return;

        foreach (Transform child in transform.Find("FaceImages").transform)
        {
            Destroy(child.gameObject);
        }

        int offset = 0;
        var team = gameManager.GetComponent<GameManager>().playerTeam;
        foreach (Transform child in team.transform)
        {
            if (team.GetComponent<Team>().activeDemons.Contains(child.gameObject)
                || child == team.GetComponent<Team>().player.transform)
                continue;

            var faceImage = Instantiate(button, new Vector2(210f + 300f * ((offset % 4) + 1), 660f - 200f * (offset / 4)), Quaternion.identity);
            faceImage.GetComponent<Image>().sprite = child.GetComponent<ActorStats>().faceSprite;
            faceImage.transform.SetParent(transform.Find("FaceImages").transform);      
            faceImage.onClick.AddListener(delegate { gameManager.GetComponent<GameManager>().ChangeDemons(child.gameObject); });
            faceImage.onClick.AddListener(DisableChangeScreen);
            ++offset;
        }

        active = gameManager.active;
    }

    private void DisableChangeScreen()
    {
        transform.GetComponent<Canvas>().enabled = false;
        mainScreen.enabled = true;
    }
}
