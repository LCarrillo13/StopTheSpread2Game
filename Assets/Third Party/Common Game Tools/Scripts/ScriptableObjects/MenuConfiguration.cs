#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace CGT
{
	[CreateAssetMenu(fileName = "MenuConfiguration", menuName = "Common Game Tools/Menu Configuration", order = 3)]
	public class MenuConfiguration : ScriptableObject
	{
		//public MenuTreeView treeView = new MenuTreeView("Main Menu", MenuTreeView.NodeType.MainMenu, false);
		public MenuListView treeView = new MenuListView("Sample Menu", false);

		public string menuName="Sample Menu";

		public enum MenuTemplates
		{
			defaultTemplate,			
			Animated,
			Office,
			Modern,
			OwnTemplate
		}

		public enum ButtonType
		{
			useTemplate,
			squared,
			chamfer,
			Blevel,
			Bordered,
			Banner1,
			Banner2,
			custom
		}
		public ButtonType buttonType;
		public Sprite normal, highlighted, pressed, disabled, panel, fill, frame;
		public float menuScale = 1f;
		//public bool escBack = false;
		//public bool rightClickBack = false;
		public MenuTemplates template;
		public GameObject overrideTemplate = null;
		
		public Font overrideFont;
		public float fontMutiplier = 1;
		public bool allowOverflow = false;
		public bool dontOverrideDropdown = true;
		public bool useColor = true;
		public Color textColor=Color.white;
		public Color btnColor = Color.white;
		public Color fgColor = Color.white;
		public Color bkColor = Color.black;
		public MenuLayout menuLayout;

		public enum MenuAnimations
		{
			NoAnimations,
			SimpleAnimations
		}

		public enum MenuLayout
		{
			useTemplate,
			AlignLeft,
			AlignCenter,
			AlignRight
		}
		public float alignPadding = 10;

		public Sprite bgImage;
		public static string[] bgAspectRatio = { "Automatic", "5:4", "4:3", "3:2", "16:10", "16:9" };
		public static float[] bgAspectRatioValues = { 0, 1.25f, 1.3333f, 1.5f, 1.6f, 1.7777f };
		public int selectedAspetRatio = 0;

		public AudioClip bgMusic, clickEffect;

		public string UID;

		public MenuConfiguration()
		{
			bkColor.a = 0.75f;
		}

		private void OnEnable()
		{
			treeView.editing = false;
		}

		private void OnValidate()
		{
#if UNITY_EDITOR
			if (UID == "")
			{
				UID = GUID.Generate().ToString();
				UnityEditor.EditorUtility.SetDirty(this);
			}
#endif

		}

	}
}
#endif