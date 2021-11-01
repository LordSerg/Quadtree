using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuadTree
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        bool serach_algo = true;//true - quadtree algorythm
                                //false - linear algorythm
        Bitmap b;
        QuadTree q;
        List<P> pOiNtS;
        Graphics g;
        int n = 1000;//num of points
        bool is_serching;
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (!is_serching)
            {
                n++;
                toolTip1.SetToolTip(pictureBox1, n.ToString());
                pOiNtS.Add(new P(Cursor.Position.X, Cursor.Position.Y));
                q.insert(pOiNtS[n - 1]);
                pictureBox1.Image = b;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            b = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(b);
            Random r = new Random();

            pOiNtS = new List<P>();
            for (int i = 0; i < n; i++)
            {
                pOiNtS.Add(new P(r.Next(pictureBox1.Width), r.Next(pictureBox1.Height)));
                //g.FillEllipse(Brushes.Red, pOiNtS[i].X - 5, pOiNtS[i].Y - 5, 10, 10);
            }
            q = new QuadTree(new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height), g);
            for (int i = 0; i < n; i++)
                q.insert(pOiNtS[i]);
                //q.insert(new P(r.Next(pictureBox1.Width), r.Next(pictureBox1.Height)));
            pictureBox1.Image = b;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {//поиск точек, принадлижащих прямоугольнику
            if (is_serching)
            {//искать
                Rectangle search = new Rectangle(Cursor.Position.X - 150, Cursor.Position.Y - 150, 300, 300);
                g.Clear(Color.White);
                if (serach_algo)
                {//Qtree algorythm
                    List<P> a = q.queryRange(search);
                    foreach (P p in a)
                    {
                        g.FillEllipse(Brushes.Red, p.X - 5, p.Y - 5, 10, 10);
                    }
                }
                else
                {//brute force algorythm
                    for (int i = 0; i < n; i++)
                    {
                        if (search.containsPoint(pOiNtS[i]))
                            g.FillEllipse(Brushes.Red, pOiNtS[i].X - 5, pOiNtS[i].Y - 5, 10, 10);
                    }
                }
                g.DrawRectangle(Pens.Red, search.x, search.y, search.w, search.h);
                pictureBox1.Image = b;
            }
        }
        private void searchPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (searchPointsToolStripMenuItem.Text == "Search points")
                searchPointsToolStripMenuItem.Text = "Put points";
            else
                searchPointsToolStripMenuItem.Text = "Search points";
            is_serching = !is_serching;
            g.Clear(Color.White);
            for (int i = 0; i < n; i++)
            {
                g.FillEllipse(Brushes.Red, pOiNtS[i].X - 5, pOiNtS[i].Y - 5, 10, 10);
            }
            pictureBox1.Image = b;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            serach_algo = radioButton1.Checked;
        }
    }
    class P//point
    {
        public float X, Y;
        public P(float x,float y)
        {
            X = x;
            Y = y;
        }
    }

    struct Rectangle
    {
        public float w, h, x, y;
        public Rectangle(float X, float Y, float width, float height)
        {
            w = width;
            h = height;
            x = X;
            y = Y;
        }
        public bool containsPoint(P pnt)
        {
            if (pnt.X <= x + w && pnt.X >= x && pnt.Y <= y + h && pnt.Y >= y)
                return true;
            else
                return false;
        }
        public bool intersectsRectangle(Rectangle other)
        {//пересекает ли данный прямоугольник другой
            return (x - other.x < other.w || other.x - x < w) && (y - other.y < other.h || other.y - y < h);
        }
    }
    class QuadTree
    {
        const int num_of_points = 3;//допустимое количество точек в одном квадранте
        Rectangle boundary;
        public Graphics gr;
        List <P> points = new List<P>();//маассив точек, которые способен хранить 1 квадрант
        //но так как у нас всего одна точка, то
        //P point;
        //bool has_point = false;
        //int w, h, x, y;
        QuadTree northWest;
        QuadTree northEast;
        QuadTree southWest;
        QuadTree southEast;
        public QuadTree(Rectangle bndr,Graphics g)
        {
            boundary = bndr;
            gr = g;
            points = new List<P>();
        }
        public bool insert(P pnt)
        {
            if (!boundary.containsPoint(pnt))
                return false;

            if (points.Count < num_of_points)
            {
                gr.FillEllipse(Brushes.Red, pnt.X - 5, pnt.Y - 5, 10, 10);
                points.Add(pnt);
                return true;
            }

            if (northWest == null)
                subdivide();//передаем точки в дочерние квадранты
            for (int i=0;i< points.Count;i++)
            {
                if (points[i] != null)
                {
                    if (!northWest.insert(points[i]))
                    {
                        if (!northEast.insert(points[i]))
                        {
                            if (!southWest.insert(points[i]))
                            {
                                if (!southEast.insert(points[i])) { }
                                else
                                    points[i] = null;
                            }
                            else points[i] = null;
                        }
                        else points[i] = null;
                    }
                    else points[i] = null;
                }
            }
            //вставляем новую точку
            if (northWest.insert(pnt))
                return true;
            if (northEast.insert(pnt))
                return true;
            if (southWest.insert(pnt))
                return true;
            if (southEast.insert(pnt))
                return true;

            return false;
        }
        void subdivide()
        {//разделить квадрант на подквадранты
            gr.DrawRectangle(Pens.Black, boundary.x, boundary.y, boundary.w / 2, boundary.h / 2);
            gr.DrawRectangle(Pens.Black, boundary.x + (boundary.w / 2), boundary.y, boundary.w / 2, boundary.h / 2);
            gr.DrawRectangle(Pens.Black, boundary.x, boundary.y + (boundary.h / 2), boundary.w / 2, boundary.h / 2);
            gr.DrawRectangle(Pens.Black, boundary.x + (boundary.w / 2), boundary.y + (boundary.h / 2), boundary.w / 2, boundary.h / 2);
            
            northWest = new QuadTree(new Rectangle(boundary.x, boundary.y, boundary.w / 2, boundary.h / 2),gr);
            northEast = new QuadTree(new Rectangle(boundary.x + (boundary.w / 2), boundary.y, boundary.w / 2, boundary.h / 2),gr);
            southWest = new QuadTree(new Rectangle(boundary.x, boundary.y + (boundary.h / 2), boundary.w / 2, boundary.h / 2),gr);
            southEast = new QuadTree(new Rectangle(boundary.x + (boundary.w / 2), boundary.y + (boundary.h / 2), boundary.w / 2, boundary.h / 2),gr);

        }
        public List<P> queryRange(Rectangle range)
        {//поиск точек, входящих в некоторое поле "range"
            List<P> answer = new List<P>();
            if (!boundary.intersectsRectangle(range))
                return answer;
            if (northWest == null)
            {
                foreach (P p in points)
                {
                    if (p != null && range.containsPoint(p))
                        answer.Add(p);
                }
                return answer;
            }
            answer.AddRange(northWest.queryRange(range));
            answer.AddRange(northEast.queryRange(range));
            answer.AddRange(southEast.queryRange(range));
            answer.AddRange(southWest.queryRange(range));
            return answer;
        }
    }
}

