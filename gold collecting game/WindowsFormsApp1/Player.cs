using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WindowsFormsApp1
{

  

   public class Player
    {
        public static int _playerIndex = 0;

        public Player(Location coordinate, Color _color, Tuple<Gold, double> _Selected_Gold,int _MovePrice, int _SelectPrice)
        {
            _playerIndex += 1;
            Coordinate = coordinate;
            PlayerIndex = _playerIndex;
            color = _color;
            Remain_Gold = 200;
            Selected_Gold = _Selected_Gold;
            MovePrice = _MovePrice;
            SelectPrice = _SelectPrice;

        }

        public Location Coordinate { get; set; }
        public int PlayerIndex { get; set; }

        public Color color { get; set; }
        public int Remain_Gold { get; set; }
        public Tuple<Gold,double> Selected_Gold { get; set; }
        public double Selected_Distance { get; set; }
        public int MovePrice { get; set; }
        public int SelectPrice { get; set; }
    }

    
}
