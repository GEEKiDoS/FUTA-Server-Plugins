using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfinityScript;

namespace Sharpshooter
{
    public static class Hud
    {
        public static void GamblerText(this Entity player, string text, Vector3 color, Vector3 glowColor, float intensity, float glowIntensity)
        {
            var hud = HudElem.CreateFontString(player, "hudbig", 2);
            hud.SetPoint("CENTERMIDDLE", "CENTERMIDDLE", 0, 0);
            hud.SetText(text);
            hud.Color = color;
            hud.GlowColor = glowColor;
            hud.Alpha = 0;
            hud.GlowAlpha = glowIntensity;
            player.SetField("hud_gamblertext", new Parameter(hud));

            hud.ChangeFontScaleOverTime(0.25f, 0.75f);
            hud.Call("FadeOverTime", 0.25f);
            hud.Alpha = intensity;

            player.AfterDelay(250, ent => player.Call("playLocalSound", "mp_bonus_end"));

            player.AfterDelay(3000, ent =>
            {
                hud.ChangeFontScaleOverTime(0.25f, 2f);
                hud.Call("FadeOverTime", 0.25f);
                hud.Alpha = 0;
            });

            player.AfterDelay(4000, ent => hud.Call("destroy"));
        }
    }
}
