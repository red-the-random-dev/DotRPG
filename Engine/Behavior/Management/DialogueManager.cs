using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

using UI_DICT = System.Collections.Generic.Dictionary<string, DotRPG.UI.UserInterfaceElement>;
using SCR_EXEC = DotRPG.Behavior.LogicEventHandler<System.String>;

namespace DotRPG.Behavior.Management
{
    public class DialogueManager
    {
        protected SCR_EXEC ReelMethod;

        protected UI_DICT UIElements;
        protected String BoxName = "";
        protected String TextName = "";
        protected Byte ScrollKey = 4;
        protected Byte FastScrollKey = 5;
        protected Byte SkipKey = 6;
        protected Int32 LetsPerSecond_Normal = 32;
        protected Int32 LetsPerSecond_Alt = 64;

        protected Boolean CanScrollManually = true;
        protected Boolean CanFastScrollManually = true;
        protected Boolean CanSkip = true;

        protected Boolean AutoScroll = false;
        protected Boolean Continue = false;

        protected String CurrentReel = "";
        protected Boolean Active = false;
        protected Boolean ConditionlessSkip = false;
        protected Boolean ReelStart = false;

        public DialogueManager(UI_DICT elements, SCR_EXEC reelFetch)
        {
            if (elements == null)
            {
                throw new ArgumentNullException("Element collection parameter is set to NULL.");
            }
            UIElements = elements;
            ReelMethod = reelFetch;
        }

        public void SetScrollKey(Int32 k)
        {
            ScrollKey = (Byte)Math.Clamp(k, 0, 255);
        }
        public void SetFastScrollKey(Int32 k)
        {
            FastScrollKey = (Byte)Math.Clamp(k, 0, 255);
        }
        public void SetSkipKey(Int32 k)
        {
            SkipKey = (Byte)Math.Clamp(k, 0, 255);
        }
        public void SetTextBoxName(String name)
        {
            if (UIElements.ContainsKey(name) && name != TextName)
            {
                BoxName = name;
            }
        }
        public void SetTextLineName(String name)
        {
            if (UIElements.ContainsKey(name) && name != BoxName)
            {
                if (UIElements[name] is UI.TextObject)
                    TextName = name;
            }
        }
        public void SetFlags(Boolean canScroll, Boolean canFastScroll = true, Boolean canSkip = true)
        {
            CanScrollManually = canScroll;
            CanFastScrollManually = canFastScroll;
            CanSkip = canSkip;
        }

        public void SetNormalSpeed(Int32 speed)
        {
            LetsPerSecond_Normal = Math.Clamp(speed, 0, Int32.MaxValue);
        }
        public void SetAltSpeed(Int32 speed)
        {
            LetsPerSecond_Alt = Math.Clamp(speed, 0, Int32.MaxValue);
        }
        public void SkipTextBox()
        {
            ConditionlessSkip = true;
        }
        public void Eject()
        {
            ConditionlessSkip = true;
            CurrentReel = "";
        }
        public void Abort()
        {
            CurrentReel = "";
        }
        public void Insert(String reel)
        {
            if (reel != CurrentReel)
            {
                CurrentReel = reel;
                ReelStart = true;
            }
        }
        public void Show(String text, Boolean autoScroll = false, Boolean goOn = false)
        {
            if (!UIElements.ContainsKey(TextName) || TextName == "")
            {
                return;
            }
            Active = true;
            UI.UserInterfaceElement uie = UIElements[TextName];
            if (uie is UI.TextObject to)
            {
                to.Text = text;
                to.ResetToStart();
            }
            AutoScroll = autoScroll;
            Continue = goOn;
        }

        public void Update(GameTime gameTime, ControlInput ctrl)
        {
            if (BoxName == "" || !UIElements.ContainsKey(BoxName) || TextName == "" || !UIElements.ContainsKey(TextName))
            {
                return;
            }
            else if (!(UIElements[TextName] is UI.TextObject))
            {
                return;
            }
            UI.UserInterfaceElement uif = UIElements[BoxName];
            UI.TextObject to = UIElements[TextName] as UI.TextObject;
            if (CanFastScrollManually && ctrl[ScrollKey])
            {
                to.ScrollDelay = 1.0f / LetsPerSecond_Alt;
            }
            else
            {
                to.ScrollDelay = 1.0f / LetsPerSecond_Normal;
            }
            if (Active)
            {
                uif.Visible = true;
                uif.Updated = true;
                if (CanFastScrollManually && ctrl.KeyPressed(FastScrollKey))
                {
                    to.SkipToEnd();
                }
                if ((to.ReachedEnd && ((ctrl.KeyPressed(ScrollKey) && CanScrollManually) || AutoScroll)) || CanSkip && ctrl.KeyPressed(SkipKey) || ConditionlessSkip)
                {
                    Active = false;
                    ConditionlessSkip = false;
                    if (Continue && CurrentReel != "")
                    {
                        ReelMethod(this, CurrentReel, gameTime);
                    }
                }
            }
            else if (ReelStart)
            {
                ReelStart = false;
                ReelMethod(this, CurrentReel, gameTime);
            }
            else
            {
                uif.Visible = false;
                uif.Updated = false;
            }
        }
    }
}
