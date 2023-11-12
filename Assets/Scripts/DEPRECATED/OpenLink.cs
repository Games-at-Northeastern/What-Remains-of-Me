using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenLink : MonoBehaviour
{
    private enum socialMedia { Discord, Itch, Website, Twitter, Instagram };

    [SerializeField] private socialMedia app;

    private string url;

    [SerializeField] private GameObject hover;
    private void OnMouseOver()
    {
        hover.SetActive(true);
    }
    private void OnMouseExit()
    {
        hover.SetActive(false);
    }

    private void OnMouseDown()
    {
        OpenURL();
    }

    public void OpenURL()
    {
        switch (app)
        {
            case socialMedia.Discord:
                url = "https://discord.gg/fBBBanPCHr";
                break;
            case socialMedia.Itch:
                url = "https://whatremainsofme.itch.io";
                break;
            case socialMedia.Website:
                url = "https://games.northeastern.edu/studio/";
                break;
            case socialMedia.Twitter:
                url = "https://twitter.com/remains_game";
                break;
            case socialMedia.Instagram:
                url = "https://instagram.com/whatremainsofme_game/";
                break;
        }
        Debug.Log(url);
        Application.OpenURL(url);
    }

}
