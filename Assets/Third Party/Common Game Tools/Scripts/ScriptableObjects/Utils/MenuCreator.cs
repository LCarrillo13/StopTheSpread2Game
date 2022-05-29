#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor.Events;


namespace CGT
{
	public class MenuCreator
	{

		public static void CreateMenuPrefab(MenuListView menu, string name, MenuConfiguration menuConfiguration)		
		{
			GameObject Menu = null;
			switch(menuConfiguration.template)
			{
				case MenuConfiguration.MenuTemplates.defaultTemplate:
					Menu = Resources.Load("MenuTemplate/DefaultTemplate") as GameObject;
					break;
				case MenuConfiguration.MenuTemplates.Modern:
					Menu = Resources.Load("MenuTemplate/ModernTemplate") as GameObject;
					break;				
				case MenuConfiguration.MenuTemplates.Animated:
					Menu = Resources.Load("MenuTemplate/AnimatedTemplate") as GameObject;
					break;
				case MenuConfiguration.MenuTemplates.Office:
					Menu = Resources.Load("MenuTemplate/OfficeTemplate") as GameObject;
					break;
				case MenuConfiguration.MenuTemplates.OwnTemplate:
					Menu = menuConfiguration.overrideTemplate;
					break;
			}			

			GameObject instanceRoot = (GameObject) PrefabUtility.InstantiatePrefab(Menu);
			MenuController menuController = instanceRoot.transform.Find("MenuController").gameObject.GetComponent<MenuController>();

			instanceRoot.transform.Find("Canvas/MenuScaler").GetComponent<RectTransform>().localScale = new Vector3(menuConfiguration.menuScale, menuConfiguration.menuScale, menuConfiguration.menuScale);
			SetTextModifiers(menuConfiguration, instanceRoot);
			SetImageModifiers(menuConfiguration, instanceRoot);
			SetAudioModifiers(menuConfiguration, instanceRoot);
			SetButtonImagesAndColors(menuConfiguration, instanceRoot);


			CreateDialogs(menu, instanceRoot);
			CreateMenuButtons(menu, menuConfiguration, instanceRoot, menuController, null, null);
			instanceRoot.transform.Find("Canvas/MenuScaler/ExitDialog").SetAsLastSibling();
			instanceRoot.transform.Find("Canvas/MenuScaler/ExitWithoutApply").SetAsLastSibling();
			instanceRoot.transform.Find("Canvas/MenuScaler/ConfirmChanges").SetAsLastSibling();

			PrefabUtility.UnpackPrefabInstance(instanceRoot, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
			GameObject.DestroyImmediate(instanceRoot.transform.Find("Canvas/MenuScaler/CustomWindow").gameObject);
			GameObject.DestroyImmediate(instanceRoot.transform.Find("Canvas/MenuScaler/MenuButtons").gameObject);
			GameObject.DestroyImmediate(instanceRoot.transform.Find("Canvas/MenuScaler/MenuDialog").gameObject);
			if(instanceRoot.transform.Find("Canvas/MenuScaler/HorizontalMenuButtons") !=null)
				GameObject.DestroyImmediate(instanceRoot.transform.Find("Canvas/MenuScaler/HorizontalMenuButtons").gameObject);


			string localPath = "Assets/Common Game Tools/GeneratedMenu/" + menu.menuRoot.Name + ".prefab";
			localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
			PrefabUtility.SaveAsPrefabAsset(instanceRoot, localPath);
			GameObject.DestroyImmediate(instanceRoot);
			EditorUtility.DisplayDialog("Menu Prefab Created!", "Your menu prefab has been created at /Assets/Common Game Tools/GeneratedMenu/", "Ok");
			
		}

		public static void CreateDialogs(MenuListView menu, GameObject menuPrefab)
		{
			GameObject dialog = menuPrefab.transform.Find("Canvas/MenuScaler/MenuDialog").gameObject;

			GameObject exitDialog = GameObject.Instantiate(dialog, menuPrefab.transform.Find("Canvas/MenuScaler"));
			exitDialog.name="ExitDialog";
			exitDialog.GetComponent<DialogIdentifier>().dialog= DialogIdentifier.StandardDialogs.ExitDialog;
			exitDialog.transform.Find("Dialog/MainText").gameObject.GetComponent<Text>().text = "Exit Game";
			exitDialog.transform.Find("Dialog/SecondaryText").gameObject.GetComponent<Text>().text = "Are you sure you wish to exit the game?\nAll progress not saved will be lost";
			exitDialog.transform.Find("Dialog/ButtonYes/ButtonText").gameObject.GetComponent<Text>().text = "Yes";
			exitDialog.transform.Find("Dialog/ButtonNo/ButtonText").gameObject.GetComponent<Text>().text = "No";
			

			GameObject noApplyDialog = GameObject.Instantiate(dialog, menuPrefab.transform.Find("Canvas/MenuScaler"));
			noApplyDialog.name = "ExitWithoutApply";
			noApplyDialog.GetComponent<DialogIdentifier>().dialog = DialogIdentifier.StandardDialogs.NoApplyDialog;
			noApplyDialog.transform.Find("Dialog/MainText").gameObject.GetComponent<Text>().text = "Don't Apply";
			noApplyDialog.transform.Find("Dialog/SecondaryText").gameObject.GetComponent<Text>().text = "Changes made in the settings will not be applied";
			noApplyDialog.transform.Find("Dialog/ButtonYes/ButtonText").gameObject.GetComponent<Text>().text = "Ok";
			noApplyDialog.transform.Find("Dialog/ButtonNo/ButtonText").gameObject.GetComponent<Text>().text = "Cancel";
			menuPrefab.transform.Find("MenuController").gameObject.GetComponent<MenuController>().ExitWithoutApply = noApplyDialog;
			

			GameObject confirmChangesDialog = GameObject.Instantiate(dialog, menuPrefab.transform.Find("Canvas/MenuScaler"));
			confirmChangesDialog.name = "ConfirmChanges";
			confirmChangesDialog.GetComponent<DialogIdentifier>().dialog = DialogIdentifier.StandardDialogs.ConfirmChangesDialog;
			confirmChangesDialog.transform.Find("Dialog/MainText").gameObject.GetComponent<Text>().text = "Apply Changes?";
			confirmChangesDialog.transform.Find("Dialog/SecondaryText").gameObject.GetComponent<Text>().text = "Are you sure you wish to apply all changes?";
			confirmChangesDialog.transform.Find("Dialog/ButtonYes/ButtonText").gameObject.GetComponent<Text>().text = "Yes";
			confirmChangesDialog.transform.Find("Dialog/ButtonNo/ButtonText").gameObject.GetComponent<Text>().text = "No";
			menuPrefab.transform.Find("MenuController").gameObject.GetComponent<MenuController>().ConfirmChanges = confirmChangesDialog;
			
		}

		public static GameObject CreateMenuButtons(MenuListView menu, MenuConfiguration menuConfiguration, GameObject menuPrefab, MenuController menuController, string parentName, GameObject parentObject)
		{
			string toSearch = "";
			if (parentName != null)
				toSearch = parentName;

			GameObject buttonRow=null;
			GameObject button=null;

			//Exception for iffuce template! yes... is ugly but this is the place to locate this
			if (menuConfiguration.template == MenuConfiguration.MenuTemplates.Office && parentName == null)
			{
				buttonRow = menuPrefab.transform.Find("Canvas/MenuScaler/HorizontalMenuButtons").gameObject;
				button = menuPrefab.transform.Find("Canvas/MenuScaler/HorizontalMenuButtons/Layout/Button").gameObject;
			}
			else
			{
				buttonRow = menuPrefab.transform.Find("Canvas/MenuScaler/MenuButtons").gameObject;
				button = menuPrefab.transform.Find("Canvas/MenuScaler/MenuButtons/Layout/Button").gameObject;
			}

			GameObject newButtonRow=null;
			//= GameObject.Instantiate(buttonRow, menuPrefab.transform.Find("Canvas/MenuScaler"));
			GameObject buttonRowLayout =null;
			//= newButtonRow.transform.Find("Layout").gameObject;

			if (parentName == null) {
				newButtonRow = GameObject.Instantiate(buttonRow, menuPrefab.transform.Find("Canvas/MenuScaler"));
				newButtonRow.name = "MainMenu";
				newButtonRow.transform.SetSiblingIndex(2);
			}
			else
			{				
				//Set the menu parent of the submenu, if needed
				TemplateSet ts = buttonRow.GetComponent<TemplateSet>();
				if (ts != null)
				{								
					if (ts.useParent)
						newButtonRow = GameObject.Instantiate(buttonRow, parentObject.transform);	
					else
						newButtonRow = GameObject.Instantiate(buttonRow, menuPrefab.transform.Find("Canvas/MenuScaler"));
					
					newButtonRow.GetComponent<TemplateSet>().parent = parentObject;
					TemplateSet tsParent = GetFirstParentedFoundTemplateSet(newButtonRow.transform);
					if(tsParent)
						newButtonRow.GetComponent<TemplateSet>().level= tsParent.level+1;
					if(newButtonRow.GetComponent<TemplateSet>().level>1)
					{
						newButtonRow.GetComponent<RectTransform>().pivot = new Vector2(-1f, 0f);
						newButtonRow.GetComponent<RectTransform>().anchoredPosition= new Vector2(0f, 0f);
					}
				}
				else
					newButtonRow = GameObject.Instantiate(buttonRow, menuPrefab.transform.Find("Canvas/MenuScaler"));

				newButtonRow.name = menu.ItemName(parentName) + "Menu";
				newButtonRow.SetActive(false);
			}

			buttonRowLayout = newButtonRow.transform.Find("Layout").gameObject;

			//Set alignment and position
			SetMenuPosition(menuConfiguration, menuPrefab, newButtonRow);

			int pos = menu.FindElement(0, toSearch);
			while (pos != -1)
			{				
				GameObject newButton = GameObject.Instantiate(button, buttonRowLayout.transform);
				SetButtonPosition(menu, menuConfiguration, newButton, pos);
				
				Button btn = newButton.GetComponent<Button>();				
				if (menu.menuItems[pos].nodeType == MenuListView.NodeType.SubMenu)
				{
					//Debug.Log("2 [" + parentName + "] [" + parentObject + "]");
					//Recursively creates submenus					
					GameObject newRow=CreateMenuButtons(menu, menuConfiguration, menuPrefab, menuController, menu.menuItems[pos].Name, newButton);
					SetButtonSubMenu(menuController, newRow, btn);					
					
				}
				else
				{
					SetButtonOption(menu, menuController, menuPrefab, newButton, btn, pos);					
				}
				pos = menu.FindElement(pos + 1, toSearch);
			}
			GameObject.DestroyImmediate(newButtonRow.transform.Find("Layout/Button").gameObject);
			return newButtonRow;
		}				

		static TemplateSet GetFirstParentedFoundTemplateSet(Transform trf)
		{
			if (trf.parent == null)
				return null;

			if (trf.parent.gameObject.GetComponent<TemplateSet>() != null)
				return trf.parent.gameObject.GetComponent<TemplateSet>();
			return GetFirstParentedFoundTemplateSet(trf.parent);

		}

		static void SetTextModifiers(MenuConfiguration menuConfiguration, GameObject instanceRoot)
		{
			Color fgDarker = new Color(menuConfiguration.fgColor.r / 2.5f, menuConfiguration.fgColor.g / 2.5f, menuConfiguration.fgColor.b / 2.5f, menuConfiguration.fgColor.a);
			Text[] allText = instanceRoot.GetComponentsInChildren<Text>(true);
			foreach (Text txt in allText)
			{
				TemplateSet templateSet = txt.GetComponent<TemplateSet>();
				if (menuConfiguration.useColor)
				{
					if (templateSet != null)
					{
						Color newColor;
						switch (templateSet.colorMode)
						{
							case TemplateSet.ColorMode.textColor:
								txt.color = menuConfiguration.textColor;
								break;
							case TemplateSet.ColorMode.buttonColor:
								txt.color = menuConfiguration.btnColor;
								break;
							case TemplateSet.ColorMode.foreground:
								txt.color = menuConfiguration.fgColor;
								break;
							case TemplateSet.ColorMode.background:
								txt.color = menuConfiguration.bkColor;
								break;
							case TemplateSet.ColorMode.darkerForeground:
								txt.color = fgDarker;
								break;
							case TemplateSet.ColorMode.changable:
								txt.color = menuConfiguration.textColor;
								break;
							case TemplateSet.ColorMode.textColorPreserveAlpha:
								newColor = menuConfiguration.textColor;
								newColor.a = txt.color.a;
								txt.color = newColor;
								break;
							case TemplateSet.ColorMode.buttonColorPreserveAlpha:
								newColor = menuConfiguration.btnColor;
								newColor.a = txt.color.a;
								txt.color = newColor;
								txt.color = newColor;
								break;
							case TemplateSet.ColorMode.foregroundPreserveAlpha:
								newColor = menuConfiguration.fgColor;
								newColor.a = txt.color.a;
								txt.color = newColor;
								break;
							case TemplateSet.ColorMode.backgroundPreserveAlpha:
								newColor = menuConfiguration.bkColor;
								newColor.a = txt.color.a;
								txt.color = newColor;
								break;
							case TemplateSet.ColorMode.unchangable:
								break;
						}
					}
					else
						txt.color = menuConfiguration.textColor;
				}
				if (menuConfiguration.overrideFont != null)
				{
					if (templateSet == null || templateSet.fontMode == TemplateSet.FontMode.changable)
					{
						if (!menuConfiguration.dontOverrideDropdown || !(txt.gameObject.name.Equals("Item Label")  || txt.gameObject.name.Equals("Label")))
							txt.font = menuConfiguration.overrideFont;
					}
						
					float newSize = ((float)txt.fontSize) * menuConfiguration.fontMutiplier;
					if (!menuConfiguration.dontOverrideDropdown || !(txt.gameObject.name.Equals("Item Label") || txt.gameObject.name.Equals("Label")))
						txt.fontSize = (int)newSize;
					if (menuConfiguration.allowOverflow)
						txt.verticalOverflow = VerticalWrapMode.Overflow;
				}

			}
		}

		static void SetImageModifiers(MenuConfiguration menuConfiguration, GameObject instanceRoot)
		{
			Color fgDarker = new Color(menuConfiguration.fgColor.r / 2.5f, menuConfiguration.fgColor.g / 2.5f, menuConfiguration.fgColor.b / 2.5f, menuConfiguration.fgColor.a);
			if (menuConfiguration.useColor)
			{
				Image[] allImages = instanceRoot.GetComponentsInChildren<Image>(true);
				foreach (Image img in allImages)
				{
					TemplateSet templateSet = img.GetComponent<TemplateSet>();
					if (templateSet != null)
					{
						Color newColor;
						switch (templateSet.colorMode)
						{
							case TemplateSet.ColorMode.textColor:
								img.color = menuConfiguration.textColor;
								break;
							case TemplateSet.ColorMode.buttonColor:
								img.color = menuConfiguration.btnColor;
								break;
							case TemplateSet.ColorMode.foreground:
								img.color = menuConfiguration.fgColor;
								break;
							case TemplateSet.ColorMode.background:
								img.color = menuConfiguration.bkColor;
								break;
							case TemplateSet.ColorMode.darkerForeground:
								img.color = fgDarker;
								break;
							case TemplateSet.ColorMode.changable:
								img.color = menuConfiguration.fgColor;
								break;
							case TemplateSet.ColorMode.textColorPreserveAlpha:
								newColor = menuConfiguration.textColor;
								newColor.a = img.color.a;
								img.color = newColor;
								break;
							case TemplateSet.ColorMode.buttonColorPreserveAlpha:
								newColor = menuConfiguration.btnColor;
								newColor.a = img.color.a;
								img.color = newColor;
								break;
							case TemplateSet.ColorMode.foregroundPreserveAlpha:
								newColor = menuConfiguration.fgColor;
								newColor.a = img.color.a;
								img.color = newColor;
								break;
							case TemplateSet.ColorMode.backgroundPreserveAlpha:
								newColor = menuConfiguration.bkColor;
								newColor.a = img.color.a;
								img.color = newColor;
								break;
							case TemplateSet.ColorMode.unchangable:
								break;
						}
					}
					else
						img.color = menuConfiguration.fgColor;
				}
			}

			if (menuConfiguration.bgImage != null)
			{
				float aspect = MenuConfiguration.bgAspectRatioValues[menuConfiguration.selectedAspetRatio];
				if (menuConfiguration.selectedAspetRatio == 0)
					aspect = menuConfiguration.bgImage.textureRect.width / menuConfiguration.bgImage.textureRect.height;
				instanceRoot.transform.Find("Canvas/BGImage").gameObject.SetActive(true);
				instanceRoot.transform.Find("Canvas/BGImage").gameObject.GetComponent<Image>().sprite = menuConfiguration.bgImage;
				instanceRoot.transform.Find("Canvas/BGImage").gameObject.GetComponent<AspectRatioFitter>().aspectRatio = aspect;
			}
		}

		static void SetAudioModifiers(MenuConfiguration menuConfiguration, GameObject instanceRoot)
		{
			MenuController menuController = instanceRoot.transform.Find("MenuController").gameObject.GetComponent<MenuController>();
			menuController.bgMusic = menuConfiguration.bgMusic;
			menuController.clickEffect = menuConfiguration.clickEffect;
		}

		static void SetButtonImagesAndColors(MenuConfiguration menuConfiguration, GameObject menuPrefab)
		{
			if (menuConfiguration.buttonType == MenuConfiguration.ButtonType.useTemplate)
				return;
			Sprite normal = null, highlighted = null, pressed = null, disabled = null, panel=null;
			Sprite fill = null, frame = null;
			switch (menuConfiguration.buttonType)
			{
				case MenuConfiguration.ButtonType.squared:
					normal = Resources.Load<Sprite>("MenuTemplate/elements/squaredButton");
					highlighted = Resources.Load<Sprite>("MenuTemplate/elements/squaredButtonHighlighted");
					pressed = Resources.Load<Sprite>("MenuTemplate/elements/squaredButtonPressed");
					disabled = null;
					panel = Resources.Load<Sprite>("MenuTemplate/elements/squaredPanel");

					fill = Resources.Load<Sprite>("MenuTemplate/elements/squaredButtonFill");
					frame = Resources.Load<Sprite>("MenuTemplate/elements/squaredButtonFrame");
					break;
				case MenuConfiguration.ButtonType.chamfer:
					normal = Resources.Load<Sprite>("MenuTemplate/elements/chamferButton");
					highlighted = Resources.Load<Sprite>("MenuTemplate/elements/chamferButtonHighlighted");
					pressed = Resources.Load<Sprite>("MenuTemplate/elements/chamferButtonPressed");
					disabled = null;
					panel = Resources.Load<Sprite>("MenuTemplate/elements/chamferPanel");

					fill = Resources.Load<Sprite>("MenuTemplate/elements/chamferButtonFill");
					frame = Resources.Load<Sprite>("MenuTemplate/elements/chamferButtonFrame");
					break;
				case MenuConfiguration.ButtonType.Blevel:
					normal = Resources.Load<Sprite>("MenuTemplate/elements/blevelButton");
					highlighted = Resources.Load<Sprite>("MenuTemplate/elements/blevelButtonHighlighted");
					pressed = Resources.Load<Sprite>("MenuTemplate/elements/blevelButtonPressed");
					disabled = null;
					panel = Resources.Load<Sprite>("MenuTemplate/elements/blevelPanel");

					fill = Resources.Load<Sprite>("MenuTemplate/elements/blevelButtonFill");
					frame = Resources.Load<Sprite>("MenuTemplate/elements/blevelButtonFrame");
					break;
				case MenuConfiguration.ButtonType.Bordered:
					normal = Resources.Load<Sprite>("MenuTemplate/elements/borderButton");
					highlighted = Resources.Load<Sprite>("MenuTemplate/elements/borderButtonHighlighted");
					pressed = Resources.Load<Sprite>("MenuTemplate/elements/borderButtonPressed");
					disabled = null;
					panel = Resources.Load<Sprite>("MenuTemplate/elements/borderPanel");

					fill = Resources.Load<Sprite>("MenuTemplate/elements/borderButtonFill");
					frame = Resources.Load<Sprite>("MenuTemplate/elements/borderButtonFrame");
					break;
				case MenuConfiguration.ButtonType.Banner1:
					normal = Resources.Load<Sprite>("MenuTemplate/elements/banner1Button");
					highlighted = Resources.Load<Sprite>("MenuTemplate/elements/banner1ButtonHighlighted");
					pressed = Resources.Load<Sprite>("MenuTemplate/elements/banner1ButtonPressed");
					disabled = null;
					panel = Resources.Load<Sprite>("MenuTemplate/elements/squaredPanel");

					fill = Resources.Load<Sprite>("MenuTemplate/elements/banner1ButtonFill");
					frame = Resources.Load<Sprite>("MenuTemplate/elements/banner1ButtonFrame");
					break;
				case MenuConfiguration.ButtonType.Banner2:
					normal = Resources.Load<Sprite>("MenuTemplate/elements/banner2Button");
					highlighted = Resources.Load<Sprite>("MenuTemplate/elements/banner2ButtonHighlighted");
					pressed = Resources.Load<Sprite>("MenuTemplate/elements/banner2ButtonPressed");
					disabled = null;
					panel = Resources.Load<Sprite>("MenuTemplate/elements/squaredPanel");

					fill = Resources.Load<Sprite>("MenuTemplate/elements/banner2ButtonFill");
					frame = Resources.Load<Sprite>("MenuTemplate/elements/banner2ButtonFrame");
					break;
				case MenuConfiguration.ButtonType.custom:
					normal = menuConfiguration.normal;
					highlighted = menuConfiguration.highlighted;
					pressed = menuConfiguration.pressed;
					disabled = menuConfiguration.disabled;
					panel = menuConfiguration.panel;

					fill = menuConfiguration.fill;
					frame = menuConfiguration.frame;
					break;
			}

			Button[] allButtons = menuPrefab.GetComponentsInChildren<Button>(true);
			foreach (Button btn in allButtons)
			{
				if (normal != null)
					btn.image.sprite = normal;

				if (menuConfiguration.template==MenuConfiguration.MenuTemplates.defaultTemplate) { 					
					SpriteState st = new SpriteState();
					st.disabledSprite = disabled;
					st.highlightedSprite = highlighted;
					st.pressedSprite = pressed;
					btn.spriteState = st;
				}
				else if (menuConfiguration.template == MenuConfiguration.MenuTemplates.Animated)
				{
					btn.transform.Find("Fill").gameObject.GetComponent<Image>().sprite = fill;
					btn.transform.Find("Frame").gameObject.GetComponent<Image>().sprite = frame;
				}

				Color fgDarker = new Color(menuConfiguration.fgColor.r / 2.5f, menuConfiguration.fgColor.g / 2.5f, menuConfiguration.fgColor.b / 2.5f, menuConfiguration.fgColor.a);
				TemplateSet templateSetBtn = btn.GetComponent<TemplateSet>();
				Image img = btn.GetComponent<Image>();

				if (templateSetBtn != null && img != null)
				{
					Color newColor;
					switch (templateSetBtn.colorMode)
					{
						case TemplateSet.ColorMode.textColor:
							img.color = menuConfiguration.textColor;
							break;
						case TemplateSet.ColorMode.buttonColor:
							img.color = menuConfiguration.btnColor;
							break;
						case TemplateSet.ColorMode.foreground:
							img.color = menuConfiguration.fgColor;
							break;
						case TemplateSet.ColorMode.background:
							img.color = menuConfiguration.bkColor;
							break;
						case TemplateSet.ColorMode.darkerForeground:
							img.color = fgDarker;
							break;
						case TemplateSet.ColorMode.changable:
							img.color = menuConfiguration.fgColor;
							break;
						case TemplateSet.ColorMode.textColorPreserveAlpha:
							newColor = menuConfiguration.textColor;
							newColor.a = img.color.a;
							img.color = newColor;
							break;
						case TemplateSet.ColorMode.buttonColorPreserveAlpha:
							newColor = menuConfiguration.btnColor;
							newColor.a = img.color.a;
							img.color = newColor;
							break;
						case TemplateSet.ColorMode.foregroundPreserveAlpha:
							newColor = menuConfiguration.fgColor;
							newColor.a = img.color.a;
							img.color = newColor;
							break;
						case TemplateSet.ColorMode.backgroundPreserveAlpha:
							newColor = menuConfiguration.bkColor;
							newColor.a = img.color.a;
							btn.GetComponent<Image>().color = newColor;
							break;
						case TemplateSet.ColorMode.unchangable:
							break;
					}
				}
				else if (img != null)
					btn.GetComponent<Image>().color = menuConfiguration.btnColor;
			}

			Image[] allImages = menuPrefab.GetComponentsInChildren<Image>(true);
			foreach (Image img in allImages)
			{
				if(img.sprite!=null && img.sprite.name.Equals("simple_panel") && panel != null)
					img.sprite = panel;
			}
		}

		static void SetMenuPosition(MenuConfiguration menuConfiguration, GameObject menuPrefab, GameObject newButtonRow)
		{
			if (menuPrefab.transform.Find("MenuController").gameObject.GetComponent<MenuController>().acceptAlign)
			{
				switch (menuConfiguration.menuLayout)
				{
					case MenuConfiguration.MenuLayout.AlignLeft:
						newButtonRow.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
						newButtonRow.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
						newButtonRow.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
						Vector3 vpl = newButtonRow.GetComponent<RectTransform>().anchoredPosition;
						vpl.x = menuConfiguration.alignPadding;
						newButtonRow.GetComponent<RectTransform>().anchoredPosition = vpl;
						break;
					case MenuConfiguration.MenuLayout.AlignCenter:
						newButtonRow.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
						newButtonRow.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
						newButtonRow.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
						break;
					case MenuConfiguration.MenuLayout.AlignRight:
						newButtonRow.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
						newButtonRow.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
						newButtonRow.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);
						Vector3 vpr = newButtonRow.GetComponent<RectTransform>().anchoredPosition;
						vpr.x = -menuConfiguration.alignPadding;
						newButtonRow.GetComponent<RectTransform>().anchoredPosition = vpr;
						break;
				}
			}
		}

