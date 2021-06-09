using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace KapautuChroma
{
    public partial class ChromaMain : Form
    {

        Bitmap InputImage;
        
        List<string> CalData1 = new List<string>();
        List<string> CalData2 = new List<string>();
        List<string> allData = new List<string>();
        List<string> xData = new List<string>();
        List<string> yData = new List<string>();
        List<string> zData = new List<string>();
        int numSamples = 0;
        int largo, ancho;
        int nerrorsMAX, nerrorsMIN = 0;
        bool showerror = true;

        double Maxdiff = 0;

        int errorOver = 0;
        double Yr ;
        double Yg;
        double Yb ;
        double smallxr ;
        double smallyr ;
        double smallxg ;
        double smallyg;
        double smallxb ;
        double smallyb ;
        double smallxr2;
        double smallyr2;
        double smallxg2;
        double smallyg2;
        double smallxb2;
        double smallyb2;
        double[, ,] x1,y1,ly1;
        double[, ,] x2, y2, ly2;
     

        double[,] numCal1, numCal2,numx,numy,numz,numr,numg,numb,numL,numsx,numsy;

        Matrix Minv;


        public ChromaMain()
        {
            InitializeComponent();


        }

        private double searchYr(double R, double[,] numC)
        {
           
            double result = 0;
            double aa = numC[2, 0];
            double bb = numC[3, 0];
            double cc = numC[4, 0];
            result = aa * R*R + bb * R + cc;
            return result;
        }
        private double searchYg(double G, double[,] numC)
        {
            
            double result = 0;
            double aa = numC[2, 1];
            double bb = numC[3, 1];
            double cc = numC[4, 1];
            result = aa * G * G + bb * G + cc;
            return result;
        }
        private double searchYb(double B, double[,] numC)
        {
            double result = 0;
            double aa = numC[2, 2];
            double bb = numC[3, 2];
            double cc = numC[4, 2];
            result = aa * B * B + bb * B + cc;
            return result;
        }

        private double searchsmallxr( double[,] numC)
        {
           return numC[0,0];
        }
        private double searchsmallyr(double[,] numC)
        {
          
            return numC[1,0];
        }

        private double searchsmallxg(double[,] numC)
        {
            
            return numC[0,1];
        }
        private double searchsmallyg(double[,] numC)
        {
           
            return numC[1, 1];
        }

        private double searchsmallxb(double[,] numC)
        {
            
            return numC[0,2];
        }
        private double searchsmallyb( double[,] numC)
        {
            
            return numC[1,2];
        }

        private double sacarR(double value, double[,] numC)
        {
            double result = 255;
            double aa = numC[2, 0];
            double bb = numC[3, 0];
            double cc = numC[4, 0] - value;

            double discriminante = bb * bb - 4 * aa * cc;
            if (discriminante <= 0)
            {
                discriminante = 0;
            }
            double raiz1 = -bb + Math.Sqrt(discriminante);
            raiz1 = raiz1 / (2 * aa);
            double raiz2 = -bb - Math.Sqrt(discriminante);
            raiz2 = raiz2 / (2 * aa);
            if (raiz1 > raiz2)
            {
                result = raiz1;
            }
            else
            {
                result = raiz2;
            }
            if (result.ToString().Contains("Nan"))
            {
                result = 0;
            }

            return result;
        }

        private double sacarG(double value, double[,] numC)
        {
            double result = 255;
            double aa = numC[2, 1];
            double bb = numC[3, 1];
            double cc = numC[4, 1] - value;

            double discriminante = bb * bb - 4 * aa * cc;
            if (discriminante <= 0)
            {
                discriminante = 0;
            }
            double raiz1 = -bb + Math.Sqrt(discriminante);
            raiz1 = raiz1 / (2 * aa);
            double raiz2 = -bb - Math.Sqrt(discriminante);
            raiz2 = raiz2 / (2 * aa);
            if (raiz1 > raiz2)
            {
                result = raiz1;
            }
            else
            {
                result = raiz2;
            }
            return result;
        }

        private double sacarB(double value, double[,] numC)
        {
            double result = 255;
            double aa = numC[2, 2];
            double bb = numC[3, 2];
            double cc = numC[4, 2] - value;

            double discriminante = bb * bb - 4 * aa * cc;
            if (discriminante <= 0)
            {
                discriminante = 0;
            }
            double raiz1 = -bb + Math.Sqrt(discriminante);
            raiz1 = raiz1 / (2 * aa);
            double raiz2 = -bb - Math.Sqrt(discriminante);
            raiz2 = raiz2 / (2 * aa);
            if (raiz1 > raiz2)
            {
                result = raiz1;
            }
            else
            {
                result = raiz2;
            }
            return result;
        }

        private double[] XYZtoLab(double[] XYZ) 
        {
            
            double[] Lab = new double[3];

            //White reference             
            double Xb = 91.36249652;
            double Yb = 100;
            double Zb = 87.26664809;

            double xr1=XYZ[0]/Xb;
            double yr1=XYZ[1]/Yb;
            double zr1=XYZ[2]/Zb;
            double epsilon = 0.008856;// (216 / 24389);
            double k = 903.3;// 24389 / 27;
            double fx, fy, fz;
            double untercio = double.Parse("1".ToString())/ double.Parse("3".ToString());
            if (xr1>epsilon)
            {
                fx = Math.Pow(xr1, untercio);
            }
            else
            {
                fx = (k * xr1 + 16 )/ 116;
            }
            if (yr1 > epsilon)
            {
                fy = Math.Pow(yr1, untercio);
            }
            else
            {
                fy = (k * yr1 + 16) / 116;
            }
            if (zr1 > epsilon)
            {
                fz = Math.Pow(zr1, untercio);
            }
            else
            {
                fz = (k * zr1 + 16) / 116;
            }
            
           

            Lab[0] = 116 * fy - 16;
            Lab[1] = 500*(fx-fy);
            Lab[2] = 200*(fy-fz);
            return Lab;
        }
        private double[] RGBtoXYZ(double[] RGB, double[,] numC)
        {
            double[] XYZ = new double[3];

            smallxr = searchsmallxr(numC);
            smallyr = searchsmallyr(numC);
            smallxg = searchsmallxg(numC);
            smallyg = searchsmallyg(numC);
            smallxb = searchsmallxb(numC);
            smallyb = searchsmallyb(numC);


            Yr = searchYr(RGB[0], numC);
            Yg = searchYg(RGB[1], numC);
            Yb = searchYb(RGB[2], numC);



            XYZ[0] = Yr * smallxr / smallyr + Yg * smallxg / smallyg + Yb * smallxb / smallyb;
            XYZ[1] = Yr + Yg + Yb;
            XYZ[2] = Yr * (1 - smallxr - smallyr) / smallyr + Yg * (1 - smallxg - smallyg) / smallyg + Yb * (1 - smallxb - smallyb) / smallyb;


            //Devolver resultado
            return XYZ;

        }       
        private double[] XYZtoRGB(double[] XYZ, double[,] numC)
        {
            double[] RGB = new double[3];
            double[] XYZ1 = new double[3];
            double[] Lab1 = new double[3];
            double[] Lab2 = new double[3];
            double[] xyY  = new double[3];
            double[] xyY1 = new double[3];
            double[] xyY2 = new double[3];
#pragma warning disable CS0219 // La variable está asignada pero nunca se usa su valor
#pragma warning disable CS0219 // La variable está asignada pero nunca se usa su valor
#pragma warning disable CS0219 // La variable está asignada pero nunca se usa su valor
            bool dataerror1 = false, dataerror2 = false, dataerror3 = false;
#pragma warning restore CS0219 // La variable está asignada pero nunca se usa su valor
#pragma warning restore CS0219 // La variable está asignada pero nunca se usa su valor
#pragma warning restore CS0219 // La variable está asignada pero nunca se usa su valor
           
            try
            {
                //XYZ[0] = (0.7414 / 1.273) * XYZ[0];
                //XYZ[1] = (0.6543 / 0.8308) * XYZ[1];
                //XYZ[2] = (0.3573 / 0.09876) * XYZ[2];
                //illuminance normalization
                //              original    june 2010
                //40 w          186lx       277.2lx
                //led 1         31lx        29.3lx
                //led2          63lx        73.4lx
                //led3          22lx        21.4lx
                //各照明の照度は、40W:341 lx 　led1:31.8 lx   led2:84.5 lx   led3:26.6 lx  です。
                //latest 40W:273lx;  led1 31lx;  led2 85.4lx;   led3 26.4lx
                if (XYZ[0] == 0) 
                {
                    dataerror1 = true;
                    XYZ[0] = 0.00000001;
                }
                if (XYZ[1] == 0)
                {
                    dataerror2 = true;
                    XYZ[1] = 0.00000001;
                }
                if (XYZ[2] == 0)
                {
                    dataerror3 = true;
                    XYZ[2] = 0.00000001;
                }
                                
                RGB[0] = Minv[1, 1].Re * XYZ[0] + Minv[1, 2].Re * XYZ[1] + Minv[1, 3].Re * XYZ[2];//Yr
                RGB[1] = Minv[2, 1].Re * XYZ[0] + Minv[2, 2].Re * XYZ[1] + Minv[2, 3].Re * XYZ[2];//Yg
                RGB[2] = Minv[3, 1].Re * XYZ[0] + Minv[3, 2].Re * XYZ[1] + Minv[3, 3].Re * XYZ[2];//Yb

                RGB[0] = this.sacarR(RGB[0], numC);
                RGB[1] = this.sacarG(RGB[1], numC);
                RGB[2] = this.sacarB(RGB[2], numC);

                if (RGB[0] > 255)
                {
                    if (this.showerror)
                    {
                        RGB[0] = 127;
                        RGB[1] = 255;
                        RGB[2] = 255;
                    }
                    else
                    {
                        RGB[0] = 255;
                    }
                }
                if (RGB[1] > 255)
                {
                    if (this.showerror)
                    {
                        RGB[0] = 127;
                        RGB[1] = 255;
                        RGB[2] = 255;
                    }
                    else
                    {
                        RGB[1] = 255;
                    }
                }
                if (RGB[2] > 255)
                {
                    if (this.showerror)
                    {
                        RGB[0] = 127;
                        RGB[1] = 255;
                        RGB[2] = 255;
                    }
                    else
                    {
                        RGB[2] = 255;
                    }
                }
                if (RGB[0].ToString().Contains("NaN"))
                {
                    RGB[0] = 127;
                    RGB[1] = 255;
                    RGB[2] = 255;
                }
                if (RGB[1].ToString().Contains("NaN"))
                {
                    RGB[0] = 255;
                    RGB[1] = 255;
                    RGB[2] = 127;
                }
                if (RGB[2].ToString().Contains("NaN")) 
                {
                    RGB[0] = 127;
                    RGB[1] = 127;
                    RGB[2] = 127;
                }
                if (RGB[0] < 0)
                {
                    if (this.showerror)
                    {
                        RGB[0] = 127;
                        RGB[1] = 255;
                        RGB[2] = 127;
                    }
                    else
                    {
                        RGB[0] = 0;
                    }
                }
                if (RGB[1] < 0)
                {
                    if (this.showerror)
                    {
                        RGB[0] = 255;
                        RGB[1] = 0;
                        RGB[2] = 255;
                    }
                    else
                    {
                        RGB[1] = 0;
                    }
                }
                if (RGB[2] < 0)
                {
                    if (this.showerror)
                    {
                        RGB[0] = 0;
                        RGB[1] = 255;
                        RGB[2] = 255;
                    }
                    else
                    {
                        RGB[2] = 0;
                    }
                }
                //some kind of color difference localization
                if (false)
                {
                    Lab1 = this.XYZtoLab(XYZ);
                    XYZ1 = this.RGBtoXYZ(RGB, numC);
                    Lab2 = this.XYZtoLab(XYZ1);
                    
                    double differen = Math.Abs(Lab1[0] - Lab2[0]);
                    //double maxpermi = 1;
                    /*
                    if (this.Maxdiff < differen)
                    {
                        this.Maxdiff = differen;
                    }
                    if (differen > maxpermi)
                    {
                        RGB[0] = 255 * 1 / 11;
                        RGB[1] = 255 * 1 / 11;
                        RGB[2] = 0;
                    }

                    if (differen > maxpermi * 2)
                    {
                        RGB[0] = 255 * 2 / 11;
                        RGB[1] = 255 * 2 / 11;
                        RGB[2] = 0;
                    }

                    if (differen > maxpermi * 3)
                    {
                        RGB[0] = 255 * 3 / 11;
                        RGB[1] = 255 * 3 / 11;
                        RGB[2] = 0;
                    }

                    if (differen > maxpermi * 4)
                    {
                        RGB[0] = 255 * 4 / 11;
                        RGB[1] = 255 * 4 / 11;
                        RGB[2] = 0;
                    }

                    if (differen > maxpermi * 5)
                    {
                        RGB[0] = 255 * 5 / 11;
                        RGB[1] = 255 * 5 / 11;
                        RGB[2] = 0;
                    }
                    if (differen > maxpermi * 6)
                    {
                        RGB[0] = 255 * 6 / 11;
                        RGB[1] = 255 * 6 / 11;
                        RGB[2] = 0;
                    }
                    if (differen > maxpermi * 7)
                    {
                        RGB[0] = 255 * 7 / 11;
                        RGB[1] = 255 * 7 / 11;
                        RGB[2] = 0;
                    }
                    if (differen > maxpermi * 8)
                    {
                        RGB[0] = 255 * 8 / 11;
                        RGB[1] = 255 * 8 / 11;
                        RGB[2] = 0;
                    }
                    if (differen > maxpermi * 9)
                    {
                        RGB[0] = 255 * 9 / 11;
                        RGB[1] = 255 * 9 / 11;
                        RGB[2] = 0;
                    }
                    if (differen > maxpermi * 10)
                    {
                        RGB[0] = 255 * 10 / 11;
                        RGB[1] = 255 * 10 / 11;
                        RGB[2] = 0;
                    }
                    if (differen > maxpermi * 20)
                    {
                        RGB[0] = 255;
                        RGB[1] = 255;
                        RGB[2] = 0;
                    }*/
                }
                
            }
            catch (Exception me) 
            {
                me.Message.ToString();
            }

            return RGB;

        }
        private double[] XYZtoxyY(double[] XYZ)
        {
            double[] xyY = new double[3];
            xyY[0] = XYZ[0] / (XYZ[0] + XYZ[1] + XYZ[2]);
            xyY[1] = XYZ[1] / (XYZ[0] + XYZ[1] + XYZ[2]);
            xyY[2] = XYZ[1] ;
            return xyY;
        }
        private double[] xyYtoXYZ(double[] xyY)
        {
            double[] XYZ = new double[3];
            XYZ[0] = xyY[0] * xyY[2] / xyY[1];
            XYZ[1] = xyY[2] ;
            XYZ[2] = (1 - xyY[0] - xyY[1]) * xyY[2]/xyY[1];
            return XYZ;
        }

        
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBoxChroma ab = new AboutBoxChroma();
            ab.ShowDialog(this);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult res = this.sfd_path.ShowDialog();
            if (res == DialogResult.OK)
            {
                this.textBox5.Text = this.sfd_path.FileName;
               

            }
        }

       
        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult res = this.ofd_path.ShowDialog();
            if (res == DialogResult.OK)
            {
                this.textBox4.Text = this.ofd_path.FileName;
                try
                {
                    // Create an instance of StreamReader to read from a file.
                    // The using statement also closes the StreamReader.
                    using (StreamReader sr = new StreamReader(this.textBox4.Text))
                    {
                        String line;

                        // Read and display lines from the file until the end of 
                        // the file is reached.
                        while ((line = sr.ReadLine()) != null)
                        {
                            this.CalData2.Add(line);
                        }
                    }
                }
                catch (Exception ej)
                {
                    // Let the user know what went wrong.
                    ej.Message.ToString();
                }

                this.numCal2 = new double[5, this.CalData2.Count];
                for (int i = 0; i < this.CalData2.Count; i++)
                {
                    string[] parts = this.CalData2.ElementAt(i).Split(',');
                    int y = 0;
                    foreach (string part in parts)
                    {
                        if (!part.Equals(""))
                        {
                            this.numCal2[y, i] = double.Parse(part, System.Globalization.CultureInfo.InvariantCulture);
                            y++;
                        }
                       
                    }


                }
                smallxr2 = searchsmallxr(this.numCal2);
                smallyr2 = searchsmallyr(this.numCal2);
                smallxg2 = searchsmallxg(this.numCal2);
                smallyg2 = searchsmallyg(this.numCal2);
                smallxb2 = searchsmallxb(this.numCal2);
                smallyb2 = searchsmallyb(this.numCal2);


                /*String mat = (smallxr2 / smallyr2) + "!" 
                    + (smallxg2 / smallyg2) + "!" + (smallxb2 / smallyb2) + 
                    ";" + "1!1!1;" + (1 - smallxr2 - smallyr2) / smallyr2 + 
                    "!" + (1 - smallxg2 - smallyg2) / smallyg2 + "!" +
                    (1 - smallxb2 - smallyb2) / smallyb2;*/
                double[,] matDouble = new double[3,3];
                matDouble[0, 0] = smallxr2 / smallyr2;
                matDouble[0, 1] = smallxg2 / smallyg2;
                matDouble[0, 2] = smallxb2 / smallyb2;
                matDouble[1, 0] = 1;
                matDouble[1, 1] = 1;
                matDouble[1, 2] = 1;
                matDouble[2, 0] = (1 - smallxr2 - smallyr2) / smallyr2;
                matDouble[2, 1] = (1 - smallxg2 - smallyg2) / smallyg2;
                matDouble[2, 2] = (1 - smallxb2 - smallyb2) / smallyb2;
                
                // String mat1 =mat.Replace(",", ".");
                //String mat2= mat1.Replace("!", ",");
                Matrix M = new Matrix(matDouble);
                Complex det = M.Determinant(); // det = 1
                this.Minv = M.Inverse();
            }
        }

      
        private void button7_Click(object sender, EventArgs e)
        {
            DialogResult res = this.ofd_path.ShowDialog();
            if (res == DialogResult.OK)
            {
                this.textBox6.Lines = this.ofd_path.FileNames;
                for (int indi = 0; indi < this.ofd_path.FileNames.Length; indi++)
                {
                    this.allData.Clear();
                    try
                    {
                        // Create an instance of StreamReader to read from a file.
                        // The using statement also closes the StreamReader.
                        using (StreamReader sr = new StreamReader(this.textBox6.Lines[indi]))
                        {
                            String line;

                            // Read and display lines from the file until the end of 
                            // the file is reached.
                            while ((line = sr.ReadLine()) != null)
                            {
                                this.allData.Add(line);
                            }
                        }
                    }
                    catch (Exception em)
                    {
                        // Let the user know what went wrong.
                        em.Message.ToString();
                    }
                    string[] parts = this.allData.ElementAt(1).Split(',');
                    this.largo = 1360;
                    this.ancho = 1024;
                    this.numx = new double[1360, 1024];
                    this.numy = new double[1360, 1024];
                    this.numz = new double[1360, 1024];
                    this.numSamples = this.allData.Count;
                    for (int i = 1; i <= 1024; i++)
                    {
                        parts = this.allData.ElementAt(i).Split(',');
                        int y = 0;
                        foreach (string part in parts)
                        {
                            if (y >= 0 && y < parts.Length - 1)
                            {
                                if (!part.Equals(""))
                                {
                                    this.numx[y, i - 1] = double.Parse(part, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                                }
                            }
                            y++;
                        }


                    }
                    for (int i = 1027; i <= 2050; i++)
                    {
                        parts = this.allData.ElementAt(i).Split(',');
                        int y = 0;
                        foreach (string part in parts)
                        {
                            if (y >= 0 && y < parts.Length - 1)
                            {
                                if (!part.Equals(""))
                                {
                                    this.numy[y, i - 1027] = double.Parse(part, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                                }
                            }
                            y++;
                        }


                    }
                    for (int i = 2053; i <= 3076; i++)
                    {
                        parts = this.allData.ElementAt(i).Split(',');
                        int y = 0;
                        foreach (string part in parts)
                        {
                            if (y >= 0 && y < parts.Length - 1)
                            {
                                if (!part.Equals(""))
                                {
                                    this.numz[y, i - 2053] = double.Parse(part, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                                }
                            }
                            y++;
                        }


                    }
                }//for each file
            }
        }

       
        private void button9_Click(object sender, EventArgs e)
        {
            
            //double[] xyY = new double[3];
            double[] XYZ = new double[3];
            double[] RGBsolution = new double[3];

            this.numr = new double[this.largo, this.ancho];
            this.numg = new double[this.largo, this.ancho];
            this.numb = new double[this.largo, this.ancho];
           

            for (int i = 0; i < this.largo; i++)
            {
                for (int j = 0; j < this.ancho; j++)
                {
                    XYZ[0] = this.numx[i, j];
                    XYZ[1] = this.numy[i, j];
                    XYZ[2] = this.numz[i, j];
                    RGBsolution = this.XYZtoRGB(XYZ, this.numCal2);

                    this.numr[i, j] = RGBsolution[0];
                    this.numg[i, j] = RGBsolution[1];
                    this.numb[i, j] = RGBsolution[2];
                }
            }
            Bitmap bmpimg = new Bitmap(largo, ancho);
            BitmapData data = new BitmapData();

            int xerror, yerror = 0;
            try
            {
                data = bmpimg.LockBits(new System.Drawing.Rectangle(0, 0, largo, ancho), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                unsafe
                {
                    byte* ptr = (byte*)data.Scan0;

                    int remain = data.Stride - data.Width * 3;

                    for (int i = 0; i < data.Height; i++)
                    {
                        for (int j = 0; j < data.Width; j++)
                        {
                            xerror = i;
                            yerror = j;
                            if(double.IsNaN(this.numr[j, i]))
                            {
                                this.numr[j, i] = 0;
                            }
                            if (double.IsNaN(this.numg[j, i]))
                            {
                                this.numg[j, i] = 0;
                            }
                            if (double.IsNaN(this.numb[j, i]))
                            {
                                this.numb[j, i] = 0;
                            }
                            double rrr=Math.Round(this.numr[j, i]);
                            double ggg=Math.Round(this.numg[j, i]);
                            double bbb=Math.Round(this.numb[j, i]);
                            if(rrr>255)rrr=255;
                            if(ggg>255)ggg=255;
                            if(bbb>255)bbb=255;
                            if(rrr<0)rrr=0;
                            if(ggg<0)ggg=0;
                            if(bbb<0)bbb=0;

                            ptr[2] = Convert.ToByte(Convert.ToInt32(rrr));
                            ptr[1] = Convert.ToByte(Convert.ToInt32(ggg));
                            ptr[0] = Convert.ToByte(Convert.ToInt32(bbb));
                            ptr += 3;
                        }

                        ptr += remain;
                    }


                }
            }
            catch (Exception el)
            {
                el.ToString();
                bmpimg.UnlockBits(data);
                MessageBox.Show(this, "Error FileFormat not supported", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            bmpimg.UnlockBits(data);
            try
            {
                bmpimg.Save(this.textBox5.Text);
            }
            catch (Exception mmm) { mmm.Message.ToString(); }

            MessageBox.Show(this, "File created ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
           
        }

    
    }
}
