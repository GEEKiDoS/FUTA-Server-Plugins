using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF3
{
    public static class BoxFunction
    {
        #region Cash Point System

        public static int GetCash(this Entity player)
        {
            return player.GetField<int>("aiz_cash");
        }

        public static void WinCash(this Entity player, int cash)
        {
            player.SetField("aiz_cash", player.GetField<int>("aiz_cash") + cash);
        }

        public static void PayCash(this Entity player, int cash)
        {
            player.SetField("aiz_cash", player.GetField<int>("aiz_cash") - cash);
        }

        public static int GetPoint(this Entity player)
        {
            return player.GetField<int>("aiz_point");
        }

        public static void WinPoint(this Entity player, int point)
        {
            player.SetField("aiz_point", player.GetField<int>("aiz_cash") + point);
        }

        public static void PayPoint(this Entity player, int point)
        {
            player.SetField("aiz_point", player.GetField<int>("aiz_cash") - point);
        }

        #endregion


    }
}
