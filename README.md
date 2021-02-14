# Dialogue System for Unity

It's a framework for edition and display of dialogues inside Unity.\

![Editor](https://github.com/TheJonu/Dialogue-Tool/blob/main/img/screen1.PNG "Editor")

![Display](https://github.com/TheJonu/Dialogue-Tool/blob/main/img/screen1.PNG "Display")

### Description

A *Dialogue* is a conversation between any number of *Actors*.\
It consists of a graph of *Sentences* connected to each other by *Responses*.\
*Dialogues* and *Actors* can be easily moved between projects and are not scene-dependent.\

### Installation

Either download and import *DialogueSystem.unitypackage* into your project or directly drop the *DialogueSystem* folder into your *Assets*.

### Content

- data framework
- view framework
- *Graph Editor* tool
- *Node Editor* tool
- example dialogue and assets

### Where to begin

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
