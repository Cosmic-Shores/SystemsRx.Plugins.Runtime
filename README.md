# About
A [SystemsRx](https://github.com/EcsRx/SystemsRx) Plugin that provides a base notion for implementing a plugin to control when a system is started and stopped with ease.

## Building a plugin using this
- Create a class implementing IConventionalSystemRuntimeHandler
- Create a class implementing ISystemsRxPlugin and Call `container.Bind<IConventionalSystemRuntimeHandler, ManualStartupSystemHandler>()` in SetupDependencies()
- To use your new plugin make sure to load both the SystemsRxPlugin class you created above as well as SystemRuntimePlugin
