using SystemsRx.Systems;

namespace SystemsRx.Plugins.Runtime {
    public interface ISystemRuntime {
        SystemState GetSystemState(ISystem system);
        void StartSystem(ISystem system);
        void DestroySystem(ISystem system);
    }
}
