using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System;   // Enum

public class EventRunner : MasterObject
{
    // Used when parsing event strings
    /*
        X, Y, Z, W refer to command parameter indices 0, 1, 2, and 3 respectively.
        S refers to the string argument.
        Ex: gameFlagJump(X, Y) = If game flag X is set, jump to event Y.
    */
    static Dictionary<string, Command> cmdmap = new Dictionary<string, Command>() {
        {"end", Command.zero},  // Exits event

        {"jump", Command.jump}, // Jump to event X
        {"wait", Command.wait}, // Pauses event for X frames
        {"advance", Command.advance},   // Waits for player input to advance event
        {"adv", Command.advance},   // ^

        {"gameflagset", Command.gameflag_set},      // Sets global game flag X
        {"gameflagclear", Command.gameflag_clear},  // Clears global game flag X
        {"gameflagjump", Command.gameflag_jump},    // If flag X is set, jump to event Y. If flag -X is NOT set, jump to event Y

        {"sceneflagset", Command.sceneflag_set},    // Sets temporary scene flag X
        {"sceneflagclear", Command.sceneflag_clear},    // Clears temporary scene flag X
        {"sceneflagjump", Command.sceneflag_jump},  // If flag X is set, jump to event Y. If flag -X is NOT set, jump to event Y

        {"playerlock", Command.lock_controls},  // Sets game flag to lock player controls
        {"playerfree", Command.free_controls},  // Clears game flag to free player controls
        {"playermoveto", Command.player_moveto},    // Move player to entity with tag X with x offset Y and y offset Z

        {"healthaddmax", Command.health_add_max},    // Adds X to max health
        {"weaponunlock", Command.weapon_unlock},    // Unlocks weapon X
        {"weaponlock", Command.weapon_lock},    // Locks weapon X
        {"objectivecomplete", Command.objective_complete},    // Marks objective X as complete

        {"textprint", Command.text_print},  // Add text S to textbox
        {"textclear", Command.text_clear},  // Clears text box text
        {"textclose", Command.text_close},  // Closes text box instance

        {"entitycreate", Command.entity_create},    // Creates entity X at position (x = Y, y = Z)
        {"entitydestroy", Command.entity_destroy},  // Destroys entity with tag X
        {"entitymove", Command.entity_move},    // Moves entity with tag X to position (x = Y, y = Z)

        {"bgmplay", Command.bgm_play},  // Plays background music with key X
        {"bgmstop", Command.bgm_stop},  // Stops background music
    };

    // Command enum. See definitions after
    public enum Command
    {
        zero,

        // Control -----------------------------
        jump,
        advance,
        wait,

        gameflag_set,
        gameflag_clear,
        gameflag_jump,

        sceneflag_set,
        sceneflag_clear,
        sceneflag_jump,

        // Player
        lock_controls,
        free_controls,
        player_moveto,
        health_add_max,
        weapon_unlock,
        weapon_lock,
        objective_complete,

        // Text --------------------------------
        text_print,
        text_clear,
        text_close,
        
        // Entity -------------------------------
        entity_create,
        entity_destroy,
        entity_move,
        entity_moveto,

        // Music --------------------------------
        bgm_play,
        bgm_fadeout,
        bgm_stop,

        sound_play,
        sound_playat,
        sound_stop,
    }

    // ----------------------------------------------------------------------------

    enum State
    {
        zero,
        running,
        wait,
        advance,
    }

    // ----------------------------------------------------------------------------

    public struct CommandDef
    {
        public Command command;
        public float[] values;
        public string text;
    }

    // ============================================================

    private CommandDef[] commanddata;
    private int commandpos;
    private CommandDef activecommand;
    private State state;
    private float waitstep;

    // ============================================================

    // Start is called before the first frame update
    void Awake()
    {
        Clear();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        switch(state)
        {
            // Continue event
            case(State.running):
            {
                ContinueEvent();
            }
            break;

            // Continue event when timer expires
            case(State.wait):
            {
                if (waitstep > 0.0f)
                {
                    waitstep -= 1.0f;
                }
                else
                {
                    waitstep = 0.0f;
                    ContinueEvent();
                }
            }
            break;

            // Continue event when JUMP is pressed
            case(State.advance):
            {
                if ( 
                    Input.GetButtonDown("Jump") ||  // Advance on press
                    (Input.GetButton("Jump") && Input.GetButton("Menu"))  // Advance on hold
                    )
                {
                    ContinueEvent();
                }
            }
            break;
        }
    }

    // Returns true if event is in progress
    public bool IsRunning()
    {
        return commanddata != null;
    }

