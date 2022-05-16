// Add context menu named "Do Something" to context menu
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Animations;
using UnityEditor;

public class NaamUtil : EditorWindow
{
	// private const string MenuPathConcave = "GameObject/Naam/Concave Mesh Colliders";
	// private const string MenuPathConvex = "GameObject/Naam/Convex Mesh Colliders";
	private const string MenuPathInstanceSetup = "GameObject/Naam/Setup Instancing";
	private const string MenuPathInstanceClean = "GameObject/Naam/Cleanup Instancing";
	// private const string MenuPathConstrainSetup = "GameObject/Naam/Constrain siblings by name";
	// private const string MenuPathUniqueFillNames = "GameObject/Naam/Unique Fill Names";
	// private const string MenuPathREFCleanup = "GameObject/Naam/Empty REF mesh objects";
	private const string MenuPathZeroParent = "GameObject/Naam/Create Zero Parent";


	/// <summary> Menu command that instances child objects based on Cinema4D native 
	/// naming schemes. </summary>
	[MenuItem( MenuPathInstanceSetup, false, 0 )]
	static void SetupInstances( MenuCommand command )
	{
		CleanupInstances( command );
		var transform = Selection.activeGameObject.transform;
		Dictionary<string,GameObject> originals = new Dictionary<string, GameObject>();
		Dictionary<string,int> originalCounts = new Dictionary<string,int>();
		foreach( var op in 
					transform.GetComponentsInChildren<Transform>()
					.Select( a=>a.gameObject )
					.Where( a=>Regex.IsMatch( a.name, @"Instance" )==false) )
		{
			if( originals.ContainsKey( op.name ) == false )
			{
				originals.Add( op.name, op.gameObject );
				originalCounts.Add( op.name, 0 );
			}
		}

		
		foreach( var op in 
					transform.GetComponentsInChildren<Transform>()
					.Select( a=>a.gameObject ) )
		{
			var m = Regex.Match( op.name, @"(.*)_Instance_?(\d*)" );
			if( m.Groups.Count > 1 // theres a match
				&& originals.ContainsKey( m.Groups[1].Value ) // we have an original
				// && op.GetComponents<Component>().Length == 1  // only on empty GameObjects NOT FOR COLORSPACE : D
			)
			{
				Debug.Log( $"Cloning {originals[m.Groups[1].Value].name} -> {op.name}" );
				var newOp = Instantiate( originals[m.Groups[1].Value] ).transform;
				newOp.gameObject.name = $"{originals[m.Groups[1].Value].name}(Clone)_{originalCounts[m.Groups[1].Value]}";
				originalCounts[m.Groups[1].Value]++;
				newOp.SetParent( op.transform.parent );
				newOp.SetSiblingIndex( op.transform.GetSiblingIndex() );
				newOp.localPosition = op.transform.localPosition;
				newOp.localRotation = op.transform.localRotation;
				newOp.localScale = op.transform.localScale;
			}
		}
	}

	/// <summary> Cleans up all the mess that SetupInstances made. </summary>
	[MenuItem( MenuPathInstanceClean, false, 0 )]
	static void CleanupInstances( MenuCommand command )
	{
		var transform = Selection.activeGameObject.transform;
		var remove = transform.GetComponentsInChildren<Transform>()
						.Where( a=>Regex.IsMatch( a.name, @"\(Clone\)" ) )
						.Select( a=>a.gameObject )
						.ToArray();
		foreach( var op in remove ) 
			DestroyImmediate( op );
	}



	/// <summary> Creates an empty parent to zero out an objects coordinates </summary>
	[MenuItem( MenuPathZeroParent, false, 0 )]
	static void CreateZeroParent( MenuCommand command )
	{
		var transform = Selection.activeTransform;
		var newParent = new GameObject( "00" ).transform;
		newParent.SetParent(transform);
		newParent.localPosition = Vector3.zero;
		newParent.localRotation = Quaternion.identity;
		newParent.localScale = Vector3.one;
		newParent.SetParent(transform.parent,true);
		newParent.SetSiblingIndex(transform.GetSiblingIndex());
		transform.SetParent(newParent,true);

		EditorGUIUtility.PingObject(transform.gameObject); // should uncollapse the newly created object 
	}


	[MenuItem( MenuPathInstanceSetup, true )]
	[MenuItem( MenuPathInstanceClean, true )]
	[MenuItem( MenuPathZeroParent, true )]
	private static bool Validate() => Selection.activeGameObject != null;
}