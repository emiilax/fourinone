﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticVariables : MonoBehaviour {

	public static Color COLOR_RED = new Color32( 0xAD, 0x4C, 0x4B , 0xFF );
	public static Color COLOR_ORANGE = new Color32( 0xDD, 0xA5, 0x5B , 0xFF );
	public static Color COLOR_BLUE = new Color32( 0x53, 0x98, 0xA7 , 0xFF );
	public static Color COLOR_GREEN = new Color32( 0x83, 0xB0, 0x6E , 0xFF );

	public static short LevelVoteMsg                  = 1000;
	public static short LevelVoteCompletedMsg         = 1001;
	public static short LevelVoteFailMsg              = 1002;

	public static short FinnishedGameVoteMsg          = 1003;
	public static short FinnishedGameVoteCompletedMsg = 1004;
	public static short FinnishVoteFailMsg              = 1005;

	//for sync screen
	public static short SyncVoteMsg          = 1006;
	public static short SyncVoteCompletedMsg = 1007;
	public static short SyncVoteFailMsg              = 1008;

	public static short ExitGameVoteMsg          = 1009;
	public static short ExitGameCompletedMsg = 1010;
	public static short ExitGameVoteFailMsg              = 1011;
}
