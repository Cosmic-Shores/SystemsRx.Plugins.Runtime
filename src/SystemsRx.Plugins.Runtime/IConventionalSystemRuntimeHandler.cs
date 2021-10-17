using System;
using SystemsRx.Systems;

namespace SystemsRx.Plugins.Runtime {
    public interface IConventionalSystemRuntimeHandler : IDisposable {
        bool CanHandleSystem(ISystem system);
        void SetupSystem(ISystemRuntime runtime, ISystem system);
        void DestroySystem(ISystemRuntime runtime, ISystem system);
    }
}
