using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Omok
{
    public partial class Form1 : Form
    {
        System.Drawing.Graphics g;
        protected List<Point> list;
        protected Point[,] point = new Point[19, 19];
        protected char[,] map = new char[19,19]; //map
        protected char turn;   //check turn '1' or '2'
        protected int setx;    //dim x
        protected int sety;    //dim y
        protected bool run;    //run test
        protected bool game_over; //finish check
        protected bool option; //with computer or alone
        int all_count;
        List<Point> same;      //for calc

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string message = "Do you want to game with computer?\n(Yes : computer, No : alone)";
            string caption = "kyo";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = System.Windows.Forms.DialogResult.No;
            result = MessageBox.Show(message, caption, buttons);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                option = true;
            }
            else
            {
                option = false;
            }
            g = this.CreateGraphics();
            list = new List<Point>();
            turn = '1'; //player 1 & 2
            run = false;
            game_over = false;
            all_count = 0;
            for (int i = 0; i < 19; i++)
                for (int j = 0; j < 19; j++)
                {
                    point[i, j] = new Point(i, j);
                    map[i, j] = '0';
                }
            this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(Form1_Paint);
        }
        private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.BurlyWood, new Rectangle(0, 24, 655, 679)); //total size
            //e.Graphics.FillRectangle(Brushes.White, new Rectangle(655, 0, 800, 655)); //empty size
            //print omok broud( 10~ 630 )
            for(int i=0; i<19; i++)
            {
                e.Graphics.DrawLine(Pens.Black, new Point(10 + i * 35, 34), new Point(10 + i * 35, 664));
                e.Graphics.DrawLine(Pens.Black, new Point(10,34 + i * 35), new Point(640,34 + i * 35));
            }
            for (int i=0; i<3; i++)
            {
                for(int j=0; j<3; j++)
                {
                    e.Graphics.FillEllipse(Brushes.Black, new Rectangle(115 - 4+i*210, 135+j*210, 8, 8));
                }
            }
            
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            if (run) return;
            run = true;
            //read x, y horizon
            int x = Int32.Parse(MousePosition.X.ToString()) - Int32.Parse(this.Location.X.ToString());
            int y = Int32.Parse(MousePosition.Y.ToString()) - Int32.Parse(this.Location.Y.ToString());

            //check in (if out then ignore)
            if (x > 655 || y > 699) return;

            //check position is available
            setx = (x - 1) / 35;
            sety = (y -48) / 35;
            if (map[setx, sety] == '0')
            {
                if (turn == '1')
                {
                    map[setx, sety] = '1';
                    turn = '2';
                    g.FillEllipse(Brushes.Black, new Rectangle(10 + setx * 35 - 16, 35 + sety * 35 - 16, 32, 32));
                    finish_check();

                    //do it except list
                    try
                    {
                        list.Remove(point[setx, sety]);
                    }catch(Exception ex) { }
                    try
                    {
                        for (int i = 0; i < 5; i++)
                            for (int j = 0; j < 5; j++)
                                if ((setx - 2 + i < 19) && (setx - 2 + i >= 0) && (sety - 2 + j >= 0) && (sety - 2 + j < 19) && (map[setx - 2 + i, sety - 2 + j] == '0') && (!list.Contains(point[setx - 2 + i, sety - 2 + j]))) list.Add(point[setx - 2 + i, sety - 2 + j]);
                    }
                    catch(Exception ex) { }
                    if (option)
                    {
                        run = false;
                        do_it();  //alphago
                        finish_check();
                    }
                }
                else if (turn == '2')
                {
                    map[setx, sety] = '2';
                    turn = '1';
                    g.FillEllipse(Brushes.White, new Rectangle(10 + setx * 35 - 16, 35 + sety * 35 -16, 32, 32));
                    finish_check();
                }

            }

            run = false;
        }
        private void finish_check()
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int x_order = 0;
            int y_order = 0;
            char old_turn = '0';
            if (turn == '1') old_turn = '2';
            if (turn == '2') old_turn = '1';

            if (all_count >= 361)
            {
                turn = '0';
                finish();
            }

            int count = 1;
            for (k = 0; k < 4; k++) {
                //left up / up / right up / right
                if (k == 0)
                {
                    x_order = 1;
                    y_order = 1;
                }
                else if (k == 1)
                {
                    x_order = 0;
                    y_order = 1;
                }
                else if (k == 2)
                {
                    x_order = 1;
                    y_order = -1;
                }
                else if (k == 3)
                {
                    x_order = 1;
                    y_order = 0;
                }
                for (j = 0; j < 2; j++)
                {
                    if (j == 1)
                    {
                        x_order = -x_order;
                        y_order = -y_order;
                    }
                    //left up test
                    for (i = 1; i < 6; i++)
                    {
                        if ((setx + i * x_order < 0) || (sety + i * y_order < 0)|| (setx + i * x_order >18) || (sety + i * y_order >18)) break;
                        if (map[setx + i*x_order, sety + i*y_order] == old_turn) count++;
                        else break;
                    }
                }
                if (count == 5) finish();
                count = 1;
            }
            all_count++;
        }
        private void finish()
        {
            game_over = true;
            this.Click -= new System.EventHandler(this.Form1_Click);
            string message;
            if (turn == '2') message = "You win\n";
            else if (turn == '1') message = "Computer win\n";
            else message = "draw\n";
            message += "Do you want to game again";
            string caption = "kyo";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = System.Windows.Forms.DialogResult.No;
            result = MessageBox.Show(message, caption, buttons);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                restart();
            }
            else
            {
                this.Close();
            }
        }

        private void restart()
        {
            game_over = false;
            turn = '1'; //player 1 & 2
            run = false;
            all_count = 0;
            list.Clear();
            for (int i = 0; i < 19; i++)
                for (int j = 0; j < 19; j++)
                {
                    map[i, j] = '0';
                }

            this.DoubleBuffered = true;
            g.FillRectangle(Brushes.BurlyWood, new Rectangle(0, 24, 655, 679)); //total size
            //g.FillRectangle(Brushes.White, new Rectangle(655, 0, 800, 655)); //empty size
           
            for (int i = 0; i < 19; i++)
            {
                g.DrawLine(Pens.Black, new Point(10 + i * 35, 34), new Point(10 + i * 35, 674));
                g.DrawLine(Pens.Black, new Point(10, 34 + i * 35), new Point(640, 34 + i * 35));
            }
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    g.FillEllipse(Brushes.Black, new Rectangle(115 - 4 + i * 210, 135 + j * 210, 8, 8));
                }
            }
            string message = "Do you want to game with computer?\nYes : computer, No : alone";
            string caption = "kyo";
            if (!option)
            {
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = System.Windows.Forms.DialogResult.No;
                result = MessageBox.Show(message, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    option = true;
                }
                else
                {
                    option = false;
                }
            }
            this.Click += new System.EventHandler(this.Form1_Click);
        }
        private void do_it()
        {
            if (game_over) return;
            if (run) return;
            run = true;
            turn = '1';
            Point result = do_search();
            while(map[result.X, result.Y] != '0')
            {
                list.Remove(point[result.X, result.Y]);
                result = do_search();
            }
            setx = result.X;
            sety = result.Y;
            map[setx, sety] = '2';
            g.FillEllipse(Brushes.White, new Rectangle(10 + setx * 35 - 16, 35 + sety * 35 - 16, 32, 32));
            //do it except list
            try
            {
                list.Remove(point[setx, sety]);
            }
            catch (Exception ex) { }
            try
            {
                for (int i = 0; i < 5; i++)
                    for (int j = 0; j < 5; j++)
                        if ((setx - 2 + i < 19) && (setx - 2 + i >= 0) && (sety - 2 + j >= 0) && (sety - 2 + j < 19) && map[setx - 2 + i, sety - 2 + j] == '0' && (!list.Contains(point[setx - 2 + i, sety - 2 + j]))) list.Add(point[setx - 2 + i, sety - 2 + j]);
            }catch(Exception ex) { }

        }
        private Point do_search()
        {
            int max_gravity=1;
            int tm1 = 0;
            int tm2 = 0;
            int tm = 0;
            List<int> tmp1 = new List<int>();
            List<Point> tmpp1 = new List<Point>();
            List<int> tmp2 = new List<int>();
            List<Point> tmpp2 = new List<Point>();
            Point best_position;
            if (list.Count == 0) return point[9, 9];
            best_position = list[0]; 
            same= new List<Point>();
            same.Add(best_position);

            foreach(Point x in list)
            {
                if (best_position.IsEmpty) best_position = point[9, 9];
                //algorithm
                tm1 = gravity(x, '1');
                tm2 = gravity(x, '2');
                if (tm2 >= 0x70000000) return x;
                tmp1.Add(tm1);
                tmpp1.Add(x);
                tmp2.Add(tm2);
                tmpp2.Add(x);
                tm = (int)(tm1*0.75) + tm2;
                //if (tm1 < tm2) tmp1 = tmp2;
                if (tm > max_gravity)
                {
                    max_gravity = tm;
                    best_position = x;
                    same.Clear();
                    same.Add(x);
                }
                else if(tm1 == max_gravity)
                {
                    same.Add(x);
                }
            }
            int c;
            for (c = 0; c < tmp1.Count; c++) if (tmp1[c] >= 0x70000000) return tmpp1[c];
            for (c = 0; c < tmp2.Count; c++) if (tmp2[c] >= 0x60000000) return tmpp2[c];
            for (c = 0; c < tmp1.Count; c++) if (tmp1[c] >= 0x60000000) return tmpp1[c];
            for (c = 0; c < tmp2.Count; c++) if (tmp2[c] >= 0x50000000) return tmpp2[c];
            for (c = 0; c < tmp1.Count; c++) if (tmp1[c] >= 0x50000000) return tmpp1[c];
            //for (c = 0; c < tmp2.Count; c++) if (tmp2[c] >= 0x400000) return tmpp2[c];
            //for (c = 0; c < tmp1.Count; c++) if (tmp1[c] >= 0x400000) return tmpp1[c];


            int n = same.Count;
            DateTime now;
            now = DateTime.Now;
            Random rand = new Random(now.Millisecond);
            int num=rand.Next(n);
            return same[num];
        }
        private int gravity(Point x, char position_turn)
        {
            int important = 1;
            int tmp_important = 1;
            int x_val = x.X;
            int y_val = x.Y;
            int block_count = 0;
            int empty_count = 0;
            int tmp_empty_count = 0;
            int total_count = 1;
            int i = 0;
            int j = 0;
            int k = 0;
            int count = 1;
            int x_order = 0;
            int y_order = 0;
            int air1 = 0;
            int air2 = 0;

            int finish = 0;

            int continue_count = 1;
            int near_char =0;
            bool continue_sig = true;
            bool continue_sig2 = true;

            char tmp;

            //left up test
            for (k = 0; k < 4; k++)
            {
                block_count = empty_count = tmp_empty_count =air1 = air2 = near_char = 0;
                total_count =  count =  tmp_important = continue_count = 1;
                continue_sig =  continue_sig2 = true;

                //left up / up / right up / right
                if (k == 0)
                {
                    x_order = 1;
                    y_order = 1;
                }
                else if (k == 1)
                {
                    x_order = 0;
                    y_order = 1;
                }
                else if (k == 2)
                {
                    x_order = 1;
                    y_order = -1;
                }
                else if (k == 3)
                {
                    x_order = 1;
                    y_order = 0;
                }

                for (j = 0; j < 2; j++)
                {
                    if (j == 1)
                    {
                        air1 = air2;
                        air2 = tmp_empty_count = 0;
                        x_order = -x_order;
                        y_order = -y_order;
                        near_char *= 0x100;
                        continue_sig = continue_sig2 = true;
                    }
                    for (i = 1; i < 6; i++)
                    {
                        if ((x_val + i*x_order < 0) || (y_val + i*y_order < 0)|| (x_val + i * x_order >18) || (y_val + i * y_order >18)) break;
                        tmp = map[x_val + i*x_order, y_val + i*y_order];
                        if (tmp == position_turn)
                        {
                            air2 = 0;
                            empty_count += tmp_empty_count;
                            count++;
                            if (continue_sig) continue_count++;
                            else if (!continue_sig && continue_sig2)
                            {
                                near_char += 0x20;
                                continue_sig2 = false;
                            }
                        }
                        else if (tmp == '0')
                        {
                            tmp_empty_count++;
                            air2++;
                            if (continue_sig)
                            {
                                continue_sig = false;
                            }
                            else if (!continue_sig && continue_sig2)
                            {
                                continue_sig2 = false;
                            }
                        }
                        else
                        {
                            block_count++;
                            if (continue_sig)
                            {
                                near_char = 1;
                                continue_sig = false;
                            }
                            else if(!continue_sig && continue_sig2)
                            {
                                near_char += 0x10;
                                continue_sig2 = false;
                            }
                            break;
                        }
                        total_count++;
                    }
                }
                //calculate 
                if (total_count < 5)
                {
                    tmp_important = 0x10;
                }
                else
                {
                    tmp_important = calc(count, block_count, empty_count, air1, air2);

                    if (count > 5)
                    {
                        if (continue_count == 5) finish += 0x100;
                        else if (continue_count == 4)
                        {
                            if (((near_char & 0x101) == 0) || (((near_char & 0x1) == 1) && ((near_char & 0x100) == 0) && ((near_char & 0x2000) != 1)) || (((near_char & 0x100) == 1) && ((near_char & 0x1) == 0) && ((near_char & 0x20) != 1))) tmp_important += 0x2000000 / 3;
                        }
                        else if (continue_count == 3)
                        {
                            if (near_char == 0) tmp_important += 0x2000000;
                        }
                    }

                    if (tmp_important >= 0x10000000) finish+=0x100;
                    else if (tmp_important >= 0x2000000) finish+=0x10;
                    else if (tmp_important >= (int)(0x200000*0.9)) finish+=0x1;
                }
                if (important < tmp_important) important = tmp_important;
            }
            //total calculate
            if (finish >= 0x100) important = 0x70000000;
            else if (finish >= 0x10) important = 0x60000000;
            else if (finish >= 0x2) important = 0x50000000;
            return important;
        }
        private int calc(int count, int block_count, int empty_count, int air1, int air2)
        {
            int tmp_important = 0;
            int max = 0x10000000;

            if (count == 5)
            {
                if (empty_count == 0) tmp_important = max;
            }
            else if (count == 4)
            {
                max = 0x2000000;
                if (block_count == 0)
                {
                    if (empty_count == 0) tmp_important = max;
                    else if (empty_count == 1) tmp_important = max/2;
                }
                else if (block_count == 1)
                {
                    if((air1>0) &&(air2>0))
                    {
                        if (empty_count == 0) tmp_important = max;
                        else if (empty_count == 1) tmp_important = max/2;
                    }
                    else
                    {
                        if (empty_count == 0) tmp_important = max / 3;
                        else if (empty_count == 1) tmp_important = max/4;
                    }
                }
                else
                {
                    if ((air1>0)&&(air2>0))
                    {
                        if (empty_count == 0) tmp_important = max;
                        else if (empty_count == 1) tmp_important = max/2;
                    }
                    else if((air1>0)||(air2>0))
                    {
                        if (empty_count == 0) tmp_important = max/3;
                        else if (empty_count == 1) tmp_important = max/4;
                    }
                }

            }
            else if (count == 3)
            {
                max = 0x200000;
                if (block_count == 0)
                {
                    if (empty_count == 0) tmp_important = max;
                    else if (empty_count == 1) tmp_important = (int)(max*0.9);
                }
                else if (block_count == 1)
                {
                    if (((air1 > 0) && (air2 > 1))|| ((air1 > 1) && (air2 > 0)))
                    {
                        if (empty_count == 0) tmp_important = max;
                        else if (empty_count == 1) tmp_important = (int)(max * 0.9);
                    }
                    else if ((air1>0)&&(air2>0))
                    {
                        if (empty_count == 0) tmp_important = (int)(max * 0.8);
                        else if (empty_count == 1) tmp_important = (int)(max * 0.7);
                        else if (empty_count == 2) tmp_important = max / 20;
                    }
                    else if ((air1>0)||(air2>0))
                    {
                        if (empty_count == 0) tmp_important = max/8;
                        else if (empty_count == 1) tmp_important = max/16;
                        else if (empty_count == 2) tmp_important = max/32;
                    }
                }
                else
                {
                    if((air1>0)&&(air2>0))
                    {
                        if (empty_count == 0) tmp_important = (int)(max * 0.6);
                        else if (empty_count == 1) tmp_important = max / 2;
                        else if (empty_count == 2) tmp_important = max / 40;
                    }
                    else if ((air1>0)||(air2>0))
                    {
                        if (empty_count == 0) tmp_important = max / 10;
                        else if (empty_count == 1) tmp_important = max / 20;
                        else if (empty_count == 2) tmp_important = max / 50;
                    }
                }
            }
            else if (count == 2)
            {
                max = 0x2000;
                if (block_count == 0)
                {
                    if (empty_count == 0) tmp_important = max;
                    else if (empty_count == 1) tmp_important = max/2;
                    else if (empty_count == 2) tmp_important = max/3;
                    else if (empty_count == 3) tmp_important = max/8;
                }
                else if (block_count == 1)
                {
                    if ((air1>0)&&(air2>0))
                    {
                        if (empty_count == 0) tmp_important = (int)(max*0.9);
                        else if (empty_count == 1) tmp_important = (int)(max*0.4);
                        else if (empty_count == 2) tmp_important = max/4;
                        else if (empty_count == 3) tmp_important = max/10;
                    }
                    else if ((air1>0)||(air2>0))
                    {
                        if (empty_count == 0) tmp_important = (int)(max*0.8);
                        else if (empty_count == 1) tmp_important = (int)(max*0.35);
                        else if (empty_count == 2) tmp_important = max/16;
                    }
                }
                else
                {
                    if ((air1 > 0) && (air2 > 0)) tmp_important = max/20;
                    else if ((air1 > 0) || (air2 > 0)) tmp_important = max / 30;
                }
            }
            else if (count == 1)
            {
                max = 0x200;
                if (block_count == 0)
                {
                    if ((air1 > 0) && (air2 > 0)) tmp_important = max / 10;
                    else if ((air1 > 0) || (air2 > 0)) tmp_important = max / 16;
                }
                else if (block_count == 1)
                {
                    if ((air1 > 0) && (air2 > 0)) tmp_important = max / 20;
                    else if ((air1 > 0) || (air2 > 0)) tmp_important = max / 30;
                }
            }
            return tmp_important;
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restart();
        }

        private void qutiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void kyoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "since 2016.08.20";
            string caption = "kyo";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, caption, buttons);
        }
    }
}
