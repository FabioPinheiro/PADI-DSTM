using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PADI_DSTM;

class Cicle
{
    static void Main(string[] args)
    {
        bool res;

        PadiDstm.Init();

        if ((args.Length > 0) && (args[0].Equals("C")))
        {
            res = PadiDstm.TxBegin();
            PadInt pi_a = PadiDstm.CreatePadInt(2);
            PadInt pi_b = PadiDstm.CreatePadInt(2000000001);
            PadInt pi_c = PadiDstm.CreatePadInt(1000000000);
            pi_a.Write(0);
            pi_b.Write(0);
            res = PadiDstm.TxCommit();
        }
        if ((args.Length > 0) && (args[0].Equals("D")))
        {
            res = PadiDstm.TxBegin();
            PadInt pi_a1 = PadiDstm.CreatePadInt(20);
            PadInt pi_b1 = PadiDstm.CreatePadInt(200010);
            PadInt pi_c1 = PadiDstm.CreatePadInt(100000);
            pi_a1.Write(0);
            pi_b1.Write(0);
            res = PadiDstm.TxCommit();
        }
        PadiDstm.Status();
        Console.WriteLine("####################################################################");
        Console.WriteLine("Finished creating PadInts. Press enter for 300 R/W transaction cycle.");
        Console.WriteLine("####################################################################");
        Console.ReadLine();
        for (int i = 0; i < 300; i++)
        {
            res = PadiDstm.TxBegin();
            PadInt pi_d = PadiDstm.AccessPadInt(2);
            PadInt pi_e = PadiDstm.AccessPadInt(2000000001);
            PadInt pi_f = PadiDstm.AccessPadInt(1000000000);
            PadInt pi_d1 = PadiDstm.AccessPadInt(20);
            PadInt pi_e1 = PadiDstm.AccessPadInt(200010);
            PadInt pi_f1 = PadiDstm.AccessPadInt(100000);
            int d = pi_d.Read();
            d++;
            pi_d.Write(d);
            int e = pi_e.Read();
            e++;
            pi_e.Write(e);
            int f = pi_f.Read();
            f++;
            pi_f.Write(f);
            int d1 = pi_d1.Read();
            d1++;
            pi_d1.Write(d1);
            int e1 = pi_e1.Read();
            e1++;
            pi_e1.Write(e1);
            int f1 = pi_f1.Read();
            f1++;
            pi_f1.Write(f1);
            Console.Write(".");
            res = PadiDstm.TxCommit();
            if (!res) Console.WriteLine("$$$$$$$$$$$$$$ ABORT $$$$$$$$$$$$$$$$$");
        }
        Console.WriteLine("####################################################################");
        Console.WriteLine("Status after cycle. Press enter for verification transaction.");
        Console.WriteLine("####################################################################");
        PadiDstm.Status();
        Console.ReadLine();
        res = PadiDstm.TxBegin();
        PadInt pi_g = PadiDstm.AccessPadInt(2);
        PadInt pi_h = PadiDstm.AccessPadInt(2000000001);
        PadInt pi_j = PadiDstm.AccessPadInt(1000000000);
        PadInt pi_g1 = PadiDstm.AccessPadInt(20);
        PadInt pi_h1 = PadiDstm.AccessPadInt(200010);
        PadInt pi_j1 = PadiDstm.AccessPadInt(100000);
        int g = pi_g.Read();
        int h = pi_h.Read();
        int j = pi_j.Read();
        int g1 = pi_g1.Read();
        int h1 = pi_h1.Read();
        int j1 = pi_j1.Read();
        res = PadiDstm.TxCommit();
        Console.WriteLine("####################################################################");
        Console.WriteLine("2 = " + g);
        Console.WriteLine("2000000001 = " + h);
        Console.WriteLine("1000000000 = " + j);
        Console.WriteLine("20 = " + g1);
        Console.WriteLine("200010 = " + h1);
        Console.WriteLine("100000 = " + j1);
        Console.WriteLine("Status post verification transaction. Press enter for exit.");
        Console.WriteLine("####################################################################");
        PadiDstm.Status();
        Console.ReadLine();
    }
}