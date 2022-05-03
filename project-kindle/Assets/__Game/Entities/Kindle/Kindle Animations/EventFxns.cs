using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFxns : MasterObject
{
    // Called by an animation event in kindle_run and kindle_run_aimup
	// Takes in an int that determines what foot step audio to play
    // 0 - 1: normal footstep 1 and 2
    // 2 - 3: metal footstep 1 and 2
	public void PlayStepSound(int key)
	{
		if(key == 0)
			game.PlaySound("Footstep1");
		else if (key == 1)
			game.PlaySound("Footstep2");
        else if (key == 2)
            game.PlaySound("FootstepMetal1");
        else if (key == 3)
			game.PlaySound("FootstepMetal2");
	}
}
