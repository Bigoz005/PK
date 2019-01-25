using System;
using System.Collections.Generic;

class GameMap
{
    public float positiveX;
    public float negativeX;
    public float positiveY;
    public float negativeY;
    private MapSquare pY;
    private MapSquare nY;


    public GameMap()
    {
        this.positiveX = 33F;
        this.negativeX = -31F;
        this.positiveY = 35F;
        this.negativeY = -20F;
        this.pY = new MapSquare(-12F,0F,12F,26F,null);
        this.nY = new MapSquare(-12F,-12F,12F,26F,null);
        GenerateSquares();
    }

    private void GenerateSquares()
    {
        this.pY.nodes.Add(new MapSquare(-12F, 0f, 12f, 13f, this.pY));
        this.pY.nodes.Add(new MapSquare(1F, 0f, 12f, 13f, this.pY));
        this.nY.nodes.Add(new MapSquare(-12F, -12f, 12f, 13f, this.nY));
        this.nY.nodes.Add(new MapSquare(1F, -12f, 12f, 13f, this.nY));
        //if(this.pY.nodes[0] == null)
        //{
        //    Console.WriteLine("lel");
        //}
        //Aktualnie trzeba dawac baseY o jeden tile w gore poniewaz pasuje to do kolizji w unity
        this.pY.nodes[0].AddObject("LeafyTree1", -9f, 4f, 3f, 3f);
        this.pY.nodes[0].AddObject("River", -12f, 0f, 2f, 13f);
        this.pY.nodes[1].AddObject("Rock", 11f, 6f, 2f, 2f);
        this.pY.nodes[1].AddObject("River", 2f, 0f, 2f, 12f);
        //Conifer - drzewo iglaste xD
        this.nY.nodes[0].AddObject("Conifer1", -11f, -8f, 4f, 2f);
        this.nY.nodes[0].AddObject("River", -12f, 0f, 2f, 13f);
        this.nY.nodes[1].AddObject("Conifer1", 11f, -5f, 4f, 2f);
        this.nY.nodes[1].AddObject("River", 2f, 0f, 2f, 12f);
    }

    public float CheckCollision(float X,float Y,string axis)
    {
        float rtn = 1;
        //Console.WriteLine("lel");
       
        
            MapSquare tmp;
            if (Y > 0)
            {
                tmp = this.pY;
            }
            else
            {
                tmp = this.nY;
            }
            while(tmp.nodes.Count != 0)
            {
                //Console.WriteLine("lel2");
                if(X < 1)
                {
                    tmp = tmp.nodes[0];
                    
                }
                else
                {
                    tmp = tmp.nodes[1];
                }
            }
            foreach(var cobject in tmp.objects)
            {
                if(axis == "x"  )
                {
                    if( X >= cobject.baseX && X < cobject.baseX + cobject.width)
                    {
                        if(Y >= cobject.baseY && Y < cobject.baseY + cobject.height)
                        {
                            rtn = 0;
                            break;
                        }
                    }

                }
                else
                {
                    if (Y >= cobject.baseY && Y < cobject.baseY + cobject.height)
                    {
                        if (X >= cobject.baseX && X < cobject.baseX + cobject.width)
                        {
                            rtn = 0;
                            break;
                        }
                    }
                }
            }
        return rtn;
    } 

     

    public class MapSquare
    {
        //baseX i baseY to tile najbardziej po lewo i w dol dla mapy i obiektow wiec przy ustalaniu rozmiaru
        //nalezy dodawac width do xow i height do ygrekow
        float baseX;
        float baseY;
        float height;
        float width;
        public List<MapSquare> nodes;
        public List<CollisionObject> objects;

        MapSquare parent;

        public MapSquare(float baseX, float baseY, float height, float width, MapSquare parent)
        {
            this.baseX = baseX;
            this.baseY = baseY;
            this.height = height;
            this.width = width;
            if (parent != null)
            {
                this.parent = parent;
            }
            this.nodes = new List<MapSquare>();
        }

        //public void AddChildren(float baseX, float baseY, float height, float width, MapSquare parent)
        //{
        //    this.nodes.Add(new MapSquare(baseX, baseY, height, width,parent));
        //}

        public void AddObject(string name, float baseX, float baseY, float height, float width)
        {
            List<CollisionObject> tmp = this.objects;
            if (tmp == null)
            {
                this.objects = new List<CollisionObject>();
            }
            this.objects.Add(new CollisionObject(name, baseX, baseY, height, width));
        }

        public class CollisionObject
        {
            string name;
            public float baseX;
            public float baseY;
            public float height;
            public float width;

            public CollisionObject(string name, float baseX, float baseY, float height, float width)
            {
                this.name = name;
                this.baseX = baseX;
                this.baseY = baseY;
                this.height = height;
                this.width = width;
            }
        }
    }
}