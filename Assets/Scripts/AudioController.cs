using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource buySound;
    public AudioSource sellSound;
    public AudioSource lobbySound;

    public void PlayBuySound()
    {
        buySound.Play();
    }

    public void PlaySellSound()
    {
        sellSound.Play();
    }

    public void PlayLobbySound()
    {
        lobbySound.Play();
    }

}
