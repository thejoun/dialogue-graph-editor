# Dialogue Tool



### What is it

It's a tool for visual edition of *Dialogue* graphs inside Unity.\
Useful in any project which involves conversations between characters.\
Feel free to use it in any way you want.

### How does it work

*Dialogues* represent conversations between any number of characters.\
A *Dialogue* consists of several *Sentences* - each represented by a node in the graph.\
Nodes can be connected by *Responses*. Each node can have several of them.\
Each sentence is spoken by an *Actor*, which represents an in-game character.\
*Dialogues* and *Actors* are ScriptableObjects, which means that you can easily move them between projects.

### How to install

Simply download and drop the contents of the *Assets* folder into your project assets.

### What's inside

- *Graph Editor* (*GE*) - editor window which shows the dialogue graph
- *Node Inspoector* (*NI*) - editor window which shows the contents of a node
- custom inspector for *Dialogue* assets
- premade *Dialogues* in the Examples folder

The editor windows can be accessed from *Tools* > *Dialogue Tool*.

### Where to start

- create a *Dialogue* asset (*RMB > Create > Dialogue*)
- open it
- click *Edit*

Two windows (*Graph Editor* and *Node Inspector*) will pop out.\
When *GE* is open, it will automatically show the currently selected *Dialogue* asset.\
*NI* shows the contents of a node selected in *GE*.
To create an *Actor*, go to *RMB > Create > Actor*

### How to use the *Graph Editor*

- *LMB* - select nodes, drag nodes, move the whole sheet around
- *RMB* - create connections (click on one node, then another)
- *Scroll wheel* - zoom in\out (or use the slider)
- green (+) button adds a a node
- red (-) buton removes selected node
- reset (R) button resets graph position and zoom

### What's next

This tool does not provide a way to actually show the dialogues in-game.\
I'm working on such a system and will share it when it's finished.\
Keep in mind that I'll be updating this Dialogue Tool quite often.\
The editor code is written using Unity's IMGUI.