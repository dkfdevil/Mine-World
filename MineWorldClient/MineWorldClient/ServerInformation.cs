namespace MineWorld
{
    public class ServerInformation
    {
        public string Servername;
        public string Ipaddress;
        public int Playercount;
        public int Maxplayercount;
        public bool Lan;

        public ServerInformation(string name,string ip,int playerc, int playermax,bool la)
        {
            Servername = name;
            Ipaddress = ip;
            Playercount = playerc;
            Maxplayercount = playermax;
            Lan = la;
        }

        public string GetTag()
        {
            return Servername + " " + Playercount + "/" + Maxplayercount;
        }
    }
}
