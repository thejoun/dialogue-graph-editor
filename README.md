# Dialogue Graph Editor for Unity

Dialogue Graph Editor is a simple tool and dialogue system for Unity that allows you to:

- edit dialogue graphs visually
- display the dialogues at runtime
- create actors with several expressions
- easily move created assets around

<img src="https://github.com/TheJonu/Dialogue-Tool/blob/main/img/screen4.png">

### Installation

To use the system in your project, download and import *DialogueSystem.unitypackage*.  
The whole project also contains these dependencies, which are needed to display the dialogues at runtime:

- LeanTween (tested v2.50) ([link](https://assetstore.unity.com/packages/tools/animation/leantween-3595))
- TextMeshPro (tested v3.0.1)

Tested in Unity 2020.2.3f1 and 2020.3.8f1.

### Content

- Runtime
    - data - structures for dialogues, nodes, actors
    - logic - controls the dialogues' flow
    - view - displays dialogue in-game
- Editor
    - *Graph Editor* - edit dialogue graphs
    - *Node Editor* - edit sentences and responses
- Examples
    - test scene with a sample dialogue
    - basic adjustable UI setup
    - other example assets

### Where to begin

- create a **Dialogue** [*RMB > Create > DialogueSystem > Dialogue*]
- create an **Actor** [*RMB > Create > DialogueSystem > Actor*]

- open the **Graph Editor** tool [*Tools > DialogueSystem > Graph Editor*]
- open the **Node Editor** tool [*Tools > DialogueSystem > Node Editor*]

You can also open the editor tools by selecting a dialogue and clicking "Edit".

### How to use the *Graph Editor*

- *LMB* - select nodes, drag nodes, move the whole sheet around
- *RMB* - create connections (click on one node, then another)
- *Scroll wheel / slider* - zoom in and out
- *green (+) button* - add a a node
- *red (-) button* - remove the selected node
- *reset (R) button* - reset graph position and zoom

### How to test

Open the example scene (*DialogueSystem > Examples > Example Scene*), run and press `T` to start the dialogue.

<img src="https://github.com/TheJonu/Dialogue-Tool/blob/main/img/screen3.png" width="60%">