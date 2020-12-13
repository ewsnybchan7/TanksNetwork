using UnityEngine;

public class TANKSGame : MonoBehaviour
{
    public const float PLAYER_RESPAWN_TIME = 3.0f;

    public const int PLAYER_MAX_LIVES = 3;
    public const int PLAYER_HP = 100;

    public const string PLAYER_LIVES = "PlayerLives";
    public const string PLAYER_READY = "isPlayerReady";
    public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";


    public static Color GetColor(int colorChoice)
    {
        switch (colorChoice)
        {
            case 0: return Color.red;
            case 1: return Color.green;
            case 2: return Color.blue;
            case 3: return Color.yellow;
            case 4: return Color.cyan;
            case 5: return Color.grey;
            case 6: return Color.magenta;
            case 7: return Color.white;
        }

        return Color.black;
    }
}
