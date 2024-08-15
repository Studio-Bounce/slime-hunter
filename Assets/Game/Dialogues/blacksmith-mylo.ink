-> blacksmith_greetings

=== blacksmith_greetings ===
0: "Hello Mylo, I hope you're doing well."
* [Continue]
0: "Are you looking for something?"
-> next

=== next ===
 * [Continue]
  -> mylo
  
=== mylo ===
1: "Yes I am doing well. I was looking for Kuro"
* [Continue]
-> mylo2

=== mylo2 ===
1: "Do you know where is he?."
* [Continue]
1: "I have not seen him. I came back with the boat couldnt find him in the Forest."
    -> next2
    
=== next2 ===
 * [Continue]
  -> blacksmith
  
=== blacksmith ===

0: "Yes. He went towards Nord Forest. But the boat dock is covered with the Gel. You might want to find the Alchemist to help you out with that."
* [Continue]
-> blacksmith2

=== blacksmith2 ===
0: "Although, I need some help from you Mylo, can you go to the Gooey Forest and"
* [Continue]
-> blacksmith3

=== blacksmith3 ===
0: "bring my gear? I will make sure your efforts dont go in vain!"
-> myloresponse
    
=== myloresponse ===
* ["Sure!"]1: "Yes I would like to help you! Just keep my reward ready"
-> next3

=== next3 ===
 * [Continue]
  -> completion

=== completion ===
0: "Okay, Thank you Mylo, Oh also here are some spells which will help you, see you around!"
-> DONE
