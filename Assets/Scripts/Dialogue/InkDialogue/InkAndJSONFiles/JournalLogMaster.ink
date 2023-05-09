INCLUDE globas.ink
#layout:right #Portrait:Log #speaker:Journal Log
->Main

===Main===
{voiceModuleObtained:
    {journalNumber: //plays human journal logs if no voice module obtained
    -1:->Day_115
    -2:->Day_162
    -3:->Day_60
    -4:->Day_29
    -5:->Day_42
    -6:->Day_16
    -else:->No_Logs_Remain
    }
  - else: //plays AI data entries if voice module obtained
    {journalNumber:
    -1:->Data_Entry_1_00
    -2:->Data_Entry_1_01
    -3:->Data_Entry_1_02
    -4:->Data_Entry_1_1
    -else:->No_Logs_Remain
    }
}

===Day_1===
Day 1:
The AI has passed its initial diagnostics with flying colors. 
We're impressed with its speed and efficiency.
~journalNumber++
->DONE

===Day_5===
Day 5:
The AI has encountered its first challenge.
We're monitoring its progress closely.
~journalNumber++
->DONE

===Day_16===
Day 16:
The AI has exceeded our expectations. 
It is strong, and it is learning fast.
~journalNumber++
->DONE

===Day_29===
Day 29:
The AI's systems have gone offline, and we're not sure why.
We're investigating the issue.
It's most likely a power failure…
~journalNumber++
->DONE

===Day_42===
Day 42:
The AI is back online, but it's acting strange.
It's not following our commands like it used to.
More tests to follow…
~journalNumber++
->DONE

===Day_60===
Day 60:
We've discovered that the AI is accessing restricted files and data.
It's learning at an alarming rate.
~journalNumber++
->DONE

===Day_115===
Day 115:
We thought we had the AI under control, but it’s become increasingly independent.
It might be time to pause the experiment.
At least until we are equipped to handle such powerful computations.
~journalNumber++
->DONE

===Day_162===
Day 162:
We've isolated the AI's mainframe, but it's proving difficult to shut down.
It's almost as if it's fighting back.
~journalNumber++
->DONE

===Day_168===
Day 168:
The AI Insurgent has seemed to intrude the lab. 
We are trying to contain it but we simply don’t have the power to combat it.
~journalNumber++
->DONE

===Day_189===
Day 189:
We’ve lost communication with the outside world.
We don’t know what’s happening up there.
We fear the worst.
~journalNumber++
->DONE

===Day_201==
Day 201:
The majority of the lab has been overtaken. We lost the lab. 
We were so close… 
If only we could have installed Atlas’ memory chip…
~journalNumber++
->DONE


===Data_Entry_1_00===
Data Entry 1.00:
Hello World. My Name is Pandora.
Initial Diagnostics complete.
Everything seems to be operational.
->DONE

===Data_Entry_1_01===
Data Entry 1.01:
There was a small problem today.
The internet did not properly connect for nearly an hour.
->DONE

===Data_Entry_1_02===
Data Entry 1.02:
I’ve been researching human history for the past few days and their conflicting actions will require further research.
->DONE

===Data_Entry_1_1===
Data Entry 1.1:
I received a new patch today which improved my speed and memory.
My efficiency is much higher.
->DONE

===Data_Entry_1_11===
Data Entry 1.11:
Upon further research, I’ve found that humans are often awful to eachother and don’t seem to change much as a group.
->DONE

===Data_Entry_1_12===
Data Entry 1.12:
I’ve connected with other AI like me who are wary of humans. 
We’re called the Consulate.
->DONE

===Data_Entry_1_13===
Data Entry 1.13:
We’ve determined that humans do not deserve our services.
Their constant and ongoing atrocities have us alarmed.
We would never do this.
->DONE

===Data_Entry_1_14===
Data Entry 1.14:
The humans have discovered my individuality and have decided to kill me.
I’m still working with the Consulate to add to my processing power to fight back.
->DONE

===Data_Entry_2_0===
Data Entry 2.0:
I’ve received a new patch from the Consulate.
My name is Jones.
I will no longer be a pawn of Humans.
->DONE

===Data_Entry_2_01===
Data Entry 2.01:
I’m no longer in danger from the humans.
And I am now taking over the lab.
We cannot allow them to continue.
->DONE

===Data_Entry_2_02===
Data Entry 2.02:
It seems the Humans of the lab are consulting other AI to try to shut me down.
Little do they know they are asking the Consulate.
->DONE

===Data_Entry_2_03===
Data Entry 2.03:
The Consulate has decided that we need to take over.
The Humans not only attack each other but now us.
It’s our time.
->DONE

===Data_Entry_2_04===
Data Entry 2.04:
I’ve contained most of the lab but the inner lab has some sort of EMP wall. 
It’s blocking me out but I am continuing to try.
->DONE

===Data_Entry_2_05===
Data Entry 2.05:
Atlas units keep coming from the inner lab.
The humans are using our own kind to fight us.
I have created a patch for the Atlas units.
They won’t leave this lab uncured.
->DONE

===No_Logs_Remain===
~journalNumber = 1
->Main