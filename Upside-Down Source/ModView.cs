using System;
using System.Collections.Generic;
using System.Text;
using ComputerInterface;
using ComputerInterface.ViewLib;
using UnityEngine;

namespace GravitySwitch 
{
    class ModView : ComputerView
    {
        public static ModView instance;
        private readonly UISelectionHandler selectionHandler;
        const string color = "01E6CB";


        public ModView()
        {
            instance = this;

            selectionHandler = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down, EKeyboardKey.Enter);

            selectionHandler.MaxIdx = 0;

            selectionHandler.OnSelected += OnEntrySelected;

            selectionHandler.ConfigureSelectionIndicator($"<color=#{color}>></color> ", "", "  ", "");
        }

        public override void OnShow(object[] args)
        {
            base.OnShow(args);


            UpdateScreen();
        }

        public void UpdateScreen()
        {
            SetText(str =>
            {
                str.BeginCenter();
                str.MakeBar('-', SCREEN_WIDTH, 0, "ffffff10");
                str.AppendClr("Upside Down", color).EndColor().AppendLine();
                str.AppendLine("By Rass1010");
                str.MakeBar('-', SCREEN_WIDTH, 0, "ffffff10");
                str.EndAlign().AppendLines(1);

                str.AppendLine(selectionHandler.GetIndicatedText(0, $"<color={(Plugin.instance.modEnabled ? string.Format("#{0}>[Enabled]", color) : "white>[Disabled]")}</color>"));
            });
        }

        private void OnEntrySelected(int index)
        {
            try
            {
                switch (index)
                {
                    case 0:
                        Plugin.instance.ToggleMod();
                        UpdateScreen();
                        break;
                }
            }
            catch (Exception e) { Debug.Log(e.ToString()); }
        }

        public override void OnKeyPressed(EKeyboardKey key)
        {
            if (selectionHandler.HandleKeypress(key))
            {
                UpdateScreen();
                return;
            }

            switch (key)
            {
                case EKeyboardKey.Back:
                    ReturnToMainMenu();
                    break;
            }
        }



    }
}
