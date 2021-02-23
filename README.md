# Dialogue System for Unity

Dialogue System is a simple framework for edition and display of dialogues inside Unity. It allows you to:

- edit dialogue graphs and data visually
- create actors with several expressions
- display the dialogues in a pleasant VN style
- handle text-based triggers
- easily move created assets around

Coming soon:

- set conditions for specific responses
- export dialogues to JSON

<img src="https://github.com/TheJonu/Dialogue-Tool/blob/main/img/screen1.png">

<img src="https://github.com/TheJonu/Dialogue-Tool/blob/main/img/screen2.png">

<img src="https://github.com/TheJonu/Dialogue-Tool/blob/main/img/screen3.png" width="60%">

### Installation

To use the system in your project, download and import *DialogueSystem.unitypackage*.

You need these other packages to view dialogues in-game:

- LeanTween (tested with v2.50) ([link](https://assetstore.unity.com/packages/tools/animation/leantween-3595))
- TextMeshPro (tested with v3.0.1)

The system has been tested in Unity 2020.2.3f1 and higher.

### Content

- Framework
    - data - stores info about dialogues, actors
    - controller - controls the flow of dialogues and handles events
    - view - displays the dialogue in-game and accepts input
- Tools
    - *Graph Editor* - edit dialogue graphs
    - *Node Editor* - edit sentences and responses
- Examples 
    - test scene with a test dialogue
    - basic adjustable UI setup
    - other example assets

### Where to begin

- create a Dialogue [*RMB > Create > DialogueSystem > Dialogue*]
- create an Actor [*RMB > Create > DialogueSystem > Actor*]

To open the editor tools, you can either hit the *Edit* button in a Dialogue's inspector, or acces them from [*Tools > DialogueSystem*].

### How to use the *Graph Editor*

- *LMB* - select nodes, drag nodes, move the whole sheet around
- *RMB* - create connections (click on one node, then another)
- *Scroll wheel / slider* - zoom in and out
- *green (+) button* - add a a node
- *red (-) button* - remove the selected node
- *reset (R) button* - reset graph position and zoom