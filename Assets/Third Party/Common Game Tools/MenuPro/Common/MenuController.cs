using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


namespace CGT
{
	public class MenuController : MonoBehaviour
	{
		public List<GameObject> menuStack = new List<GameObject>();

		[Header("General Dialogs")]
		public GameObject ExitWithoutApply;
		public GameObject ConfirmChanges;

		[Header("Audio settings")]
		//Audio settings
		public AudioMixer audioMixer;
		public string masterVariable = "Master";
		public string musicVariable = "musicVolume";
		public string FXVariable = "FXVolume";
		public string voiceVariable = "voiceVolume";
		public Slider masterVolume, musicVolume, FXVolume, voiceVolume;

		[Header("Video settings")]
		public Toggle fullscreen;
		public Toggle vSync, softParticles, softVegetation;
		public Dropdown resolutions, qualityPreset, antiAliasing, shadowQuality, shadowRes, textureQuality, anisotropic;

		[Header("Language settings")]
		public Dropdown masterLanguage;
		public Toggle subTitlesOnOf;
		public Dropdown subtitleLanguage;

		[Header("Other settings")]
		public bool askToConfirmAudioChanges = true;
		public bool askToConfirmVideoChanges = true;
		public bool askToConfirmLanguageChanges = true;

		bool changes = false;
		Resolution[] availableResolutions;
		string[] availableQPresrts;
		int[] avaiableTexQual = new int[] { 0, 1, 2, 3 };
		string[] avaiableTexQualNames = new string[] { "Very High", "High", "Medium", "Low", "", "", "", "", "" };
		int[] avaiableAliasign = new int[] { 0, 2, 4, 8 };
		string[] avaiableAliasignNames = new string[] { "Off", "2 x Multi-Sampling", "4 x Multi-Sampling", "8 x Multi-Sampling", "", "", "", "", "" };
		string[] availableShadowQuality;
		string[] availableShadowQualityNames = new string[] { "Disable", "Hard Only", "All", "", "", "", "", "" };
		string[] availableShadowResolution;
		string[] availableShadowResolutionNames = new string[] { "Low", "Medium", "High", "Very High", "", "", "", "" };
		int[] availableAnisoFilter = new int[] { 0, 2, 4, 8, 16 };
		string[] anisoFilterNames = new string[] { "Off", "2 x filter", "4 x filter", "8 x filter", "16 x filter", "", "", "", "" };
		int currentAniso;
		bool disableCheks = false;

		//Language settings
		public enum availableLanguages { English, Spanish};
		public enum availableSubtitleLanguages { English, Spanish };
		public int currentLanguage;
		public int currentSubTitleLanguage;
		public bool integrateWithLocalizationPro = true;
		string[] languages;

		[Header("Behavior settings")]
		public AudioClip bgMusic;
		public AudioClip clickEffect;
		public AudioSource backgroundMusic, effects;
		public bool acceptAlign = true;


		public enum BuiltInDialog
		{
			None,
			Audio,
			Video,
			Language
		}

		BuiltInDialog waitingConfirmation;

		//******************************************************************
		public void CustomOption(GameObject Invoker)
		{
			Debug.Log("Click on " + Invoker.GetComponent<CustomOption>().userOption + " option");
			TemplateSet ts = null;
			if (Invoker != null)
				ts = Invoker.GetComponentInParent<TemplateSet>();
			if (ts != null && ts.useParent)
				HideWindowsGreater(ts.level);

		}

		public void NewGame(GameObject Invoker)
		{
			Debug.Log("Click on New Game");
		}

		public void LoadGame(GameObject Invoker)
		{
			Debug.Log("Click on Load Game");
		}

		public void ContinueGame(GameObject Invoker)
		{
			Debug.Log("Click on Continue Game");
		}

		public void TrySetLanguage()
		{
			Debug.Log("Trying to set language to "+ languages[currentLanguage]);
#if CGT_LOCALIZATIO_PRO
			if (integrateWithLocalizationPro)
			{
				//Try to integrate with Localization Pro
				for (int i = 0, j = 0; i < typeof(LocalizationConfiguration.Language)).Length; i++) {
					if ((LocalizationManager.instance.localizationConfiguration.maskValue & (1 << i)) != 0)
					{
						if (currentLanguage == j)
						{
							LocalizationManager.instance.localizationConfiguration.currentLanguage = (LocalizationConfiguration.Language)i;
							break;
						}
					}
				}
			}
#endif
		}
		//******************************************************************	

