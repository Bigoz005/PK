using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperWebSocket;
using Newtonsoft.Json;

namespace Server_base
{
    class Program
    {
        private static WebSocketServer wsServer;
        private static Dictionary<WebSocketSession, Player> players;
        private static GameMap gameMap = new GameMap();
        static void Main(string[] args)
        {
            wsServer = new WebSocketServer();
            int port = 8088;
            wsServer.Setup(port);
            wsServer.NewSessionConnected += WsServer_NewSessionConnected;
            wsServer.NewMessageReceived += WsServer_NewMessageReceived;
            wsServer.NewDataReceived += WsServer_NewDataReceived;
            wsServer.SessionClosed += WsServer_SessionClosed;
            wsServer.Start();
            Console.WriteLine("Server is running on port " + port + ". Press ENTER to exit....");
            players = new Dictionary<WebSocketSession, Player>();
            //Player[] players = new Player[] { }; 
            while(Console.ReadKey().Key != ConsoleKey.Enter) { };
            
        }

        private static void WsServer_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            Console.WriteLine("SessionClosed");
            players.Remove(session);
        }

        private static void WsServer_NewDataReceived(WebSocketSession session, byte[] value)
        {
            Console.WriteLine("NewDataReceived");
        }

        private static void WsServer_NewMessageReceived(WebSocketSession session, string value)
        {

            string metoda;
            int pozycja;

            pozycja = value.IndexOf('.');
            metoda = value.Substring(0, pozycja);
            Console.WriteLine("z klienta: " + value);

            if (metoda.Equals("Move"))
            {
                //String tmp = Move(value.Substring(pozycja, value.Length - pozycja), players[session]);
                string tmp = Move(value.Substring(pozycja + 1, value.Length - (pozycja + 1)),players[session]);
                

                if (players.Count < 2)
                {
                    session.Send("Position" + tmp);
                }
                else
                {
                    session.Send("Position" + tmp);
                    Player tmpPlayer = players[session];
                    foreach (KeyValuePair<WebSocketSession, Player> entry in players)
                    {
                        if (!entry.Key.Equals(session))
                        {
                            //Console.WriteLine("EnemyPlayer Move");
                            entry.Key.Send("Position;" + tmpPlayer.getName() + tmp);
                        }


                    }


                }
                Console.WriteLine("Position" + tmp);
            }
            //Console.WriteLine("NewMessageReceived: " + value);
            //if (value == "Hello server")
            //{
            //    session.Send("Hello client");
            //}
        }

        private static void WsServer_NewSessionConnected(WebSocketSession session)
        {
            Console.WriteLine("NewSessionConnected");
            
           players.Add(session, new Player("Player"+(players.Count+1)));

            Player tmp = players[session];

            Console.WriteLine("Name: " + tmp.getName());

            if (players.Count < 2)
            {
                session.Send("Name{" + tmp.getName());
                session.Send("Position" + serialize(tmp.Getposition()));
            }
            else
            {
                session.Send("Name{" + tmp.getName());
                session.Send("Position" + serialize(tmp.Getposition()));
                foreach (KeyValuePair<WebSocketSession,Player> entry in players)
                {
                    if (!entry.Key.Equals(session))
                    {
                        //Console.WriteLine("EnemyPlayer Joins");
                        entry.Key.Send("Joined{" + tmp.getName());
                        entry.Key.Send("Position;"+ tmp.getName() + serialize(tmp.Getposition()));
                    }
                        
                    
                }
               

            }
           
           //Console.WriteLine(serialize(tmp.Getposition()));
            
        }

        public static string serialize(Object obj)
        {
            //Test test = new Test { x = 300, y = 200 };
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return json;
        }


        public static string Move(string direction,Player player)
        {
            string serialized;
            Player.Position position = player.Getposition();
            Player.Statistics statistics = player.Getstatistics();
            //position = JsonConvert.DeserializeObject<Player.Position>(json);
            if (direction.Equals("Right"))
            {
                float newPosition = position.X + statistics.MovementSpeed;
                if (newPosition > gameMap.positiveX)
                {
                    position.X = gameMap.positiveX;
                }
                else
                {
                    if (gameMap.CheckCollision(newPosition, position.Y, "x") == 1)
                    {
                        position.X = newPosition;
                    }
                    else
                    {
                        position.X = position.X;
                    }
                }
            }
            if (direction.Equals("Left"))
            {
                float newPosition = position.X - statistics.MovementSpeed;
                if (newPosition < gameMap.negativeX)
                {
                    position.X = gameMap.negativeX;
                }
                else
                {
                    if (gameMap.CheckCollision(newPosition, position.Y, "x") == 1)
                    {
                        position.X = newPosition;
                    }
                    else
                    {
                        position.X = position.X;
                    }
                   
                }
            }
            if (direction.Equals("Up"))
            {
                float newPosition =  position.Y + statistics.MovementSpeed;
                if (newPosition > gameMap.positiveY)
                {
                    position.Y = gameMap.positiveY;
                }
                else
                {
                    if (gameMap.CheckCollision(position.X, newPosition, "y") == 1)
                    {
                        position.Y = newPosition;
                    }
                    else
                    {
                        position.Y = position.Y;
                    }
                }
                //position.Y = position.Y + statistics.MovementSpeed;
            }
            if (direction.Equals("Down"))
            {
                float newPosition = position.Y - statistics.MovementSpeed;
                if (newPosition < gameMap.negativeY)
                {
                    position.Y = gameMap.negativeY;
                }
                else
                {
                    if (gameMap.CheckCollision(position.X, newPosition, "y") == 1)
                    {
                        position.Y = newPosition;
                    }
                    else
                    {
                        position.Y = position.Y;
                    }
                }
            }
            serialized = JsonConvert.SerializeObject(position, Formatting.Indented);
            return serialized;
             
        }
    }
}
