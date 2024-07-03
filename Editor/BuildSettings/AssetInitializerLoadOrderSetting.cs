#if BEDTIME_ASSET_SYSTEM
using System;
using System.Linq;
using BedtimeCore.Assets;
using UnityEditor;
using UnityEngine;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class AssetInitializerLoadOrderSetting : BuildSetting<AssetInitializerLoadOrder>
	{
		protected override AssetInitializerLoadOrder TryCopy(AssetInitializerLoadOrder from) => new AssetInitializerLoadOrder(from);

		public override AssetInitializerLoadOrder DefaultValue => new AssetInitializerLoadOrder();

		public AssetInitializerLoadOrderSetting(Action<AssetInitializerLoadOrder> applyAction) : base(applyAction)
		{
			
		}

		public override void DrawValue(BuildConfiguration topLevel, SerializedProperty property)
		{
			if (property == null)
			{
				return;
			}

			EditorGUILayout.PropertyField(property);
			EditorGUILayout.Space();
		}
	}
}
#endif