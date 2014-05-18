using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PADI_DSTM;
using System.Diagnostics;
namespace Client2
{
    class Program
    {
        static void Main(string[] args)
        {
            bool res;
            bool resA = false;
            bool resB = false;
            PadiDstm.Init();
            //2 Servidores
            if ((args.Length > 0) && (args[0].Equals("A")))
            {

                resA = true;
            }
            if ((args.Length > 0) && (args[0].Equals("B")))
            {
                resB = true;

            }
            if ((args.Length > 0) && (args[0].Equals("C")))
            {
                res = PadiDstm.TxBegin();
                PadInt pi_a = PadiDstm.CreatePadInt(2);
                PadInt pi_b = PadiDstm.CreatePadInt(3);
                PadInt pi_c = PadiDstm.CreatePadInt(4);
                PadInt pi_d = PadiDstm.CreatePadInt(5);
                PadInt pi_e = PadiDstm.CreatePadInt(1);
                pi_a.Write(0);
                pi_b.Write(0);
                res = PadiDstm.TxCommit();
            }
            if ((args.Length > 0) && (args[0].Equals("D")))
            {
                res = PadiDstm.TxBegin();
                PadInt pi_a = PadiDstm.CreatePadInt(6);
                PadInt pi_b = PadiDstm.CreatePadInt(7);
                PadInt pi_c = PadiDstm.CreatePadInt(8);
                PadInt pi_d = PadiDstm.CreatePadInt(9);
                PadInt pi_e = PadiDstm.CreatePadInt(10);
                pi_a.Write(0);
                pi_b.Write(0);
                res = PadiDstm.TxCommit();
            }
            //4 Servidores


            if ((args.Length > 0) && (args[0].Equals("E")))
            {
                res = PadiDstm.TxBegin();
                PadInt pi_a = PadiDstm.CreatePadInt(1);
                PadInt pi_b = PadiDstm.CreatePadInt(2);
                PadInt pi_c = PadiDstm.CreatePadInt(3);
              

                pi_a.Write(0);
                pi_b.Write(0);
                res = PadiDstm.TxCommit();
            }
            if ((args.Length > 0) && (args[0].Equals("F")))
            {
                res = PadiDstm.TxBegin();
                PadInt pi_a = PadiDstm.CreatePadInt(4);
                PadInt pi_b = PadiDstm.CreatePadInt(5);
                PadInt pi_c = PadiDstm.CreatePadInt(6);
               
               
                pi_a.Write(0);
                pi_b.Write(0);
                res = PadiDstm.TxCommit();
            }

            if ((args.Length > 0) && (args[0].Equals("G")))
            {
                res = PadiDstm.TxBegin();
                PadInt pi_a = PadiDstm.CreatePadInt(7);
                PadInt pi_b = PadiDstm.CreatePadInt(8);
                pi_a.Write(0);
                pi_b.Write(0);
                res = PadiDstm.TxCommit();
            }
            if ((args.Length > 0) && (args[0].Equals("H")))
            {
                res = PadiDstm.TxBegin();
                PadInt pi_a = PadiDstm.CreatePadInt(9);
                PadInt pi_b = PadiDstm.CreatePadInt(10);
                pi_a.Write(0);
                pi_b.Write(0);
                res = PadiDstm.TxCommit();
            }

            //10 Servidores
            if ((args.Length > 0) && (args[0].Equals("a")))
            {
                res = PadiDstm.TxBegin();
                PadInt pi_a = PadiDstm.CreatePadInt(1);
               pi_a.Write(0);
                res = PadiDstm.TxCommit();
            }
            if ((args.Length > 0) && (args[0].Equals("b")))
            {
                res = PadiDstm.TxBegin();
                PadInt pi_a = PadiDstm.CreatePadInt(2);
                pi_a.Write(0);
                res = PadiDstm.TxCommit();
            }

            if ((args.Length > 0) && (args[0].Equals("c")))
            {
                res = PadiDstm.TxBegin();
                PadInt pi_a = PadiDstm.CreatePadInt(3);
                pi_a.Write(0);
                res = PadiDstm.TxCommit();
            }
            if ((args.Length > 0) && (args[0].Equals("d")))
            {
                res = PadiDstm.TxBegin();
                PadInt pi_a = PadiDstm.CreatePadInt(4);
                pi_a.Write(0);
                res = PadiDstm.TxCommit();
            }
            if ((args.Length > 0) && (args[0].Equals("e")))
            {
                res = PadiDstm.TxBegin();
                PadInt pi_a = PadiDstm.CreatePadInt(5);
                pi_a.Write(0);
                res = PadiDstm.TxCommit();
            }
            if ((args.Length > 0) && (args[0].Equals("f")))
            {
                res = PadiDstm.TxBegin();
                PadInt pi_a = PadiDstm.CreatePadInt(6);
                pi_a.Write(0);
                res = PadiDstm.TxCommit();
            }

            if ((args.Length > 0) && (args[0].Equals("g")))
            {
                res = PadiDstm.TxBegin();
                PadInt pi_a = PadiDstm.CreatePadInt(7);
                pi_a.Write(0);
                res = PadiDstm.TxCommit();
            }
            if ((args.Length > 0) && (args[0].Equals("h")))
            {
                res = PadiDstm.TxBegin();
                PadInt pi_a = PadiDstm.CreatePadInt(8);
                pi_a.Write(0);
                res = PadiDstm.TxCommit();
            }
            if ((args.Length > 0) && (args[0].Equals("i")))
            {
                res = PadiDstm.TxBegin();
                PadInt pi_a = PadiDstm.CreatePadInt(9);
                pi_a.Write(0);
                res = PadiDstm.TxCommit();
            }
            if ((args.Length > 0) && (args[0].Equals("j")))
            {
                res = PadiDstm.TxBegin();
                PadInt pi_a = PadiDstm.CreatePadInt(10);
                pi_a.Write(0);
                res = PadiDstm.TxCommit();
            }



            Console.WriteLine("####################################################################");
            Console.WriteLine("Finished creating PadInts. Press enter for 1000 R/W transaction cycle.");
            Console.WriteLine("####################################################################");
            Console.ReadLine();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
           

         

            if (resA)
            {
                for (int i = 0; i < 1000; i++)
                {
                    res = PadiDstm.TxBegin();
                    PadInt pi_a1 = PadiDstm.AccessPadInt(2);
                    PadInt pi_b1 = PadiDstm.AccessPadInt(3);
                    PadInt pi_c1 = PadiDstm.AccessPadInt(4);
                    PadInt pi_d1 = PadiDstm.AccessPadInt(5);
                    PadInt pi_e1 = PadiDstm.AccessPadInt(1);
                    int a1 = pi_a1.Read();
                    a1++;
                    pi_a1.Write(a1);

                    int b1 = pi_b1.Read();
                    b1++;
                    pi_b1.Write(b1);

                    int c1 = pi_c1.Read();
                    c1++;
                    pi_c1.Write(c1);

                    int d1 = pi_d1.Read();
                    d1++;
                    pi_d1.Write(d1);

                    int e1 = pi_e1.Read();
                    e1++;
                    pi_e1.Write(e1);
               
                    Console.Write(".");
                    res = PadiDstm.TxCommit();
                    //          if (!res) Console.WriteLine("$$$$$$$$$$$$$$ ABORT $$$$$$$$$$$$$$$$$");
                }
            }
            if (resB)
            {

                for (int i = 0; i < 1000; i++)
                {
                    res = PadiDstm.TxBegin();
                    PadInt pi_a1 = PadiDstm.AccessPadInt(6);
                    PadInt pi_b1 = PadiDstm.AccessPadInt(7);
                    PadInt pi_c1 = PadiDstm.AccessPadInt(8);
                    PadInt pi_d1 = PadiDstm.AccessPadInt(9);
                    PadInt pi_e1 = PadiDstm.AccessPadInt(10);

                    int a1 = pi_a1.Read();
                    a1++;
                    pi_a1.Write(a1);

                    int b1 = pi_b1.Read();
                    b1++;
                    pi_b1.Write(b1);

                    int c1 = pi_c1.Read();
                    c1++;
                    pi_c1.Write(c1);

                    int d1 = pi_d1.Read();
                    d1++;
                    pi_d1.Write(d1);

                    int e1 = pi_e1.Read();
                    e1++;
                    pi_e1.Write(e1);

                    Console.Write(".");
                    res = PadiDstm.TxCommit();
                    //          if (!res) Console.WriteLine("$$$$$$$$$$$$$$ ABORT $$$$$$$$$$$$$$$$$");
                }
            }



            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            Console.WriteLine("####################################################################");
            Console.WriteLine("Status after cycle. Press enter for verification transaction.");
            Console.WriteLine("####################################################################");
            PadiDstm.Status();
            Console.ReadLine();
            res = PadiDstm.TxBegin();
            PadInt pi_g = PadiDstm.AccessPadInt(1);
            PadInt pi_h = PadiDstm.AccessPadInt(2);
            PadInt pi_j = PadiDstm.AccessPadInt(3);
            PadInt pi_g1 = PadiDstm.AccessPadInt(4);
            PadInt pi_h1 = PadiDstm.AccessPadInt(5);
            PadInt pi_j1 = PadiDstm.AccessPadInt(6);
            PadInt pi_g2 = PadiDstm.AccessPadInt(7);
            PadInt pi_h2 = PadiDstm.AccessPadInt(8);
            PadInt pi_j2 = PadiDstm.AccessPadInt(9);
            PadInt pi_g3 = PadiDstm.AccessPadInt(10);
            int g = pi_g.Read();
            int h = pi_h.Read();
            int j = pi_j.Read();
            int g1 = pi_g1.Read();
            int h1 = pi_h1.Read();
            int j1 = pi_j1.Read();
            int g2 = pi_g2.Read();
            int h2 = pi_h2.Read();
            int j2 = pi_j2.Read();
            int g3 = pi_g3.Read();
            res = PadiDstm.TxCommit();
            Console.WriteLine("####################################################################");
            Console.WriteLine("1 = " + g);
            Console.WriteLine("2 = " + h);
            Console.WriteLine("3 = " + j);
            Console.WriteLine("4 = " + g1);
            Console.WriteLine("5 = " + h1);
            Console.WriteLine("6 = " + j1);
            Console.WriteLine("7 = " + g2);
            Console.WriteLine("8 = " + h2);
            Console.WriteLine("9 = " + j2);
            Console.WriteLine("10 = " + g3);

            Console.WriteLine("Status post verification transaction. Press enter for exit.");
            Console.WriteLine("####################################################################");
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
            PadiDstm.Status();
            Console.ReadLine();
        }
    }
}
