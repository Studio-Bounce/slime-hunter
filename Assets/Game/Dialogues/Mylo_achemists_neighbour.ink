-> neighbour_greetings

=== neighbour_greetings ===
0: "Greetings, Traveller!"
* ["Hello!"]1: "Hello, sir. I am looking for the Alchemist. Have you seen him around?"
  ** [Continue]
  -> alchemist_location

=== alchemist_location ===
0: "Hmm... I haven't seen him since this morning."
* [Continue]
0: "He mentioned going to Orn Island for some research. You might want to check there."
-> mylo_response

=== mylo_response ===
* ["Okay"]1: "Thank you for your help. I’ll head to the Fish Store to take the boat to Orn Island."

-> DONE
