using ComputerInterface.Interfaces;
using Zenject;

namespace GravitySwitch
{
    class ModInstaller : Installer
    {
        public override void InstallBindings()
        {
            base.Container.Bind<IComputerModEntry>().To<ModEntry>().AsSingle();
        }
    }
}
