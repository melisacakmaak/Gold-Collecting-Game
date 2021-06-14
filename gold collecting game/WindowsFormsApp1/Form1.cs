using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

 
namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private static int BOARD_SIZE = 15;
        private static double GOLD_RATIO = 0.2;
        private static double HIDDEN_GOLD_RATIO = 0.15;
        private static int HIDDEN_GOLD_REVEAL_COUNT = 2;

        private static int BOARD_CELL_COUNT = BOARD_SIZE * BOARD_SIZE;
        private int TOTAL_GOLD_COUNT = Convert.ToInt32(BOARD_CELL_COUNT * (GOLD_RATIO + HIDDEN_GOLD_RATIO));
        private int VISIBLE_GOLD_COUNT = Convert.ToInt32(BOARD_CELL_COUNT * (GOLD_RATIO));
        private int HIDDEN_GOLD_COUNT = Convert.ToInt32(BOARD_CELL_COUNT * (HIDDEN_GOLD_RATIO));

        private List<Player> players = new List<Player>();
        private List<Gold> golds = new List<Gold>();

        private List<Location> CoordinateList = new List<Location>();



        public Form1()
        {
            InitializeComponent();




        }

        private void DrawGame(bool state = false)
        {
            
            if(!state) {
                DrawBoard();
                players.Clear();

                players.Add(new Player(new Location(0, 0), Color.Red, null, 5, 5));
                players.Add(new Player(new Location(0, BOARD_SIZE - 1), Color.Blue, null, 5, 10));
                players.Add(new Player(new Location(BOARD_SIZE - 1, 0), Color.Purple, null, 5, 15));
                players.Add(new Player(new Location(BOARD_SIZE - 1, BOARD_SIZE - 1), Color.Green, null, 5, 20));
                CreateGolds();
                RevealGoldForPlayerC();


                RenderCells();
            }else
            {
                players[0].Selected_Gold = GetPlayerMove_A(0);
                players[1].Selected_Gold = GetPlayerMove_B(1);
                players[2].Selected_Gold = GetPlayerMove_B(2);
                players[3].Selected_Gold = GetPlayerMove_D(3);

                for (int i = 0; i < 4; i++)
                {
                    players[i].Remain_Gold -= players[i].SelectPrice;
                    AddLog("Oyuncu Hedef Seçti", players[i], -1 * players[i].SelectPrice);
                }

                ResetList();
                RenderCells();
            }
           

          

        }


        void AddLog(string message,Player _player,int goldAmount,bool isMoving = false)
        {
           


            ListViewItem _log = listView1.Items.Add(message);
            _log.UseItemStyleForSubItems = false;

         
            _log.SubItems.Add(_player.PlayerIndex.ToString() + ". oyuncu").ForeColor = _player.color;



            ListViewItem.ListViewSubItem _subitem = _log.SubItems.Add(goldAmount.ToString());
            _subitem.ForeColor = goldAmount < 0 ? Color.Gray  : Color.Goldenrod;
            _subitem.Font = new Font(listView1.Font, FontStyle.Bold);


           
            listView2.Items[0].SubItems[_player.PlayerIndex].Text = _player.Remain_Gold.ToString();
           
            if(isMoving)
            {
                listView2.Items[1].SubItems[_player.PlayerIndex].Text = (Convert.ToInt32(listView2.Items[1].SubItems[_player.PlayerIndex].Text) + 1).ToString();
            }


            

            string folderPath = Application.StartupPath + "\\";
 

            StringBuilder listViewContent = new StringBuilder();
          
                listViewContent.Append(message + " (Altın kazanma/kaybetme : " + goldAmount.ToString() + ")");
                listViewContent.Append(Environment.NewLine);

           



            string textPathForPlayer = folderPath + "oyuncu_" + _player.PlayerIndex.ToString() + ".txt";

            File.AppendAllText(textPathForPlayer, listViewContent.ToString());
     





        }
      

        public void ResetList()
        {
            CoordinateList.Clear();
            foreach (Player item in players)
            {
                CoordinateList.Add(item.Coordinate);
            }

            foreach (Gold item in golds)
            {
                CoordinateList.Add(item.Coordinate);
            }
        }
        static Random rnd = new Random();
        public Location MakeCoordinate()
        {


            return new Location()
            {
                row = rnd.Next(0, BOARD_SIZE),
                cell = rnd.Next(0, BOARD_SIZE)
            };
        }

        public Location GetFreeCoordinate()
        {
            bool canAdd = false;
            Location newCoordinate = MakeCoordinate();
            do
            {
                newCoordinate = MakeCoordinate();
                canAdd = CoordinateList.Any(x => (x.cell == newCoordinate.cell && x.row == newCoordinate.row));


            } while (canAdd);

            return newCoordinate;
        }

        public void CreateGolds()
        {

            golds.Clear();
            for (int i = 0; i < TOTAL_GOLD_COUNT; i++)
            {
                Location coord = GetFreeCoordinate();
                int _amount = rnd.Next(1, 5) * 5;
                golds.Add(new Gold(coord, i >= VISIBLE_GOLD_COUNT, _amount));
                ResetList();
            }

        }

        public void RenderCells()
        {
            foreach (Player item in players)
            {
                dataGridView1.Rows[item.Coordinate.row].Cells[item.Coordinate.cell].Style.BackColor = item.color;
                dataGridView1.Rows[item.Coordinate.row].Cells[item.Coordinate.cell].Value = item.Remain_Gold;
            }

            foreach (Gold item in golds)
            {
                dataGridView1.Rows[item.Coordinate.row].Cells[item.Coordinate.cell].Value = item.Amount;
                dataGridView1.Rows[item.Coordinate.row].Cells[item.Coordinate.cell].Style.BackColor = item.isHidden ? Color.Gray : Color.Gold;
            }

            dataGridView1.ClearSelection();
        }

        public void DrawBoard()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            int cellSize = dataGridView1.Height / BOARD_SIZE;

            for (int i = 0; i < BOARD_SIZE; i++)
            {

                DataGridViewButtonColumn _button = new DataGridViewButtonColumn();
                _button.FlatStyle = FlatStyle.Flat;
                _button.Name = "col" + i.ToString();


                int insertedColumn = dataGridView1.Columns.Add(_button);

                dataGridView1.Columns[insertedColumn].Width = cellSize;
            }
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                DataGridViewButtonCell newButton = new DataGridViewButtonCell();
                {
                    newButton.FlatStyle = FlatStyle.Flat;

                }
                int insertedColumn = dataGridView1.Rows.Add(Enumerable.Repeat("", BOARD_SIZE).ToArray());
                dataGridView1.Rows[insertedColumn].Height = cellSize;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
             DrawGame();
           
        }


        public int HamleTotal(int dist)
        {
            return (dist - (dist % 3)) / 3 + (dist % 3 > 0 ? 1 : 0);
        }

        public Tuple<Gold, double> GetPlayerMove_A(int playerIndex = 0, bool onlyHidden = false)
        {
            double distanceFilter = 999999;
            Gold selectedItem = null;
            double _dist = -1;
            foreach (Gold item in golds.FindAll(x => onlyHidden ? x.isHidden : !x.isHidden))
            {
                int t = players[playerIndex].Coordinate.row - item.Coordinate.row;
                int z = players[playerIndex].Coordinate.cell - item.Coordinate.cell;
                double hypo = Math.Abs(t) + Math.Abs(z);

                if (hypo < distanceFilter)
                {
                    distanceFilter = hypo;
                    selectedItem = item;
                    _dist = hypo;
                }
            }
            return Tuple.Create(selectedItem, _dist);
        }
        public Tuple<Gold, double> GetPlayerMove_B(int playerIndex = 1, List<Gold> removedGolds = null )
        {
            double distanceFilter = -999999;
            Gold selectedItem = null;
            double _dist = -1;

            foreach (Gold item in golds.FindAll(x => !x.isHidden &&
            (removedGolds == null ? true : !removedGolds.Any(y => x.Coordinate.cell == 
            y.Coordinate.cell && x.Coordinate.row == y.Coordinate.row)) ))
            {
                int t = players[playerIndex].Coordinate.row - item.Coordinate.row;
                int z = players[playerIndex].Coordinate.cell - item.Coordinate.cell;
                double hypo = Math.Abs(t) + Math.Abs(z);
                int total =item.Amount- (players[playerIndex].MovePrice * (int)hypo );

                if (total > distanceFilter)
                {
                    distanceFilter = total;
                    selectedItem = item;
                    _dist = hypo;
                }
            }
            return Tuple.Create(selectedItem, _dist); 
        } 
        
        public Tuple<Gold, double> GetPlayerMove_D(int playerIndex = 3)
        {
            Tuple<Gold, double> _result = null;
            List<Gold> removedList = new List<Gold>();
          
            

                for (int i = 0; i < 3; i++)
                {
                    _result = GetPlayerMove_B(3, removedList);
                    if (players[i].Selected_Gold.Item1 != null && _result.Item1 != null)
                    {
                        if (players[i].Selected_Gold.Item1.Coordinate.cell == 
                        _result.Item1.Coordinate.cell &&
                       players[i].Selected_Gold.Item1.Coordinate.row == _result.Item1.Coordinate.row)
                        {
                      

                            int _other = HamleTotal(Convert.ToInt32(players[i].Selected_Gold.Item2));
                            int _mine = HamleTotal(Convert.ToInt32(_result.Item2));

                            if (_other <= _mine)
                            {
                                removedList.Add(players[i].Selected_Gold.Item1);
                            }
                        }
                    }
                }
            _result = GetPlayerMove_B(3, removedList);


            return _result;
        }

        private int CalculatePrice(Gold _gold,Player _player)
        {


            int totalMovement = Math.Abs(_gold.Coordinate.cell - _player.Coordinate.cell) +
                Math.Abs(_gold.Coordinate.row - _player.Coordinate.row);

            int totalCost = totalMovement * _player.MovePrice + _player.SelectPrice;
            return totalCost;
        }

        public void showTargets()
        {
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Style.Font = new Font(dataGridView1.Font, FontStyle.Regular);
                    dataGridView1.Rows[i].Cells[j].Style.ForeColor = Color.Black;
                }
            }

            foreach (Player item in players)
            {
                Gold moveTo = item.Selected_Gold.Item1;
                if(moveTo != null)
                {
                    DataGridViewCell _cell = dataGridView1.Rows[moveTo.Coordinate.row].Cells[moveTo.Coordinate.cell];

                    _cell.Style.ForeColor = item.color;
                    _cell.Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                    _cell.Value = moveTo.Amount.ToString() + " " + CalculatePrice(moveTo, item).ToString();
                }

            }
        }
        private void button2_Click(object sender, EventArgs e)
        {


            showTargets();

        }

        public int nextPlayer = 0;
        public int remainingMovementCount = 3;


        private void goNextPlayer()
        {
            remainingMovementCount = 3;
            nextPlayer = nextPlayer == 3 ? 0 : nextPlayer + 1;
        }

        public void startMoving()
        {
            Player currentPlayer = players[nextPlayer];
           
            if(currentPlayer.Selected_Gold.Item1 == null)
            {

                if(players.Any(item => item.Selected_Gold.Item1 != null))
                {
                    goNextPlayer();
                }
                
                else
                {
                    MessageBox.Show("Oyun Bitti");
                    timer1.Enabled = false;
                }

              
                return;
            }

            if (currentPlayer.Coordinate.cell == currentPlayer.Selected_Gold.Item1.Coordinate.cell &&
             currentPlayer.Coordinate.row == currentPlayer.Selected_Gold.Item1.Coordinate.row)
            {

                AddLog("Oyuncu Altını Aldı", currentPlayer, currentPlayer.Selected_Gold.Item1.Amount);



                currentPlayer.Remain_Gold += currentPlayer.Selected_Gold.Item1.Amount;
                golds.Remove(currentPlayer.Selected_Gold.Item1);
                switch (nextPlayer)
                {
                    case 0:
                        currentPlayer.Selected_Gold = GetPlayerMove_A(nextPlayer);
                        break;
                    case 3:
                        currentPlayer.Selected_Gold = GetPlayerMove_D(nextPlayer);
                        break;
                    case 1:
                    case 2:
                        currentPlayer.Selected_Gold = GetPlayerMove_B(nextPlayer);
                        break;
                    default:
                        break;

                }
                if(currentPlayer.Selected_Gold.Item1 != null)
                {
                    AddLog("Oyuncu Hedef Seçti", currentPlayer, -1 * currentPlayer.SelectPrice);
                    currentPlayer.Remain_Gold -= currentPlayer.SelectPrice;
                }
    

                int p = 0;

                foreach (Player item in players)
                {
                    bool isRemoved = false;
                    try
                    {
                        if (item.Selected_Gold.Item1 != null)
                            isRemoved = golds.Any(x => x.Coordinate.cell == item.Selected_Gold.Item1.Coordinate.cell &&
                                                  x.Coordinate.row == item.Selected_Gold.Item1.Coordinate.row);
                    }
                    catch (Exception)
                    {

                        isRemoved = false;
                    }
                    if (!isRemoved && item.Remain_Gold >= item.SelectPrice)
                    {
                        switch (p)
                        {
                            case 0:
                                item.Selected_Gold = GetPlayerMove_A(p);
                                break;
                            case 3:
                                item.Selected_Gold = GetPlayerMove_D(p);
                                break;
                            case 1:
                            case 2:
                                item.Selected_Gold = GetPlayerMove_B(p);
                                break;
                            default:
                                break;

                        }


                        if (item.Selected_Gold.Item1 != null)
                        {
                            AddLog("Oyuncu Hedef Seçti", item, -1 * item.SelectPrice);
                            item.Remain_Gold -= item.SelectPrice;
                        }

                      
                    }
                    p += 1;
                }


                remainingMovementCount = 0;
                RenderCells();
                showTargets();
            }
            else
            {
                if (remainingMovementCount > 0 &&
                    currentPlayer.Remain_Gold > 0)
                {

                    #region MOVEMENT
                    if (currentPlayer.Selected_Gold.Item1.Coordinate.cell != currentPlayer.Coordinate.cell)
                    {
                        int _cell = currentPlayer.Selected_Gold.Item1.Coordinate.cell;
                        currentPlayer.Coordinate.cell += currentPlayer.Coordinate.cell < _cell ? 1 : -1;
                    }
                    else
                    {
                        int _row = currentPlayer.Selected_Gold.Item1.Coordinate.row;
                        currentPlayer.Coordinate.row += currentPlayer.Coordinate.row < _row ? 1 : -1;
                    }
                    #endregion


                    List<Gold> _goldList = golds.FindAll(x => x.Coordinate.cell == currentPlayer.Coordinate.cell &&
                                  x.Coordinate.row == currentPlayer.Coordinate.row &&
                                  x.isHidden).ToList();

                    if (_goldList.Count > 0)
                    {

                        _goldList[0].isHidden = false;
                        AddLog("Oyuncu bir gizli altın açtı", currentPlayer, 0);
                    }
                    currentPlayer.Remain_Gold -= currentPlayer.MovePrice;

                    AddLog("Oyuncu Hareket Etti", currentPlayer, -1 * currentPlayer.MovePrice, true);

                    remainingMovementCount -= 1;
                    RenderCells();
                    showTargets();
                }
                else
                {
                    goNextPlayer();

                    #region OPEN_HIDDEN_FOR_PLAYER_C
                    if (nextPlayer == 2)
                    {


                        RevealGoldForPlayerC();
                        RenderCells();
                    }
                    #endregion
                }
            }

        }

        private void RevealGoldForPlayerC()
        {

            if(players[2].Remain_Gold > 0)
            {
                for (int i = 0; i < HIDDEN_GOLD_REVEAL_COUNT; i++)
                {
                    Gold selectedGold = GetPlayerMove_A(2, true).Item1;

                    if (selectedGold != null)
                    {

                        selectedGold.isHidden = false;
                        AddLog("Oyuncu bir gizli altın açtı (Özelliği ile)", players[2], 0);
                    }
                }

                

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
            //startMoving();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
 startMoving();
        }

        private void oyunuÇizToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawGame(true);
        }

        private void başlatDurdurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            startMoving();
        }





    }
}
