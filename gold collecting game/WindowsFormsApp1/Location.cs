using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 
namespace WindowsFormsApp1
{
    public class Location
    {
        public Location()
        {
        }

        public Location(int row, int cell)
        {
            this.row = row;
            this.cell = cell;
        }

        public int row { get; set; }
        public int cell { get; set; }

    }
}
