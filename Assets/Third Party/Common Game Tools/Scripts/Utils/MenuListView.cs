#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace CGT
{
	//We need to use a list to serialize in ScriptableObject
	[System.Serializable]
	public class MenuListView
	{

		public enum NodeType { MainMenu, SubMenu, MenuOption }

		public enum NodeState { None, All, Mixed }

		public enum MenuOptions { Custom, Audio, Video, Language, NewGame, LoadGame, Credits, Exit, Back, Continue }

		[System.Serializable]
		public class NodeData
		{
			public bool Opened;  //Is opened
			public string Name;  //Name of the node
			public NodeType nodeType; //Type of node to add
			public NodeState nodeState = NodeState.None;
			public bool markForDelete = false;
			public MenuOptions menuOption;

			//Audio Checks
			public bool masterSlider=true, musicSlider = true, FXSlider = true, voiceSlider = true, askForAudioChanges = true;

			//Video checks
			public bool fullScreen = true, presets = true, resolution = true, textureQuality = true,
				antialiasing = true, vSync = true, anisotropic = true, shadowquality = true,
				shadowresolution = true, softParticles = true, softVegetation = true, askForVideoChanges=true;

			//Language checks
			public bool masterLanguage = true, subtitlesOnOff = true, subtitlesLanguage = true, 
				customShowWindow=true, customShowButton=true, askForLanguageChanges = true, integrateLocalizationPro=true;
			public int customOptionId;

			//Exit & others
			public bool showConfirmExit = true, showConfirmNewGame = true, scrollCredits=false;
			public float scrollCreditsSpeed = 1;

			public void CopyFrom(NodeData p, bool onlyName=false)
			{				
				Opened = p.Opened;				
				if (onlyName)
				{
					List<string> fullPath = new List<string>(p.Name.Split(new string[] { separator }, System.StringSplitOptions.RemoveEmptyEntries));					
					Name = fullPath[fullPath.Count - 1];
				}
				else
					Name = p.Name;
				nodeType = p.nodeType;
				nodeState = p.nodeState;
				markForDelete = p.markForDelete;
				menuOption = p.menuOption;
				masterSlider = p.masterSlider;
				musicSlider = p.musicSlider;
				FXSlider = p.FXSlider;
				voiceSlider = p.voiceSlider;
				askForAudioChanges = p.askForAudioChanges;
				fullScreen = p.fullScreen;
				presets = p.presets;
				resolution = p.resolution;
				textureQuality = p.textureQuality;
				antialiasing = p.antialiasing;
				vSync = p.vSync;
				anisotropic = p.anisotropic;
				shadowquality = p.shadowquality;
				shadowresolution = p.shadowresolution;
				softParticles = p.softParticles;
				softVegetation = p.softVegetation;
				askForVideoChanges = p.askForVideoChanges;
				masterLanguage = p.masterLanguage;
				subtitlesOnOff = p.subtitlesOnOff;
				subtitlesLanguage = p.subtitlesLanguage;
				askForLanguageChanges = p.askForLanguageChanges;
				integrateLocalizationPro = p.integrateLocalizationPro;
				showConfirmExit = p.showConfirmExit;
				showConfirmNewGame = p.showConfirmNewGame;
				customShowWindow = p.customShowWindow;
				customOptionId = p.customOptionId;
				customShowButton = p.customShowButton;
				scrollCredits = p.scrollCredits;
				scrollCreditsSpeed = p.scrollCreditsSpeed;

			}
		}

		public NodeData menuRoot;
		public List<NodeData> menuItems = new List<NodeData>();

		//Character used to separate menu paths. It is the only one cannot appear in the menu names
		public static string separator = "#";
		public bool editing = false;
		int itemEdited = 0;


		public MenuListView(string name, bool enabled)
		{
			NodeData nodeData = new NodeData();
			nodeData.Name = name;
			nodeData.nodeType = NodeType.MainMenu;
			nodeData.Opened = true;
			menuRoot = nodeData;
			InitializeMainMenu();
		}

		public void RenameMenu(string newValue)
		{
			menuRoot.Name = newValue;
		}

		void InitializeMainMenu()
		{
			AddListElement("Play", NodeType.SubMenu);
			AddListElement("Play#New Game", NodeType.MenuOption, MenuOptions.NewGame);
			AddListElement("Play#Load Game", NodeType.MenuOption, MenuOptions.LoadGame);
			AddListElement("Play#Continue Game", NodeType.MenuOption, MenuOptions.Continue);
			AddListElement("Play#Back", NodeType.MenuOption, MenuOptions.Back);
			AddListElement("Options", NodeType.SubMenu);
			AddListElement("Options#Video", NodeType.MenuOption, MenuOptions.Video);
			AddListElement("Options#Audio", NodeType.MenuOption, MenuOptions.Audio);
			AddListElement("Options#Language", NodeType.MenuOption, MenuOptions.Language);
			AddListElement("Options#Back", NodeType.MenuOption, MenuOptions.Back);
			AddListElement("Credits", NodeType.MenuOption, MenuOptions.Credits);
			AddListElement("Exit", NodeType.MenuOption, MenuOptions.Exit);
		}

		void AddListElement(string name, NodeType nType)
		{
			NodeData nodeData = new NodeData();
			nodeData.Name = name;
			nodeData.nodeType = nType;
			nodeData.Opened = true;
			menuItems.Add(nodeData);
		}

		void AddListElement(string name, NodeType nType, MenuOptions option)
		{
			NodeData nodeData = new NodeData();
			nodeData.Name = name;
			nodeData.nodeType = nType;
			nodeData.menuOption = option;
			nodeData.Opened = true;
			menuItems.Add(nodeData);
		}

		public void OpenAll()
		{
			for (int i = 0; i < menuItems.Count; i++)
				menuItems[i].Opened = true;
		}

		public void CloseAll()
		{
			for (int i = 0; i < menuItems.Count; i++)
				menuItems[i].Opened = false;
		}

		public static void PropagateChanges(string ltv)
		{

		}


		public static Texture2D iconMainMenu, iconSubMenu, iconOption, iconExit,
			iconBack, iconUp, iconDown, blever, iconAdd, iconDelete;
		public void LoadTextures()
		{
			iconMainMenu = Resources.Load("Icons/MainMenu") as Texture2D;
			iconSubMenu = Resources.Load("Icons/SubMenu") as Texture2D;
			iconOption = Resources.Load("Icons/MenuOption") as Texture2D;
			iconExit = Resources.Load("Icons/Exit") as Texture2D;
			iconBack = Resources.Load("Icons/Back") as Texture2D;
			iconUp = Resources.Load("Icons/MoveUp") as Texture2D;
			iconDown = Resources.Load("Icons/MoveDown") as Texture2D;
			blever = Resources.Load("Icons/blever") as Texture2D;
			iconAdd = Resources.Load("Icons/Add") as Texture2D;
			iconDelete = Resources.Load("Icons/Delete") as Texture2D;
		}


		public void DrawGUILayout(float width, float Height)
		{
			if (iconSubMenu == null)
				LoadTextures();

			float elementHeight = 20;

			Rect ltvRect = new Rect(0, 0, width, elementHeight);

			DrawElement(ref ltvRect, -1);

			int pos = FindElement(0, "");
			while (pos != -1)
			{
				DrawRow(ref ltvRect, pos);
				pos = FindElement(pos + 1, "");
			}
		}

		void DrawRow(ref Rect ltvRect, int i)
		{
			if (i == -1 || i>=menuItems.Count)
				return;
			DrawElement(ref ltvRect, i);
			if (!menuItems[i].Opened)
				return;
			int pos = FindElement(0, menuItems[i].Name);
			while (pos != -1)
			{

				DrawRow(ref ltvRect, pos);
				pos = FindElement(pos + 1, menuItems[i].Name);
			}
		}

		bool IdenticalPaths(List<string> p1, List<string> p2)
		{
			if (p1.Count == p2.Count)
			{
				for (int i = 0; i < p1.Count; i++)
					if (!p1[i].Equals(p2[i]))
						return false;
				return true;
			}
			return false;
		}

		public int FindElement(int from, string parent)
		{
			List<string> parentPath = new List<string>(parent.Split(new string[] { separator }, System.StringSplitOptions.RemoveEmptyEntries));
			for (int i = from; i < menuItems.Count; i++)
			{
				List<string> fullPath = new List<string>(menuItems[i].Name.Split(new string[] { separator }, System.StringSplitOptions.RemoveEmptyEntries));
				string nodeName = fullPath[fullPath.Count - 1];
				fullPath.RemoveAt(fullPath.Count - 1);
				if (IdenticalPaths(fullPath, parentPath))
					return i;
			}
			return -1;
		}

		bool ElementExists(string name)
		{
			for (int i = 0; i < menuItems.Count; i++)
				if (menuItems[i].Name.Equals(name))
					return true;
			return false;
		}

		int ItemLevel(int index)
		{
			List<string> fullPath = new List<string>(menuItems[index].Name.Split(new string[] { separator }, System.StringSplitOptions.RemoveEmptyEntries));
			return fullPath.Count - 1;
			//string nodeName = fullPath[fullPath.Count - 1];
			//fullPath.RemoveAt(fullPath.Count - 1);
		}

		public string ItemName(int index)
		{
			List<string> fullPath = new List<string>(menuItems[index].Name.Split(new string[] { separator }, System.StringSplitOptions.RemoveEmptyEntries));
			return fullPath[fullPath.Count - 1];
		}

		public string ItemName(string fullName)
		{
			List<string> fullPath = new List<string>(fullName.Split(new string[] { separator }, System.StringSplitOptions.RemoveEmptyEntries));
			return fullPath[fullPath.Count - 1];
		}

		public string ItemParent(int index)
		{
			List<string> fullPath = new List<string>(menuItems[index].Name.Split(new string[] { separator }, System.StringSplitOptions.RemoveEmptyEntries));
			if (fullPath.Count == 1)
				return "";
			string parent = menuItems[index].Name.Substring(0, menuItems[index].Name.Length - (fullPath[fullPath.Count - 1].Length + 1));
			return parent;
		}

		void MoveUp(int index)
		{
			int swap = 0;
			int found = FindElement(0, ItemParent(index));
			if (found == index || found == -1)
			{
				Debug.Log("Cannot find element to swap up... this should not happen!");
				return;
			}
			while (found != index && found != -1)
			{
				swap = found;
				found = FindElement(found + 1, ItemParent(index));
			}
			NodeData temp = menuItems[index];
			menuItems[index] = menuItems[swap];
			menuItems[swap] = temp;
		}

		void MoveDown(int index)
		{
			int swap = FindElement(index + 1, ItemParent(index));
			if (swap == -1)
			{
				Debug.Log("Cannot find element to swap down... this should not happen!");
				return;
			}
			NodeData temp = menuItems[index];
			menuItems[index] = menuItems[swap];
			menuItems[swap] = temp;
		}

		void DeleteCascade(int index)
		{
			RecurseDelete(index);
			for (int i = menuItems.Count - 1; i >= 0; i--)
				if (menuItems[i].markForDelete)
					menuItems.RemoveAt(i);
		}

		void RecurseDelete(int index)
		{
			int pos = FindElement(0, menuItems[index].Name);
			while (pos != -1)
			{
				RecurseDelete(pos);
				pos = FindElement(pos + 1, menuItems[index].Name);
			}
			menuItems[index].markForDelete = true;
		}

		protected void DrawElement(ref Rect ltvRect, int pos)
		{
			if (pos == -1) //Root Element => Menu Name
			{
				Rect newRect = ltvRect;
				newRect.y += 5;
				newRect.x += 5;

				Rect textureRect = new Rect(10, newRect.y + 2, 16, 16);
				if (GUI.Button(textureRect, iconAdd, GUIStyle.none))
				{
					Debug.Log("Add");
					AddItem(-1);
				}
				newRect.x += 22;
				newRect.width = 5000;
				GUIStyle style = new GUIStyle(EditorStyles.label);
				style.onNormal.textColor = new Color(0.6f, 0.6f, 1.0f, 1.0f);
				style.fontStyle = FontStyle.Bold;
				style.fontSize = 18;
				GUI.Label(newRect, menuRoot.Name, style);
				ltvRect.y += ltvRect.height;
			}
			else
				DrawElement(ref ltvRect, pos, new Color(0.6f, 0.6f, 1.0f, 1.0f));
		}

		void ReparentItemsAt(int index)
		{
			string toSearch = menuItems[index].Name + separator;
			string parent = ItemParent(index);
			if (parent.Length > 0)
				parent += separator;

			for (int i = 0; i < menuItems.Count; i++)
				if (menuItems[i].Name.StartsWith(toSearch))
					menuItems[i].Name = parent + ItemName(i);
		}

		void RenameParentAndChilds(string parent, string newParent)
		{
			string toSearch = parent + separator;
			string toChange = newParent + separator;

			for (int i = 0; i < menuItems.Count; i++)
				if (menuItems[i].Name.StartsWith(toSearch))
					menuItems[i].Name = toChange + ItemName(i);
		}

		void AddItem(int pos)
		{
			int insertAt = 0;
			string name = "";
			if (pos != -1)
			{
				insertAt = FindElement(0, menuItems[pos].Name) + 1;
				name = menuItems[pos].Name + separator;
			}

			NodeData nodeData = new NodeData();
			nodeData.Name = name + "New Element";

			int i = 1;
			while (ElementExists(nodeData.Name))
			{
				nodeData.Name = name + "New Element(" + i + ")";
				i++;
			}

			nodeData.nodeType = NodeType.MenuOption;
			nodeData.Opened = true;
			nodeData.menuOption = MenuOptions.Custom;
			menuItems.Insert(insertAt, nodeData);

			editing = true;
			itemEdited = insertAt;
			editedItem.CopyFrom(menuItems[itemEdited], true);
		}

		protected void DrawElement(ref Rect ltvRect, int pos, Color BoldColor)
		{
			Rect newRect = ltvRect;
			newRect.y += 10;

			Rect textureRect = new Rect(10, newRect.y, 16, 16);
			if (menuItems[pos].nodeType == NodeType.SubMenu)
			{
				if (GUI.Button(textureRect, iconAdd, GUIStyle.none))
				{
					Debug.Log("ADD");
					menuItems[pos].Opened = true;
					AddItem(pos);
				}
			}
			textureRect = new Rect(32, newRect.y, 16, 16);
			if (GUI.Button(textureRect, iconDelete, GUIStyle.none))
			{
				editing = false;
				if (EditorUtility.DisplayDialog("Delete this Menu Item?",
				"Are you sure you want to delete this Menu Item?", "Delete", "No Delete"))
				{
					DeleteCascade(pos);
					return;
				}
			}

			bool top = false;
			bool bottom = false;
			if (FindElement(0, ItemParent(pos)) == pos)
				top = true;
			if (FindElement(pos + 1, ItemParent(pos)) == -1)
				bottom = true;

			textureRect = new Rect(54, newRect.y, 16, 16);
			if (!top)
			{
				if (GUI.Button(textureRect, iconUp, GUIStyle.none))
				{
					editing = false;
					MoveUp(pos);
				}
			}

			textureRect = new Rect(76, newRect.y, 16, 16);
			if (!bottom)
			{
				if (GUI.Button(textureRect, iconDown, GUIStyle.none))
				{
					editing = false;
					MoveDown(pos);
				}
			}

			newRect.x += 109;

			newRect.x += 20 * ItemLevel(pos);

			if (menuItems[pos].nodeType == NodeType.SubMenu)
			{
				newRect.width = 13;
				menuItems[pos].Opened = EditorGUI.Foldout(newRect, menuItems[pos].Opened, "", true);
				newRect.x += 2;
			}


			newRect.width = 1000;
			GUIStyle style = new GUIStyle(EditorStyles.label);
			style.onNormal.textColor = BoldColor;
			Texture2D tx = null;
			switch (menuItems[pos].nodeType)
			{
				case NodeType.MainMenu:
					tx = iconMainMenu;
					break;
				case NodeType.SubMenu:
					tx = iconSubMenu;
					break;
				case NodeType.MenuOption:
					tx = iconOption;
					break;
			}


			textureRect = new Rect(newRect.x, newRect.y, 16, 16);
			GUI.DrawTexture(textureRect, tx);
			newRect.x += 18;

			if (GUI.Button(newRect, ItemName(pos), GUIStyle.none))
			{
				if (editing == true && itemEdited == pos)
					editing = false;
				else 
				{
					editing = true;
					itemEdited = pos;
					editedItem.CopyFrom(menuItems[itemEdited], true);
				}
			}

			Rect windowSize = EditorGUILayout.GetControlRect();
			if (!top || !bottom)
			{
				textureRect = new Rect(50, newRect.y - 2, windowSize.width - 58, 1);
				GUI.DrawTexture(textureRect, blever);
			}

			if (editing && pos == itemEdited)
				ltvRect.y += DrawEditBox(ltvRect);
			
			ltvRect.y += ltvRect.height;
		}

		Vector2 scrollPosition;
		NodeData editedItem=new NodeData();
		float DrawEditBox(Rect oldRect)
		{
			int toolbarInt;
			float defaultHeight = 150;
			GUIStyle style = new GUIStyle(EditorStyles.label);
			style.onNormal.textColor = new Color(0.6f, 0.6f, 1.0f, 1.0f);
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 14;
			style.alignment = TextAnchor.MiddleLeft;
			
			Vector2 itemName = new Vector2(5, 0);
			Vector2 itemType = new Vector2(5, 45);
			Vector2 itemAction = new Vector2(140, 45);			

			Rect windowSize = EditorGUILayout.GetControlRect(false, 25);
			Rect scr = new Rect(8, oldRect.y + 34, windowSize.width - 8, 82);
			

			GUI.Label(new Rect(scr.x + itemName.x, scr.y + itemName.y, 220, 15), "Item Name", style);
			GUI.Label(new Rect(scr.x+ itemType.x, scr.y+ itemType.y, 150, 15), "Item Type", style);			

			//Item type controls
			Texture[] myTextures = new Texture[] { iconOption, iconSubMenu };
			toolbarInt= editedItem.nodeType == NodeType.MenuOption?0 : 1;
			toolbarInt = GUI.Toolbar(new Rect(scr.x + itemType.x, scr.y+ itemType.y + 18, 110, 20), toolbarInt, myTextures);
			editedItem.nodeType = toolbarInt == 0 ? NodeType.MenuOption : NodeType.SubMenu;

			//Item name controls
			editedItem.Name = GUI.TextField(new Rect(scr.x + itemName.x, scr.y + itemName.y + 18, 290, 22), editedItem.Name);

			//Item, action controls
			if (editedItem.nodeType == NodeType.MenuOption)
			{
				GUI.Label(new Rect(scr.x + itemAction.x, scr.y + itemAction.y, 150, 15), "Item Action", style);
				editedItem.menuOption = (MenuOptions)EditorGUI.EnumPopup(new Rect(scr.x + itemAction.x, scr.y + itemAction.y + 20, 150, 15), "", editedItem.menuOption);
			}

			style = new GUIStyle(EditorStyles.label);
			style.alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(scr.x+ itemType.x, scr.y + itemType.y + 37, 55, 14), "Option", style);
			GUI.Label(new Rect(scr.x + itemType.x + 55, scr.y + itemType.y + 37, 55, 14), "Submenu", style);

			if (editedItem.nodeType == NodeType.MenuOption) {
				//Draw Action Options
				switch (editedItem.menuOption)
				{
					case MenuOptions.Audio:
						defaultHeight += DrawAudioOptions(scr);
						break;
					case MenuOptions.Video:
						defaultHeight += DrawVideoOptions(scr);
						break;
					case MenuOptions.Language:
						defaultHeight += DrawLanguageOptions(scr);
						break;
					case MenuOptions.Exit:
						defaultHeight += DrawExitOptions(scr);
						break;
					case MenuOptions.NewGame:
						defaultHeight += DrawNewGameOptions(scr);
						break;
					case MenuOptions.Credits:
						defaultHeight += DrawCreditsOptions(scr);
						break;
					case MenuOptions.Custom:
						defaultHeight += DrawCustomOptions(scr);
						break;
				}
			}
			Vector2 butonApply = new Vector2(5, defaultHeight-45);
			Vector2 butonCancel = new Vector2(160, defaultHeight-45);

			if (GUI.Button(new Rect(scr.x + butonApply.x, scr.y + butonApply.y, 130, 23), "Apply Changes"))
			{
				string parent = ItemParent(itemEdited);
				if (parent.Length > 0)
					parent += separator;
				editedItem.Name = parent + editedItem.Name;

				if (menuItems[itemEdited].nodeType == NodeType.SubMenu && !editedItem.Name.Equals(menuItems[itemEdited].Name))
				{
					RenameParentAndChilds(menuItems[itemEdited].Name, editedItem.Name);
					menuItems[itemEdited].CopyFrom(editedItem);
				}

				else if (menuItems[itemEdited].nodeType == NodeType.SubMenu && editedItem.nodeType == NodeType.MenuOption)
				{
					if (EditorUtility.DisplayDialog("Change from Submenu to Option?",
					"If you change the Item Type from Submenu to Option all Options currently in this Submenu will pass to the parent.", 
					"Apply", "Cancel"))
					{
						ReparentItemsAt(itemEdited);
						menuItems[itemEdited].CopyFrom(editedItem);
					}
				}
				else
					menuItems[itemEdited].CopyFrom(editedItem);

				editing = false;
			}

			if (GUI.Button(new Rect(scr.x + butonCancel.x, scr.y + butonCancel.y, 130, 23), "Cancel"))
				editing = false;

			GUILayout.Box("", EditorStyles.helpBox, GUILayout.Height(defaultHeight - 10));
			return defaultHeight;
		}

		float DrawAudioOptions(Rect scr)
		{
			GUIStyle style = new GUIStyle(EditorStyles.label);
			style.onNormal.textColor = new Color(0.6f, 0.6f, 1.0f, 1.0f);
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 14;
			style.alignment = TextAnchor.MiddleLeft;

			Vector2 audioTitle = new Vector2(5, 103);
			Vector2 toggleBlock = new Vector2(5, 125);
			Vector2 dialogTitle = new Vector2(5, 165);

			GUI.Label(new Rect(scr.x + audioTitle.x, scr.y + audioTitle.y, 220, 18), "Audio Settings to Show", style);

			editedItem.masterSlider = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x, scr.y + toggleBlock.y, 140, 15), "Master Slider", editedItem.masterSlider);
			editedItem.musicSlider = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x + 150, scr.y + toggleBlock.y, 140, 15), "Music Slider", editedItem.musicSlider);
			editedItem.FXSlider = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x, scr.y + toggleBlock.y + 20, 140, 15), "FX Slider", editedItem.FXSlider);
			editedItem.voiceSlider = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x + 150, scr.y + toggleBlock.y + 20, 140, 15), "Voice Slider", editedItem.voiceSlider);

			GUI.Label(new Rect(scr.x + dialogTitle.x, scr.y + dialogTitle.y, 220, 18), "Other Options", style);
			editedItem.askForAudioChanges = EditorGUI.ToggleLeft(new Rect(scr.x + dialogTitle.x, scr.y + dialogTitle.y + 22, 140, 15), "Ask for Confirmation", editedItem.askForAudioChanges);

			return 107;
		}

		float DrawVideoOptions(Rect scr)
		{
			GUIStyle style = new GUIStyle(EditorStyles.label);
			style.onNormal.textColor = new Color(0.6f, 0.6f, 1.0f, 1.0f);
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 14;
			style.alignment = TextAnchor.MiddleLeft;

			Vector2 audioTitle = new Vector2(5, 103);
			Vector2 toggleBlock = new Vector2(5, 125);
			Vector2 dialogTitle = new Vector2(5, 245);

			GUI.Label(new Rect(scr.x + audioTitle.x, scr.y + audioTitle.y, 220, 18), "Video Settings to Show", style);

			editedItem.fullScreen = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x, scr.y + toggleBlock.y, 140, 15), "Full Screen", editedItem.fullScreen);
			editedItem.presets = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x + 150, scr.y + toggleBlock.y, 140, 15), "Quality Presets", editedItem.presets);
			editedItem.resolution = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x, scr.y + toggleBlock.y + 20, 140, 15), "Available Resolutions", editedItem.resolution);
			editedItem.textureQuality = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x + 150, scr.y + toggleBlock.y + 20, 140, 15), "Texture Quality", editedItem.textureQuality);
			editedItem.antialiasing = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x, scr.y + toggleBlock.y+40, 140, 15), "Antialiasing", editedItem.antialiasing);
			editedItem.vSync = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x + 150, scr.y + toggleBlock.y+40, 140, 15), "VSync Enabler", editedItem.vSync);
			editedItem.anisotropic = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x, scr.y + toggleBlock.y + 60, 140, 15), "Anisotropic Filtering", editedItem.anisotropic);
			editedItem.softParticles = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x + 150, scr.y + toggleBlock.y + 60, 140, 15), "Soft Particles", editedItem.softParticles);
			editedItem.shadowquality = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x, scr.y + toggleBlock.y + 80, 140, 15), "Shadow Quality", editedItem.shadowquality);
			editedItem.shadowresolution = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x + 150, scr.y + toggleBlock.y + 80, 140, 15), "Shadow Resolution", editedItem.shadowresolution);
			editedItem.softVegetation = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x, scr.y + toggleBlock.y + 100, 140, 15), "Soft Vegetation", editedItem.softVegetation);

			GUI.Label(new Rect(scr.x + dialogTitle.x, scr.y + dialogTitle.y, 220, 18), "Other Options", style);
			editedItem.askForVideoChanges = EditorGUI.ToggleLeft(new Rect(scr.x + dialogTitle.x, scr.y + dialogTitle.y+22, 140, 15), "Ask for Confirmation", editedItem.askForVideoChanges);

			return 187;
		}

		float DrawLanguageOptions(Rect scr)
		{
			GUIStyle style = new GUIStyle(EditorStyles.label);
			style.onNormal.textColor = new Color(0.6f, 0.6f, 1.0f, 1.0f);
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 14;
			style.alignment = TextAnchor.MiddleLeft;

			Vector2 audioTitle = new Vector2(5, 103);
			Vector2 toggleBlock = new Vector2(5, 125);
			Vector2 dialogTitle = new Vector2(5, 165);

			GUI.Label(new Rect(scr.x + audioTitle.x, scr.y + audioTitle.y, 220, 18), "Language Settings to Show", style);

			editedItem.masterLanguage = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x, scr.y + toggleBlock.y, 140, 15), "Language Selector", editedItem.masterLanguage);
			editedItem.subtitlesOnOff = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x + 150, scr.y + toggleBlock.y, 140, 15), "Subtitles On/Off", editedItem.subtitlesOnOff);
			editedItem.subtitlesLanguage = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x, scr.y + toggleBlock.y + 20, 140, 15), "Subtitles language", editedItem.subtitlesLanguage);

			GUI.Label(new Rect(scr.x + dialogTitle.x, scr.y + dialogTitle.y, 220, 18), "Other Options", style);
			editedItem.askForLanguageChanges = EditorGUI.ToggleLeft(new Rect(scr.x + dialogTitle.x, scr.y + dialogTitle.y + 22, 140, 15), "Ask for Confirmation", editedItem.askForLanguageChanges);
			editedItem.integrateLocalizationPro = EditorGUI.ToggleLeft(new Rect(scr.x + dialogTitle.x+150, scr.y + dialogTitle.y + 22, 140, 15), "Use Localization Pro", editedItem.integrateLocalizationPro);

			if(editedItem.integrateLocalizationPro)
			{
				style.fontStyle = FontStyle.Normal;
				style.fontSize = 12;
				GUI.Label(new Rect(scr.x + dialogTitle.x, scr.y + dialogTitle.y + 42, 220, 38), "This asset may integrate directly with\nLocalization Pro", style);
				return 144;
			}

			return 107;
		}

		float DrawCreditsOptions(Rect scr)
		{
			GUIStyle style = new GUIStyle(EditorStyles.label);
			style.onNormal.textColor = new Color(0.6f, 0.6f, 1.0f, 1.0f);
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 14;
			style.alignment = TextAnchor.MiddleLeft;

			Vector2 audioTitle = new Vector2(5, 103);
			Vector2 toggleBlock = new Vector2(5, 125);

			GUI.Label(new Rect(scr.x + audioTitle.x, scr.y + audioTitle.y, 220, 18), "Credits Settings to Show", style);

			editedItem.scrollCredits = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x, scr.y + toggleBlock.y, 140, 15), "Add Scrollview", editedItem.scrollCredits);
			if (editedItem.scrollCredits)
			{
				style.fontStyle = FontStyle.Normal;
				style.fontSize = 12;
				GUI.Label(new Rect(scr.x + toggleBlock.x, scr.y + toggleBlock.y + 20, 220, 18), "Scroll speed: ", style);
				editedItem.scrollCreditsSpeed = EditorGUI.Slider(new Rect(scr.x + toggleBlock.x + 150, scr.y + toggleBlock.y + 20, 140, 15), editedItem.scrollCreditsSpeed, 0.1f, 5f);
				return 65;
			}

			return 45;
		}

		float DrawExitOptions(Rect scr)
		{
			GUIStyle style = new GUIStyle(EditorStyles.label);
			style.onNormal.textColor = new Color(0.6f, 0.6f, 1.0f, 1.0f);
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 14;
			style.alignment = TextAnchor.MiddleLeft;

			Vector2 audioTitle = new Vector2(5, 103);
			Vector2 toggleBlock = new Vector2(5, 125);

			GUI.Label(new Rect(scr.x + audioTitle.x, scr.y + audioTitle.y, 220, 18), "Exit Options", style);

			editedItem.showConfirmExit= EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x, scr.y + toggleBlock.y, 140, 15), "Show Confirm Dialog", editedItem.showConfirmExit);			

			return 45;
		}

		float DrawNewGameOptions(Rect scr)
		{
			GUIStyle style = new GUIStyle(EditorStyles.label);
			style.onNormal.textColor = new Color(0.6f, 0.6f, 1.0f, 1.0f);
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 14;
			style.alignment = TextAnchor.MiddleLeft;

			Vector2 audioTitle = new Vector2(5, 103);
			Vector2 toggleBlock = new Vector2(5, 125);

			GUI.Label(new Rect(scr.x + audioTitle.x, scr.y + audioTitle.y, 220, 18), "New Game Options", style);

			editedItem.showConfirmNewGame = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x, scr.y + toggleBlock.y, 140, 15), "Show Confirm Dialog", editedItem.showConfirmNewGame);

			return 45;
		}

		float DrawCustomOptions(Rect scr)
		{
			GUIStyle style = new GUIStyle(EditorStyles.label);
			style.onNormal.textColor = new Color(0.6f, 0.6f, 1.0f, 1.0f);
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 14;
			style.alignment = TextAnchor.MiddleLeft;

			Vector2 audioTitle = new Vector2(5, 103);
			Vector2 toggleBlock = new Vector2(5, 125);

			GUI.Label(new Rect(scr.x + audioTitle.x, scr.y + audioTitle.y, 220, 18), "Custom Options", style);

			editedItem.customShowWindow = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x, scr.y + toggleBlock.y, 140, 15), "Display a Window", editedItem.customShowWindow);
			if (editedItem.customShowWindow)
				editedItem.customShowButton = EditorGUI.ToggleLeft(new Rect(scr.x + toggleBlock.x + 150, scr.y + toggleBlock.y, 140, 15), "Back Button", editedItem.customShowButton);
			else
			{
				editedItem.customOptionId = EditorGUI.IntField(new Rect(scr.x + toggleBlock.x, scr.y + toggleBlock.y + 22, 240, 15), "Option ID", editedItem.customOptionId);
				return 67;
			}

			return 45;
		}

		public int TreeElements()
		{
			return menuItems.Count;
		}

	}
}
#endif