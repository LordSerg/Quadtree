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
        Bitmap b;
        QuadTree q;
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            q.insert(new P(Cursor.Position.X, Cursor.Position.Y));
            pictureBox1.Image = b;
        }
        Graphics g,g2;
        private void Form1_Load(object sender, EventArgs e)
        {
            b = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(b);
            //for (int i = 0; i < 100; i++)
            //    p[i] = new Point(r.Next(pictureBox1.Width),r.Next(pictureBox1.Height));
            q = new QuadTree(new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height),g);
            to_do = false;
            //Random r = new Random();
            //for (int i = 1; i < 100; i++)
            //    if(q.insert(new P/*(i*100,i*100)*/(r.Next(pictureBox1.Width), r.Next(pictureBox1.Height))))
            //    {//визуализация вставки
            //        pictureBox1.Image = b;
            //    }
            //for(int i = 0; i < 100; i++)
            //    if( )
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {//поиск точек, принадлижащих прямоугольнику
            if (to_do)
            {
                Rectangle search = new Rectangle(Cursor.Position.X - 50, Cursor.Position.Y - 50, 100, 100);

                q.queryRange(search, null);
                pictureBox1.Image = b;
            }
        }
        bool to_do;
        private void searchPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (searchPointsToolStripMenuItem.Text == "Search points")
            {
                searchPointsToolStripMenuItem.Text = "Put points";
                g2 = q.gr;
                pictureBox1.Image = b;
            }
            else
            {
                searchPointsToolStripMenuItem.Text = "Search points";
                g = g2;
                q.gr = g2;
                pictureBox1.Image = b;
            }
            to_do = !to_do;
        }
    }
    struct P//point
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
        //P center, halfDemention;
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
            if ((x-other.x<other.w||other.x-x<w)
                &&(y-other.y<other.h||other.y-y<h))
            {
                return true;
            }
            else
                return false;
        }
    }
    class QuadTree
    {
        
        const int num_of_points = 1;
        Rectangle boundary;
        //public Bitmap b;
        public Graphics gr;
        //List <P> points=new List<P>();//маассив точек, которые способен хранить 1 квадрант
        //но так как у нас всего одна точка, то
        P point;
        bool has_point = false;
        //int w, h, x, y;
        QuadTree northWest;
        QuadTree northEast;
        QuadTree southWest;
        QuadTree southEast;
        public QuadTree(Rectangle bndr,Graphics g)
        {
            boundary = bndr;
            //b = new Bitmap((int)bndr.w, (int)bndr.h);
            gr = g;
        }
        public bool insert(P pnt)
        {
            if (!boundary.containsPoint(pnt))
                return false;

            if (!has_point)
            {
                gr.FillEllipse(Brushes.Red, pnt.X - 5, pnt.Y - 5, 10, 10);
                point = pnt;
                has_point = true;
                return true;
            }

            if (northWest == null)
                subdivide();

            if (northWest.insert(pnt)&& northWest.insert(point))
                return true;
            if (northEast.insert(pnt)&& northEast.insert(point))
                return true;
            if (southWest.insert(pnt)&& southWest.insert(point))
                return true;
            if (southEast.insert(pnt)&& southEast.insert(point))
                return true;

            return false;
        }
        void subdivide()
        {
            gr.DrawRectangle(Pens.Black, boundary.x, boundary.y, boundary.w / 2, boundary.h / 2);
            gr.DrawRectangle(Pens.Black, boundary.x + (boundary.w / 2), boundary.y, boundary.w / 2, boundary.h / 2);
            gr.DrawRectangle(Pens.Black, boundary.x, boundary.y + (boundary.h / 2), boundary.w / 2, boundary.h / 2);
            gr.DrawRectangle(Pens.Black, boundary.x + (boundary.w / 2), boundary.y + (boundary.h / 2), boundary.w / 2, boundary.h / 2);
            
            northWest = new QuadTree(new Rectangle(boundary.x, boundary.y, boundary.w / 2, boundary.h / 2),gr);
            northEast = new QuadTree(new Rectangle(boundary.x + (boundary.w / 2), boundary.y, boundary.w / 2, boundary.h / 2),gr);
            southWest = new QuadTree(new Rectangle(boundary.x, boundary.y + (boundary.h / 2), boundary.w / 2, boundary.h / 2),gr);
            southEast = new QuadTree(new Rectangle(boundary.x + (boundary.w / 2), boundary.y + (boundary.h / 2), boundary.w / 2, boundary.h / 2),gr);

        }
        public void Show(Rectangle r)
        {

        }
        public P[] queryRange(Rectangle range,Graphics g)
        {//поиск точек, входящих в некоторое поле "range"
            /*P[] pointsInRange=null;
            if (!boundary.intersectsRectangle(range))
                return pointsInRange;
            for(int i=0;i<points.Length;i++)
            {
                if (range.containsPoint(points[i]))
                    pointsInRange.Append(points[i]);
            }
            if (northWest == null)
                return pointsInRange;

            //pointsInRange = points;
            return pointsInRange;
            */
            if (g != null)
                gr = g;
            else
                gr.Clear(Color.White);
            if (!boundary.intersectsRectangle(range))
            {
                return null;
            }
            if (range.containsPoint(point))
                gr.FillEllipse(Brushes.Red, point.X - 5, point.Y - 5, 10, 10);
            //if (!has_point)
            //    return null;
            //else
            //{
                
            //}
            if (northWest == null)
                return null;
            northWest.queryRange(range,gr);
            northEast.queryRange(range,gr);
            southEast.queryRange(range,gr);
            southWest.queryRange(range,gr);

            gr.DrawRectangle(Pens.Red, range.x, range.y, range.w, range.h);
            return null;

        }
        //public QuadTree(Point P)
        //{
        //    if (contains(P))
        //    {

        //        QuadTree r_u = new QuadTree(P);
        //        QuadTree r_d = new QuadTree(P);
        //        QuadTree l_u = new QuadTree(P);
        //        QuadTree l_d = new QuadTree(P);
        //    }
        //}
    }
}

