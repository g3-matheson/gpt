/*
    File I/O for chatgpt.csx
        * called by chatgpt-parser.csx for reading when --c present
        * called by chatgpt.csx for writing
*/

// use chatgpt-data.GPTJson here to store conversation and update accordingly
// expose public methods that add or remove from this structure
    // e.g AddPrompt() that adds the user message in the right place