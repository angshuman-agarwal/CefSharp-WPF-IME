I have created this project based on the work done by [@Antoyo](https://github.com/Antonyo/CefSharp/tree/IME_Support) here - https://github.com/Antonyo/CefSharp/tree/IME_Support
Idea is to create a CEF based IME without touching the internal C++ code but looks like there is a bug with cursor position 
and [@Antoyo](https://github.com/Antonyo/CefSharp/tree/IME_Support) has fixed that [here](https://github.com/Antonyo/CefSharp/commit/f39c4bd6d31d67c878367744ec9e45e5e9911bfa) 

This code is a great start but needs more polishing as the [IME Candidate Window](https://github.com/cefsharp/CefSharp/issues/1262#issuecomment-450226821) 
is not getting set correctly.
