using System;
using System.Collections;

namespace LogicGates
{
    public static class Logic
    {
        public static bool not(bool b0)
        {
            // b0 | q
            // 0  | 1
            // 1  | 0
            bool q = !b0;
            //Console.WriteLine($"!{b0}=>{q}");
            return q;
        }

        public static bool and(bool b0, bool b1)
        {
            // b0 b1 | q
            // 0  0  | 0
            // 0  1  | 0
            // 1  0  | 0
            // 1  1  | 1
            bool q = b0 && b1;
            //Console.WriteLine($"{b0}&{b1}=>{q}");
            return q;
        }

        public static bool nand(bool b0, bool b1)
        {
            // b0 b1 | q
            // 0  0  | 1
            // 0  1  | 1
            // 1  0  | 1
            // 1  1  | 0
            return not(and(b0, b1));
        }

        public static bool or(bool b0, bool b1)
        {
            // b0 b1 | q
            // 0  0  | 0
            // 0  1  | 1
            // 1  0  | 1
            // 1  1  | 1
            return nand(not(b0), not(b1));
        }

        public static bool nor(bool b0, bool b1)
        {
            // b0 b1 | q
            // 0  0  | 1
            // 0  1  | 0
            // 1  0  | 0
            // 1  1  | 0
            return not(or(b0, b1));
        }

        public static bool xor(bool b0, bool b1)
        {
            // b0 b1 | q
            // 0  0  | 0
            // 0  1  | 1
            // 1  0  | 1
            // 1  1  | 0
            return and(or(b0, b1), nand(b0, b1));
        }

        public static bool xnor(bool b0, bool b1)
        {
            // b0 b1 | q
            // 0  0  | 1
            // 0  1  | 0
            // 1  0  | 0
            // 1  1  | 1
            return or(and(b0,b1), nor(b0,b1));
        }

        /// <summary>
        /// 1bit Adder (Full)
        /// </summary>
        public static void add(bool b0, bool b1, ref bool q, ref bool cb)
        {
            // b0 b1 cin | q  cout
            // 0  0  0   | 0  0
            // 0  1  0   | 1  0
            // 1  0  0   | 1  0
            // 1  1  0   | 0  1
            // 0  0  1   | 1  0
            // 0  1  1   | 0  1
            // 1  0  1   | 0  1
            // 1  1  1   | 1  1
            bool t = xor(b0, b1);
            q = xor(t, cb);
            cb = or(and(b0, b1), and(t, cb));
        }
    }

    public static class Processor
    {
        public static void alu(bool[] a, bool[] b, ref bool[] q, bool substract,
                                ref bool cb, ref bool nf, ref bool zf)
        {
            int numBits = q.Length;
            if (a.Length != b.Length || a.Length != q.Length || numBits == 0)
                throw new ArgumentOutOfRangeException();

            cb = substract;
            zf = true;
            for (int i = 0; i < numBits; i++)
            {
                b[i] = Logic.xor(b[i], substract);
                Logic.add(a[i], b[i], ref q[i], ref cb);
                if (i > 0 && i%2 == 0)
                {
                    zf = Logic.and(zf, Logic.nor(q[i-1], q[i]));
                }
            }

            nf = q[numBits-1];
        }
    }

    class Program
    {

        // gates - done
        // adder - done
        // alu
        // latch
        // flip-flop
        // register
        // memory
        // tri-state
        // bus
        static void Main(string[] args)
        {
            const int bitsCount = 64;
            var r = new bool[bitsCount];
            bool cb = false, zf = false, nf = false;
            Processor.alu(toBits(bitsCount, -6917529027641081856), toBits(bitsCount, 6917529027641081857), ref r, true, ref cb, ref nf, ref zf);
            Console.WriteLine($"r:{fromBits(r)} c:{cb} n:{nf} z:{zf}");
        }

        static bool[] toBits(int numBits, long value)
        {
            if (numBits > (sizeof(long)*8))
                throw new ArgumentOutOfRangeException("numBits");
            bool[] r = new bool[numBits];
            if (value > 0)
            {
                for (int i = 0; i < numBits; i++)
                    r[i] = (value & ((long)1 << i)) > 0;
            }
            else
            {
                r[numBits - 1] = true;
                long v = value + ((long)1 << (numBits - 1));
                for (int i = 0; i < numBits - 1; i++)
                    r[i] = (v & ((long)1 << i)) > 0;

            }
            return r;
        }

        static long fromBits(bool[] bits)
        {
            if (bits.Length > (sizeof(long)*8))
                throw new ArgumentOutOfRangeException("bits.Length");
            long r = 0;
            for (int i = 0; i < bits.Length - 1; i++)
            {
                r = r | ((long)(bits[i] ? 1 : 0) << i);
            }
            if (bits[bits.Length-1])
            {
                r -= ((long)1 << (bits.Length - 1));
            }
            return r;
        }
    }
}
