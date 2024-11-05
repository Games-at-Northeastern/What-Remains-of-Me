INCLUDE ../../../InkDialogue/InkAndJSONFiles/globas.ink
->main

=== main ===
{
    - voiceModuleObtained:
    ->withVoiceModule
    
    - else:
    ->withoutVoiceModule
}


===withoutVoiceModule===
ACCESS DENIED: Force Door Deactivation #layout:left #portrait:isp #speaker:Force Door
ACCESS REQUIREMENT: Voice Command Module
->DONE
->END

===withVoiceModule===
ACCESS GRANTED: Force Door Deactivation #layout:left #portrait:isp #speaker:Force Door
WARNING: Surface Access. Hostile Forces Expected. Proceed with Caution.

...#layout:left #portrait:jones1 #speaker:Jones A.I.
...
...

You did it Atlas! Quickly, come to the surface. You can save us all! #layout:left #portrait:default #speaker:The Handler

->DONE
->END

//%:;&::%:;%:& #layout:left #portrait:isp #speaker:I.S.P