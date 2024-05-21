//------------------------------------------
// Name: Search and Replace Objects
// By: jandd661
// Support: http://https://jandd661.wixsite.com/website
//-----------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace jandd661.EditorTools.SearchAndReplace
{
	public class SearchAndReplace : EditorWindow
	{
		private const string MENU_PATH = "Tools/Search and Replace Objects";
		private const string WINDOW_TITLE = "Search and Replace Objects";
		private const string MESSAGE_GREETING = "Please save your project before continuing!\nWARNING: Replacing prefabs that have scripts with references to other scene objects will break those references!";
		private const string MESSAGE_WARNING_TOP = "There are object warnings! Please review the below list.";

		private const string MESSAGE_NONPREFAB_CHILD_OBJECT = "This object has one or more non-prefab object(s) as a child. These objects will be retained and reatached to the root of the new prefab. This may break references! ({0})";
		private const string MESSAGE_SELECTION_CHILD_OBJECT = "This object has one or more child objects. These objects will be retained and reatached to the new prefab. This may break references! ({0})";

		private const string MESSAGE_ADDEDCOMPONENTS = "This object has unsaved added components ({0})";

		private const string MESSAGE_NO_NEW_PREFAB = "Please select a new prefab to swap with!";
		private const string TOOLTIP_PREVIEW_IMAGE_SIZE = "Size of the preview images.";
		private const string TOOLTIP_OBJECTS_PER_BATCH = "The max number of objects to list in the tool. Reduce this number if you have frame rate issues.";

		private const float TRANSFORM_LABLE_WIDTH = 70f;
		private const float WINDOW_MINSIZE_WIDTH = 500f;
		private const float WINDOW_MINSIZE_HEIGHT = 500f;
		private const int ROUNDTO_DECIMALS = 2;
		private const int SEARCH_MINCHARS = 2;

		private string[] _tabArray = new string[] { "Search by Prefab", "Select Objects", "Search by Name" };
		[SerializeField] private GameObject _sourcePrefab;
		[SerializeField] private GameObject _newPrefab;
		private int _objectsPerBatch = 20;
		private float _buttonSize = 100f;
		private float _prefabButtonScale = 0.8f;
		private Vector2 _svPosition;
		private List<GameObject> _currentDerivedList = new List<GameObject>();
		private List<GameObject> _currentSelectionList = new List<GameObject>();
		private List<GameObject> _workingSelectionList = new List<GameObject>();


		private GameObject _lastSelectedPrefab = null;
		private bool _matchRotation = true;
		private bool _matchScale = true;
		private bool _dataUpdateInprogress = false;
		private bool _showMainWarningMessage = false;
		private bool _showNoNewPrefabMessage = false;
		private int _currentTab = 0;
		private bool _tabChanged = false;
		private string _searchText = "";
		private SearchUtils.SearchType _searchType = SearchUtils.SearchType.StartsWith;

		[MenuItem(MENU_PATH)]
		public static void ShowWindow()
		{
			SearchAndReplace window = GetWindow<SearchAndReplace>(WINDOW_TITLE, true);
			window.Show();
		}

		public void Awake()
		{
			Undo.undoRedoPerformed += UndoCallback;
			this.minSize = new Vector2(WINDOW_MINSIZE_WIDTH, WINDOW_MINSIZE_HEIGHT);
		}

		private void OnDestroy()
		{
			CleanUp();
		}

		private void OnDisable()
		{
			CleanUp();
		}

		private void OnHierarchyChange()
		{
			UpdatAndRepaint("OnHierarchyChange");
		}

		private void OnSelectionChange()
		{
			if (_currentTab == 1)
			{
				UpdateWorkingSelectionList();
			}
			UpdatAndRepaint("OnSelectionChange");
		}

		private void OnGUI()
		{
			if (_dataUpdateInprogress == true)
			{
				return;
			}
			else if(_tabChanged == true)
			{
				return;
			}
			else
			{
				DrawGui();
			}
		}
		/// <summary>
		/// Force the UI to update if nothing else is going on
		/// </summary>
		private void OnInspectorUpdate()
		{
			if(_dataUpdateInprogress == false && _tabChanged == false)
			{
				this.Repaint();
			}
		}
		/// <summary>
		/// Process the Tab Change
		/// </summary>
		private void TabChanged()
		{
			_searchText = "";
			_showMainWarningMessage = false;
			Selection.objects = null;
			_sourcePrefab = null;
			_tabChanged = false;
			UpdateWorkingSelectionList();
			UpdatAndRepaint("DrawGui:TabChanged");
		}
		/// <summary>
		/// Updates the target prefab list and repaints the GUI
		/// </summary>
		/// <param name="arg1"></param>
		private void UpdatAndRepaint(string arg1)
		{
			if(_currentTab == 0)
			{
				SetCurrentDerivedList(GetDerivatives(_sourcePrefab));
			}
			else if(_currentTab == 1)
			{
				SetCurrentSelectionList(_workingSelectionList);
			}
			this.Repaint();
		}
		/// <summary>
		/// Unregisters from the Undo callback
		/// </summary>
		private void CleanUp()
		{
			Undo.undoRedoPerformed -= UndoCallback;
			_currentDerivedList = null;
			_currentSelectionList = null;
		}
		/// <summary>
		/// Draws the main GUI
		/// </summary>
		private void DrawGui()
		{

			List<GameObject> localList = GetCurrentList();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginVertical();
			EditorGUILayout.EndVertical();

			// TOP TABS
			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			_currentTab = GUILayout.Toolbar(_currentTab, _tabArray);
			if(EditorGUI.EndChangeCheck())
			{
				_tabChanged = true;
			}
			EditorGUILayout.EndHorizontal();
			// TOP MESSAGE_GREETING
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.HelpBox(MESSAGE_GREETING, MessageType.Info, true);
			EditorGUILayout.EndHorizontal();

			// TOP MESSAGE_WARNING_TOP
			if (_showMainWarningMessage == true)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.HelpBox(MESSAGE_WARNING_TOP, MessageType.Warning, true);
				EditorGUILayout.EndHorizontal();
			}

			if (_showNoNewPrefabMessage == true && _newPrefab == null)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.HelpBox(MESSAGE_NO_NEW_PREFAB, MessageType.Warning, true);
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				_showNoNewPrefabMessage = false;
			}

			// PREVIEW IMAGE SCALE SLIDER
			EditorGUILayout.BeginHorizontal();
			_prefabButtonScale = EditorGUILayout.Slider(new GUIContent("Image Scale: ", TOOLTIP_PREVIEW_IMAGE_SIZE), _prefabButtonScale, 0.5f, 1.5f);
			_buttonSize = 100f * _prefabButtonScale;
			EditorGUILayout.EndHorizontal();

			// BATCH SIZE
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical(GUILayout.Width(150));
			_objectsPerBatch = EditorGUILayout.IntField(new GUIContent("Objects per batch: ", TOOLTIP_OBJECTS_PER_BATCH), _objectsPerBatch);
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(GUILayout.Width(50));
			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(GUILayout.Width(this.position.width - 200));
			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();

			// LINE
			EditorGUILayout.BeginHorizontal();
			DrawUILine(Color.gray);
			EditorGUILayout.EndHorizontal();

			if(_currentTab == 0)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.BeginVertical(GUILayout.Width(_buttonSize));
				// PREVIEW IMAGE
				EditorGUILayout.BeginHorizontal();
				if (_sourcePrefab != null)
				{
					Texture2D tex = AssetPreview.GetAssetPreview(_sourcePrefab);
					if (GUILayout.Button(tex, GUILayout.Width(_buttonSize), GUILayout.Height(_buttonSize)))
					{
						Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GetAssetPath(_sourcePrefab));
					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical();
				// SEARCH FOR BOX
				DrawObjectsDerivedFromBox(localList);
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}
			else if(_currentTab == 1)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Selected scene objecs: " + localList.Count);
				EditorGUILayout.EndHorizontal();
			}
			else if (_currentTab == 2)
			{
				// SEARCH BOX
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.BeginVertical(GUILayout.Width(100));
				GUILayout.Label("Object names: ");
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical(GUILayout.Width(100));
				_searchType = (SearchUtils.SearchType)EditorGUILayout.EnumPopup(_searchType);
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical(GUILayout.Width(this.position.width - 220));

				GUI.SetNextControlName("SearchTextField");
				_searchText = EditorGUILayout.DelayedTextField(_searchText);
				EditorGUI.FocusTextInControl("SearchTextField");

				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
				// SEARCH TYPE & COUNT
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.BeginVertical(GUILayout.Width(300));
				GUILayout.Label("Total objects found: " + localList.Count);
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical(GUILayout.Width(this.position.width - 320));
				if (GUILayout.Button("Clear"))
				{
					_searchText = "";
				}

				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
				if (EditorGUI.EndChangeCheck() == true)
				{
					_workingSelectionList.Clear();
					_showMainWarningMessage = false;
					if (_searchText.Length >= SEARCH_MINCHARS)
					{
						GameObject[] allObjects = FindObjectsOfType<GameObject>();
						foreach (GameObject ob in allObjects)
						{
							if (SearchUtils.IsASearchMatch(ob.name, _searchText, _searchType) == true)
							{
								_workingSelectionList.Add(ob);
							}
						}
					}
					SetCurrentSelectionList(_workingSelectionList);
				}
			}
			// LINE
			EditorGUILayout.BeginHorizontal();
			DrawUILine(Color.gray);
			EditorGUILayout.EndHorizontal();

			// REPLACE WITH BOX
			DrawReplaceWithBox();

			// LINE
			EditorGUILayout.BeginHorizontal();
			DrawUILine(Color.gray);
			EditorGUILayout.EndHorizontal();

			// LIST OF OBJECTS
			EditorGUILayout.BeginHorizontal();
			List<GameObject> currentPageOfObjects = new List<GameObject>();
			if (localList.Count > 0 && _dataUpdateInprogress != true)
			{
				_svPosition = EditorGUILayout.BeginScrollView(_svPosition);
				_showMainWarningMessage = false;
				for (int i = 0; i < localList.Count && i < _objectsPerBatch; i++)
				{
					if (localList[i] != null)
					{
						currentPageOfObjects.Add(localList[i]);
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.BeginVertical(GUILayout.Width(_buttonSize));
						EditorGUILayout.BeginHorizontal();
						Texture2D tex = AssetPreview.GetAssetPreview(localList[i]);
						if (GUILayout.Button(tex, GUILayout.Width(_buttonSize), GUILayout.Height(_buttonSize)))
						{
							Selection.activeObject = localList[i];
							SceneView.FrameLastActiveSceneViewWithLock();
							if(_currentTab == 1)
							{
								Selection.objects = localList.ToArray();
							}
						}
						EditorGUILayout.EndHorizontal();

						EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button("Replace"))
						{
							if (_newPrefab != null)
							{
								ReplaceWithPrefab(localList[i], _newPrefab, _matchRotation, _matchScale, "", true);
								_showMainWarningMessage = false;
								_showNoNewPrefabMessage = false;
							}
							else
							{
								_showNoNewPrefabMessage = true;
							}

						}
						EditorGUILayout.EndHorizontal();

						EditorGUILayout.EndVertical();
						EditorGUILayout.BeginVertical();
						// DERIVED OBJECT FIELD
						GUI.enabled = false;
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.ObjectField("", localList[i], typeof(GameObject), false);
						EditorGUILayout.EndHorizontal();
						GUI.enabled = true;
						// DERIVED OBJECT TRANSFORM
						DrawTransformLable(localList[i], 50f);

						if(localList[i] != null)
						{
							bool hasNonPrefabObjectAsChild = false;
							bool hasAddedComponent = false;
							string objectWarningMessage = "";
							string addedComponentsMessage = "";

							// IS THIS A PREFAB?
							if(PrefabUtility.IsPartOfAnyPrefab(localList[i]) == true)
							{
								// SEE IF THERE ARE UNSAVED ADDED COMPONENTS - GetAddedComponents() checks child objects too.
								List<AddedComponent> addedComponentsList = PrefabUtility.GetAddedComponents(localList[i]);
								if (addedComponentsList.Count > 0)
								{
									foreach (AddedComponent component in addedComponentsList)
									{
										if (addedComponentsMessage != "")
										{
											addedComponentsMessage += ", ";
										}
										addedComponentsMessage += component.instanceComponent.ToString();
										hasAddedComponent = true;
									}
								}
								addedComponentsList.Clear();
								// SEE IF THER IS ANY UNSAVED CHILD OBJECTS
								List<GameObject> childList = GetChildRecursive(localList[i]);
								for (int j = 0; j < childList.Count; j++)
								{
									GameObject childDerivedObject = PrefabUtility.GetCorrespondingObjectFromSource(childList[j]);
									if (childDerivedObject == null)
									{
										if (objectWarningMessage != "")
										{
											objectWarningMessage += ", ";
										}
										objectWarningMessage += childList[j].name;
										hasNonPrefabObjectAsChild = true;
									}
								}
							}

							if (hasNonPrefabObjectAsChild == true)
							{
								_showMainWarningMessage = true;
								string message = _currentTab == 0 ? MESSAGE_NONPREFAB_CHILD_OBJECT : MESSAGE_SELECTION_CHILD_OBJECT;
								EditorGUILayout.BeginHorizontal();
								EditorGUILayout.HelpBox(string.Format(message, objectWarningMessage), MessageType.Warning, true);
								EditorGUILayout.EndHorizontal();
							}
							if (hasAddedComponent == true)
							{
								_showMainWarningMessage = true;
								string message = _currentTab == 0 ? MESSAGE_ADDEDCOMPONENTS : MESSAGE_ADDEDCOMPONENTS;
								EditorGUILayout.BeginHorizontal();
								EditorGUILayout.HelpBox(string.Format(message, addedComponentsMessage), MessageType.Warning, true);
								EditorGUILayout.EndHorizontal();
							}
							EditorGUILayout.EndVertical();
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();
							DrawUILine(Color.gray);
							EditorGUILayout.EndHorizontal();
						}
					}
				}
				EditorGUILayout.EndScrollView();
			}
			EditorGUILayout.EndHorizontal();

			// REPLACE ALL BUTTON
			EditorGUILayout.BeginHorizontal();
			if (localList.Count > 0)
			{
				if (GUILayout.Button("Replace all"))
				{
					if(_newPrefab != null)
					{
						ReplaceSceneObjects(currentPageOfObjects, _newPrefab, _matchRotation, _matchScale);
						_showMainWarningMessage = false;
						_showNoNewPrefabMessage = false;
					}
					else
					{
						_showNoNewPrefabMessage = true;
					}
				}
			}
			EditorGUILayout.EndHorizontal();
			localList = null;
			if(_tabChanged == true)
			{
				TabChanged();
			}
		}
		/// <summary>
		/// Returns a LIST<> of all child objects for the given object
		/// </summary>
		/// <param name="inObject">Object to search</param>
		/// <param name="inList">Self callback for iterations.</param>
		/// <returns></returns>
		private List<GameObject> GetChildRecursive(GameObject inObject, List<GameObject> inList = null)
		{
			List<GameObject> outList = null;
			outList = inList == null ? new List<GameObject>() : inList;
			if (inObject == null)
			{
				return outList;
			}

			foreach (Transform child in inObject.transform)
			{
				if (child == null)
				{
					continue;
				}
				outList.Add(child.gameObject);
				GetChildRecursive(child.gameObject, outList);
			}
			return outList;
		}
		/// <summary>
		/// Draws the "Replace with" asset selection box
		/// </summary>
		private void DrawReplaceWithBox()
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical(GUILayout.Width(_buttonSize));
			// PREVIEW IMAGE
			EditorGUILayout.BeginHorizontal();
			if (_newPrefab != null)
			{
				Texture2D tex = AssetPreview.GetAssetPreview(_newPrefab);
				if (GUILayout.Button(tex, GUILayout.Width(_buttonSize), GUILayout.Height(_buttonSize)))
				{
					Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GetAssetPath(_newPrefab));
				}
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			// NEW PREFAB LABLE
			EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Replace with this prefab:");
			EditorGUILayout.EndHorizontal();
			// NEW PREFAB INPUT BOX
			EditorGUILayout.BeginHorizontal();
			_newPrefab = (GameObject)EditorGUILayout.ObjectField("", _newPrefab, typeof(GameObject), false);
			EditorGUILayout.EndHorizontal();
			// CHECK BOXES
			EditorGUILayout.BeginHorizontal();
			_matchRotation = GUILayout.Toggle(_matchRotation, "Match scene object rotation");
			_matchScale = GUILayout.Toggle(_matchScale, "Match scene object scale");
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}
		/// <summary>
		/// Draws the "Search for" asset selection box
		/// </summary>
		/// <param name="localDerivitiveList">Current cashed list</param>
		private void DrawObjectsDerivedFromBox(List<GameObject> localDerivitiveList)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Find scene objects derived from:");
			EditorGUILayout.EndHorizontal();
			// SOURCE PREFAB INPUT BOX
			EditorGUILayout.BeginHorizontal();
			_sourcePrefab = (GameObject)EditorGUILayout.ObjectField("", _sourcePrefab, typeof(GameObject), false);
			if ((_sourcePrefab == null || _sourcePrefab != _lastSelectedPrefab))
			{
				localDerivitiveList.Clear();
			}
			EditorGUILayout.EndHorizontal();
			// FIND DERIVITIVES BUTTON
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Find Derivatives"))
			{
				if (_sourcePrefab != null)
				{
					SetCurrentDerivedList(GetDerivatives(_sourcePrefab));
					_lastSelectedPrefab = _sourcePrefab;
				}
				else
				{
					localDerivitiveList.Clear();
				}
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField(" Total objects found: " + localDerivitiveList.Count);
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical();
			if (GUILayout.Button("Clear"))
			{
				_sourcePrefab = null;
				localDerivitiveList.Clear();
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();

		}
		/// <summary>
		/// Searches the scene for objects derived from the selected prefab in DrawObjectsDerivedFromBox()
		/// </summary>
		/// <param name="sourcePrefab"></param>
		/// <returns>The list of objects derived from the selected prefab</returns>
		private List<GameObject> GetDerivatives(GameObject sourcePrefab)
		{
			_dataUpdateInprogress = true;
			List<GameObject> outList = new List<GameObject>();
			if (sourcePrefab == null)
			{
				_currentDerivedList.Clear();
				outList = _currentDerivedList;
			}
			else
			{
				foreach (GameObject sceneObject in FindObjectsOfType(typeof(GameObject)))
				{
					GameObject derivedObject = PrefabUtility.GetCorrespondingObjectFromSource(sceneObject);
					if (derivedObject == sourcePrefab)
					{
						outList.Add(sceneObject);
					}
				}
			}
			_dataUpdateInprogress = false;
			return outList;
		}
		/// <summary>
		/// Replaces the scene objects in provided list with the selected prefab
		/// </summary>
		/// <param name="objectList">The current target objects list</param>
		/// <param name="prefab">The prefab used to replace the targets with</param>
		/// <param name="matchRotation">Match target rotation</param>
		/// <param name="matchScale">Match target scale</param>
		private void ReplaceSceneObjects(List<GameObject> objectList, GameObject prefab, bool matchRotation, bool matchScale)
		{
			List<GameObject> workinglist = objectList;
			_dataUpdateInprogress = true;
			if(workinglist == null || prefab == null)
			{
				Debug.LogError("[ReplaceSceneObjects] is missing required peramiter! (objectList=" + objectList + ", prefab=" + prefab + ")");
				return;
			}
			for (int i = 0; i < workinglist.Count; i++)
			{
				string name = prefab.name + " (" + (i + 1) + ")";
				ReplaceWithPrefab(workinglist[i], prefab, matchRotation, matchScale, name, false);
			}
			SetCurrentList(workinglist);
			_dataUpdateInprogress = false;

		}
		/// <summary>
		/// Replaces a scene object with the provided prefab
		/// </summary>
		/// <param name="objectToReplace">Target object</param>
		/// <param name="prefab">Prefab to replace the target with</param>
		/// <param name="matchRotation">Match target rotation</param>
		/// <param name="matchScale">Match target scale</param>
		/// <param name="name">Optional name overide. Default is prefab name</param>
		/// <param name="setInprogress">setInprogess is used to toggle _swapInprogress. Use (true) if NOT toggled in the calling method, otherwize set false</param>
		/// <returns>Returns a reference to the newly spawned object</returns>
		private GameObject ReplaceWithPrefab(GameObject objectToReplace, GameObject prefab, bool matchRotation, bool matchScale, string name = "", bool setInprogress = true)
		{

			if (objectToReplace == null || prefab == null)
			{
				return null;
			}

			if (setInprogress == true)
			{
				_dataUpdateInprogress = true;
			}

			List<GameObject> nonPrefabs = new List<GameObject>();
			List<GameObject> childList = GetChildRecursive(objectToReplace);

			for (int i = 0; i < childList.Count; i++)
			{
				GameObject childDerivedObject = PrefabUtility.GetCorrespondingObjectFromSource(childList[i]);
				if (childDerivedObject == null)
				{
					nonPrefabs.Add(childList[i]);
				}
			}

			Undo.RegisterCompleteObjectUndo(objectToReplace, objectToReplace.name + " object state");
			GameObject newObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
			Undo.RegisterCreatedObjectUndo(newObject, "Spawn new prefab");
			Undo.SetTransformParent(newObject.transform, objectToReplace.transform.parent, newObject.name + " root parenting");
			Undo.RegisterCompleteObjectUndo(newObject, newObject.name + " object state");
			newObject.transform.localPosition = objectToReplace.transform.localPosition;
			newObject.name = name != "" ? name : prefab.name;
			if (matchRotation == true) newObject.transform.localRotation = objectToReplace.transform.localRotation;
			if (matchScale == true) newObject.transform.localScale = objectToReplace.transform.localScale;

			for (int i = 0; i < nonPrefabs.Count; i++)
			{
				Undo.RegisterCompleteObjectUndo(nonPrefabs[i], nonPrefabs[i].name + " object state");
				Undo.SetTransformParent(nonPrefabs[i].transform, newObject.transform, nonPrefabs[i].name + " parent change");
			}
			Undo.DestroyObjectImmediate(objectToReplace);
			if (setInprogress == true)
			{
				_dataUpdateInprogress = false;
			}
			return newObject;
		}
		/// <summary>
		/// Returns the current Derived Objects list
		/// </summary>
		/// <returns>List<GameObject></returns>
		private List<GameObject> GetCurrentList()
		{
			if(_currentTab == 0)
			{
				return _currentDerivedList;
			}
			else
			{
				return _currentSelectionList;
			}
		}
		/// <summary>
		/// Sets the current Derived Objects list
		/// </summary>
		/// <param name="inList"></param>
		/// <returns>List<GameObject></returns>
		private List<GameObject> SetCurrentList(List<GameObject> inList)
		{
			if (_currentTab == 0)
			{
				_currentDerivedList = inList;
			}
			else
			{
				return _currentSelectionList = inList;
			}
			return inList;
		}
		/// <summary>
		/// Sets the current Derived Objects list
		/// </summary>
		/// <param name="inList"></param>
		/// <returns>List<GameObject></returns>
		private List<GameObject> SetCurrentDerivedList(List<GameObject> inList)
		{
			_currentDerivedList = inList;
			return inList;
		}
		/// <summary>
		/// Sets the current Selection Objects list
		/// </summary>
		/// <param name="inList"></param>
		/// <returns>List<GameObject></returns>
		private List<GameObject> SetCurrentSelectionList(List<GameObject> inList)
		{
			_currentSelectionList = inList;
			return inList;
		}
		/// <summary>
		/// Updates the "working" selection list. Later assign it to the current selection list.
		/// </summary>
		private void UpdateWorkingSelectionList()
		{
			_dataUpdateInprogress = true;
			_workingSelectionList.Clear();
			Object[] selectionArray = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable);
			for (int i = 0; i < selectionArray.Length; i++)
			{
				_workingSelectionList.Add((GameObject)selectionArray[i]);
			}
			_dataUpdateInprogress = false;
		}
		/// <summary>
		/// Draws the objects transform information box
		/// </summary>
		/// <param name="gameObject">Target object</param>
		/// <param name="lableWidth">Width of the individual lables</param>
		private void DrawTransformLable(GameObject gameObject, float lableWidth)
		{
			if(gameObject != null)
			{
				Transform inTransform = gameObject.transform;
				// POSITION
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.BeginVertical(GUILayout.Width(TRANSFORM_LABLE_WIDTH));
				GUILayout.Label("Position:");
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(GUILayout.Width(lableWidth));
				GUILayout.Label(RoundFloat(inTransform.localPosition.x, ROUNDTO_DECIMALS).ToString());
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(GUILayout.Width(lableWidth));
				GUILayout.Label(RoundFloat(inTransform.localPosition.y, ROUNDTO_DECIMALS).ToString());
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(GUILayout.Width(lableWidth));
				GUILayout.Label(RoundFloat(inTransform.localPosition.z , ROUNDTO_DECIMALS).ToString());
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
				// ROTATION
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.BeginVertical(GUILayout.Width(TRANSFORM_LABLE_WIDTH));
				GUILayout.Label("Rotation:");
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(GUILayout.Width(lableWidth));
				GUILayout.Label(RoundFloat(inTransform.localRotation.eulerAngles.x, ROUNDTO_DECIMALS).ToString());
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(GUILayout.Width(lableWidth));
				GUILayout.Label(RoundFloat(inTransform.localRotation.eulerAngles.y, ROUNDTO_DECIMALS).ToString());
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(GUILayout.Width(lableWidth));
				GUILayout.Label(RoundFloat(inTransform.localRotation.eulerAngles.z, ROUNDTO_DECIMALS).ToString());
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
				// SCALE
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.BeginVertical(GUILayout.Width(TRANSFORM_LABLE_WIDTH));
				GUILayout.Label("Scale:");
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(GUILayout.Width(lableWidth));
				GUILayout.Label(RoundFloat(inTransform.localScale.x, ROUNDTO_DECIMALS).ToString());
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(GUILayout.Width(lableWidth));
				GUILayout.Label(RoundFloat(inTransform.localScale.y, ROUNDTO_DECIMALS).ToString());
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(GUILayout.Width(lableWidth));
				GUILayout.Label(RoundFloat(inTransform.localScale.z, ROUNDTO_DECIMALS).ToString());
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}
		}
		/// <summary>
		/// Draws a line on the GUI
		/// </summary>
		/// <param name="color"></param>
		/// <param name="thickness"></param>
		/// <param name="padding"></param>
		private void DrawUILine(Color color, int thickness = 2, int padding = 10)
		{
			Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
			r.height = thickness;
			r.y += padding / 2;
			r.x -= 2;
			r.width += 6;
			EditorGUI.DrawRect(r, color);
		}
		/// <summary>
		/// Clears the current Derived list and updates it when a UNDO event happens
		/// </summary>
		public void UndoCallback()
		{
			_currentDerivedList.Clear();
			UpdatAndRepaint("UndoCallback");
		}
		/// <summary>
		/// Rounds a float to [digits] decimal points
		/// </summary>
		/// <param name="value">Value to be rounded</param>
		/// <param name="digits">Digits passed the decimal</param>
		/// <returns></returns>
		private float RoundFloat(float value, int digits)
		{
			float mult = Mathf.Pow(10.0f, (float)digits);
			return Mathf.Round(value * mult) / mult;
		}

	}
}
