using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class SerializedBuildSettings
	{
		public MainModule Main = new();

		public ApplicationModule Application = new();
		
		public GeneralPlayerSettingsModule GeneralPlayerSettings = new();

		public RenderingModule Rendering = new();
		
		public IL2CPPModule IL2CPP = new();

		public BuildStepsModule BuildSteps = new();

		public DebugSettingsModule Debugging = new();

		public StandaloneModule Standalone = new();

		public NintendoSwitchModule NintendoSwitch = new();

		public PS4Module PS4 = new();
		
		public WebGlModule WebGL = new();

		public SharedMobileModule SharedMobile = new();

		public IOSModule iOS = new();

		public AndroidModule Android = new();

		public SteamModule Steam = new();

		public EpicModule Epic = new();
		
		public BedtimeLoggingModule BedtimeLogging = new();

		public T GetModule<T>() where T : class, ISettingsModule => GetModules().FirstOrDefault(a => a is T) as T;

		public bool TryGetModule<T>(out T container) where T : class, ISettingsModule
		{
			container = GetModules().FirstOrDefault(a => a is T) as T;
			return container != null;
		}

		private IEnumerable<ISettingsModule> GetModules()
		{
			if (!_defaultModules.Any())
			{
				var fields = typeof(SerializedBuildSettings).GetFields()
				                                          .Where(f => typeof(ISettingsModule).IsAssignableFrom(f.FieldType))
				                                          .Select(f => f.GetValue(this) as ISettingsModule)
				                                          .Where(c => c != null);
				_defaultModules.AddRange(fields);
			}

			foreach (var container in _defaultModules)
			{
				yield return container;
			}
				
			foreach (var container in _additionalModules)
			{
				yield return container;
			}
		}

		[SerializeReference, SerializeField]
		internal List<ISettingsModule> _additionalModules;

		private List<ISettingsModule> _defaultModules = new();
	}
}