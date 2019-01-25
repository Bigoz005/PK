class Player
{
    string name;
    Position position;
    Statistics statistics;
    public Player(string name)
    {
        this.name = name;
        this.position = new Position();
        this.statistics = new Statistics();
        position.X = 0f;
        position.Y = 0f;
        statistics.MovementSpeed = 0.2F;
    }

    public Position Getposition()
    {
        return this.position;
    }
    public Statistics Getstatistics()
    {
        return this.statistics;
    }

    public string getName()
    {
        return this.name;
    }
    public class Position
    {

        public float X { get; set; }
        public float Y { get; set; }
    }

    public class Statistics
    {
       
        public float MovementSpeed { get; set; }

        //public Statistics()
        //{
        //    this.MovementSpeed = 0.05F;
        //}


    }
}