    // Resets runner values and stops event
    public void Clear()
    {
        state = State.zero;
        commanddata = null;
        commandpos = 0;
    }

    // Set commands and run event
    public void StartEvent(CommandDef[] _commands)
    {
        commanddata = _commands;
        commandpos = 0;
        state = State.running;
    }

    // Defines event in _GameHeader from given text
    static public CommandDef[] ParseEventText(string text)
    {
        char[] textchar = text.ToCharArray();
        char c;
        int pos = 0;
        int n = textchar.Length;

        string word = "";
        int readmode = 0;
        char stringstart;
        Command activecommand = Command.zero;
        List<float> values = new List<float>();
        string valuetext = "";
        List<CommandDef> commandlist = new List<CommandDef>();
        
        // Iterate through string characters
        while ( pos < n )
        {
            c = textchar[pos];

            switch(readmode)
            {
                // Read function names and event keys
                case(0):
                {
                    // Comments
                    if ( c == '/' )
                    {
                        if (pos < n-1 && textchar[pos] == '/')
                        {
                            while (textchar[pos] > '\n') // Skip until newline
                            {
                                pos++;
                            }
                        }
                    }

                    // Event Keys
                    if ( c == '#' )
                    {
                        string eventkey = "";
                        pos++;
                        c = textchar[pos];
                        while ( ( (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ) && pos < n )
                        {
                            eventkey += c;
                            pos++;
                            c = textchar[pos];
                        }

                        commandlist = game.DefineEvent(eventkey);
                    }
                    // Legal characters
                    else if ( (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') )
                    {
                        word += c;
                    }
                    // New Command
                    else if (c == '(')
                    {
                        //Debug.Log(word);
                        // Transition to value state
                        if ( cmdmap.ContainsKey(word.ToLower()) )
                        {
                            activecommand = cmdmap[word.ToLower()];
                            readmode = 1;
                            word = "";

                            values.Clear();
                            valuetext = "";
                        }
                        // Skip until end of unknown command
                        else
                        {
                            while ( textchar[pos] != ')' && pos < n )
                            {
                                pos++;
                            }
                        }
                    }
                    // Reset word on illegal character
                    else
                    {
                        word = "";
                    }
                }
                    break;
                
                // Read Function Values
                case(1):
                {
                    // New Value
                    if ( (c>='0' && c<='9') || c=='.' || c=='-' )
                    {
                        word = "";

                        // Read up to non numerical character
                        while ( ((c>='0' && c<='9') || c=='.' || c=='-') && pos < n )
                        {
                            word += c;
                            pos++;
                            c = textchar[pos];
                        }
                        values.Add( float.Parse(word) );
                        //Debug.Log(word);
                    }
                    // New String
                    else if (c=='\"' || c=='\'')
                    {
                        stringstart = c;
                        word = "";
                        pos++;
                        c = textchar[pos];

                        // Read up to next quote marks
                        while ( c != stringstart && pos < n )
                        {
                            word += c;
                            pos++;
                            c = textchar[pos];
                        }
                        word = word.Replace("\\n", "\n");
                        valuetext = word;
                        //Debug.Log(word);
                    }

                    c = textchar[pos];
                    
                    // End of command arguments. Return to previous mode
                    if (c==')')
                    {
                        var cmd = new CommandDef();
                        cmd.command = activecommand;
                        cmd.values = values.ToArray();
                        cmd.text = valuetext;
                        
                        commandlist.Add(cmd);

                        word = "";
                        readmode = 0;

                        //Debug.Log(string.Format("[{0} {1} \"{2}\"]", cmd.command, cmd.values.ToString(), cmd.text) );
                    }
                }
                    break;
            }

            pos++;
        }
    

        return commandlist.ToArray();
    }

    // ============================================================

    // Move event to next command
    public void ContinueEvent()
    {
        if (commanddata != null)
        {
            ProgressEvent();
        }
    }

    // ============================================================

    // Execute commands until interrupt or end of event
    private void ProgressEvent()
    {
        state = State.running;

        while (state == State.running)
        {
            // End of command data
            if (commandpos >= commanddata.Length)
            {
                Clear();
                return;
            }
            
            // Set active command
            activecommand = commanddata[commandpos];

            //Debug.Log(string.Format("{0}: {1}", commandpos, activecommand.command));

            // Execute code based on command index
            switch(activecommand.command)
            {
                // End Event
                default:
                    Debug.Log(string.Format("Unknown/undefined command \"{0}\" at position {1}", activecommand.command, commandpos));
                    goto case(Command.zero);

                case(Command.zero):
                    Clear();
                    break;
                
                // Control ---------------------------------------------------------------

                // Switch to a different Event
                case(Command.jump):
                    game.RunEvent(activecommand.text);
                    return;
                
                // Advance Event
                case(Command.advance):
                    state = State.advance;
                    break;
                
                // Wait
                case(Command.wait):
                    waitstep = activecommand.values[0];
                    state = State.wait;
                    break;
                
                // Game Flags
                case(Command.gameflag_set):
                    // Use text
                    if (activecommand.text != "")
                        game.GameFlagSet((GameFlag)Enum.Parse(typeof(GameFlag), activecommand.text));
                    // Use first param
                    else
                        game.GameFlagSet((int)activecommand.values[0]);
                    
                    break;
                case(Command.gameflag_clear):
                    // Use text
                    if (activecommand.text != "")
                        game.GameFlagClear((GameFlag)Enum.Parse(typeof(GameFlag), activecommand.text));
                    // Use first param
                    else
                        game.GameFlagClear((int)activecommand.values[0]);
                    break;
                case(Command.gameflag_jump): {
                    int flagindex = (int)activecommand.values[0];
                    // Test if flag is set if flag index is positive
                    // Test if flag is not set if flag index is negative
                    if (flagindex >= 0? game.GameFlagGet(flagindex): !game.GameFlagGet(-flagindex))
                    {
                        game.RunEvent(activecommand.text);
                        return;
                    }
                    break;
                }
                
                // Scene Flags -----------------------------------------------------------
                case(Command.sceneflag_set):
                    game.SceneFlagSet((int)activecommand.values[0]);
                    break;
                case(Command.sceneflag_clear):
                    game.SceneFlagClear((int)activecommand.values[0]);
                    break;
                case(Command.sceneflag_jump): {
                    int flagindex = (int)activecommand.values[0];
                    // Test if flag is set if flag index is positive
                    // Test if flag is not set if flag index is negative
                    if (flagindex >= 0? game.SceneFlagGet(flagindex): !game.SceneFlagGet(-flagindex))
                    {
                        game.RunEvent(activecommand.text);
                        return;
                    }
                    break;
                }

                // Player ------------------------------------------------------------------

                // Lock Player Controls
                case(Command.lock_controls):
                    game.GameFlagSet(GameFlag.lock_player);
                    break;
                
                // Free Player Controls
                case(Command.free_controls):
                    game.GameFlagClear(GameFlag.lock_player);
                    break;
                
                // Move Player to Entity
                case(Command.player_moveto): {    // entityPosition(xoffset, yoffset, "<tag>")
                    var e = Entity.FindEntity(activecommand.text);
                    if (e != null)
                    {
                        var pos = e.transform.position;
                        game.GetPlayer().PositionSet(
                            pos.x + activecommand.values[0],
                            pos.y + activecommand.values[1]
                        );
                    }
                }
                    break;
                
                // Add X to Max Health
                case(Command.health_add_max):
                    game.GetPlayerData().AddHealthMax((int)activecommand.values[0]);
                    break;
                
                // Unlocks weapon X
                case(Command.weapon_unlock):
                    game.GetPlayerData().UnlockWeapon((int)activecommand.values[0]);
                    break;
                
                // Unlocks weapon X
                case(Command.weapon_lock):
                    game.GetPlayerData().LockWeapon((int)activecommand.values[0]);
                    break;
                
                // Marks objective X as complete
                case(Command.objective_complete):
                    game.GetPauseMenu().CompleteObjective((int)activecommand.values[0]);
                    break;
                
                // Text ------------------------------------------------------------------

                // Add text to textbox
                case(Command.text_print):
                    game.GetTextbox().AddText(activecommand.text);
                    state = State.zero;
                    break;
                
                // Clear text from textbox
                case(Command.text_clear):
                    game.GetTextbox().ClearText();
                    break;
                
                // Close textbox
                case(Command.text_close):
                    game.GetTextbox().Close();
                    //state = State.zero;
                    break;
                
                // Entities ------------------------------------------------------------
                case(Command.entity_destroy): {
                    foreach (Entity e in Entity.FindEntities(activecommand.text))
                    {
                        Destroy(e.gameObject);
                    }
                }
                    break;
                
                // Sound ---------------------------------------------------------------
                case(Command.bgm_play):
                    game.PlayBGM(activecommand.text);
                    break;
                
                case(Command.bgm_stop):
                    //game.PlaySound(activecommand.text);
                    break;
            }

            commandpos += 1;
        }
    }
}
