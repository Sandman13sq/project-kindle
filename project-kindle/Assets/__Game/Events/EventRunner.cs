using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public class EventRunner : MasterObject
{
    public enum Command 
    {
        zero,

        // Control -----------------------------
        advance,    // Go to next event
        wait,   // Wait X frames

        lock_controls,  // Lock player controls
        free_controls,  // Enable player cotrols

        // Text --------------------------------
        text_print,
        text_clear,
        text_close,
        
        // Entity
        entity_create,
        entity_destroy,
        entity_move,
    }

    static Dictionary<string, Command> cmdmap = new Dictionary<string, Command>() {
        {"end", Command.zero},

        {"wait", Command.wait},
        {"advance", Command.advance},
        {"adv", Command.advance},
        {"playerlock", Command.lock_controls},
        {"playerfree", Command.free_controls},

        {"textprint", Command.text_print},
        {"textclear", Command.text_clear},
        {"textclose", Command.text_close},

        {"entitycreate", Command.entity_create},
        {"entitydestroy", Command.entity_destroy},
        {"entitymove", Command.entity_move},
    };

    enum State
    {
        zero,
        running,
        wait,
        advance,
    }

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
    [SerializeField][TextArea(10, 10)] private string eventtext = "";

    // ============================================================

    // Start is called before the first frame update
    void Start()
    {
        state = State.running;
        waitstep = 0.0f;

        if (eventtext != "")
        {
            commanddata = ParseEventText(eventtext);
            game.RunEvent(this);
        }
    }

    // Update is called once per frame
    void Update()
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
                if ( Input.GetButtonDown("Jump") )
                {
                    ContinueEvent();
                }
            }
            break;
        }
    }

    public void SetEventCommands(CommandDef[] _commands)
    {
        commanddata = _commands;
        commandpos = 0;
        state = State.running;
    }

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
                // Read function name
                case(0):
                {
                    // Legal characters
                    if ( (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') )
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

    public void ContinueEvent()
    {
        if (commanddata != null)
        {
            ProgressEvent();
        }
    }

    // ============================================================

    private void ProgressEvent()
    {
        state = State.running;

        while (state == State.running)
        {
            // End of command data
            if (commandpos >= commanddata.Length)
            {
                state = State.zero;
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
                    state = State.zero;
                    break;
                
                // Control ---------------------------------------------------------------
                
                // Advance Event
                case(Command.advance):
                    state = State.advance;
                    break;
                
                // Wait
                case(Command.wait):
                    waitstep = activecommand.values[0];
                    state = State.wait;
                    break;

                // Lock Player Controls
                case(Command.lock_controls):
                    game.SetContolsLocked(true);
                    break;
                
                // Free Player Controls
                case(Command.free_controls):
                    game.SetContolsLocked(false);
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
            }

            commandpos += 1;
        }
    }
}