		//**********************************************
		// This two functions controls the show and 
		//  hide of menus and submenus
		//**********************************************
		public void GotoMenu(GameObject Menu)
		{
			//Creates an internal stack for windows
			if (menuStack.Count == 0)
			{
				var parent = Menu.transform.parent;
				while (parent != null)
				{
					if (parent.name.Equals("MenuScaler"))
					{
						menuStack.Add(parent.Find("MainMenu").gameObject);
						break;
					}
					parent = parent.transform.parent;
				}
				if (menuStack.Count == 0)
				{
					Debug.LogError("Cannot find Main Menu");
					return;
				}
			}

			TemplateSet ts = Menu.GetComponent<TemplateSet>();
			if (ts != null && ts.useParent)
			{
				bool closeOpened = false;
				closeOpened = IsAlreadyOpened(Menu);
				HideWindowsGreaterOrEqualTo(ts.level);
				if (!closeOpened)
				{
					Menu.SetActive(true);
					menuStack.Add(Menu);
				}
			}
			else if(ts!=null && ts.overlap)
			{
				Menu.SetActive(true);
				menuStack.Add(Menu);
			}
			else
			{
				//Show and hide
				menuStack[menuStack.Count - 1].SetActive(false);
				Menu.SetActive(true);
				menuStack.Add(Menu);
			}
		}		

		public void GoBack(GameObject Invoker)
		{
			//Show and hide
			TemplateSet ts = null;// Invoker.GetComponent<TemplateSet>();
			if (Invoker!=null)
				ts = Invoker.GetComponentInParent<TemplateSet>();
			if (ts != null && ts.useParent)
			{
				HideWindowsGreaterOrEqualTo(ts.level);
			}
			else
			{	//No office menu
				menuStack[menuStack.Count - 1].SetActive(false);
				menuStack.RemoveAt(menuStack.Count - 1);
				menuStack[menuStack.Count - 1].SetActive(true);
			}

		}

		void HideWindowsGreaterOrEqualTo(int level)
		{
			int toDelete = FindLevelGreaterOrEqualTo(level);
			while (toDelete != -1)
			{
				menuStack[toDelete].SetActive(false);
				menuStack.RemoveAt(toDelete);
				toDelete = FindLevelGreaterOrEqualTo(level);
			}
		}

		void HideWindowsGreater(int level)
		{
			int toDelete = FindLevelGreaterTo(level);
			while (toDelete != -1)
			{
				menuStack[toDelete].SetActive(false);
				menuStack.RemoveAt(toDelete);
				toDelete = FindLevelGreaterTo(level);
			}
		}

		int FindLevelGreaterOrEqualTo(int level)
		{
			for (int i = 0; i < menuStack.Count; i++)
			{
				TemplateSet ts = menuStack[i].GetComponent<TemplateSet>();
				if (ts != null && ts.level >= level)
					return i;
			}
			return -1;
		}

		int FindLevelGreaterTo(int level)
		{
			for (int i = 0; i < menuStack.Count; i++)
			{
				TemplateSet ts = menuStack[i].GetComponent<TemplateSet>();
				if (ts != null && ts.level > level)
					return i;
			}
			return -1;
		}

		bool IsAlreadyOpened(GameObject Menu)
		{
			for (int i = 0; i < menuStack.Count; i++)			
				if (menuStack[i]==Menu)
					return true;
			return false;
		}

		//**********************************************
		// Displays and controls all window options
		//**********************************************

		//Custom window
		public void CustomWindow(GameObject Window)
		{
			GotoMenu(Window);
		}

		//Credits window (static)
		public void CreditsWindowStatic(GameObject Window)
		{
			Window.GetComponent<CreditsAnimation>().ShowWithNoAnimation();
			GotoMenu(Window);
		}

		//Credits window (vertical scroll of content)
		public void CreditsWindowScrolling(GameObject Window)
		{
			Window.GetComponent<CreditsAnimation>().StartAnimation();
			GotoMenu(Window);
		}

