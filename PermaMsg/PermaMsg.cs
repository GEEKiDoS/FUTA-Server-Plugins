namespace PermaMsg
{
    using InfinityScript;
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml;

    public class PermaMsg : BaseScript
    {
        private string CanScroll;
        private string defaultxt;
        private HudElem info;
        private int ScrollSpeed;
        private float TBS;
        private int TBX;
        private int TBY;
        private float TLS;
        private int TLX;
        private int TLY;
        private float TRS;
        private int TRX;
        private int TRY;
        private float TTS;
        private int TTX;
        private int TTY;
        private string TxtBottom;
        private string TxtLeft;
        private string TxtRight;
        private string TxtTop;

        public PermaMsg()
        {
            Func<bool> function = null;
            this.ScrollSpeed = 30;
            this.TTS = 0.5f;
            this.TBS = 0.5f;
            this.TRS = 0.5f;
            this.TLS = 0.5f;
            this.TRX = -5;
            this.TLX = 6;
            this.TTY = 5;
            this.TBY = -5;
            this.TRY = 5;
            this.TLY = 0x69;
            string path = Environment.CurrentDirectory + @"\scripts\PermaMsg.xml";
            if (!File.Exists(path))
            {
                string[] contents = new string[] {
                    "<?xml version=\"1.0\" encoding=\"utf-8\"?>", "<Settings>", "   <Settings ID=\"TOP\" text=\"^5Top ^2Message\" size=\"0.5\" XPOS=\"0\" YPOS=\"5\" />", "   <Settings ID=\"BOTTOM\" text=\"^5Created By ^2T0T3NK0PF ^5aka ^2Banshee10000 ^5for ^1TeknoMW3\" size=\"0.5\" XPOS=\"0\" YPOS=\"-5\" />", "   <Settings ID=\"RIGHT\" text=\"^5Right ^2Message\" size=\"0.5\" XPOS=\"-5\" YPOS=\"5\" />", "   <Settings ID=\"LEFT\" text=\"^5Left ^2Message\" size=\"0.5\" XPOS=\"6\" YPOS=\"105\" />", "   <Settings ID=\"SCROLL\" OPTION=\"Yes\" SPEED=\"30\" />", "</Settings>", "<!-- INFORMATION", "TOP, BOTTOM, LEFT, AND RIGHT are locations on the screen where text will be placed", "Text is the message you want to Display, Colour codes are supported ^1 - ^9", "ADVANCE FEATURE XPOS AND YPOS OF TEXT PLACEMENT", "XPOS and YPOS are Advance settings that will move the text on the screen to Offset Locations from the Primary Nodes", "On XPOS and YPOS Both Negative and Positive Values are used, Experiment to get this Right Positions for you", "If your Text Doesn't Display, you've moved the Offset too Far. Try a Different Location with Offsets ", "XPOS = Left and Right YPOS = Up and Down. Negative Values go in the other Direction, Depending on Primary Location",
                    "SCROLL OPTION Allows you to either have scrolling text at the bottom or have it Static. Usage Yes / No / True / False", "SCROLL OPTION Will not Use any X/Y Position Settings or custom Font Sizes. BOTTOM TEXT MUST BE SET TO USE", "END -->"
                };
                File.WriteAllLines(path, contents);
                Log.Write(InfinityScript.LogLevel.Info, "Default PermaMsg.xml file was created");
            }
            if (File.Exists(path))
            {
                try
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(path);
                    XmlNode node = document.DocumentElement.SelectSingleNode("//Settings[@ID = 'TOP']/@text");
                    XmlNode node2 = document.DocumentElement.SelectSingleNode("//Settings[@ID = 'BOTTOM']/@text");
                    XmlNode node3 = document.DocumentElement.SelectSingleNode("//Settings[@ID = 'RIGHT']/@text");
                    XmlNode node4 = document.DocumentElement.SelectSingleNode("//Settings[@ID = 'LEFT']/@text");
                    XmlNode node5 = document.DocumentElement.SelectSingleNode("//Settings[@ID = 'SCROLL']/@OPTION");
                    XmlNode node6 = document.DocumentElement.SelectSingleNode("//Settings[@ID = 'SCROLL']/@SPEED");
                    XmlNode node7 = document.DocumentElement.SelectSingleNode("//Settings[@ID = 'TOP']/@size");
                    XmlNode node8 = document.DocumentElement.SelectSingleNode("//Settings[@ID = 'BOTTOM']/@size");
                    XmlNode node9 = document.DocumentElement.SelectSingleNode("//Settings[@ID = 'RIGHT']/@size");
                    XmlNode node10 = document.DocumentElement.SelectSingleNode("//Settings[@ID = 'LEFT']/@size");
                    XmlNode node11 = document.DocumentElement.SelectSingleNode("//Settings[@ID = 'TOP']/@XPOS");
                    XmlNode node12 = document.DocumentElement.SelectSingleNode("//Settings[@ID = 'BOTTOM']/@XPOS");
                    XmlNode node13 = document.DocumentElement.SelectSingleNode("//Settings[@ID = 'RIGHT']/@XPOS");
                    XmlNode node14 = document.DocumentElement.SelectSingleNode("//Settings[@ID = 'LEFT']/@XPOS");
                    XmlNode node15 = document.DocumentElement.SelectSingleNode("//Settings[@ID = 'TOP']/@YPOS");
                    XmlNode node16 = document.DocumentElement.SelectSingleNode("//Settings[@ID = 'BOTTOM']/@YPOS");
                    XmlNode node17 = document.DocumentElement.SelectSingleNode("//Settings[@ID = 'RIGHT']/@YPOS");
                    XmlNode node18 = document.DocumentElement.SelectSingleNode("//Settings[@ID = 'LEFT']/@YPOS");
                    this.TxtTop = node.Value;
                    this.TxtBottom = node2.Value;
                    this.TxtRight = node3.Value;
                    this.TxtLeft = node4.Value;
                    this.CanScroll = node5.Value;
                    this.ScrollSpeed = Convert.ToInt32(node6.Value);
                    this.TTS = Convert.ToSingle(node7.Value);
                    this.TBS = Convert.ToSingle(node8.Value);
                    this.TRS = Convert.ToSingle(node9.Value);
                    this.TLS = Convert.ToSingle(node10.Value);
                    this.TTX = Convert.ToInt16(node11.Value);
                    this.TBX = Convert.ToInt16(node12.Value);
                    this.TRX = Convert.ToInt16(node13.Value);
                    this.TLX = Convert.ToInt16(node14.Value);
                    this.TTY = Convert.ToInt16(node15.Value);
                    this.TBY = Convert.ToInt16(node16.Value);
                    this.TRY = Convert.ToInt16(node17.Value);
                    this.TLY = Convert.ToInt16(node18.Value);
                }
                catch (Exception exception)
                {
                    Log.Write(InfinityScript.LogLevel.Error, "Error in the PermaMsg.xml Settings file \n\n" + exception);
                    this.defaultxt = "^6FUTA ^3Server";
                }
            }
            if (this.TxtTop != "")
            {
                this.info = HudElem.CreateServerFontString("objective", this.TTS);
                this.info.SetPoint("TOPCENTER", "TOPCENTER", this.TTX, this.TTY);
                this.info.HideWhenInMenu = true;
                this.info.SetText(this.TxtTop);
            }
            if (new string[] { "Yes", "yes", "True", "true" }.Contains<string>(this.CanScroll) && (this.TxtBottom != ""))
            {
                this.info = HudElem.CreateServerFontString("default", 1f);
                this.info.SetPoint("CENTER", "BOTTOM", 0, -5);
                this.info.Foreground = true;
                this.info.HideWhenInMenu = true;
                if (function == null)
                {
                    function = delegate {
                        this.info.SetText(this.TxtBottom);
                        this.info.SetPoint("CENTER", "BOTTOM", 0x44c, -5);
                        this.info.Call("moveovertime", new Parameter[] { this.ScrollSpeed });
                        this.info.X = -700f;
                        return true;
                    };
                }
                base.OnInterval(0x7530, function);
            }
            else if (this.TxtBottom != "")
            {
                this.info = HudElem.CreateServerFontString("default", this.TBS);
                this.info.SetPoint("BOTTOMCENTER", "BOTTOMCENTER", this.TBX, this.TBY);
                this.info.HideWhenInMenu = true;
                this.info.SetText(this.TxtBottom);
            }
            if (this.TxtRight != "")
            {
                this.info = HudElem.CreateServerFontString("default", this.TRS);
                this.info.SetPoint("TOPRIGHT", "TOPRIGHT", this.TRX, this.TRY);
                this.info.HideWhenInMenu = true;
                this.info.SetText(this.TxtRight);
            }
            if (this.TxtLeft != "")
            {
                this.info = HudElem.CreateServerFontString("default", this.TLS);
                this.info.SetPoint("TOPLEFT", "TOPLEFT", this.TLX, this.TLY);
                this.info.HideWhenInMenu = true;
                this.info.SetText(this.TxtLeft);
            }
            if (((this.TxtTop == "") && (this.TxtBottom == "")) && ((this.TxtRight == "") && (this.TxtLeft == "")))
            {
                this.defaultxt = "^6FUTA ^3Server";
            }
            if (this.defaultxt != "")
            {
                this.info = HudElem.CreateServerFontString("default", 1f);
                this.info.SetPoint("TOPCENTER", "TOPCENTER", 0, 5);
                this.info.HideWhenInMenu = true;
                this.info.SetText(this.defaultxt);
            }
        }
    }
}
