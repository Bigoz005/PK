using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocket4Net;

public class ClientScript : MonoBehaviour {

    public static WebSocket websocket;
    public static string myName;
    public static Player playerscript;
    public static GameObject player;
    //public static GameObject enemyPlayer1;
    //public static EnemyPlayerScript enemyPlayerScript1;
    static EnemyPlayerData[] enemyPlayers;
    static Dictionary<string, EnemyPlayerData> enemyPlayerIdentity;
    public static int whoToActivate;

    //public Player
    //private Position position;
    // Use this for initialization
    void Start () {
        websocket = new WebSocket("ws://localhost:8088/");
        websocket.Opened += new EventHandler(websocket_Opened);
        websocket.Closed += new EventHandler(websocket_Closed);
        websocket.MessageReceived += Websocket_MessageReceived; 
        websocket.Open();

        myName = "unidentified";
        player = GameObject.Find("Player");
        playerscript = player.GetComponent<Player>();

        enemyPlayers = new EnemyPlayerData[4];
        enemyPlayers[0] = new EnemyPlayerData(GameObject.Find("EnemyPlayer1"));
        enemyPlayers[1] = new EnemyPlayerData(GameObject.Find("EnemyPlayer2"));
        enemyPlayerIdentity = new Dictionary<string, EnemyPlayerData>();
        whoToActivate = -1;


        //enemyPlayer1 = GameObject.Find("EnemyPlayer1");
        //enemyPlayerScript1 = enemyPlayer1.GetComponent<EnemyPlayerScript>();
        //enemyPlayer1.SetActive(false);
        //state = false;


        //position = new Position();

    }
	
	// Update is called once per frame
	void Update () {
        PlayerState();
		
	}

    class Test
    {
        public int x { get; set; }
        public int y { get; set; }

    }

    //public static void startclient()
    //{

    //    //Console.ReadKey();
    //}
    private void OnApplicationQuit()
    {
       closeclient();
    }

    public static void closeclient()
    {
        websocket.Close();
        
    }

    private static void Websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        //Console.WriteLine("NewMessageReceived: {0}", e.Message);

        string method,who;
        int dataStart;

        dataStart = e.Message.IndexOf('{');
        if(e.Message.IndexOf(";") < 0)
        {
            method = e.Message.Substring(0, dataStart);
            who = myName;
            //websocket.Send("client. w namie: " + e.Message.IndexOf(";"));
            if (method.Equals("Name"))
             {
               
                //websocket.Send("client. w namie" + e.Message.Substring(dataStart + 1, e.Message.Length - 1));
                myName = e.Message.Substring(dataStart + 1, e.Message.Length - (dataStart+1) );
                //websocket.Send("client. w namie: " + myName);
            }

            if (method.Equals("Joined"))
            {
                int c = enemyPlayerIdentity.Count;
                string name = e.Message.Substring((dataStart + 1), e.Message.Length - (dataStart + 1));
                enemyPlayerIdentity.Add(name, enemyPlayers[c]);
                whoToActivate = c;
            }
        }
        else
        {
            int nameStart = e.Message.IndexOf(";");
            method = e.Message.Substring(0, nameStart);
            who = e.Message.Substring((nameStart + 1), dataStart - (nameStart+1));
            websocket.Send("client. w joinie: " + who);

        }


        if (method.Equals("Position"))
        {
            Getposition(e.Message.Substring(dataStart, e.Message.Length - dataStart),who);
        }
       
     

    }

    private static void websocket_Opened(object sender, EventArgs e)
    {
        //websocket.Send("Hello server!");
        //websocket.Send(serialize());

    }
    private static void websocket_Closed(object sender, EventArgs e)
    {

    }

    public static string serialize()
    {
        Test test = new Test { x = 300, y = 200 };
        string json = JsonConvert.SerializeObject(test, Formatting.Indented);
        return json;
    }

    public static void Getposition(string json,String who)
    {
        //Position position = new Position();
        Position position = JsonConvert.DeserializeObject<Position>(json);
        if (who.Equals(myName))
        {
            playerscript.Getposition(position.X, position.Y);
        }
        else
        {
            enemyPlayerIdentity[who].enemyPlayerScript.Getposition(position.X, position.Y);
        }


    }


     static void PlayerState()
    {
        if(whoToActivate >= 0)
        {
            enemyPlayers[whoToActivate].enemyPlayer.SetActive(true);
            whoToActivate = -1;
        //enemyPlayerScript1 = enemyPlayer1.GetComponent<EnemyPlayerScript>();
        //Getposition(e.Message.Substring(place, e.Message.Length - place), "EnemyPlayer1");
        }
        

    }
    //public static void Setposition(string json,String who)
    //{
    //    //Position position = new Position();
    //    Position position = JsonConvert.DeserializeObject<Position>(json);

    //    if (who.Equals("Player"))
    //    {
    //        playerscript.Setposition(position.X, position.Y);
    //    }

    //    if (who.Equals("EnemyPlayer1"))
    //    {
    //        enemyPlayerScript1.Setposition(position.X, position.Y);
    //    }

    //}

    public static void Move(string direction)
    {
        websocket.Send("Move." + direction);
    }

    class Position
    {
        public  float X { get; set; }
        public  float Y { get; set; }
        
    }

    class EnemyPlayerData
    {
        public GameObject enemyPlayer;
        public EnemyPlayerScript enemyPlayerScript;

        public EnemyPlayerData(GameObject enemyPlayer)
        {
            this.enemyPlayer = enemyPlayer;
            this.enemyPlayerScript = enemyPlayer.GetComponent<EnemyPlayerScript>();
            enemyPlayer.SetActive(false);
        }
    }

}
