using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808Server.Utility.Common
{
    public class Tool
    {
        /// <summary>
        /// 车牌颜色转数字
        /// </summary>
        /// <param name="_color">车牌颜色</param>
        /// <returns></returns>
        public static int ColorToInt(string _color)
        {
            switch (_color)
            {
                case "蓝色":
                    return 1;
                case "黄色":
                    return 2;
                case "黑色":
                    return 3;
                case "白色":
                    return 4;
                case "绿色":
                    return 5;
                case "其它":
                    return 9;
                case "农黄色":
                    return 91;
                case "农绿色":
                    return 92;
                case "黄绿色":
                    return 6;
                case "渐变绿":
                    return 94;
                case "白绿色":
                    return 225;
                default:
                    return 9;
            }
        }

        public static byte Get_CheckXor(byte[] temp, int len)
        {
            byte A = 0;
            for (int i = 0; i < len; i++)
            {
                A = (byte)(A ^ temp[i]);
            }
            return A;
        }

        public static byte Get_CheckXorG(byte[] temp, int len)
        {
            byte A = 0;
            for (int i = 1; i < len; i++)
            {
                A = (byte)(A ^ temp[i]);
            }
            return A;
        }
        public static byte[] AssembleParkge(byte[] RecvAffirmBuff)
        {
            byte[] Respans = new byte[RecvAffirmBuff.Length + 13];
            Respans[0] = 40;
            Respans[1] = 40;
            Respans[2] = 254;
            byte[] Num = BitConverter.GetBytes(8 + RecvAffirmBuff.Length);
            if (Num.Length >= 2)
            {
                Respans[3] = Num[1];
            }
            if (Num.Length >= 1)
            {
                Respans[4] = Num[0];
            }
            Respans[5] = RecvAffirmBuff[5];
            Respans[6] = RecvAffirmBuff[6];
            Respans[7] = RecvAffirmBuff[7];
            Respans[8] = RecvAffirmBuff[8];
            Respans[9] = RecvAffirmBuff[9];
            Respans[10] = RecvAffirmBuff[10];
            for (int i = 0; i < RecvAffirmBuff.Length; i++)
            {
                Respans[11 + i] = RecvAffirmBuff[i];
            }
            Respans[Respans.Length - 2] = Get_CheckXor(Respans, Respans.Length - 2);
            Respans[Respans.Length - 1] = 12;
            return Respans;
        }

        public static byte[] FZY808(byte[] Rec)
        {
            int len = Rec.Length;
            for (int j = 1; j < Rec.Length - 1; j++)
            {
                if (Rec[j] == 125 && Rec[j + 1] == 1)
                {
                    len--;
                    j++;
                }
                else if (Rec[j] == 125 && Rec[j + 1] == 2)
                {
                    len--;
                    j++;
                }
            }
            byte[] Return = new byte[len];
            Return[0] = 126;
            Return[len - 1] = 126;
            len = 0;
            for (int i = 1; i < Rec.Length - 1; i++)
            {
                if (Rec[i] == 125 && Rec[i + 1] == 1)
                {
                    Return[i - len] = 125;
                    len++;
                    i++;
                }
                else if (Rec[i] == 125 && Rec[i + 1] == 2)
                {
                    Return[i - len] = 126;
                    len++;
                    i++;
                }
                else
                {
                    Return[i - len] = Rec[i];
                }
            }
            return Return;
        }

        public static string BytesToStr(byte[] buff)
        {
            return BytesToStr(buff, " ");
        }

        public static string BytesToStr(byte[] buff, string split)
        {
            string text = "";
            for (int i = 0; i < buff.Length; i++)
            {
                text = text + buff[i].ToString("X2") + split;
            }
            return text;
        }

    }
}
