#if ADDRESSABLES
using System;
using System.ComponentModel;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;

namespace BedtimeCore.NestBuilder
{
    [Serializable]
    public class AddressablesModule : ISettingsModule
    {
        [Category("Addressables")]
        public EnumSetting<AddressableAssetSettings.PlayerBuildOption>  BuildOptions = new(x => { AddressableAssetSettingsDefaultObject.Settings.BuildAddressablesWithPlayerBuild = x; });
        
        [Category("Addressables")]
        public ObjectFieldSetting<BuildScriptBase> BuildScript = new(x =>
        {
            var index = AddressableAssetSettingsDefaultObject.Settings.DataBuilders.IndexOf(x);
            if (index != -1)
            {
                AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilderIndex = index;
            }
            else
            {
                throw new InvalidOperationException($"The selected build script {x} is not in the list of available build scripts.");
            }
        });
    }
}
#endif