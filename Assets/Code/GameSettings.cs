using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed public class GameSettings : SingletonAuto<GameSettings>
{
	public static string sceneName_Game			= "MainGame";
	public static string sceneName_Menu			= "MainMenu";



	public static bool 	gameSettingsFirstPass	= true;

	public bool			mainGamePaused			= false;

	public void LoadSettings()
	{
	}

	public void PlayMainIntroMusic()
	{
		if (gameSettingsFirstPass)
		{
			gameSettingsFirstPass = false;
			MasterAudio.PlaySound("SumGame_MusicBoot");
		}
	}

}

