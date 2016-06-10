using System;
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

        public static void ExplodePredatorStaticEffect(Entity self, float duration)
        {
            HudElem hudElem = HudElem.NewClientHudElem(self);
            hudElem.HorzAlign = "fullscreen";
            hudElem.VertAlign = "fullscreen";
            hudElem.SetShader("white", 640, 480);
            hudElem.Archived = true;
            hudElem.Sort = 10;
            HudElem hudElem1 = HudElem.NewClientHudElem(self);
            hudElem1.HorzAlign = "fullscreen";
            hudElem1.VertAlign = "fullscreen";
            hudElem1.SetShader("ac130_overlay_grain", 640, 480);
            hudElem1.Archived = true;
            hudElem1.Sort = 20;
            self.AfterDelay(Convert.ToInt32(duration * 1000f), (l) =>
            {
                hudElem.Call("destroy");
                hudElem1.Call("destroy");
            });
        }

        public static void UpLink(Entity self, float duration)
        {
            HudElem darkScreenOverlay = HudElem.NewClientHudElem(self);
            darkScreenOverlay.X = 0;
            darkScreenOverlay.Y = 0;
            darkScreenOverlay.AlignX = "left";
            darkScreenOverlay.AlignY = "top";
            darkScreenOverlay.VertAlign = "fullscreen";
            darkScreenOverlay.HorzAlign = "fullscreen";
            darkScreenOverlay.SetShader("black", 640, 480);
            darkScreenOverlay.Sort = 20;
            darkScreenOverlay.Alpha = 1.0f;
            self.AfterDelay(Convert.ToInt32(duration * 1000f), (l) =>
            {
                darkScreenOverlay.Call("fadeOverTime", 1.5f);
                darkScreenOverlay.Alpha = 0.0f;
            });
        }

        public static HudElem DropCrateIconScreen(Entity self, Vector3 origin, string shader)
        {
            HudElem icon = HudElem.NewTeamHudElem(self.GetTeam());
            icon.SetShader(shader, 20, 20);
            icon.X = origin.X;
            icon.Y = origin.Y;
            icon.Z = origin.Z + 20f;
            icon.Call("SetWayPoint", false, false);
            return icon;
        }

        public static HudElem Hud_timer(Entity self, int timeOut)
        {
            HudElem timer = HudElem.NewClientHudElem(self);
            timer.X = -100;
            timer.Y = 0;
            timer.AlignX = "right";
            timer.AlignY = "bottom";
            timer.VertAlign = "bottom_adjustable";
            timer.HorzAlign = "right_adjustable";
            timer.FontScale = 2.5f;
            timer.Alpha = 1.0f;
            timer.Call("settimer", timeOut);
            self.AfterDelay(timeOut * 1000, (q) =>
            {
                timer.Call("destroy");
            });
            return timer;
        }
    }
}
