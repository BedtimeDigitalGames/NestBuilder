using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace BedtimeCore.BuildPipeline
{
	/// <summary>
	/// Android settings
	/// </summary>
	public partial class BuildSettingMapper
	{
		public static bool BuildAppBundle
		{
			get => EditorUserBuildSettings.buildAppBundle;
			set => EditorUserBuildSettings.buildAppBundle = value;
		}

		public static bool UseCustomKeyStore
		{
			get => PlayerSettings.Android.useCustomKeystore;
			set => PlayerSettings.Android.useCustomKeystore = value;
		}
		
		public static string KeyStorePath
		{
			get => PlayerSettings.Android.keystoreName;
			set => PlayerSettings.Android.keystoreName = value;
		}

		public static string KeyStorePassword
		{
			get => PlayerSettings.Android.keystorePass;
			set => PlayerSettings.Android.keystorePass = value;
		}

		public static string KeyStoreAlias
		{
			get => PlayerSettings.Android.keyaliasName;
			set => PlayerSettings.Android.keyaliasName = value;
		}

		public static string KeyStoreAliasPassword
		{
			get => PlayerSettings.Android.keyaliasPass;
			set => PlayerSettings.Android.keyaliasPass = value;
		}
	}
}