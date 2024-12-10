public class GameConstants {
    private string startGameMessage = "KUYA04L_GAMEHOST";
    private string newPlayerMessage = "KUYA04L_NEW_PLAYER";
    
    private string exceedMaxClients = "KUYA04L_EXCEED_MAX_CLIENTS";
    private string gameOverLoseMessage = "KUYA04L_GAME_OVER_LOSE";
    private string gameOverWinMessage = "KUYA04L_GAME_OVER_WIN";
    private int startGamePort = 7778;
    private int newPlayerPort = 7780;
    private int gameOverPort = 7782;
    private  int maxClients = 3;

    public string START_GAME_MESSAGE
    {
        get => startGameMessage;
        set
        {
            startGameMessage = value;
        }
    }

    public string NEW_PLAYER_MESSAGE
    {
        get => newPlayerMessage;
        set
        {
            newPlayerMessage = value;
        }
    }

    public int MAX_CLIENTS
    {
        get => maxClients;
        set
        {
            maxClients = value;
        }
    }

    public int START_GAME_PORT
    {
        get => startGamePort;
        set
        {
            startGamePort = value;
        }
    }

    public int NEW_PLAYER_PORT
    {
        get => newPlayerPort;
        set
        {
            newPlayerPort = value;
        }
    }

    public int GAME_OVER_PORT {
        get => gameOverPort;
        set
        {
            gameOverPort = value;
        }
    }

    public string EXCEED_MAX_CLIENTS
    {
        get => exceedMaxClients;
        set
        {
            exceedMaxClients = value;
        }
    }


    public string GAME_OVER_WIN
    {
        get => gameOverWinMessage;
        set
        {
            gameOverWinMessage = value;
        }
    }

    public string GAME_OVER_LOSE
    {
        get => gameOverLoseMessage;
        set
        {
            gameOverLoseMessage = value;
        }
    }
}