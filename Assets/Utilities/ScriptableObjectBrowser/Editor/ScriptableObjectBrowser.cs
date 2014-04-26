using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class ScriptableObjectBrowser : EditorWindow
{
	private string[] letters = {"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","X","Y","Z"};
	private const string documentationUrl = "http://alexanderbirke.dk/?page_id=223";
	private const string supportMailAdress = "toolsupport@alexanderbirke.dk";
	
	enum TapOpened{Search, Favourites, About, Create};
	TapOpened tapOpened = TapOpened.Search;
	
	private List<System.Type> scriptableObjectTypes;
	string searchCriteria = "";
	Vector2 scrollPosition = Vector2.zero;
	
	private static ScriptableObjectBrowser window;
	private System.Type typeToCreate;
	private string nameOfNewObject;
	
	bool enterPressed = false;

	string pathToNewAsset;

	//used to gain keyboard focus
	private const string searchFieldName = "searchField";
	private const string createNamehField = "createNamehField";
	
	[MenuItem("Assets/Create/Scriptale object... &s")]
	public static void OpenWindow()
	{
		window = GetWindow<ScriptableObjectBrowser>(true, "ScriptableObject Browser");
		window.Init();
	}
	
	public void Init()
	{
		scriptableObjectTypes = SubTypeReflector.GetSubTypes<ScriptableObject>();

		//get rid of all user defined editors and editor windows
		scriptableObjectTypes = scriptableObjectTypes.Where(x => !x.IsSubclassOf(typeof(Editor))).ToList();
		scriptableObjectTypes = scriptableObjectTypes.Where(x => !x.IsSubclassOf(typeof(EditorWindow))).ToList();
	}

	void OnGUI()
	{
		
		enterPressed = false; //we do this here because the text fields later on will eat the event otherwise
		if(Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.keyUp)
		{
			Event.current.Use();
			enterPressed = true;
		}
		EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			if(GUILayout.Button("Search & Create", EditorStyles.toolbarButton))
				tapOpened = TapOpened.Search;
			if(GUILayout.Button("Help & About", EditorStyles.toolbarButton))
				tapOpened = TapOpened.About;
		EditorGUILayout.EndHorizontal();
		
		
		
		EditorGUILayout.BeginVertical();
			switch(tapOpened)
			{
			case TapOpened.Search:
				DrawSearch();
				break;
			case TapOpened.About:
				DrawAbout();
				break;
			case TapOpened.Create:
				DrawCreate();
				break;
			}
		EditorGUILayout.EndVertical();
		
		if(GUI.GetNameOfFocusedControl() == string.Empty)
			SelectTextField();
	}
	
	void SelectTextField()
	{
		
		switch(tapOpened)
		{
		case TapOpened.Search:
			EditorGUI.FocusTextInControl(searchFieldName);
			break;
		case TapOpened.Create:
			EditorGUI.FocusTextInControl(createNamehField);
			break;
		}
		
		window.Focus();
	}
	
	void DrawSearch()
	{
		if(scriptableObjectTypes.Count == 0)
		{
			GUILayout.Label("No scriptable objects could be found in the project");
			return;
		}
		
		GUILayout.Label("Search or pick the Scriptable Object you want to create", EditorStyles.label);
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Search:");
			GUI.SetNextControlName(searchFieldName);
			searchCriteria = EditorGUILayout.TextField(searchCriteria);
		EditorGUILayout.EndHorizontal();
		
		string legend = "All Scriptable Objects sorted alphabetically";
		if(!string.IsNullOrEmpty(searchCriteria))
			legend = "Search result";
		
		GUILayout.Label(legend, EditorStyles.miniBoldLabel);
		
		//result list1
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
		
			
			if( string.IsNullOrEmpty(searchCriteria) )
				DrawAlphabeticalList();
			else
				DrawSearchListResult();
			
		EditorGUILayout.EndScrollView();

	}

	private void DrawSearchListResult()
	{
		List<System.Type> selectedTypes;
		selectedTypes = 				
			scriptableObjectTypes.Where
				( 
				 x => x.Name.ToLowerInvariant().Contains(searchCriteria.ToLowerInvariant())
				 ).ToList();
		
		foreach(System.Type type in selectedTypes)
		{
			if(GUILayout.Button(type.Name))
			{
				typeToCreate = type;
				SwitchTap(TapOpened.Create);
			}
		}	

		if(selectedTypes.Count == 1)
			GUILayout.Label("Hit \"enter\" to select", EditorStyles.miniLabel);
		
		if(enterPressed && selectedTypes.Count == 1)
		{
			typeToCreate = selectedTypes[0];
			SwitchTap(TapOpened.Create);
		}
	}

	private void DrawAlphabeticalList()
	{
		foreach(string letter in letters)
		{
			List<System.Type> matchedTypes = 				
				scriptableObjectTypes.Where
					( 
					 x => x.Name.ToLowerInvariant().StartsWith(letter.ToLowerInvariant())
					 ).ToList();
			
			if(matchedTypes.Count > 0)
			{
				GUILayout.Label(letter, EditorStyles.boldLabel);
				foreach(System.Type type in matchedTypes)
				{
					if(GUILayout.Button(type.Name))
					{
						typeToCreate = type;
						SwitchTap(TapOpened.Create);
					}
				}	
			}
		}
		
	}

	void DrawAbout()
	{
		EditorGUILayout.BeginVertical();
			GUILayout.Label("Scriptable Object Browser", EditorStyles.boldLabel);
			GUILayout.Label("by Alexander Birke ", EditorStyles.label);
			
			if(GUILayout.Button("Go to online documentation"))
				Application.OpenURL(documentationUrl);

		EditorGUILayout.Space();

		//EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Support mail", EditorStyles.label);
			EditorGUILayout.SelectableLabel(supportMailAdress, EditorStyles.boldLabel);
		//EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();		
	}
	
	void DrawCreate()
	{
		GUIStyle style = EditorStyles.wordWrappedLabel;
		style.richText = true;

		EditorGUILayout.BeginVertical();
		
		GUILayout.Label("Name new scriptable object", EditorStyles.label);
			
				GUI.SetNextControlName(createNamehField);	
				nameOfNewObject = EditorGUILayout.TextField(nameOfNewObject, EditorStyles.textField);

			EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button("Cancel", EditorStyles.miniButton))
				{
					SwitchTap(TapOpened.Search);
				}
				if(GUILayout.Button("Create", EditorStyles.miniButton))
				{
					enterPressed = true;
				}
			EditorGUILayout.EndHorizontal();
			
		GUILayout.Label("Hit \"enter\" to select", EditorStyles.miniLabel);

		GUILayout.Label("The instance of <b>" +typeToCreate.Name+ "</b> will be created at: \n<b><i>" + pathToNewAsset + "/" + nameOfNewObject + "</i></b>", style);

		EditorGUILayout.EndVertical();
		
		if(enterPressed)
		{
			if(!string.IsNullOrEmpty(nameOfNewObject))
				CreateScriptableObject(typeToCreate, nameOfNewObject);
			else
				GUI.FocusControl(createNamehField);
		}
	}
	
	void SwitchTap(TapOpened tapToOpen)
	{
		tapOpened = tapToOpen;
		nameOfNewObject = "";
		searchCriteria = "";
		pathToNewAsset = GetPath();
		SelectTextField();
	}
	
	void CreateScriptableObject(System.Type type, string name)
	{
		string path = pathToNewAsset;
		ScriptableObject objectToSave = ScriptableObject.CreateInstance(type);

		path += "/" + name + ".asset";
		AssetDatabase.CreateAsset(objectToSave, path);
		
		Selection.activeObject = objectToSave;
		
		window.Close();
	}

	private string GetPath()
	{
		string path = "Assets";
		
		foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
		{
			path = AssetDatabase.GetAssetPath(obj);
			
			if (File.Exists(path))
			{
				path = Path.GetDirectoryName(path);
			}
			break;
		}

		return path;
	}

}
 	