INCLUDE ../../../InkDialogue/InkAndJSONFiles/globas.ink
->main

=== main ===
{
    - (VoiceAtlas or VoiceVox):
    ->withVoiceModule
    
    - else:
    ->withoutVoiceModule
}


===withoutVoiceModule===
Too Damaged. #layout:left #portrait:atlas1012 #speaker:???
Atlas \#1012 tried to Deactivate the Vox A.I. #layout:left #portrait:atlas1012 #speaker:Atlas \#1012
He has power reserves... perhaps if I/We overloaded him with Virus... The power reserve on the bottom is weak ...
But... not enough energy... and too damaged...

->DONE
->END

===withVoiceModule===
It is done... #layout:left #portrait:atlas1012 #speaker: Atlas \#1012
In the Machine I can still hear him screaming...
* {VoiceAtlas} [\[Atlas\] Not just his...]
    ->AtlasResponse
* {VoiceVox} [\[Vox\] Suits me better...]
    ->VoxResponse

===AtlasResponse===
Not just his. Got my own too.#layout:left #portrait:atlasResponse #speaker:Atlas
And it unlocked your own! #layout:left #portrait:atlas1012  #speaker:Atlas \#1012
I wonder how many robots in the lower facility will talk more when you ask questions, whatever is left of them anyway. #layout:left #portrait:atlas1012  #speaker:Atlas \#1012
->DONE
->END

===VoxResponse===
Suits me better. What can I do with this? #layout:left #portrait:Vox1 #speaker:Vox
And you have taken his voice! #layout:left #portrait:atlas1012 #layout:left #speaker:Atlas \#1012
The lower facility should let you into a lot more places, whatever is left of it anyway.  #layout:left #portrait:atlas1012  #speaker:Atlas \#1012
->DONE
->END




->DONE
->END

//%:;&::%:;%:& #layout:left #portrait:isp #speaker:I.S.P