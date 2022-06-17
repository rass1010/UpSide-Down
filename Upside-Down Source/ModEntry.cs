using System;
using ComputerInterface.Interfaces;


namespace GravitySwitch
{
    class ModEntry : IComputerModEntry
    {
        public string EntryName => "Upside down";

        // This is the first view that is going to be shown if the user select you mod
        // The Computer Interface mod will instantiate your view 
        public Type EntryViewType => typeof(ModView);
    }
}