		public void DialogClickYes(DialogIdentifier dialog)
		{
			switch(dialog.dialog)
			{
				case DialogIdentifier.StandardDialogs.ExitDialog:
					ExitDialogYes();
					break;
				case DialogIdentifier.StandardDialogs.NoApplyDialog:
					ExitSettingsDialogYes();
					break;
				case DialogIdentifier.StandardDialogs.ConfirmChangesDialog:
					ConfirmSettingsDialogYes();
					break;
			}
		}

		public void DialogClickNo(DialogIdentifier dialog)
		{
			switch (dialog.dialog)
			{
				case DialogIdentifier.StandardDialogs.ExitDialog:
					ExitDialogNo();
					break;
				case DialogIdentifier.StandardDialogs.NoApplyDialog:
					ExitSettingsDialogNo();
					break;
				case DialogIdentifier.StandardDialogs.ConfirmChangesDialog:
					ConfirmSettingsDialogNo();
					break;
			}
		}

		//Press YES on the exit dialog
		public void ExitDialogYes()
		{
			ExitGame(null);
		}

		//Press NO on the exit dialog
		public void ExitDialogNo()
		{
			GoBack(null);
		}

		//Press YES confirming that changes wont be applied
		public void ExitSettingsDialogYes()
		{
			RevertChanges();
			changes = false;
			GoBack(null);
			GoBack(null);
		}

		//Press NO when asked for confirming that changes wont be applied
		//Then navigation will go back to the correspondant dialg
		public void ExitSettingsDialogNo()
		{
			GoBack(null);
		}


		//Press YES confirming that changes will be applied
		public void ConfirmSettingsDialogYes()
		{
			switch(waitingConfirmation)
			{
				case BuiltInDialog.Audio:
					ApplyAudioChangesConfirmed();
					GoBack(null);
					GoBack(null);
					break;
				case BuiltInDialog.Video:
					ApplyVideoChangesConfirmed();
					GoBack(null);
					GoBack(null);
					break;
				case BuiltInDialog.Language:
					ApplyLanguageChangesConfirmed();
					GoBack(null);
					GoBack(null);
					break;
			}			
		}

		//Press NO discarding changes
		public void ConfirmSettingsDialogNo()
		{
			GoBack(null);
		}

