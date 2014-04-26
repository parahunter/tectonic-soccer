/*
 * This code is written by and belongs to DFT Games.
 * Its usage is allowed to the licensee in conjunction
 * with the package that has been licensed and also
 * in other final products produced by the licensee
 * and aimed to the end user. It's forbidden to use
 * this code as part of packages or assets 
 * aimed to be used by Unity developers other
 * than the licensee of the original package.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A simple, dirty and fast typed pooling system.
/// </summary>
public class PoolingSystem<T> where T: class {
	
	private Stack<GameObject> available = new Stack<GameObject>();
	
	/// <summary>
	/// The original prefab reference.
	/// </summary>
	private GameObject original = null;
	
	/// <summary>
	/// Is this a GameObject pool?
	/// </summary>
	private bool isGameObject = false;
	
	public int Count
	{
		get{return available.Count;}	
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="PoolingSystem`1"/> class.
	/// </summary>
	/// <param name='prefab'>
	/// Prefab.
	/// </param>
	/// <param name='initialSize'>
	/// Initial size.
	/// </param>
	public PoolingSystem (GameObject prefab, int initialSize)
	{
		GameObject temp = null;
		original = prefab;
		
		// Remember if this is a GameObject type or not
		if (typeof(T) == typeof(GameObject))
			isGameObject = true;
		
		// Populate the initial pool
		for (int i = 0; i < initialSize; i++)
		{
			// Instantiate the object
			temp = GameObject.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
			// Set the object inactive
#if UNITY_4_0
			temp.SetActive(false);
#else
			temp.SetActiveRecursively (false);
#endif
			// Add it to the list of the available elements
			available.Push(temp);
		}
	}
	
	
	/// <summary>
	/// Releases the element. To be called instead of Destroy.
	/// </summary>
	/// <param name='element'>
	/// Element.
	/// </param>
	/// <param name='SetOutOfTheWay'>
	/// Set out of the way.
	/// </param>
	public void ReleaseElement(T element)
	{
		GameObject temp;
		if (isGameObject) // Are we dealing with GameObject?
		{
			temp = element as GameObject;
		}
		else // if not...
		{
			// Get the component to get its GameObject
			Component cTemp = element as Component;
			temp = cTemp.gameObject;
		}
		
		// change the object position id the flag is true
		// Set the object inactive
#if UNITY_4_0
		temp.SetActive(false);
#else
		temp.SetActiveRecursively (false);
#endif
		available.Push(temp);
	}
	
	/// <summary>
	/// Gets the element. To be called instead of Instantiate.
	/// </summary>
	/// <returns>
	/// The element.
	/// </returns>
	public T GetElement()
	{
		GameObject temp = null;
		// Check if the pool contains an usable element
		if (available.Count == 0)
		{
			// No free elements, so we create a new one.
			temp = GameObject.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
#if UNITY_4_0
			temp.SetActiveRecursively (false);
#else
			temp.SetActive(false);
#endif
			
		}
		else // an element is available
		{
			// fetch the element
			temp = available.Pop();
			// remove it from the active list
			
		}
		
		// Activate the object
#if UNITY_4_0
		temp.SetActive (true);
#else
		temp.SetActiveRecursively (true);
#endif
		// Return the proper object
		if (isGameObject)
			return temp as T;
		else
		{
			return temp.GetComponent(typeof(T)) as T;
		}
	}
}
