using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class SerializedBuildSettings
	{
		public MainModule Main;

		public ApplicationModule Application;
		
		public GeneralPlayerSettingsModule GeneralPlayerSettings;

		public RenderingModule Rendering;
		
		public IL2CPPModule IL2CPP;

		public BuildStepsModule BuildSteps;

		public DebugSettingsModule Debugging;

		public StandaloneModule Standalone;

		public NintendoSwitchModule NintendoSwitch;

		public PS4Module PS4;
		
		public WebGlModule WebGL;

		public SharedMobileModule SharedMobile;

		public IOSModule iOS;

		public AndroidModule Android;

		public SteamModule Steam;

		public EpicModule Epic;
		
		public BedtimeLoggingModule BedtimeLogging;

		public T GetModule<T>() where T : class, ISettingsModule => GetModules().FirstOrDefault(a => a is T) as T;

		public bool TryGetModule<T>(out T container) where T : class, ISettingsModule
		{
			container = GetModules().FirstOrDefault(a => a is T) as T;
			return container != null;
		}

		private IEnumerable<ISettingsModule> GetModules()
		{
			if (!_defaultContainers.Any())
			{
				var fields = typeof(SerializedBuildSettings).GetFields()
				                                          .Where(f => typeof(ISettingsModule).IsAssignableFrom(f.FieldType))
				                                          .Select(f => f.GetValue(this) as ISettingsModule)
				                                          .Where(c => c != null);
				_defaultContainers.AddRange(fields);
			}

			foreach (var container in _defaultContainers)
			{
				yield return container;
			}
				
			foreach (var container in _additionalContainers)
			{
				yield return container;
			}
		}
		
		[SerializeReference, SerializeField]
		internal List<ISettingsModule> _additionalContainers;
		
		internal List<ISettingsModule> _defaultContainers = new List<ISettingsModule>();
	}
}