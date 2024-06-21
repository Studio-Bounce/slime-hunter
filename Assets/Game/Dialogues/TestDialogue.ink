-> blacksmith_greetings
=== blacksmith_greetings ===
Hello Traveller
-> blacksmith

=== blacksmith ===
I am the blacksmith. Need to sharpen your blade?
*   Yes[y] but I have no money.
    If you get rid of the slimes near my home, I'll do it for free.
    * *     Sure[], I'll help you!
            Great! Go to outskirt of the town and look to your right
            -> completion
    * *     Sorry! Maybe some other time.
            -> completion
*   No[]. Maybe some other time
    -> completion

=== completion ===
Okay, see you around!
-> DONE
