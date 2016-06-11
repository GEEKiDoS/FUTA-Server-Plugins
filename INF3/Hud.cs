﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfinityScript;

namespace Exo
{
    public static class Hud
    {
        public static HudElem GamblerText(this Entity player, string text, Vector3 color, Vector3 glowColor, float intensity, float glowIntensity)
        {
            var hud = HudElem.CreateFontString(player, "hudbig", 2);
            hud.SetPoint("CENTERMIDDLE", "CENTERMIDDLE", 0, 0);
            hud.SetText(text);
            hud.Color = color;
            hud.GlowColor = glowColor;
            hud.Alpha = 0;
            hud.GlowAlpha = glowIntensity;

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

            player.AfterDelay(4000, ent =>
            {
                hud.ChangeFontScaleOverTime(0.25f, 2f);
                hud.Call("FadeOverTime", 0.25f);
                hud.Alpha = 0;
            });

            return hud;
        }

        public static HudElem PerkHud(this Entity player, string shader, Vector3 color, string text)
        {
            player.Call("setblurforplayer", 6, 0.5f);
            int perksAmount = player.GetField<int>("aiz_perks") - 1;
            int MultiplyTimes = 28 * perksAmount;

            var hudtext = HudElem.NewClientHudElem(player);
            hudtext.AlignX = "center";
            hudtext.VertAlign = "middle";
            hudtext.AlignY = "middle";
            hudtext.HorzAlign = "center";
            hudtext.Font = "objective";
            hudtext.FontScale = 1.5f;
            hudtext.X = 0;
            hudtext.Y = 0;
            hudtext.Foreground = true;
            hudtext.Color = color;
            hudtext.Alpha = 0;
            hudtext.SetText(text);

            var hudshader = HudElem.NewClientHudElem(player);
            hudshader.AlignX = "center";
            hudshader.VertAlign = "middle";
            hudshader.AlignY = "middle";
            hudshader.HorzAlign = "center";
            hudshader.X = 0;
            hudshader.Y = 0;
            hudshader.Foreground = true;
            hudshader.SetShader(shader, 30, 30);
            hudshader.Alpha = 1;

            player.AfterDelay(300, e =>
            {
                hudshader.Call("moveovertime", 0.5f);
                hudshader.X = -200;
            });
            player.AfterDelay(700, e =>
            {
                player.Call("setblurforplayer", 0, 0.3f);
                hudtext.Alpha = 1;
            });
            player.AfterDelay(3700, e =>
            {
                hudtext.Call("fadeovertime", 0.25f);
                hudtext.Alpha = 0;
                hudshader.Call("scaleovertime", 1, 25, 25);
                hudshader.Call("moveovertime", 1);
                hudshader.X = -410 + MultiplyTimes;
                hudshader.Y = -187;
            });
            player.AfterDelay(4700, e => 
            {
                hudtext.Call("destroy");
            });

            return hudshader;
        }
    }
}