		static void SetButtonPosition(MenuListView menu, MenuConfiguration menuConfiguration, GameObject newButton, int pos)
		{			
			newButton.name = menu.ItemName(pos);
			newButton.transform.Find("ButtonText").gameObject.GetComponent<Text>().text = menu.ItemName(pos);

			TemplateSet templateSet = newButton.transform.Find("ButtonText").gameObject.GetComponent<TemplateSet>();
			if (templateSet == null || templateSet.alignMode == TemplateSet.AlignMode.changable)
			{
				switch (menuConfiguration.menuLayout)
				{
					case MenuConfiguration.MenuLayout.AlignLeft:
						newButton.transform.Find("ButtonText").gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
						break;
					case MenuConfiguration.MenuLayout.AlignCenter:
						newButton.transform.Find("ButtonText").gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
						break;
					case MenuConfiguration.MenuLayout.AlignRight:
						newButton.transform.Find("ButtonText").gameObject.GetComponent<Text>().alignment = TextAnchor.MiddleRight;
						break;
				}
			}
		}

		static void SetButtonSubMenu(MenuController menuController, GameObject newRow, Button btn)
		{
			System.Reflection.MethodInfo targetinfo = UnityEvent.GetValidMethodInfo(menuController, "GotoMenu", new System.Type[] { typeof(GameObject) });
			UnityAction<GameObject> action = System.Delegate.CreateDelegate(typeof(UnityAction<GameObject>), menuController, targetinfo, false) as UnityAction<GameObject>;
			UnityEventTools.AddObjectPersistentListener<GameObject>(btn.onClick, action, newRow);
		}

