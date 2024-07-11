[![openupm](https://img.shields.io/npm/v/com.bedtime.nest-builder?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.bedtime.nest-builder/)
# NestBuilder for Unity
A configurable build system for Unity that supports multiple configurations as assets, with each configuration able to inherit from other configurations.

![image](https://github.com/BedtimeDigitalGames/BuildPipeline/assets/1178324/7bc76a74-980f-4886-a5f6-bdd216e47dfd)

## Getting Started
After installing the package, you can access the build queue from the menu item "BedtimeCore/Build Queue"
From here you can build your configurations directly, or create new configuration assets.
Configuration assets live under "/Assets/EditorResources/Editor/BuildPipeline/Configurations"

![image](https://github.com/BedtimeDigitalGames/BuildPipeline/assets/1178324/d893e3d6-5067-4ab6-bbc6-aa3fa7b5a1d9)

## Configuring a build
Each configuration asset has options for most useful features to set for a build. You will ideally have one base configuration that supplies default values such as product name, company name, version, etc. 
After this, you can create configurations for more specific purposes such as platforms, demos, and publishers. 

Each configurable value has two important features besides its actual value; The override toggle to the left of the value, and the source parent button if not overriden.
You can only modify a value when its overridde toggle is active. If it is not active, the value will be greyed out and show the value from the nearest inherited configuration asset with its override toggled active for that value.

![image](https://github.com/BedtimeDigitalGames/BuildPipeline/assets/1178324/d7c068bf-f39a-4a75-8ea8-32e5563bfad2)

## Custom Build Settings
Additional settings can be serialized and configured for your build configurations by creating a class that implements the *ISettingsModule* interface.
The class must be marked as Serializable.

You can then access these values from any place where a *SerializedBuildConfiguration* is available, for example during a build step or custom editor code.
You must retrieve the instance of your module by calling *GetModule<T>* on your configuration. 

Your new module can then contain any amount of settings. Each setting can be one of the built in Setting types, or a custom type inheriting from *BuildSetting<T>*.
Some of the built in types are:
- BoolSetting : BuildSetting\<bool\>
- FloatSetting : BuildSetting\<float\>
- IntSetting : BuildSetting\<int\>
- SecretSetting : BuildSetting\<string\>
- EnumSetting : BuildSetting\<T\> where T : unmanaged, Enum
- PathSetting : BuildSetting\<string\>
- SceneSetting : BuildSetting\<string[]\>
- StringSetting : BuildSetting\<string\>
- Texture2DSetting : BuildSetting\<Texture2D\>
- Vector2Setting : BuildSetting\<Vector2\>

Most settings will accept a *System.Action\<T\>* in their constructor. If defined, this function will be executed after the **PreConfiguration** build step.
An example from the built in DebugSettingsModule:
```
  [Category("Debug")]
  public BoolSetting AutoConnectProfiler = new(x => UnityEditor.EditorUserBuildSettings.connectProfiler = x);
```

## Custom Build Processors / Build Steps
By inheriting from the interface *IBuildProcessor* you can add custom processing of a build to your configurations.
An implmentation must specify which Build Steps it should be available for (if any) and implement the method for processing the build
```
public interface IBuildProcessor
{
  BuildStep ValidBuildSteps { get; }
  Task<bool> ProcessBuild(Build build);
}
```
After this, if your build processor has defined any valid build steps, it will become available for selection in the "Build Step Methods" section of a configuration asset.

![image](https://github.com/BedtimeDigitalGames/BuildPipeline/assets/1178324/a8875cf2-5703-4bf8-986e-4e35d2117ccc)

## Customizing the build queue
By inheriting from the interface *IBuildQueueExtension* you can run custom IMGUI code for each displayed build in the queue window. 
This allows you do display custom information or add functionality to your finished/failed builds. 
An implementation must implement the *ShouldDisplay* method where you can read the build information and decide whether your IMGUI code should be run or not.
Implement the *OnGUI* method to actually draw your IMGUI content for the specific build.
```
public interface IBuildQueueExtension
{
  bool ShouldDisplay(Build build);
  void OnGUI(Build build);
}
```
An example of both a Build Processor and a Build Queue Extension that comes with the package is the Steam Uploader.

![image](https://github.com/BedtimeDigitalGames/BuildPipeline/assets/1178324/d8a356e1-bd91-4215-9b85-d6ed1a304299)

### Steam Uploader
The built in Steam upload system supports uploading a build directly to SteamWorks and optionally set it live on a branch. You can of course not set a build live on your default branch through this, but any other branch is valid.
The Steam uploader supports Steam Guard, and will show a pop up window in the case of Steam Guard protection. In a headless system you should set up Steam Guard for your build environment manually.

The values you can set here are a special type of configuration value called Secret values, where you must specify a key name, but do not need to set a value, though a local value can be set. 
Local values are never stored into the config file, but stored as a simple hashed string through Unity's Editorprefs API. *Local values are not a secure way to store values, and should only be used on a computer you trust.*

During a build, secrets will attempt to get their value from command line key/value arguments first, then from environment variables, and finally any locally set values.

![image](https://github.com/BedtimeDigitalGames/BuildPipeline/assets/1178324/b4c52ac9-9a57-4cdb-b59b-c5bae8c60af8)
