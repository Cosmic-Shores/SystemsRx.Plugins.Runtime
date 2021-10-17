using System;
using SystemsRx.Systems;

namespace EcsRx.Plugins.Bootstrap {
    /// <summary>
    /// Delays the startup of other aspects of this system until an Observable emits.
    /// </summary>
    public interface IManualStartupSystem<T> : ISystem {
        /// <summary>
        /// Run when the system has been registered.
        /// </summary>
        IObservable<T> Ready();
    }
}
