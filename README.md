# Breathing Forest

Video showcase at the bottom.

Reference: BreathVR: Leveraging Breathing as a Directly ControlledInterface for Virtual Reality Games
https://dl.acm.org/doi/epdf/10.1145/3173574.3173914

BreathVR: Leveraging Breathing as a Directly Controlled Interface for Virtual Reality Games serves as the main reference for this project due to its exploration of respiration as an embodied interaction technique in virtual reality. The paper investigates how breathing can function as a direct control interface rather than simply passive biometric monitoring. Instead of relying solely on traditional controllers and button-based interaction, the authors propose that physiological processes such as respiration can become meaningful and intentional forms of interaction within virtual environments. This philosophy aligns strongly with embodied interaction theory, where interaction is understood as a lived bodily experience rather than an abstract exchange between user and machine.

A central idea presented in the paper is that breathing creates a more natural and immersive relationship between the user and the virtual world. Because respiration is continuous, rhythmic and deeply connected to emotional and physical states, it enables interaction that feels more organic than conventional input methods. BreathVR demonstrates how breathing can directly manipulate gameplay mechanics, creating a feedback loop between bodily action and environmental response. This coupling between internal physiological state and external virtual space increases presence and encourages greater bodily awareness during interaction.

The philosophy behind BreathVR is highly relevant to this project, although the implementation differs in focus. While BreathVR primarily explores respiration as a gameplay mechanic, this project investigates breathing as an exploratory and atmospheric interaction technique. 

# Implementation and Design Description

Breathing Forest is a Virtual Reality experience allowing users to experiement with breathing patterns, resulting in changes in the environment.
Live respiration data is used to manipulate variables within a forest ecosystem simulation, allowing the user’s breathing patterns to shape environmental behaviour in real time. The user therefore becomes physiologically connected to the virtual environment. Rather than emphasizing challenge or performance, the project explores slower and more exploratory and experimental forms of interaction inspired by ideas from embodied interaction and somaesthetic design. In this way, breathing becomes both a method of control and a way of experiencing and inhabiting the virtual environment itself.

Rather than using respiration data as a direct one-to-one control signal, the project instead derives broader behavioural metrics from the user’s breathing patterns, such as calmness and intensity. These interpreted states are then used to influence environmental variables within the forest simulation. This approach shifts the interaction away from explicit machine-like control and toward a more indirect and embodied relationship between the user and the virtual environment, where the world responds to the overall qualities of bodily behaviour rather than isolated input values.

# Mappings

Breath depth rolling average 5 samples --> Short Term Fertility
			                      10 samples --> Long Term Fertility				

Breath rate rolling average 5 samples --> Short Term Calmness
			                      10 samples --> Long Term Calmness

Depth (fertility) and Rate (Calmness) combined 5 samples --> Short Term Warmth
					                                    10 samples --> Long Term Warmth

Seasons: Controlled by Long Term averages
- Spring: high calmness, medium fertility
- Summer: max calmness, max fertility 
- Fall: low calmness, medium fertility 
- Winter: min calmness, min fertility 

Light:
- Intensity: Short Term Fertility
- Colour (Cold To Warm): Short Term Warmth

Grass:
- Waving Speed: Short Term Calmness
- Bending: Short Term Fertility

Trees:
- Tree Leaves and Trunk Colours: Controlled by seasons
- Tree Amount of Leaves: Short Term Fertility (Using Motion Time Animation)
  
<img width="800" height="800" alt="image" src="https://github.com/user-attachments/assets/1538fdb3-7a9b-4a67-b9e4-7f3a39c07112" />

# Pipeline
CODE:
https://github.com/laurman1/EmbodiedInteraction/tree/main/Assets/Laus%20rod/EI/Scripts

Using the BioSignal Plux Piezo-Electric Respiration (PZT) Sensor I record live breathing data from the user in OpenSignals app. I receive the data in Unity using LSL's provided "SimpleInletScaleObject" script. My "BreathDataHandler" script then takes the raw value and computes polishes it to compute clean peaks and troughs. Finally it passes on the 4 varibales; "short/longTermAverageBreathRate/Depth". My "EcoSystemControls" script then takes these metrics, normalizes them and using them creates the environmetal metrics that are, Fertility, Calmness and Warmth. Finally "EcoSystemEffetcts" receives them, and creates a 2D space of Fertility and Calmness and assigning the 4 seasons to specific locations in the space. This script is also where all of the environmental effects are applied.
<img width="800" height="800" alt="image" src="https://github.com/user-attachments/assets/abb77dbf-5a1a-47cc-9fc3-1ee40af9b47a" />

#Note
This project is made as a spin-off to the main semsester project. I made a copy of the semester Unity project, and made the Embodied Interaction project in a new scene, hence the big file. Everything related to this project is in the 'Assets/Laus rod/EI/ folder. Inside there is also the scripts folder containing the scripts i made for this project, also linked above.

# Video
https://www.youtube.com/watch?v=UG2g0TyRfzI


