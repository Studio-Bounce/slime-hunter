-> blacksmith_greetings

=== blacksmith_greetings ===
0: "Hello Mylo, I hope you're doing well, the village is not by the way."
* [Continue]
0: "The slimes really destroyed a lot of the property in the village."
-> next

=== next ===
 * [Continue]
  -> mylo
  
=== mylo ===
1: "Yes I am doing well. I was fighting against the slimes at the Main gate but"
* [Continue]
-> mylo2

=== mylo2 ===
1: "I got knocked out, Thanks to Lyra and Kuro they both took good care of me."
* [Continue]
1: "Speaking of which, have you seen Kuro around? I am not able to find him."
    -> next2
    
=== next2 ===
 * [Continue]
  -> blacksmith
  
=== blacksmith ===

0: "No. I have not seen him since the devastation of this village."
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
0: "Okay, Thank you Mylo, see you around!"
-> DONE
