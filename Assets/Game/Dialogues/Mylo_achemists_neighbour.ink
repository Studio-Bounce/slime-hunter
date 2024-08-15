-> neighbour_greetings

=== neighbour_greetings ===
0: "Greetings Traveller!"
* ["Hello!"]1: "Hello sir, I am looking for the Alchemist. Have you seen him around?"
  ** [Continue]
  -> alchemist_location

=== alchemist_location ===
0: "Hmm.. I have not seen him since morning."
* [Continue]
0: "He was talking about going to the Orn Island to do some research. Maybe check there?"
-> mylo_response

=== mylo_response ===
* ["Okay"]1: "Sure, Need to take the boat from the Fish Store to go there. Thank you! Have a good day."

-> DONE