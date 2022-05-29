using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CGT
{
	public class TemplateSet : MonoBehaviour
	{
		//Creation time modifiers
		public enum FontMode
		{
			unchangable,
			changable
		}

		public enum ColorMode
		{
			unchangable,
			textColor,
			buttonColor,
			foreground,
			background,
			darkerForeground,
			changable,
			textColorPreserveAlpha,
			buttonColorPreserveAlpha,
			foregroundPreserveAlpha,
			backgroundPreserveAlpha,
		}

		public enum AlignMode
		{
			unchangable,
			changable
		}

		public FontMode fontMode;
		public ColorMode colorMode;
		public AlignMode alignMode;

		public GameObject parent = null;
		public bool useParent = false;
		[HideInInspector]
		public int level = 0;
		public bool overlap = false;
	}
}