using UnityEngine;
using UnityEditor;

namespace DCM.Old
{
		[System.Obsolete("This Class is obsolete")]
		public class RevertFromDevelopmentBake : ScriptableWizard
		{
				public GameObject parentToCombinedObjects = null;
	
				[MenuItem("Window/Draw Call Minimizer/Obsolete/Revert From Development Bake")]
				static void CreateWizard()
				{
						ScriptableWizard.DisplayWizard<RevertFromDevelopmentBake>("Revert Object", "Revert");
				}
	
				void OnWizardUpdate()
				{
				}
	
				//Export combined mesh
				void OnWizardCreate()
				{
						foreach(Renderer r in parentToCombinedObjects.GetComponentsInChildren<Renderer>())
						{
								r.enabled = true;
						}
				}
		}
}
