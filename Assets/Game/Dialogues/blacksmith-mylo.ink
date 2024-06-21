-> blacksmith_greetings

=== blacksmith_greetings ===
0: "Hello Mylo, I hope you're doing well, the village is not by the way. The slimes really destroyed a lot of the property in the village."
-> next

=== blacksmith ===

0: "No. I have not seen him since the devastation of this village. <>
 Although, I need some help from you Mylo, can you go to the Gooey Forest and bring my gear? I will make sure your efforts dont go in vain!"

    -> myloresponse

=== mylo ===
1: "Yes I am doing well. I was fighting against the slimes at the Main gate but I got knocked out, Thanks to Lyra and Kuro they both took good care of me. <>
    speaking of which, have you seen Kuro around? I am not able to find him."
    -> next2
    
    === myloresponse ===
 * ["Sure!"]1: "Yes I would like to help you! Just keep my reward ready"
 -> completion
 
=== next ===
 * [Continue]
  -> mylo
  
=== next2 ===
 * [Continue]
  -> blacksmith

=== completion ===
0: "Okay, Thank you Mylo, see you around!"
-> DONE


 