		static void SetButtonOption(MenuListView menu, MenuController menuController, GameObject menuPrefab, GameObject newButton, Button btn, int pos)
		{
			System.Reflection.MethodInfo targetinfo = null;
			GameObject argument = newButton;
			switch (menu.menuItems[pos].menuOption)
			{
				case MenuListView.MenuOptions.Credits:
					argument = menuPrefab.transform.Find("Canvas/MenuScaler/Credits").gameObject;
					if (menu.menuItems[pos].scrollCredits)
						targetinfo = UnityEvent.GetValidMethodInfo(menuController, "CreditsWindowScrolling", new System.Type[] { typeof(GameObject) });
					else
						targetinfo = UnityEvent.GetValidMethodInfo(menuController, "CreditsWindowStatic", new System.Type[] { typeof(GameObject) });
					break;
				case MenuListView.MenuOptions.Back:
					targetinfo = UnityEvent.GetValidMethodInfo(menuController, "GoBack", new System.Type[] { typeof(GameObject) });
					break;
				case MenuListView.MenuOptions.Exit:
					if (menu.menuItems[pos].showConfirmExit)
					{
						targetinfo = UnityEvent.GetValidMethodInfo(menuController, "GotoMenu", new System.Type[] { typeof(GameObject) });
						argument = menuPrefab.transform.Find("Canvas/MenuScaler/ExitDialog").gameObject;
					}
					else
						targetinfo = UnityEvent.GetValidMethodInfo(menuController, "ExitGame", new System.Type[] { typeof(GameObject) });
					break;
				case MenuListView.MenuOptions.Audio:
					argument = menuPrefab.transform.Find("Canvas/MenuScaler/AudioWindow").gameObject;
					if (!menu.menuItems[pos].masterSlider)
						menuPrefab.transform.Find("Canvas/MenuScaler/AudioWindow/MenuPanel/Layout/MasterVolume").gameObject.SetActive(false);
					if (!menu.menuItems[pos].musicSlider)
						menuPrefab.transform.Find("Canvas/MenuScaler/AudioWindow/MenuPanel/Layout/MusicVolume").gameObject.SetActive(false);
					if (!menu.menuItems[pos].FXSlider)
						menuPrefab.transform.Find("Canvas/MenuScaler/AudioWindow/MenuPanel/Layout/FXVolume").gameObject.SetActive(false);
					if (!menu.menuItems[pos].voiceSlider)
						menuPrefab.transform.Find("Canvas/MenuScaler/AudioWindow/MenuPanel/Layout/VoiceVolume").gameObject.SetActive(false);
					targetinfo = UnityEvent.GetValidMethodInfo(menuController, "AudioWindow", new System.Type[] { typeof(GameObject) });
					if (menu.menuItems[pos].askForAudioChanges)
						menuPrefab.transform.Find("MenuController").gameObject.GetComponent<MenuController>().askToConfirmAudioChanges = true;
					else
						menuPrefab.transform.Find("MenuController").gameObject.GetComponent<MenuController>().askToConfirmAudioChanges = false;
					break;
				case MenuListView.MenuOptions.Video:
					argument = menuPrefab.transform.Find("Canvas/MenuScaler/VideoWindow").gameObject;
					if (!menu.menuItems[pos].fullScreen)
						menuPrefab.transform.Find("Canvas/MenuScaler/VideoWindow/MenuPanel/Layout/FullScreen").gameObject.SetActive(false);
					if (!menu.menuItems[pos].resolution)
						menuPrefab.transform.Find("Canvas/MenuScaler/VideoWindow/MenuPanel/Layout/Resolution").gameObject.SetActive(false);
					if (!menu.menuItems[pos].presets)
						menuPrefab.transform.Find("Canvas/MenuScaler/VideoWindow/MenuPanel/Layout/PresetQuality").gameObject.SetActive(false);
					if (!menu.menuItems[pos].textureQuality)
						menuPrefab.transform.Find("Canvas/MenuScaler/VideoWindow/MenuPanel/Layout/TextureQuality").gameObject.SetActive(false);
					if (!menu.menuItems[pos].antialiasing)
						menuPrefab.transform.Find("Canvas/MenuScaler/VideoWindow/MenuPanel/Layout/Antialiasing").gameObject.SetActive(false);
					if (!menu.menuItems[pos].shadowquality)
						menuPrefab.transform.Find("Canvas/MenuScaler/VideoWindow/MenuPanel/Layout/ShadowQuality").gameObject.SetActive(false);
					if (!menu.menuItems[pos].shadowresolution)
						menuPrefab.transform.Find("Canvas/MenuScaler/VideoWindow/MenuPanel/Layout/ShadowResolution").gameObject.SetActive(false);
					if (!menu.menuItems[pos].anisotropic)
						menuPrefab.transform.Find("Canvas/MenuScaler/VideoWindow/MenuPanel/Layout/Anisotropic").gameObject.SetActive(false);
					if (!menu.menuItems[pos].vSync)
						menuPrefab.transform.Find("Canvas/MenuScaler/VideoWindow/MenuPanel/Layout/VSync").gameObject.SetActive(false);
					if (!menu.menuItems[pos].softParticles)
						menuPrefab.transform.Find("Canvas/MenuScaler/VideoWindow/MenuPanel/Layout/SoftParticles").gameObject.SetActive(false);
					if (!menu.menuItems[pos].softVegetation)
						menuPrefab.transform.Find("Canvas/MenuScaler/VideoWindow/MenuPanel/Layout/SoftVegetation").gameObject.SetActive(false);
					targetinfo = UnityEvent.GetValidMethodInfo(menuController, "VideoWindow", new System.Type[] { typeof(GameObject) });
					if (menu.menuItems[pos].askForVideoChanges)
						menuPrefab.transform.Find("MenuController").gameObject.GetComponent<MenuController>().askToConfirmVideoChanges = true;
					else
						menuPrefab.transform.Find("MenuController").gameObject.GetComponent<MenuController>().askToConfirmVideoChanges = false;
					break;
				case MenuListView.MenuOptions.Language:
					argument = menuPrefab.transform.Find("Canvas/MenuScaler/LanguageWindow").gameObject;
					if (!menu.menuItems[pos].masterLanguage)
						menuPrefab.transform.Find("Canvas/MenuScaler/LanguageWindow/MenuPanel/Layout/MasterLanguage").gameObject.SetActive(false);
					if (!menu.menuItems[pos].subtitlesOnOff)
						menuPrefab.transform.Find("Canvas/MenuScaler/LanguageWindow/MenuPanel/Layout/SubtitlesOnOff").gameObject.SetActive(false);
					if (!menu.menuItems[pos].subtitlesLanguage)
						menuPrefab.transform.Find("Canvas/MenuScaler/LanguageWindow/MenuPanel/Layout/SubtitlesLanguage").gameObject.SetActive(false);
					targetinfo = UnityEvent.GetValidMethodInfo(menuController, "LanguageWindow", new System.Type[] { typeof(GameObject) });
					if (menu.menuItems[pos].askForLanguageChanges)
						menuPrefab.transform.Find("MenuController").gameObject.GetComponent<MenuController>().askToConfirmLanguageChanges = true;
					else
						menuPrefab.transform.Find("MenuController").gameObject.GetComponent<MenuController>().askToConfirmLanguageChanges = false;
					if (menu.menuItems[pos].integrateLocalizationPro)
						menuPrefab.transform.Find("MenuController").gameObject.GetComponent<MenuController>().integrateWithLocalizationPro = true;
					else
						menuPrefab.transform.Find("MenuController").gameObject.GetComponent<MenuController>().integrateWithLocalizationPro = false;
					break;
				case MenuListView.MenuOptions.Custom:
					if (menu.menuItems[pos].customShowWindow)
					{
						GameObject customWindow = menuPrefab.transform.Find("Canvas/MenuScaler/CustomWindow").gameObject;
						GameObject newCustomWindow = GameObject.Instantiate(customWindow, menuPrefab.transform.Find("Canvas/MenuScaler"));
						if (!menu.menuItems[pos].customShowButton)
							newCustomWindow.transform.Find("MenuPanel/Back").gameObject.SetActive(false);
						newCustomWindow.name = menu.ItemName(pos);
						targetinfo = UnityEvent.GetValidMethodInfo(menuController, "CustomWindow", new System.Type[] { typeof(GameObject) });
						argument = newCustomWindow;
					}
					else
					{
						CustomOption option= btn.gameObject.AddComponent<CustomOption>();
						option.userOption = menu.menuItems[pos].customOptionId;

						targetinfo = UnityEvent.GetValidMethodInfo(menuController, "CustomOption", new System.Type[] { typeof(GameObject) });
						argument = btn.gameObject;
						/*
						targetinfo = UnityEvent.GetValidMethodInfo(menuController, "CustomOption", new System.Type[] { typeof(int) });
						UnityAction<int> action = System.Delegate.CreateDelegate(typeof(UnityAction<int>), menuController, targetinfo, false) as UnityAction<int>;
						UnityEventTools.AddIntPersistentListener(btn.onClick, action, menu.menuItems[pos].customOptionId);
						targetinfo = null;
						*/
					}
					break;
				case MenuListView.MenuOptions.NewGame:
					targetinfo = UnityEvent.GetValidMethodInfo(menuController, "NewGame", new System.Type[] { typeof(GameObject) });
					break;
				case MenuListView.MenuOptions.LoadGame:
					targetinfo = UnityEvent.GetValidMethodInfo(menuController, "LoadGame", new System.Type[] { typeof(GameObject) });
					break;
				case MenuListView.MenuOptions.Continue:
					targetinfo = UnityEvent.GetValidMethodInfo(menuController, "ContinueGame", new System.Type[] { typeof(GameObject) });
					break;
			}
			if (targetinfo != null)
			{
				UnityAction<GameObject> action = System.Delegate.CreateDelegate(typeof(UnityAction<GameObject>), menuController, targetinfo, false) as UnityAction<GameObject>;
				UnityEventTools.AddObjectPersistentListener<GameObject>(btn.onClick, action, argument);
			}
		}
	}	
}
#endif