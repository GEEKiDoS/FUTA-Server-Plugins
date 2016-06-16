using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF3
{
    public static class Hud
    {
        public static void CreateCashHud(this Entity player)
        {
            HudElem hud = HudElem.CreateFontString(player, "hudbig", 1f);
            hud.SetPoint("right", "right", -50, 100);
            hud.HideWhenInMenu = true;
            player.OnInterval(100, delegate (Entity ent)
            {
                if (player.GetTeam() == "allies")
                {
                    hud.SetText("^3$ ^7" + player.GetField<int>("aiz_cash"));
                }
                else
                {
                    hud.SetText("");
                }
                return player.IsPlayer;
            });
        }

        public static void CreatePointHud(this Entity player)
        {
            HudElem hud = HudElem.CreateFontString(player, "default", 1f);
            hud.SetPoint("right", "right", -50, 120);
            hud.HideWhenInMenu = true;
            player.OnInterval(100, delegate (Entity ent)
            {
                if (player.GetTeam() == "allies")
                {
                    hud.SetText("^5Bonus Points ^7" + player.GetField<int>("aiz_point"));
                }
                else
                {
                    hud.SetText("");
                }
                return player.IsPlayer;
            });
        }

        public static void GamblerText(this Entity player, string text, Vector3 color, Vector3 glowColor, float intensity, float glowIntensity)
        {
            HudElem hud;
            if (!player.HasField("gambletexthud"))
            {
                hud = HudElem.CreateFontString(player, "hudbig", 2);
                player.SetField("gambletexthud", new Parameter(hud));
            }
            else
            {
                hud = player.GetField<HudElem>("gambletexthud");
            }
            hud.Call("destroy");

            hud = HudElem.CreateFontString(player, "hudbig", 2);
            var entref = hud.Entity.EntRef;
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
                if (hud.Entity.EntRef == entref)
                {
                    hud.ChangeFontScaleOverTime(0.25f, 2f);
                    hud.Call("FadeOverTime", 0.25f);
                    hud.Alpha = 0;
                }
            });

            player.AfterDelay(4000, ent =>
            {
                if (hud.Entity.EntRef == entref)
                {
                    hud.ChangeFontScaleOverTime(0.25f, 2f);
                    hud.Call("FadeOverTime", 0.25f);
                    hud.Alpha = 0;
                }
            });
        }

        public static void WelcomeMessage(this Entity player, List<string> messages, Vector3 color, Vector3 glowColor, float intensity, float glowIntensity)
        {
            var list = new List<HudElem>();

            foreach (var item in messages)
            {
                player.AfterDelay((messages.IndexOf(item) + 1) * 500, e =>
                {
                    var hud = HudElem.CreateFontString(player, "objective", 1.5f);
                    hud.SetPoint("TOPMIDDLE", "TOPMIDDLE", 0, 45 + messages.IndexOf(item) * 15);
                    hud.FontScale = 6;
                    hud.Color = color;
                    hud.SetText(item);
                    hud.Alpha = 0;
                    hud.GlowColor = glowColor;
                    hud.GlowAlpha = glowIntensity;

                    hud.ChangeFontScaleOverTime(0.2f, 1.5f);
                    hud.Call("fadeovertime", 0.2f);
                    hud.Alpha = intensity;

                    list.Add(hud);
                });
            }
            player.AfterDelay(messages.Count * 500 + 4000, e =>
            {
                foreach (var item in list)
                {
                    player.AfterDelay((list.IndexOf(item) + 1) * 500, en =>
                    {
                        item.ChangeFontScaleOverTime(0.2f, 4.5f);
                        item.Call("fadeovertime", 0.2f);
                        item.Alpha = 0;
                    });
                }
            });
            player.AfterDelay(60000, e =>
            {
                foreach (var item in list)
                {
                    item.Call("destroy");
                }
            });
        }

        public static HudElem PerkHudNoEffect(this Entity player, string shader)
        {
            player.Call("setblurforplayer", 6, 0.5f);
            int perksAmount = player.GetField<int>("aiz_perks") - 1;
            int MultiplyTimes = 28 * perksAmount;

            var hudshader = HudElem.NewClientHudElem(player);
            player.AfterDelay(700, e =>
            {
                player.Call("setblurforplayer", 0, 0.3f);
                hudshader.AlignX = "center";
                hudshader.VertAlign = "middle";
                hudshader.AlignY = "middle";
                hudshader.HorzAlign = "center";
                hudshader.X = -410 + MultiplyTimes;
                hudshader.Y = 160;
                hudshader.Foreground = true;
                hudshader.SetShader(shader, 25, 25);
                hudshader.Alpha = 1;
            });

            return hudshader;
        }

        [Obsolete]
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
            hudshader.SetShader(shader, 25, 25);
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
                hudshader.Y = 160;
            });
            player.AfterDelay(4700, e =>
            {
                hudtext.Call("destroy");

            });

            return hudshader;
        }

        public static HudElem CreateRandomPerkHud(this Entity player)
        {
            var hudshader = HudElem.NewClientHudElem(player);
            hudshader.AlignX = "center";
            hudshader.VertAlign = "middle";
            hudshader.AlignY = "middle";
            hudshader.HorzAlign = "center";
            hudshader.X = 0;
            hudshader.Y = 0;
            hudshader.Foreground = true;
            hudshader.Alpha = 1;

            return hudshader;
        }

        public static void Credits(this Entity player)
        {
            HudElem credits = HudElem.CreateFontString(player, "hudbig", 1.0f);
            credits.SetPoint("CENTER", "BOTTOM", 0, -70);
            credits.Call("settext", "Project Cirno (INF3)");
            credits.Alpha = 0f;
            credits.SetField("glowcolor", new Vector3(1f, 0.5f, 1f));
            credits.GlowAlpha = 1f;

            HudElem credits2 = HudElem.CreateFontString(player, "hudbig", 0.6f);
            credits2.SetPoint("CENTER", "BOTTOM", 0, -90);
            credits2.Call("settext", "Vesion 0.2.1 Beta. Code in: https://github.com/A2ON");
            credits2.Alpha = 0f;
            credits2.SetField("glowcolor", new Vector3(1f, 0.5f, 1f));
            credits2.GlowAlpha = 1f;

            player.Call("notifyonplayercommand", "tab", "+scores");
            player.OnNotify("tab", entity =>
            {
                credits.Alpha = 1f;
                credits2.Alpha = 1f;
            });

            player.Call("notifyonplayercommand", "-tab", "-scores");
            player.OnNotify("-tab", entity =>
            {
                credits.Alpha = 0f;
                credits2.Alpha = 0f;
            });
        }

        public static void TextPopup(this Entity player, string text)
        {
            HudElem hud;

            if (!player.HasField("textpopup"))
            {
                player.SetField("textpopup", new Parameter(HudElem.NewClientHudElem(player)));
            }
            hud = player.GetField<HudElem>("textpopup");
            if (hud != null)
            {
                hud.Call("destroy");
            }

            hud = HudElem.CreateFontString(player, "hudbig", 0.8f);
            hud.SetPoint("BOTTOMCENTER", "BOTTOMCENTER", 0, -65);
            hud.SetText(text);
            hud.Alpha = 0.85f;
            hud.GlowColor = new Vector3(0.3f, 0.9f, 0.9f);
            hud.GlowAlpha = 0.55f;
            hud.Call("SetPulseFX", 100, 2100, 1000);
            hud.ChangeFontScaleOverTime(0.1f, 0.75f);
            player.AfterDelay(100, e =>
            {
                hud.ChangeFontScaleOverTime(0.1f, 0.65f);
            });
        }

        public static void TextPopup2(this Entity player, string text)
        {
            HudElem hud;

            if (!player.HasField("textpopup2"))
            {
                player.SetField("textpopup2", new Parameter(HudElem.NewClientHudElem(player)));
            }
            hud = player.GetField<HudElem>("textpopup2");
            if (hud != null)
            {
                hud.Call("destroy");
            }

            hud = HudElem.CreateFontString(player, "hudbig", 0.8f);
            hud.SetPoint("BOTTOMCENTER", "BOTTOMCENTER", 0, -105);
            hud.SetText(text);
            hud.Alpha = 0.85f;
            hud.GlowColor = new Vector3(0.3f, 0.9f, 0.9f);
            hud.GlowAlpha = 0.55f;
            hud.Call("SetPulseFX", 100, 3000, 1000);
            hud.ChangeFontScaleOverTime(0.1f, 0.75f);
            player.AfterDelay(100, e =>
            {
                hud.ChangeFontScaleOverTime(0.1f, 0.65f);
            });
        }

        private static void CreateRankHud(this Entity player)
        {
            var hud = HudElem.CreateFontString(player, "hudbig", 1);
            hud.SetPoint("BOTTOMCENTER", "BOTTOMCENTER", 0, -80);
            hud.Alpha = 0;
            hud.Color = new Vector3(0.5f, 0.5f, 0.5f);

            player.SetField("scorepopup", new Parameter(hud));
        }

        public static void ScorePopup(this Entity player, int amount, Vector3 hudcolor, float glowalpha)
        {
            if (amount == 0)
            {
                return;
            }
            if (player.HasField("scorepopup"))
            {
                var temphud = player.GetField<HudElem>("scorepopup");
                if (temphud != null)
                {
                    temphud.Call("destroy");
                }
            }

            player.CreateRankHud();

            player.SetField("xpUpdateTotal", player.GetField<int>("xpUpdateTotal") + amount);

            var hud = player.GetField<HudElem>("scorepopup");

            if (player.GetField<int>("xpUpdateTotal") < 0)
            {
                hud.Color = new Vector3(25.5f, 25.5f, 3.6f);
                hud.GlowColor = new Vector3(0.9f, 0.3f, 0.3f);
                hud.GlowAlpha = 0.55f;
            }
            else
            {
                hud.Color = new Vector3(25.5f, 25.5f, 3.6f);
                hud.GlowColor = new Vector3(0.3f, 0.9f, 0.3f);
                hud.GlowAlpha = 0.55f;
            }

            hud.SetText(player.GetField<int>("xpUpdateTotal") > 0 ? "+" : "" + player.GetField<int>("xpUpdateTotal").ToString());
            hud.Alpha = 1;
            hud.Call("SetPulseFX", 100, 3000, 1000);
            player.AfterDelay(3000, e =>
            {
                if (hud != null)
                {
                    player.SetField("xpUpdateTotal", 0);
                }
            });
        }
    }
}
