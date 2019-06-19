using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRAPCommon
{

    /// <summary>
    /// 此类是从IRAP数据库中的数学函数转换而来,已经过单位测试
    /// </summary>
    public class IRAPMath
    {
       
        public static Int64 sfn_PGa(Int16 a, int x, int y)
        {
            Int64 v;
            Int64 r;
            if (x == 0 && y == 0)
            { return 0; }
            v = (Int64)((x + a) / (a + 1)) + (Int64)y;
            r = v * (v - 1) / 2 * (Int64)(a + 1) + (Int64)x + v;

            return r;
        }

     
        public static Int64 sfn_Ka(Int16 a, Int64 p)
        {
            Int64 v;
            Int64 r;
            if (a == 0) { return 0; }
            v = (Int64)Math.Floor(Math.Sqrt(8.0 * (Double)p * (Double)(a + 1) + (Double)(a - 1) * (Double)(a - 1)));

            if (a > 1) { v = v + (Int64)a - 1; }
            r = (Int64)Math.Floor((Double)v / (2 * (Int64)a + 2));

            if (r > 1) { r = r + ((Int64)a + 1) * r * (r - 1) / 2; }
            if (p > r) { return (Int64)p - r; }

            return 0;
        }


       
        public static Int64 sfn_La(Int16 a, Int64 p)
        {
            Int64 v;
            Int64 r;
            Int64 t;

            if (a == 0) { return 0; }
            v = (Int64)Math.Floor(Math.Sqrt(8.0 * (Double)p * (Double)(a + 1) + (Double)(a - 1) * (Double)(a - 1)));
            if (a > 1) { v = v + (Int64)a - 1; }
            r = (Int64)Math.Floor((Double)v / (2 * (Int64)a + 2));

            if (r > 1) { t = r + ((Int64)a + 1) * r * (r - 1) / 2; }
            else
            { t = r; }
            if (p > t) { v = (Int64)p - t; }
            else
            {
                v = 0;

            }
            t = (Int64)Math.Floor(((Double)v + (Int64)a) / ((Int64)a + 1));

            if (r > t) { return r - t; }

            return 0;
        }


         
        public static byte sfn_nTPTransform(byte a)
        {
            Byte r;
            r = (Byte)a;
            if ((r & 128) != ((r & 32) << 2))
            {
                if ((r & 128) == 0) { r = (Byte)(r + 96); }
                else { r = (Byte)(r - 96); }
            }

            if ((r & 64) != ((r & 4) << 4))
            {
                if ((r & 64) == 0) { r = (Byte)(r + 60); }
                else { r = (Byte)(r - 60); }
            }

            return r;
        }


       
        public static string sfn_cTPTransform(string aString)
        {
            int L, P;
            String a1, a3, a2, a6, newString;
            String rString;

            L = ((String)aString).Length;
            rString = (String)aString;
            P = 0;
            newString = "";
            while (P * 8 < L)
            {

                a1 = rString.Substring(P * 8 + 1, 1);
                a3 = rString.Substring(P * 8 + 3, 1);
                a2 = rString.Substring(P * 8 + 2, 1);
                a6 = rString.Substring(P * 8 + 6, 1);

                newString = newString + a3 + a6 + a1 + rString.Substring(P * 8 + 4, 2) + a2 + rString.Substring(P * 8 + 7, 2);

                P++;
            }


            if (L - (P - 1) * 8 >= 3)
            {
                a1 = rString.Substring((P - 1) * 8 + 1, 1);
                a3 = rString.Substring((P - 1) * 8 + 3, 1);
                newString = newString + a3 + rString.Substring((P - 1) * 8 + 2, 1) + a1 + rString.Substring((P - 1) * 8 + 4, 3);
                return newString;

            }

            //new SqlString((string)cmd.ExecuteScalar());
            return newString;
        }

       
        public static Int64 RandomBigint(Int64 Seed)
        {
            int k = 30393;
            int j = 18010;
            Int64 x, y, r1, r2, r;

            x = (Int64)Seed / 65536;
            y = (Int64)Seed & 65535;

            if (Seed == 0)
            { x = 65535; y = 65535; }

            r1 = (k * (x & 65535) + x / 65536) & 65535;
            r2 = (j * (y & 65535) + y / 65536) & 65535;
            r = r1 * 65536 + @r2;
            return  r;

        }
    }
}
