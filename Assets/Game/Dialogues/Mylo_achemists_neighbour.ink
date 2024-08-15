-> neighbour_greetings

=== neighbour_greetings ===
0: "Greetings Traveller!"
* ["Hello!"]1: "Hello sir, I am looking for the Alchemist. Have you seen him around?"
  ** [Continue]
  -> alchemist_location

=== alchemist_location ===
0: "Hmm.. I have not seen him since morning."
* [Continue]
0: "Last I saw, he went to the old fish store. Maybe check there?"
-> mylo_response

=== mylo_response ===
* ["Okay"]1: "Sure, thank you! Have a good day."

-> DONE