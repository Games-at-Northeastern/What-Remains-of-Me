VAR deathCount = 0

Hey, look at me! #speaker:green #portrait:default #layout:left
I am green! You have died {deathCount}
-> first

=== first ===
What color are you?
+Blue -> really
+Orange -> interjection

=== really ===
Really, well, I guess you can be whatever you want to be!
-> END
=== interjection === 
Hmm, I feel like you are more of a brown color. #speaker:yellow #portrait:TestCharacter #layout:right
-> grr

=== grr ===
Butt out you poo poo. #speaker:green #portrait:default #layout:left
-> gr 

=== gr ===
Fine! #speaker:yellow #portrait:TestCharacter #layout:right
-> END

===function getDeathCount(newCount)===
~deathCount = newCount