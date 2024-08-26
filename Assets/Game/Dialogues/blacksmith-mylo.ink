-> blacksmith_greetings

=== blacksmith_greetings ===
0: "Ah, Mylo! Good to see you've made it back."
* [Continue]
0: "What brings you here today? Looking for something special, perhaps?"
-> next

=== next ===
 * [Continue]
  -> mylo
  
=== mylo ===
1: "I'm doing well, thanks. I was actually hoping to find Master Kuro."
* [Continue]
-> mylo2

=== mylo2 ===
1: "Do you have any idea where he might be?"
* [Continue]
1: "I just arrived with Master Kuro by boat but he rushed ahead of me. Master said you would know where he is."
    -> next2
    
=== next2 ===
 * [Continue]
  -> blacksmith
  
=== blacksmith ===

0: "Classic Kuro, leaving his disciple to fend for themselves. Kuro just left for the Nord Forest. But beware—the dock is swarmed with slime gel. You might need the Alchemist's help to clear a path."
* [Continue]
-> blacksmith2

=== blacksmith2 ===
0: "Before you go, though, there's something I need from you. Can you venture into the Gooey Forest and fetch my gear?"
* [Continue]
-> blacksmith3

=== blacksmith3 ===
0: "I'll make sure your efforts are well rewarded. What do you say, Mylo?"
-> myloresponse
    
=== myloresponse ===
* ["Sure!"]1: "Absolutely, I'll take care of it."
-> next3

=== next3 ===
 * [Continue]
  -> completion

=== completion ===
0: "Thank you, Mylo. I'll be waiting for your return. Safe travels!"
-> DONE
