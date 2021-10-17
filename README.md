# About
[![Build Status](https://github.com/Cosmic-Shores/SystemsRx.Plugins.Runtime/actions/workflows/publish.yml/badge.svg)](https://github.com/Cosmic-Shores/SystemsRx.Plugins.Runtime/actions)
[![License](https://badgen.net/github/license/Naereen/Strapdown.js)](https://github.com/Cosmic-Shores/SystemsRx.Plugins.Runtime/blob/main/LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)


## SystemsRx.Plugins.Runtime [![NuGet version](https://badgen.net/nuget/v/SystemsRx.Plugins.Runtime/latest)](https://nuget.org/packages/SystemsRx.Plugins.Runtime)

A [SystemsRx](https://github.com/EcsRx/SystemsRx) Plugin that provides a base notion for implementing a plugin to control when a system is started and stopped with ease.

### Building a plugin using this
- Create a class implementing IConventionalSystemRuntimeHandler
- Create a class implementing ISystemsRxPlugin and call `container.Bind<IConventionalSystemRuntimeHandler, MyHandler>()` in SetupDependencies()
- To use your new plugin make sure to load both the SystemsRxPlugin class you created above as well as SystemRuntimePlugin


## EcsRx.Plugins.Bootstrap [![NuGet version](https://badgen.net/nuget/v/EcsRx.Plugins.Bootstrap/latest)](https://nuget.org/packages/EcsRx.Plugins.Bootstrap)
A [SystemsRx](https://github.com/EcsRx/SystemsRx) Plugin to delay the start of a system until it is ready. Depends on the Runtime-Plugin.

### Basic usage
To use this plugin you have to load both these plugins in your application.
- SystemRuntimePlugin
- SystemBootstrapPlugin

The following snippet ilustrates the basic usage on a system using UniRx in the Ready-method.
```cs
using EcsRx.Plugins.Bootstrap;
using UniRx;
using System;
using SystemsRx.Systems.Conventional;

sealed class ExampleSystem : IManualSystem, IManualStartupSystem<long> {
    public IObservable<long> Ready() => Observable.EveryUpdate().Take(1);

    public void StartSystem() {
        // delayed start once ready has emited
    }

    public void StopSystem() {}
}
```
