I have created this project based on the work done by [@Antoyo](https://github.com/Antonyo/CefSharp/tree/IME_Support) here - https://github.com/Antonyo/CefSharp/tree/IME_Support
Idea is to create a CEF based IME without touching the internal C++ code. But, looks like there is a bug with cursor position 
and [@Antoyo](https://github.com/Antonyo/CefSharp/tree/IME_Support) has fixed that [here](https://github.com/Antonyo/CefSharp/commit/f39c4bd6d31d67c878367744ec9e45e5e9911bfa) 

The [CEFSHARP](https://github.com/cefsharp/CefSharp) code needs to be changed to have the correct IME support.

 [@Antoyo's](https://github.com/Antonyo/CefSharp/tree/IME_Support) code is a great start and needs further polishing because the 
 [IME Candidate Window](https://github.com/cefsharp/CefSharp/issues/1262#issuecomment-450226821) is not getting set correctly.
 
 **UPDATE
 Fixed it here now  https://github.com/angshuman-agarwal/CEFSHAPRP_WPF/tree/ime_candidate_window_fix
