using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;

namespace Projekt
{
    class Pilka
    {
        private int temp;
        private PointF wsp;
        private int r = 1;
        private Image obraz;
        private int kat;
        private bool ruch = false;

        //konstruktor
        public Pilka()
        {
            obraz = Image.FromFile("pilka.png");
            wsp = new PointF(432, 280);
        }

        public void zmienWsp()
        {
            if (ruch)
            {
                //zmiana katow
                if ((wsp.X > 834 || wsp.X < 25) && (wsp.Y > 325 || wsp.Y < 225))
                {
                    if (kat > 0 & kat < 90) kat = 180 - kat;
                    else if (kat > 90 & kat < 180) kat = 180 - kat;
                    else if (kat > 180 & kat < 270) kat = 270 - kat + 270;
                    else kat = 360 - kat + 180;
                }
                else if (wsp.Y > 544 || wsp.Y < 15)
                {
                    if (kat > 0 & kat < 90) kat = 360 - kat;
                    else if (kat > 90 & kat < 180) kat = 360 - kat;
                    else if (kat > 180 & kat < 270) kat = 360 - kat;
                    else kat = 360 - kat;
                }
                //zmiana wsp
                wsp.X += (float)(r * Math.Cos((kat * Math.PI) / 180));
                wsp.Y += (float)(r * Math.Sin((kat * Math.PI) / 180));
            }
        }

        //wlasciwosci
        public int Temp
        {
            get
            {
                return temp;
            }
            set
            {
                temp = value;
            }
        }
        public PointF Wsp
        {
            get
            {
                return wsp;
            }
            set
            {
                wsp = value;
            }
        }
        public bool Ruch
        {
            set
            {
                ruch = value;
            }
            get
            {
                return ruch;
            }
        }
        public PointF Srodek
        {
            get
            {
                return new PointF(wsp.X + 18, wsp.Y + 18);
            }
        }

        public int Kat
        {
            get
            {
                return kat;
            }
            set
            {
                kat = value;
            }
        }
        public Image Obraz
        {
            get
            {
                return obraz;
            }
            set
            {
                obraz = value;
            }
        }
    }
}