		//Actual exit game code
		public void ExitGame(GameObject Invoker)
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
		}

		//Receives a call to open the AUDIO SETTINGS window
		public void AudioWindow(GameObject Window)
		{
			masterVolume.value = PlayerPrefs.GetFloat("masterVolume", 0);
			musicVolume.value = PlayerPrefs.GetFloat("musicVolume", 0);
			FXVolume.value = PlayerPrefs.GetFloat("FXVolume", 0);
			voiceVolume.value = PlayerPrefs.GetFloat("voiceVolume", 0);
			changes = false;
			GotoMenu(Window);
		}

		//callback for master volume
		public void SetMasterVolume(float volume)
		{
			audioMixer.SetFloat(masterVariable, volume);
			changes = true;
		}

		//callback for music volume
		public void SetMusicVolume(float volume)
		{
			audioMixer.SetFloat(musicVariable, volume);
			changes = true;
		}

		//callback for FX volume
		public void SetFXVolume(float volume)
		{
			audioMixer.SetFloat(FXVariable, volume);
			changes = true;
		}

		//callback for Voice volume
		public void SetVoiceVolume(float volume)
		{
			audioMixer.SetFloat(voiceVariable, volume);
			changes = true;
		}

		//Press the button APPLY in Audio Window
		public void ApplyAudioChanges()
		{
			if (askToConfirmAudioChanges)
			{
				waitingConfirmation = BuiltInDialog.Audio;
				GotoMenu(ConfirmChanges);
			}
			else
			{
				ApplyAudioChangesConfirmed();
				GoBack(null);
			}
		}

		//Apply changes. Called when user press YES on the confirmation dialog
		//or directly if this option hasa not been set in the configuration
		public void ApplyAudioChangesConfirmed()
		{
			PlayerPrefs.SetFloat("masterVolume", masterVolume.value);
			PlayerPrefs.SetFloat("musicVolume", musicVolume.value);
			PlayerPrefs.SetFloat("FXVolume", FXVolume.value);
			PlayerPrefs.SetFloat("voiceVolume", voiceVolume.value);
			changes = false;
		}

		//Exit dialog. Called when user press BACK on the Audio Window
		public void ExitAudioChanges()
		{
			if (changes)
				GotoMenu(ExitWithoutApply);
			else
				ConfirmExitAudioChanges();
		}

		//Internal function to revert any changes
		//Take in mind that AUDIO makes changes in realtime inside the dialog
		void RevertChanges()
		{
			masterVolume.value = PlayerPrefs.GetFloat("masterVolume", 0);
			musicVolume.value = PlayerPrefs.GetFloat("musicVolume", 0);
			FXVolume.value = PlayerPrefs.GetFloat("FXVolume", 0);
			voiceVolume.value = PlayerPrefs.GetFloat("voiceVolume", 0);

			audioMixer.SetFloat(masterVariable, masterVolume.value);
			audioMixer.SetFloat(musicVariable, musicVolume.value);
			audioMixer.SetFloat(FXVariable, FXVolume.value);
			audioMixer.SetFloat(voiceVariable, voiceVolume.value);
		}
		
		public void ConfirmExitAudioChanges()
		{
			masterVolume.value = PlayerPrefs.GetFloat("masterVolume", 0);
			musicVolume.value = PlayerPrefs.GetFloat("musicVolume", 0);
			FXVolume.value = PlayerPrefs.GetFloat("FXVolume", 0);
			voiceVolume.value = PlayerPrefs.GetFloat("voiceVolume", 0);

			audioMixer.SetFloat(masterVariable, masterVolume.value);
			audioMixer.SetFloat(musicVariable, musicVolume.value);
			audioMixer.SetFloat(FXVariable, FXVolume.value);
			audioMixer.SetFloat(voiceVariable, voiceVolume.value);

			GoBack(null);
			changes = false;
		}

		public void VideoWindow(GameObject Window)
		{
			RefreshVideoDialog();
			changes = false;
			GotoMenu(Window);
		}

		void RefreshVideoDialog()
		{
			fullscreen.isOn = Screen.fullScreen;

			for (int i = 0; i < availableResolutions.Length; i++)
				if (availableResolutions[i].height == Screen.height && availableResolutions[i].width == Screen.width)
					resolutions.value = i;

			for (int i = 0; i < avaiableTexQual.Length; i++)
				if (i == QualitySettings.masterTextureLimit)
					textureQuality.value = i;

			for (int i = 0; i < avaiableAliasign.Length; i++)
				if (avaiableAliasign[i] == QualitySettings.antiAliasing)
					antiAliasing.value = i;

			for (int i = 0; i < availableShadowQuality.Length; i++)
				if (i == (int)QualitySettings.shadows)
					shadowQuality.value = i;

			for (int i = 0; i < availableShadowResolution.Length; i++)
				if (i == (int)QualitySettings.shadowResolution)
					shadowRes.value = i;

			for (int i = 0; i < availableAnisoFilter.Length; i++)
				if (i == currentAniso)
					anisotropic.value = i;

			vSync.isOn = (QualitySettings.vSyncCount > 0) ? true : false;
			softParticles.isOn = QualitySettings.softParticles;
			softVegetation.isOn = QualitySettings.softVegetation;
		}

		public void ApplyVideoChanges()
		{
			if (askToConfirmVideoChanges)
			{
				waitingConfirmation = BuiltInDialog.Video;
				GotoMenu(ConfirmChanges);
			}
			else
			{
				ApplyVideoChangesConfirmed();
				GoBack(null);
			}
		}

		public void ApplyVideoChangesConfirmed()
		{
			Screen.SetResolution(availableResolutions[resolutions.value].width, availableResolutions[resolutions.value].height, fullscreen.isOn);
			//Quality presets
			QualitySettings.masterTextureLimit = avaiableTexQual[textureQuality.value];
			QualitySettings.antiAliasing = avaiableAliasign[antiAliasing.value];
			QualitySettings.shadows = (ShadowQuality)System.Enum.Parse(typeof(ShadowQuality), availableShadowQualityNames[shadowQuality.value]);
			QualitySettings.shadowResolution = (ShadowResolution)System.Enum.Parse(typeof(ShadowResolution), availableShadowResolution[shadowRes.value]);
			SetAnisotropicFiltering(anisotropic.value);
			QualitySettings.vSyncCount = (vSync.isOn) ? 1 : 0;
			QualitySettings.softParticles = softParticles.isOn;
			QualitySettings.softVegetation = softVegetation.isOn;

			//Save to preferences
			PlayerPrefs.SetInt("fullscreen", fullscreen.isOn ? 1 : 0);
			PlayerPrefs.SetInt("resolution", resolutions.value);
			PlayerPrefs.SetInt("textureQuality", textureQuality.value);
			PlayerPrefs.SetInt("antiAliasing", antiAliasing.value);
			PlayerPrefs.SetInt("shadowQuality", shadowQuality.value);
			PlayerPrefs.SetInt("shadowRes", shadowRes.value);
			PlayerPrefs.SetInt("currentAniso", anisotropic.value);
			PlayerPrefs.SetInt("vSync", (vSync.isOn) ? 1 : 0);
			PlayerPrefs.SetInt("softParticles", softParticles.isOn ? 1 : 0);
			PlayerPrefs.SetInt("softVegetation", softVegetation.isOn ? 1 : 0);
			changes = false;
		}

		public void ExitVideoChanges()
		{
			if (changes)
				GotoMenu(ExitWithoutApply);
			else
				ConfirmExitVideoChanges();
		}

		public void ConfirmExitVideoChanges()
		{
			GoBack(null);
			changes = false;
		}

		void SetAnisotropicFiltering(int value)
		{	//0  => 0
			//2  => 1
			//4  => 2
			//8  => 3
			//16 => 4
			currentAniso = value;
			if (value > 0)
			{
				QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
				Texture.SetGlobalAnisotropicFilteringLimits(value, availableAnisoFilter[currentAniso]);
			}
			else
				QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
		}

		public void PlayButtonClick()
		{
			if (clickEffect != null)
				effects.PlayOneShot(clickEffect);
		}

		private void Start()
		{
			//Set audio preferences
			masterVolume.value = PlayerPrefs.GetFloat("masterVolume", 0);
			musicVolume.value = PlayerPrefs.GetFloat("musicVolume", 0);
			FXVolume.value = PlayerPrefs.GetFloat("FXVolume", 0);
			voiceVolume.value = PlayerPrefs.GetFloat("voiceVolume", 0);

			
			AudioMixerGroup[] audioMixGroup = audioMixer.FindMatchingGroups(masterVariable);
			backgroundMusic.outputAudioMixerGroup = audioMixGroup[1];
			effects.outputAudioMixerGroup = audioMixGroup[2];
			if(bgMusic!=null)
			{
				backgroundMusic.clip = bgMusic;
				backgroundMusic.loop = true;
				backgroundMusic.Play();
			}

			if (clickEffect != null)
			{
				effects.loop = false;
				effects.Stop();
			}

			LoadAvailableSystemQuality();

			//Set video preferences
			if (PlayerPrefs.HasKey("fullscreen"))
				fullscreen.isOn = PlayerPrefs.GetInt("fullscreen") == 1 ? true : false;
			else
				PlayerPrefs.SetInt("fullscreen", fullscreen.isOn ? 1 : 0);

			if (PlayerPrefs.HasKey("resolution"))
				Screen.SetResolution(availableResolutions[PlayerPrefs.GetInt("resolution")].width, availableResolutions[PlayerPrefs.GetInt("resolution")].height, fullscreen.isOn);
			else
				PlayerPrefs.SetInt("resolution", availableResolutions.Length - 1);

			if (PlayerPrefs.HasKey("textureQuality"))
				QualitySettings.masterTextureLimit = PlayerPrefs.GetInt("textureQuality");
			else
				PlayerPrefs.SetInt("textureQuality", QualitySettings.masterTextureLimit);

			if (PlayerPrefs.HasKey("antiAliasing"))
				QualitySettings.antiAliasing = avaiableAliasign[PlayerPrefs.GetInt("antiAliasing")];
			else
				PlayerPrefs.SetInt("antiAliasing", AntialasingIndex(QualitySettings.antiAliasing));

			if (PlayerPrefs.HasKey("shadowQuality"))
				QualitySettings.shadows = (ShadowQuality) PlayerPrefs.GetInt("shadowQuality");
			else
				PlayerPrefs.SetInt("shadowQuality", (int)QualitySettings.shadows);

			if (PlayerPrefs.HasKey("shadowRes"))
				QualitySettings.shadowResolution = (ShadowResolution)PlayerPrefs.GetInt("shadowRes");
			else
				PlayerPrefs.SetInt("shadowRes", (int)QualitySettings.shadowResolution);

			if (PlayerPrefs.HasKey("currentAniso"))
				currentAniso = PlayerPrefs.GetInt("currentAniso");
			else
				PlayerPrefs.SetInt("currentAniso", 4);

			currentAniso = PlayerPrefs.GetInt("currentAniso");
			SetAnisotropicFiltering(currentAniso);

			if (PlayerPrefs.HasKey("vSync"))
				QualitySettings.vSyncCount = PlayerPrefs.GetInt("vSync");
			else
				PlayerPrefs.SetInt("vSync", QualitySettings.vSyncCount);

			if (PlayerPrefs.HasKey("softParticles"))
				QualitySettings.softParticles = PlayerPrefs.GetInt("softParticles")==1?true:false;
			else
				PlayerPrefs.SetInt("softParticles", QualitySettings.softParticles?1:0);

			if (PlayerPrefs.HasKey("softVegetation"))
				QualitySettings.softVegetation = PlayerPrefs.GetInt("softVegetation") == 1 ? true : false;
			else
				PlayerPrefs.SetInt("softVegetation", QualitySettings.softVegetation ? 1 : 0);			

			PopulateDropDowns();
			TestAgainstQuality();

			if (PlayerPrefs.HasKey("masterLanguage"))
				masterLanguage.value = PlayerPrefs.GetInt("masterLanguage");
			else
			{
				PlayerPrefs.SetInt("masterLanguage", 0);
				masterLanguage.value = 0;
			}

			if (PlayerPrefs.HasKey("subtitlesOnOff"))
				subTitlesOnOf.isOn = PlayerPrefs.GetInt("subtitlesOnOff") == 1 ? true : false;
			else
			{
				PlayerPrefs.SetInt("subtitlesOnOff", 0);
				subTitlesOnOf.isOn = false;
			}

			if (PlayerPrefs.HasKey("subtitleLanguage"))
				subtitleLanguage.value = PlayerPrefs.GetInt("subtitleLanguage");
			else
			{
				PlayerPrefs.SetInt("subtitleLanguage", 0);
				subtitleLanguage.value = 0;
			}

			//Try to set current language for the project
			currentLanguage = masterLanguage.value;
			TrySetLanguage();

		}

		void PopulateDropDowns()
		{
			resolutions.options = new List<Dropdown.OptionData>();
			for (int i = 0; i < availableResolutions.Length; i++)
			{
				Dropdown.OptionData option = new Dropdown.OptionData();
				option.text = availableResolutions[i].width + " x " + availableResolutions[i].height;
				resolutions.options.Add(option);
			}

			qualityPreset.options = new List<Dropdown.OptionData>();
			for (int i = 0; i < availableQPresrts.Length; i++)
			{
				Dropdown.OptionData option = new Dropdown.OptionData();
				option.text = availableQPresrts[i];
				qualityPreset.options.Add(option);
			}
			Dropdown.OptionData optionNew = new Dropdown.OptionData();
			optionNew.text = "Custom Settings";
			qualityPreset.options.Add(optionNew);

			textureQuality.options = new List<Dropdown.OptionData>();
			for (int i = 0; i < avaiableTexQual.Length; i++)
			{
				Dropdown.OptionData option = new Dropdown.OptionData();
				option.text = avaiableTexQualNames[i];
				textureQuality.options.Add(option);
			}

			antiAliasing.options = new List<Dropdown.OptionData>();
			for (int i = 0; i < avaiableAliasign.Length; i++)
			{
				Dropdown.OptionData option = new Dropdown.OptionData();
				option.text = avaiableAliasignNames[i];
				antiAliasing.options.Add(option);
			}

			shadowQuality.options = new List<Dropdown.OptionData>();
			for (int i = 0; i < availableShadowQuality.Length; i++)
			{
				Dropdown.OptionData option = new Dropdown.OptionData();
				option.text = availableShadowQualityNames[i];
				shadowQuality.options.Add(option);
			}

			shadowRes.options = new List<Dropdown.OptionData>();
			for (int i = 0; i < availableShadowResolution.Length; i++)
			{
				Dropdown.OptionData option = new Dropdown.OptionData();
				option.text = availableShadowResolutionNames[i];
				shadowRes.options.Add(option);
			}

			anisotropic.options = new List<Dropdown.OptionData>();
			for (int i = 0; i < availableAnisoFilter.Length; i++)
			{
				Dropdown.OptionData option = new Dropdown.OptionData();
				option.text = anisoFilterNames[i];
				anisotropic.options.Add(option);
			}
			RefreshVideoDialog();


			languages = Enum.GetNames(typeof(availableLanguages));
			
#if CGT_LOCALIZATIO_PRO
			if(integrateWithLocalizationPro) {
				//Try to integrate with Localization Pro
				string[] allLanguages = Enum.GetNames(typeof(LocalizationConfiguration.Language));
				List<string> usedLanguages = new List<string>();
				for (int i = 0; i < allLanguages.Length; i++)
					if ((LocalizationManager.instance.localizationConfiguration.maskValue & (1 << i)) != 0)
						usedLanguages.Add(allLanguages[i]);
				languages = usedLanguages.ToArray();
			}
#endif

			masterLanguage.options = new List<Dropdown.OptionData>();
			for (int i = 0; i < languages.Length; i++)
			{
				Dropdown.OptionData option = new Dropdown.OptionData();
				option.text = languages[i];
				masterLanguage.options.Add(option);
			}

			string[] subtitles = Enum.GetNames(typeof(availableSubtitleLanguages));
			subtitleLanguage.options = new List<Dropdown.OptionData>();
			for (int i = 0; i < subtitles.Length; i++)
			{
				Dropdown.OptionData option = new Dropdown.OptionData();
				option.text = subtitles[i];
				subtitleLanguage.options.Add(option);
			}
		}

		void LoadAvailableSystemQuality()
		{
			availableResolutions = Screen.resolutions;
			availableQPresrts = QualitySettings.names;
			availableShadowQuality = System.Enum.GetNames(typeof(ShadowQuality));
			availableShadowResolution = System.Enum.GetNames(typeof(ShadowResolution));
		}

		public void VideoChanges()
		{
			if (!disableCheks)
			{
				changes = true;
				TestAgainstQuality();
			}
		}

		void TestAgainstQuality()
		{
			disableCheks = true;
			if (!vSync.isOn &&
			antiAliasing.value == 0 &&
			textureQuality.value == 3 &&
			shadowQuality.value == (int)ShadowQuality.Disable &&
			shadowRes.value == (int)ShadowResolution.Low &&
			anisotropic.value == 0)
				qualityPreset.value = 0;

			else if (!vSync.isOn &&
			antiAliasing.value == 0 &&
			textureQuality.value == 2 &&
			shadowQuality.value == (int)ShadowQuality.HardOnly &&
			shadowRes.value == (int)ShadowResolution.Low &&
			anisotropic.value == 0)
				qualityPreset.value = 1;

			else if (!vSync.isOn &&
			antiAliasing.value == 0 &&
			textureQuality.value == 1 &&
			shadowQuality.value == (int)ShadowQuality.All &&
			shadowRes.value == (int)ShadowResolution.Medium &&
			anisotropic.value == 1)
				qualityPreset.value = 2;

			else if (!vSync.isOn &&
			antiAliasing.value == 1 &&
			textureQuality.value == 0 &&
			shadowQuality.value == (int)ShadowQuality.All &&
			shadowRes.value == (int)ShadowResolution.Medium &&
			anisotropic.value == 2)
				qualityPreset.value = 3;

			else if (vSync.isOn &&
			antiAliasing.value == 2 &&
			textureQuality.value == 0 &&
			shadowQuality.value == (int)ShadowQuality.All &&
			shadowRes.value == (int)ShadowResolution.High &&
			anisotropic.value == 3)
				qualityPreset.value = 4;

			else if (vSync.isOn &&
			antiAliasing.value == 3 &&
			textureQuality.value == 0 &&
			shadowQuality.value == (int)ShadowQuality.All &&
			shadowRes.value == (int)ShadowResolution.VeryHigh &&
			anisotropic.value == 4)
				qualityPreset.value = 5;

			else
				qualityPreset.value = 6;
			disableCheks = false;
		}

		public void SetQualityPreset()
		{
			disableCheks = true;
			switch (qualityPreset.value)
			{
				case 0:
					vSync.isOn = false;
					antiAliasing.value = 0;
					textureQuality.value = 3;
					shadowQuality.value = (int) ShadowQuality.Disable;
					shadowRes.value= (int) ShadowResolution.Low;
					anisotropic.value = 0;
					changes = true;
					break;

				case 1:
					vSync.isOn = false;
					antiAliasing.value = 0;
					textureQuality.value = 2;
					shadowQuality.value = (int)ShadowQuality.HardOnly;
					shadowRes.value = (int)ShadowResolution.Low;
					anisotropic.value = 0;
					changes = true;
					break;

				case 2:
					vSync.isOn = false;
					antiAliasing.value = 0;
					textureQuality.value = 1;
					shadowQuality.value = (int)ShadowQuality.All;
					shadowRes.value = (int)ShadowResolution.Medium;
					anisotropic.value = 1;
					changes = true;
					break;

				case 3:
					vSync.isOn = false;
					antiAliasing.value = 1;
					textureQuality.value = 0;
					shadowQuality.value = (int)ShadowQuality.All;
					shadowRes.value = (int)ShadowResolution.Medium;
					anisotropic.value = 2;
					changes = true;
					break;

				case 4:
					vSync.isOn = true;
					antiAliasing.value = 2;
					textureQuality.value = 0;
					shadowQuality.value = (int)ShadowQuality.All;
					shadowRes.value = (int)ShadowResolution.High;
					anisotropic.value = 3;
					changes = true;
					break;

				case 5:
					vSync.isOn = true;
					antiAliasing.value = 3;
					textureQuality.value = 0;
					shadowQuality.value = (int)ShadowQuality.All;
					shadowRes.value = (int)ShadowResolution.VeryHigh;
					anisotropic.value = 4;
					changes = true;
					break;
			}
			disableCheks = false;
		}

		int AntialasingIndex(int value)
		{
			for (int i = 0; i < avaiableAliasign.Length; i++)
				if (value == avaiableAliasign[i])
					return i;
			return 0;
		}




		public void LanguageWindow(GameObject Window)
		{
			masterLanguage.value = PlayerPrefs.GetInt("masterLanguage", 0);
			subTitlesOnOf.isOn = PlayerPrefs.GetInt("subtitlesOnOff") == 1 ? true : false;
			subtitleLanguage.value = PlayerPrefs.GetInt("subtitleLanguage", 0);

			currentLanguage = masterLanguage.value;
			currentSubTitleLanguage = subtitleLanguage.value;

			//Try to set current language for the project
			TrySetLanguage();

			changes = false;
			GotoMenu(Window);			
		}		

		public void ApplyLanguageChanges()
		{
			if (askToConfirmLanguageChanges)
			{
				waitingConfirmation = BuiltInDialog.Language;
				GotoMenu(ConfirmChanges);
			}
			else
			{
				ApplyLanguageChangesConfirmed();
				GoBack(null);
			}
		}

		public void ApplyLanguageChangesConfirmed()
		{
			PlayerPrefs.SetInt("masterLanguage", masterLanguage.value);
			PlayerPrefs.SetInt("subtitlesOnOff", subTitlesOnOf.isOn?1:0);
			PlayerPrefs.SetInt("subtitleLanguage", subtitleLanguage.value);
			currentLanguage = masterLanguage.value;
			currentSubTitleLanguage = subtitleLanguage.value;

			//Try to set current language for the project
			TrySetLanguage();

			changes = false;
		}


		public void ExitLanguageChanges()
		{
			if (changes)
				GotoMenu(ExitWithoutApply);
			else
				ConfirmExitLanguageChanges();
		}

		public void ConfirmExitLanguageChanges()
		{
			masterLanguage.value = PlayerPrefs.GetInt("masterLanguage", 0);
			subTitlesOnOf.isOn = PlayerPrefs.GetInt("subtitlesOnOff") == 1 ? true : false;
			subtitleLanguage.value = PlayerPrefs.GetInt("subtitleLanguage", 0);
			currentLanguage = masterLanguage.value;
			currentSubTitleLanguage = subtitleLanguage.value;

			//Try to set current language for the project
			TrySetLanguage();

			GoBack(null);
			changes = false;
		}
	}
}