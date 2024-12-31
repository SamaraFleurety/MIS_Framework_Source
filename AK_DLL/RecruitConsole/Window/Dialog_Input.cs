using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AK_DLL
{
	//fixme：从power++抄的输入框，有空看一下
    public class Dialog_Input : Window
    {
		protected string curName;

		private bool focusedFolderNameField;

		private Action<string> onOkCb;

		private Func<string, bool> validationCb;

		protected virtual int MaxNameLength => 96;

		public override Vector2 InitialSize => new Vector2(280f, 175f);

		public Dialog_Input(Action<string> eventOnOk, Func<string, bool> valCb, string defVal = "")
		{
			forcePause = true;
			doCloseX = true;
			absorbInputAroundWindow = true;
			closeOnAccept = false;
			closeOnClickedOutside = true;
			validationCb = valCb;
			onOkCb = eventOnOk;
			curName = defVal;
		}

		protected virtual AcceptanceReport NameIsValid(string name)
		{
			if (name.Length == 0)
			{
				return false;
			}
			return true;
		}

		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Small;
			bool flag = false;
			if ((int)Event.current.type == 4 && Event.current.keyCode == KeyCode.Return)
			{
				flag = true;
				Event.current.Use();
			}
			GUI.SetNextControlName("FolderNameField");
			string text = Widgets.TextField(new Rect(0f, 15f, inRect.width, 35f), curName);
			if (text.Length < MaxNameLength)
			{
				curName = text;
			}
			if (!focusedFolderNameField)
			{
				Verse.UI.FocusControl("FolderNameField", this);
				focusedFolderNameField = true;
			}
			if (!(Widgets.ButtonText(new Rect(15f, inRect.height - 35f - 15f, inRect.width - 15f - 15f, 35f), "OK", drawBackground: true, doMouseoverSound: false) || flag))
			{
				return;
			}
			AcceptanceReport acceptanceReport = NameIsValid(curName);
			if (!acceptanceReport.Accepted)
			{
				if (acceptanceReport.Reason.NullOrEmpty())
				{
					Messages.Message("NameIsInvalid".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				}
				else
				{
					Messages.Message(acceptanceReport.Reason, MessageTypeDefOf.RejectInput, historical: false);
				}
			}
			else if (validationCb(curName))
			{
				if (onOkCb != null)
				{
					onOkCb(curName);
				}
				Find.WindowStack.TryRemove(this);
			}
		}
	}
}
