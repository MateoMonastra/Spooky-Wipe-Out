using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class akGameStateMenu : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        AkSoundEngine.SetState("GameState", "Menu");
    }
    public void setGameStateMenu ()
    {
        AkSoundEngine.SetState("GameState", "Menu");
    }

    public void setGameStateCredits()
    {
        AkSoundEngine.SetState("GameState", "Credits");
    }
}
