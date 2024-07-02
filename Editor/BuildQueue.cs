using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.ObjectModel;
using System;

namespace BedtimeCore.BuildPipeline
{
	public class BuildQueue : ScriptableObject
	{
		private const string BUILD_QUEUE_PATH = "EditorResources/Editor/BuildPipeline/";
		private const string FILE_NAME = "BuildQueue.asset";

		[SerializeField]
		private List<Build> queue = new List<Build>();
		[SerializeField]
		private List<Build> history = new List<Build>();

		/// <summary>
		/// Is the Queue processing on hold?
		/// </summary>
		public bool IsPaused { get; set; }

		/// <summary>
		/// The amount of currently enqueued Builds
		/// </summary>
		public int Count => queue.Count;

		/// <summary>
		/// The current state of the Queue
		/// </summary>
		public List<Build> Queue => queue;

		/// <summary>
		/// Empty the queue.
		/// </summary>
		public void Clear()
		{
			queue.Clear();
		}

		/// <summary>
		/// Empty the queue.
		/// </summary>
		public void ClearHistory() => history.Clear();

		/// <summary>
		/// Does the queue contain a specific Build?
		/// </summary>
		/// <param name="build">The Build to locate</param>
		/// <returns>Returns whether the queue contains the specified Build</returns>
		public bool Contains(Build build) => queue.Contains(build);

		/// <summary>
		/// Does the queue contain a Build that is based on a specific BuildConfiguration?
		/// </summary>
		/// <param name="configuration">The BuildConfiguration to locate</param>
		/// <returns>Returns whether the queue contains a Build with the specified Configuration</returns>
		public bool Contains(BuildConfiguration configuration) => GetBuild(configuration) != null;

		/// <summary>
		/// Attempt to retrieve an enqueued build based on a specific BuildConfiguration.
		/// </summary>
		/// <param name="configuration">The BuildConfiguration to locate</param>
		/// <returns>Returns a build containing the specified Configuration if it exists in the queue; otherwise, null</returns>
		public Build GetBuild(BuildConfiguration configuration)
		{
			foreach (var build in queue)
			{
				if (build.Configuration == configuration)
				{
					return build;
				}
			}
			return null;
		}

		/// <summary>
		/// The currently bottom-most Build in the queue. The Build will be null if the queue is empty.
		/// </summary>
		public Build ActiveBuild
		{
			get
			{
				if (queue.Count == 0)
				{
					return null;
				}

				return queue[0];
			}
		}

		/// <summary>
		/// A list of Builds that have previously been processed
		/// </summary>
		public List<Build> History => history;

		/// <summary>
		/// Attempt to dequeue a Build from the queue.
		/// </summary>
		/// <param name="build">The Build to dequeue</param>
		/// <returns>Returns whether the Build could be found and successfully removed</returns>
		public bool DequeueBuild(Build build)
		{
			bool state = queue.Remove(build);
			if (state)
			{
				history.Add(build);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
			return state;
		}

		/// <summary>
		/// Attempt to enqueue a Build to be processed by the Builder.
		/// </summary>
		/// <param name="build">The Build to enqueue. The build cannot be a duplicate of an already enqueued Build</param>
		/// <returns>Returns whether the Build could be enqueued successfully</returns>
		public bool EnqueueBuild(Build build)
		{
			if (!queue.Contains(build))
			{
				queue.Add(build);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Get the BuildQueue asset, either by creating a new asset, or finding an existing one.
		/// </summary>
		public static BuildQueue GetAsset()
		{
			const string ROOT_FOLDER = "Assets/";

			string directory = Path.Combine(ROOT_FOLDER, BUILD_QUEUE_PATH);
			var finalPath = Path.Combine(directory, FILE_NAME);

			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			if (File.Exists(finalPath))
			{
				return AssetDatabase.LoadMainAssetAtPath(finalPath) as BuildQueue;
			}

			var instance = CreateInstance<BuildQueue>();
			AssetDatabase.CreateAsset(instance, finalPath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			return instance;
		}
	}
}