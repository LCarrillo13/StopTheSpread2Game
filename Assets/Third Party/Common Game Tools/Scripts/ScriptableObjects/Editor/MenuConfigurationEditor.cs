using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CGT
{
	[CustomEditor(typeof(MenuConfiguration))]
	public class MenuConfigurationEditor : Editor
	{
		public Vector2 scrollPosition;
		bool seeLegend = false;
		MenuListView treeView;

		private SerializedProperty listViewSerialized, menuName, escBack, rightClickBack, template, overrideTemplate, useColor;
		private SerializedProperty overrideFont, fontMutiplier, allowOverflow, dontOverrideDropdown;
		private SerializedProperty textColor, btnColor, fgColor, bkColor, menuLayout, alignPadding, bgImage, selectedAspetRatio;
		private SerializedProperty menuScale,bgMusic, clickEffect, backEffect;
		private SerializedProperty buttonType, normal, highlighted, pressed, disabled, panel, fill, frame;

		void OnEnable()
		{
			listViewSerialized = serializedObject.FindProperty("treeView");
			menuName = serializedObject.FindProperty("menuName");
			escBack = serializedObject.FindProperty("escBack");
			rightClickBack = serializedObject.FindProperty("rightClickBack");
			template = serializedObject.FindProperty("template");
			overrideTemplate = serializedObject.FindProperty("overrideTemplate");
			overrideFont = serializedObject.FindProperty("overrideFont");
			useColor = serializedObject.FindProperty("useColor");
			textColor = serializedObject.FindProperty("textColor");
			btnColor = serializedObject.FindProperty("btnColor");
			fgColor = serializedObject.FindProperty("fgColor");
			bkColor = serializedObject.FindProperty("bkColor");
			menuLayout = serializedObject.FindProperty("menuLayout");
			alignPadding = serializedObject.FindProperty("alignPadding");
			bgImage = serializedObject.FindProperty("bgImage");
			selectedAspetRatio = serializedObject.FindProperty("selectedAspetRatio");
			bgMusic = serializedObject.FindProperty("bgMusic");
			clickEffect = serializedObject.FindProperty("clickEffect");
			backEffect = serializedObject.FindProperty("backEffect");
			fontMutiplier = serializedObject.FindProperty("fontMutiplier");
			allowOverflow = serializedObject.FindProperty("allowOverflow");
			dontOverrideDropdown = serializedObject.FindProperty("dontOverrideDropdown");
			buttonType = serializedObject.FindProperty("buttonType");
			normal = serializedObject.FindProperty("normal");
			highlighted = serializedObject.FindProperty("highlighted");
			pressed = serializedObject.FindProperty("pressed");
			disabled = serializedObject.FindProperty("disabled");
			menuScale = serializedObject.FindProperty("menuScale");
			panel = serializedObject.FindProperty("panel");
			fill = serializedObject.FindProperty("fill");
			frame = serializedObject.FindProperty("frame");
		}

		public override void OnInspectorGUI()
		{
			MenuConfiguration menuConfiguration = (MenuConfiguration)target;
			treeView = menuConfiguration.treeView;
			//treeView = (MenuListView)listViewSerialized.objectReferenceValue;
			string oldName = menuConfiguration.menuName;
			GUIStyle myStyle = new GUIStyle();
			myStyle.fontSize = 20;
			EditorGUILayout.LabelField("Menu Configuration", myStyle);
			seeLegend = EditorGUILayout.Foldout(seeLegend, "Show Help", true);
			if (seeLegend)
				DrawLegend();
			Rect windowSize = Rect.zero;
			windowSize.height = 15 + treeView.TreeElements() * 20;
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, EditorStyles.helpBox);			
			treeView.DrawGUILayout(windowSize.width, windowSize.height);
			GUILayout.Space(30f);
			GUILayout.EndScrollView();

			ButtonArea();
			/*			
			*/
			EditorUtils.ThinLine(1);

			GUIStyle style = new GUIStyle(EditorStyles.helpBox);
			style.richText = true;
			style.fontSize = 11;
			EditorGUILayout.TextArea("<b>GENERATE PREFAB</b>\nCreate a Menu prefab with all items created in menu editor. Use this prefab directly in your scene.", style);

			menuName.stringValue = EditorGUILayout.TextField("Prefab Name", menuName.stringValue);			
			menuName.serializedObject.ApplyModifiedProperties();
			treeView.RenameMenu(menuName.stringValue);
			EditorGUILayout.LabelField("");

			/*
			escBack.boolValue = EditorGUILayout.ToggleLeft("Escape = Back/Continue", escBack.boolValue);
			escBack.serializedObject.ApplyModifiedProperties();
			rightClickBack.boolValue = EditorGUILayout.ToggleLeft("Right Click = Back/Continue", rightClickBack.boolValue);
			rightClickBack.serializedObject.ApplyModifiedProperties();
			EditorGUILayout.LabelField("");
			*/

			menuScale.floatValue = EditorGUILayout.Slider("Menu Scale:", menuScale.floatValue, 0.5f, 2);;
			menuScale.serializedObject.ApplyModifiedProperties();

			menuConfiguration.template = (MenuConfiguration.MenuTemplates) EditorGUILayout.EnumPopup("Template to Use:", menuConfiguration.template);
			template.intValue = (int)menuConfiguration.template;
			template.serializedObject.ApplyModifiedProperties();
			if (menuConfiguration.template == MenuConfiguration.MenuTemplates.OwnTemplate)
			{
				EditorGUILayout.TextArea("Please, read the manual to learn how to create your own template.", style);
				overrideTemplate.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Custom template:", overrideTemplate.objectReferenceValue, typeof(GameObject), false);
				overrideTemplate.serializedObject.ApplyModifiedProperties();
			}

			EditorGUILayout.LabelField("");
			if (menuConfiguration.template == MenuConfiguration.MenuTemplates.Office)
			{
				EditorGUILayout.TextArea("Office template force aligment to own template.", style);
				menuConfiguration.menuLayout = MenuConfiguration.MenuLayout.useTemplate;
				menuLayout.intValue = (int)menuConfiguration.menuLayout;
			}
			menuConfiguration.menuLayout = (MenuConfiguration.MenuLayout)EditorGUILayout.EnumPopup("Menu Layout:", menuConfiguration.menuLayout);
			menuLayout.intValue = (int)menuConfiguration.menuLayout;
			menuLayout.serializedObject.ApplyModifiedProperties();
			if(menuConfiguration.menuLayout== MenuConfiguration.MenuLayout.AlignLeft || menuConfiguration.menuLayout == MenuConfiguration.MenuLayout.AlignRight)
			{
				alignPadding.floatValue = EditorGUILayout.FloatField("Align Padding", alignPadding.floatValue);
				alignPadding.serializedObject.ApplyModifiedProperties();
			}

			
			//EditorGUILayout.LabelField("");			
			overrideFont.objectReferenceValue = (Font)EditorGUILayout.ObjectField("Override Font:", overrideFont.objectReferenceValue, typeof(Font), false);
			overrideFont.serializedObject.ApplyModifiedProperties();
			if(menuConfiguration.overrideFont!=null)
			{

				fontMutiplier.floatValue = EditorGUILayout.FloatField("Font Multiplier:", fontMutiplier.floatValue);
				fontMutiplier.serializedObject.ApplyModifiedProperties();
				allowOverflow.boolValue= EditorGUILayout.ToggleLeft("Allow Text Overflow", allowOverflow.boolValue);
				allowOverflow.serializedObject.ApplyModifiedProperties();
				dontOverrideDropdown.boolValue = EditorGUILayout.ToggleLeft("Don't Override Dropdowns", dontOverrideDropdown.boolValue);
				dontOverrideDropdown.serializedObject.ApplyModifiedProperties();
			}

			if (menuConfiguration.template == MenuConfiguration.MenuTemplates.defaultTemplate
				|| menuConfiguration.template == MenuConfiguration.MenuTemplates.Animated)
			{
				EditorGUILayout.LabelField("");
				menuConfiguration.buttonType = (MenuConfiguration.ButtonType)EditorGUILayout.EnumPopup("Button Type:", menuConfiguration.buttonType);
				buttonType.intValue = (int)menuConfiguration.buttonType;
				buttonType.serializedObject.ApplyModifiedProperties();
				if (menuConfiguration.buttonType == MenuConfiguration.ButtonType.custom &&
					menuConfiguration.template == MenuConfiguration.MenuTemplates.defaultTemplate)
				{
					EditorGUILayout.TextArea("You need to set sprites for Normal, Highlighted, Pressed and Disabled states. If any of this sprites is not set then the default template one will be used.", style);
					normal.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("Normal State:", (Sprite)normal.objectReferenceValue, typeof(Sprite), false);
					normal.serializedObject.ApplyModifiedProperties();
					highlighted.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("Highlighted State:", (Sprite)highlighted.objectReferenceValue, typeof(Sprite), false);
					highlighted.serializedObject.ApplyModifiedProperties();
					pressed.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("Pressed State:", (Sprite)pressed.objectReferenceValue, typeof(Sprite), false);
					pressed.serializedObject.ApplyModifiedProperties();
					disabled.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("Disabled State:", (Sprite)disabled.objectReferenceValue, typeof(Sprite), false);
					disabled.serializedObject.ApplyModifiedProperties();
					EditorGUILayout.TextArea("You may set also the frame image of dialogs and windows.", style);
					panel.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("Panel Frame:", (Sprite)panel.objectReferenceValue, typeof(Sprite), false);
					panel.serializedObject.ApplyModifiedProperties();
				}
				else if (menuConfiguration.buttonType == MenuConfiguration.ButtonType.custom &&
					menuConfiguration.template == MenuConfiguration.MenuTemplates.Animated)
				{
					EditorGUILayout.TextArea("You need to set sprites for Frame button and Fill button to do a proper animation. If any of this sprites is not set then the default template one will be used.", style);
					frame.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("Frame Sprite:", (Sprite)frame.objectReferenceValue, typeof(Sprite), false);
					frame.serializedObject.ApplyModifiedProperties();
					fill.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("Fill Sprite:", (Sprite)fill.objectReferenceValue, typeof(Sprite), false);
					fill.serializedObject.ApplyModifiedProperties();
				}
			}

			EditorGUILayout.LabelField("");
			useColor.boolValue = EditorGUILayout.ToggleLeft("Override Colors", useColor.boolValue);
			if (useColor.boolValue)
			{
				textColor.colorValue = (Color)EditorGUILayout.ColorField("Text Color:", textColor.colorValue);
				textColor.serializedObject.ApplyModifiedProperties();
				btnColor.colorValue = (Color)EditorGUILayout.ColorField("Button Color:", btnColor.colorValue);
				btnColor.serializedObject.ApplyModifiedProperties();
				fgColor.colorValue = (Color)EditorGUILayout.ColorField("Foreground Color:", fgColor.colorValue);
				fgColor.serializedObject.ApplyModifiedProperties();
				bkColor.colorValue = (Color)EditorGUILayout.ColorField("Background Color:", bkColor.colorValue);
				bkColor.serializedObject.ApplyModifiedProperties();
			}

			bgImage.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("Backgroung Image:", (Sprite)bgImage.objectReferenceValue, typeof(Sprite), false);
			bgImage.serializedObject.ApplyModifiedProperties();
			if(menuConfiguration.bgImage!=null)
			{
				selectedAspetRatio.intValue = EditorGUILayout.Popup("Aspect Ratio: ", selectedAspetRatio.intValue, MenuConfiguration.bgAspectRatio);
				selectedAspetRatio.serializedObject.ApplyModifiedProperties();
			}
			EditorGUILayout.LabelField("");
			bgMusic.objectReferenceValue = (AudioClip)EditorGUILayout.ObjectField("Backgroung Music:", (AudioClip)bgMusic.objectReferenceValue, typeof(AudioClip), false);
			bgMusic.serializedObject.ApplyModifiedProperties();
			clickEffect.objectReferenceValue = (AudioClip)EditorGUILayout.ObjectField("Button Click:", (AudioClip)clickEffect.objectReferenceValue, typeof(AudioClip), false);
			clickEffect.serializedObject.ApplyModifiedProperties();

			EditorUtils.ThinLine(1);
			if (GUILayout.Button("Save Prefab", new GUILayoutOption[0]))
			{
				if (menuConfiguration.template == MenuConfiguration.MenuTemplates.OwnTemplate && menuConfiguration.overrideTemplate == null)
					EditorUtility.DisplayDialog("Cannot create prefab", "You must indicate the Custom Template to use.", "Ok");
				else
					MenuCreator.CreateMenuPrefab(treeView, menuName.stringValue, menuConfiguration);				
			}

			serializedObject.Update();
			serializedObject.ApplyModifiedProperties();
			EditorApplication.update.Invoke();
			//EditorUtility.SetDirty(target);
		}


		public void DrawLegend()
		{
			GUIStyle style = new GUIStyle(EditorStyles.helpBox);
			style.richText = true;
			style.fontSize = 11;

			//EditorGUI.HelpBox(new Rect(18,20,350,120),"", MessageType.None);
			//GUILayout.Box("", EditorStyles.helpBox, GUILayout.Height(180));
			EditorGUILayout.TextArea("<b>MENU EDITOR</b>\nUse the modificators to create, delete and move menu items. Click in any item name to edit the properties.", style);
			EditorGUILayout.LabelField("Menu Modificators", EditorStyles.boldLabel);
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			DrawLegendItem(MenuListView.iconAdd, "Add Menu Item");
			DrawLegendItem(MenuListView.iconDelete, "Delete Menu Item");			
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
			DrawLegendItem(MenuListView.iconUp, "Move Item Up");
			DrawLegendItem(MenuListView.iconDown, "Move Item Down");			
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			
			EditorGUILayout.LabelField("Icon Legend", EditorStyles.boldLabel);
			GUILayout.BeginHorizontal();			
			DrawLegendItem(MenuListView.iconSubMenu, "Sub Menu");
			DrawLegendItem(MenuListView.iconOption, "Menu Window");	
			GUILayout.EndHorizontal();
			EditorUtils.ThinLine(1);
		}

		public void DrawLegendItem(Texture2D tx, string label)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(tx);
			GUILayout.Label(label);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void ButtonArea()
		{
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			if (GUILayout.Button("Expand All", new GUILayoutOption[0]))			
			{
				treeView.OpenAll();
			}
			if (GUILayout.Button("Colapse All", new GUILayoutOption[0]))
			{
				treeView.CloseAll();
			}

			GUILayout.FlexibleSpace();
/*			if (GUILayout.Button("Expand All", new GUILayoutOption[0]))
			{
				treeView.OpenAll();
			}
			if (GUILayout.Button("Colapse All", new GUILayoutOption[0]))
			{
				treeView.CloseAll();
			}
			*/
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();			
			GUILayout.EndVertical();
		}
	}
}