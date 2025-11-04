INCLUDE globas.ink
->main

=== main ===    
Test dialgoue1. #layout:left #portrait:atlas1012 #speaker:???

* [Test dialgoue with none response]
    ->NoVoice
* {VoiceAtlas} [\[Atlas\]Test dialgoue with Atlas response]
    ->AtlasResponse
* {VoiceVox} [\[Vox\] Test dialgoue with Vox response]
    ->VoxResponse

===NoVoice===
Test dialgoue with none response. #layout:left #portrait:atlas1012 #speaker:???
->DONE
->END

===AtlasResponse===
Test comment with Atlas #layout:left #portrait:atlasResponse #speaker:Atlas
Test dialgoue with Atlas response. #layout:left #portrait:atlas1012 #speaker:???
->DONE
->END

===VoxResponse===
Test comment with Vox #layout:left #portrait:Vox1 #speaker:Vox
Test dialgoue with Vox response. #layout:left #portrait:atlas1012 #speaker:???
->DONE
->END

//%:;&::%:;%:& #layout:left #portrait:isp #speaker:I.S.P