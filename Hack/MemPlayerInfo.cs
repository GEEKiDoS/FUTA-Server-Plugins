using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using InfinityScript;

namespace SATm
{
    public static class MemPlayerInfo
    {
        public static readonly IntPtr ClantagOffset = (IntPtr)0x01AC5564;
        public static readonly IntPtr TitleOffset = (IntPtr)0x01AC5548;

        public static readonly int PlayerDataSize2 = 0x38A4;

        public static string ReadClantag(this Entity player)
        {
            return Marshal.PtrToStringAnsi(ClantagOffset + PlayerDataSize2 * player.EntRef);
        }

        public static string ReadTitle(this Entity player)
        {
            return Marshal.PtrToStringAnsi(TitleOffset + PlayerDataSize2 * player.EntRef);
        }

        public static void WriteClantag(this Entity player, string newclantag)
        {
            if (newclantag.Length > 4)
            {
                throw new Exception("Clantag Too Long");
            }
            Marshal.Copy(Encoding.ASCII.GetBytes(newclantag), 0, ClantagOffset + PlayerDataSize2 * player.EntRef, newclantag.Length);
        }

        public static void WriteTitle(this Entity player, string newtitle)
        {
            if (newtitle.Length > 25)
            {
                throw new Exception("Title Too Long");
            }
            Marshal.Copy(Encoding.ASCII.GetBytes(newtitle), 0, TitleOffset + PlayerDataSize2 * player.EntRef, newtitle.Length);
        }
    }
}
