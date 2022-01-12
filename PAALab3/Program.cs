using System;
using System.Drawing;

namespace PAALab3
{
    class Program
    {
        static void Main(string[] args)
        {
            //string fileName = "Brojevi.txt";
            //uneti imena fajlova razlicitih duzina i karakteristika

           // AditionalFunctions.TestHuffman(fileName, true);
            //AditionalFunctions.TestShannonFano(fileName, false);

            Color c = Color.FromArgb(190, 46, 99);

            Console.WriteLine("r = 0, g = 255, b = 255 ");
            double hue = 0;
            double sat = 0;
            double val = 0;

            HSV.ColorToHSV(c, out hue, out sat, out val);
            Console.WriteLine("hue = " + hue + " sat = " + sat + " val = " + val);

            Color c2 = HSV.ColorFromHSV(hue, sat, val);
            Console.WriteLine("r = " + c2.R + " g = " + c2.G + " b = " + c2.B);



        }
    }
}