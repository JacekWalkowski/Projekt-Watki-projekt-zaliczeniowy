using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace Projekt
{
    public partial class Form1 : Form
    {
        private Random losuj;
        private Pilka pilka;
        private Zawodnik[] z;
        private Thread[] Threads;
        private object o;
        private Barrier bariera1;
        private Barrier bariera2;

        //konstruktor
        public Form1()
        {
            InitializeComponent();
            losuj = new Random();
            pilka = new Pilka();
            z = new Zawodnik[6];
            Threads = new Thread[7];
            o = new object();
            bariera2 = new Barrier(7);
            bariera1 = new Barrier(7, (b) => {pictureBox1.Invalidate();});

            for (int i = 0; i < 7; ++i) Threads[i] = new Thread(odswiez);
            z[0] = new Zawodnik(new PointF(220, 130), Brushes.Blue, losuj.Next(360));
            z[1] = new Zawodnik(new PointF(320, 280), Brushes.Blue, losuj.Next(360));
            z[2] = new Zawodnik(new PointF(220, 430), Brushes.Blue, losuj.Next(360));
            z[3] = new Zawodnik(new PointF(630, 130), Brushes.Red, losuj.Next(360));
            z[4] = new Zawodnik(new PointF(535, 280), Brushes.Red, losuj.Next(360));
            z[5] = new Zawodnik(new PointF(630, 430), Brushes.Red, losuj.Next(360));
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            for (int i = 0; i < 6; ++i) g.FillEllipse(z[i].Kolor, new RectangleF(z[i].Wsp.X, z[i].Wsp.Y, 50, 50));
            g.DrawImage(pilka.Obraz, pilka.Wsp);
        }

        private void odswiez(object a)
        {
            while (true)
            {
                if (a is Zawodnik)
                {
                    ((Zawodnik)a).zmienWsp();
                    try { bariera1.SignalAndWait(); }
                    catch (BarrierPostPhaseException e) { }
                    //sprawdzenie odbicia
                    for (int i = 0; i < 6; ++i)
                    {
                        if (odleglosc(z[i].Srodek, ((Zawodnik)a).Srodek) < 50 && odleglosc(z[i].Srodek, ((Zawodnik)a).Srodek) != 0)
                        {
                            lock (o)
                            {
                                if (((Zawodnik)a).Kat != z[i].Kat)
                                {
                                    ((Zawodnik)a).Temp = ((Zawodnik)a).Kat;
                                    ((Zawodnik)a).Kat = z[i].Kat;
                                }
                                else
                                {
                                    ((Zawodnik)a).Kat = z[i].Temp;
                                }
                            }
                        }
                    }
                }
                else
                {
                    pilka.zmienWsp();
                    try { bariera1.SignalAndWait(); }
                    catch (BarrierPostPhaseException e) { }
                    if (pilka.Wsp.X > 839)
                    {
                        reset();
                        MessageBox.Show("Gol! WYGRALI NIEBIESCY");
                        for (int i = 0; i < 7; ++i) Threads[i].Suspend();
                    }
                    else if (pilka.Wsp.X < 20)
                    {
                        reset();
                        MessageBox.Show("Gol! WYGRALI CZERWONI");
                        for (int i = 0; i < 7; ++i) Threads[i].Suspend();
                    }

                    for (int i = 0; i < 6; ++i)
                    {
                        if (odleglosc(z[i].Srodek, pilka.Srodek) < 43)
                        {
                            if (!pilka.Ruch)
                            {
                                pilka.Ruch = true;
                                pilka.Kat = z[i].Kat;
                                z[i].Kat += 180;
                            }
                            else
                            {
                                pilka.Temp = pilka.Kat;
                                pilka.Kat = z[i].Kat;
                                z[i].Kat = pilka.Temp;
                            }
                        }
                    }
                }
                bariera2.SignalAndWait();
                Thread.Sleep(5);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Threads[0].ThreadState.Equals(ThreadState.Unstarted))
            {
                for (int i = 0; i < 6; ++i) Threads[i].Start(z[i]);
                Threads[6].Start(pilka);
            }
            else if (Threads[0].ThreadState.Equals(ThreadState.Running) || Threads[0].ThreadState.Equals(ThreadState.WaitSleepJoin))
            {
                //nic nie rob
            }
            else
            {
                for (int i = 0; i < 7; ++i) Threads[i].Resume();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            for (int i = 0; i < 7; ++i) Threads[i].Abort();
        }

        public static double odleglosc(PointF p, PointF q)
        {
            double a = p.X - q.X;
            double b = p.Y - q.Y;
            double distance = Math.Sqrt(a * a + b * b);
            return distance;
        }

        public void reset()
        {

            if (pilka.Wsp.X > 820)
            {
                label1.Invoke(new ThreadStart(delegate()
                    {
                        label1.Text = (Int32.Parse(label1.Text) + 1).ToString();
                    }));
                //label1.Text = (Int32.Parse(label1.Text) + 1).ToString();
            }
            else
            {
                label2.Invoke(new ThreadStart(delegate()
                {
                    label2.Text = (Int32.Parse(label2.Text) + 1).ToString();
                }));
            }
            z[0].Wsp = new PointF(225, 130);
            z[1].Wsp = new PointF(325, 280);
            z[2].Wsp = new PointF(225, 430);
            z[3].Wsp = new PointF(630, 130);
            z[4].Wsp = new PointF(535, 280);
            z[5].Wsp = new PointF(630, 430);
            pilka.Wsp = new PointF(432, 280);
            pilka.Ruch = false;
            pictureBox1.Invalidate();
        }
    }
}
