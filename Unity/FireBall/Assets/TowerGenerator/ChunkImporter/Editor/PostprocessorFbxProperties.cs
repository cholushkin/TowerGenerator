using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.ChunkImporter
{
	public class PostprocessorFbxProperties : AssetPostprocessor
	{
		// Due to a bug in unity that doesn't allow attach more than one script to game object
		// warning appears in the console - "Identifier uniqueness violation: 'xxx'. Scripted Importers do not guarantee that subsequent imports of this asset will properly re-link to these targets."
		
		public void OnPostprocessGameObjectWithUserProperties(GameObject gObj, string[] names, System.Object[] values)
		{
			ModelImporter modelImporter = assetImporter as ModelImporter;
			Debug.Assert(modelImporter != null, nameof(modelImporter) + " != null");
			if (ChunkImporterHelper.IsChunkPackFbx(modelImporter.assetPath))
			{
				ProcessFbxProps(gObj, names, values);
				return;
			}
		}

		private void ProcessFbxProps(GameObject gObj, string[] names, object[] values)
		{
			Assert.IsTrue(names.Length == values.Length);
			Assert.IsNotNull(gObj);

			var fbxProps = gObj.AddComponent<FbxProps>();

			for (int i = 0; i < names.Length; i++)
			{
				fbxProps.AddProperty(names[i], values[i].ToString());
			}
		}
	}
}
