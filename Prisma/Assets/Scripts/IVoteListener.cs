﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVoteListener  {

	void OnVoteComplete(string winner);
	void OnVoteFail();

	void ServerVoteComplete (string winner);


}
