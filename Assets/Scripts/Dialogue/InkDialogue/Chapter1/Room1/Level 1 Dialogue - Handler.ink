INCLUDE ../../../InkDialogue/InkAndJSONFiles/globas.ink
->main

=== main ===
{hasMoved: 
    ->doneMoving
  - else:
    ->firstInteraction
}


===firstInteraction===
%:;&::%:;%:& ...H-ello?...%:;&::%:;%:& Atlas! You’re operational! #layout:left #portrait:Handler #speaker:The Handler

You seem to be intact. That’s good. Try moving around while I run a few tests on my %:;&::%:;%:&
~hasMoved = true
->DONE


=== doneMoving ===
Everything looks good! Now listen here, the lab is overrun with %:;&::%:;%:& #layout:left #portrait:Handler #speaker:The Handler

I don't have much time to explain the gravity of the situation, but I need your help.

Their leader %:;&::%:;%:& uploaded themselves to the lab’s defense system, and now we’re locked out of all our own equipment!

I %:;&::%:;%:& you to reclaim this lab. %:;&::%:;%:& fate of humanity %:;&::%:;%:& on you.
->END

//%:;&::%:;%:& #layout:left #portrait:Handler #speaker:The Handler