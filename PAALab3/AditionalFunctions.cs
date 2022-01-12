using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace PAALab3
{
    public class AditionalFunctions
    {
        public static string LoadText(string fileName)
        {
            string res = "";
            string location = @"C:\Users\SmartPC\Desktop\things\Fajlovi\" + fileName;
            StreamReader sr;

            using (sr = new StreamReader(location))
            {
                while (!sr.EndOfStream)
                    res = res + sr.ReadLine();
            }
            return res;
        }

        public static void PercentEfficiency(string filePathSrc, string filePathDst)
        {
            long compressed = 0, initialSize = 0;
            FileStream fs = new FileStream(filePathDst, FileMode.Open);
            compressed = fs.Length;
            fs.Close();
            fs = new FileStream(filePathSrc, FileMode.Open);
            initialSize = fs.Length;
            fs.Close();
            double procenat = (double)compressed / initialSize;
            Console.WriteLine("Pocetna velicina " + initialSize + " B");
            Console.WriteLine("Kompresovana velicina " + compressed + " B");
            Console.WriteLine("Procentualno " + (1 - procenat));
        }

        public static void TestHuffman(string fileName, bool printDictionary)
        {
            string input = LoadText(fileName);
            Huffman huffman = new Huffman();
            Stopwatch s = new Stopwatch();

            huffman.Build(input);
            Console.WriteLine("Pocetak: ");
            s.Start();
            huffman.SaveCompression2(@"C:\Users\SmartPC\Desktop\things\Fajlovi\Transformacije\tmp1.bin");
            Console.WriteLine("Dekodiranje Huffman");
            huffman.LoadCompressed2(@"C:\Users\SmartPC\Desktop\things\Fajlovi\Transformacije\tmp1.bin", @"C:\Users\SmartPC\Desktop\things\Fajlovi\Transformacije\decode1.txt");
            s.Stop();
            Console.WriteLine("Huffman : " + s.ElapsedMilliseconds + " ms");
            PercentEfficiency(@"C:\Users\SmartPC\Desktop\things\Fajlovi\Transformacije\decode1.txt", @"C:\Users\SmartPC\Desktop\things\Fajlovi\Transformacije\tmp1.bin");
            Console.WriteLine();

            if (printDictionary)
            {
                huffman.PrintDictionary();
            }
        }

        public static void TestShannonFano(string fileName, bool printDictionary)
        {
            string input = LoadText(fileName);
            ShannonFano sf = new ShannonFano();
            Stopwatch s = new Stopwatch();

            sf.Build(input);

            Console.WriteLine("Pocetak: ");
            s.Start();

            sf.SaveCompression2(@"C:\Users\SmartPC\Desktop\things\Fajlovi\Transformacije\tmp2.bin");
            Console.WriteLine("Dekodiranje Shannon Fano");
            sf.LoadCompressed2(@"C:\Users\SmartPC\Desktop\things\Fajlovi\Transformacije\tmp2.bin", @"C:\Users\SmartPC\Desktop\things\Fajlovi\Transformacije\decode2.txt");

            s.Stop();

            Console.WriteLine("Shanon Fano : " + s.ElapsedMilliseconds + " ms");

            PercentEfficiency(@"C:\Users\SmartPC\Desktop\things\Fajlovi\Transformacije\decode2.txt", @"C:\Users\SmartPC\Desktop\things\Fajlovi\Transformacije\tmp2.bin");

            if (printDictionary)
                sf.PrintDictionary();


        }
    }
}
