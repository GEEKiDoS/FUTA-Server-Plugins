using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace BiliBili
{
    public class BiliBili : BaseScript
    {
        public Random rng = new Random();

        public void NormalDanmaku(string text)
        {
            HudElem danmaku = HudElem.CreateServerFontString("bigfixed", 1f);
            int Y = 10 + rng.Next(1, 24) * 20;
            danmaku.SetPoint("center", "top", 900 + text.Length * 2, Y);
            danmaku.SetText(text);
            danmaku.Color = new Vector3((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble());
            danmaku.HideWhenInMenu = true;
            danmaku.Call("moveovertime", 10);
            danmaku.SetPoint("center", "top", -700 - (text.Length * 2), Y);
            AfterDelay(10000, () =>
            {
                danmaku.Call("destroy");
            });
        }

        public override EventEat OnSay3(Entity player, ChatType type, string name, ref string message)
        {
            try
            {
                if (!message.StartsWith("!"))
                {
                    if (type != ChatType.Team)
                    {
                        NormalDanmaku(message);
                    }
                }
            }
            catch (Exception)
            {
                return EventEat.EatNone;
            }
            return EventEat.EatNone;
        }
    }
}
