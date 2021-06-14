using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 
namespace WindowsFormsApp1
{
   public class Gold
    {
        public Gold(Location coordinate,bool isHidden,int amount)
        {
            this.Coordinate = coordinate;
            this.isHidden = isHidden;
            this.Amount = amount;
         
        }
        
        public bool isHidden { get; set; }
        
        public Location Coordinate { get; set; }
     
        public int Amount { get; set; }
    }
}
