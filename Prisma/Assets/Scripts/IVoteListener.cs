using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVoteListener  {

	// Use this for initialization
	void OnVoteComplete(string winner);
}
