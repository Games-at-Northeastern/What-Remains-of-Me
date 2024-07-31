INCLUDE ../../../InkDialogue/InkAndJSONFiles/globas.ink
->main

=== main ===

{levelStart == true:
    ->firstInteraction
  - else:
    ->doneMoving
}

===firstInteraction===
%&.:.. ...H-ello?... &%3.:; Atlas! You’re operational! #layout:right #portrait:Handler #speaker:The Handler

You seem to be intact. That’s good. Try moving around while I run a few tests on my %:%:&

~levelStart = false

->DONE


=== doneMoving ===
Everything looks good! Now listen here, the lab is overrun with %;.:-%% #layout:right #portrait:Handler #speaker:The Handler

I don't have much time to explain the gravity of the situation, but I need your help.

Their leader uploaded themselves to the our defense system at the lab's lobby
Now we’re :;%-%.. inside the lab with no escape!

I :;&%#- you to reclaim this lab. ;:7#%.,: fate of humanity *%&.;-. on you.
->END

//%:;&::%:;%:& #layout:right #portrait:Handler #speaker:The Handler