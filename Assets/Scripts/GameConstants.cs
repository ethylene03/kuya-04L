public class GameConstants {
    private string startGameMessage = "KUYA04L_GAMEHOST";
    private string newPlayerMessage = "KUYA04L_new_player";
    
    private string exceedMaxClients = "EXCEED_MAX_CLIENTS";
    private int startGamePort = 7778;
    private int newPlayerPort = 7780;
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

    public string EXCEED_MAX_CLIENTS
    {
        get => exceedMaxClients;
        set
        {
            exceedMaxClients = value;
        }
    }
}