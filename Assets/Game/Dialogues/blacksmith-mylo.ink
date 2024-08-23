-> blacksmith_greetings

=== blacksmith_greetings ===
0: "Ah, Mylo! Good to see you. How have you been since our last encounter?"
* [Continue]
0: "What brings you here today? Looking for something special, perhaps?"
-> next

=== next ===
 * [Continue]
  -> mylo
  
=== mylo ===
1: "I'm doing well, thanks. I was actually hoping to find Kuro."
* [Continue]
-> mylo2

=== mylo2 ===
1: "Do you have any idea where he might be?"
* [Continue]
1: "I haven't crossed paths with him recently. I returned from the forest by boat, but Kuro was nowhere to be found."
    -> next2
    
=== next2 ===
 * [Continue]
  -> blacksmith
  
=== blacksmith ===

0: "Ah, I see. Kuro ventured towards the Nord Forest. But beware—the dock is swarmed with Gel. You might need the Alchemist's help to clear a path."
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
* ["Sure!"]1: "Absolutely, I'll take care of it. Just make sure you have my reward ready when I return."
-> next3

=== next3 ===
 * [Continue]
  -> completion

=== completion ===
0: "Thank you, Mylo. I'll be waiting for your return. Safe travels!"
-> DONE
