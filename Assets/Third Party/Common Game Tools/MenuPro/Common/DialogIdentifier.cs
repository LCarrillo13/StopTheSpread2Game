using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CGT
{
	public class DialogIdentifier : MonoBehaviour
	{
		public enum StandardDialogs
		{
			ExitDialog,
			NoApplyDialog,
			ConfirmChangesDialog
		}

		public StandardDialogs dialog;
	}
}