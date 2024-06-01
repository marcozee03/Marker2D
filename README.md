# Marker2D
 Adds a Marker2D attribute for Godot to be used in C#. In order to avoid cluttering up the scene tree with marker 2D nodes.  A Vector 2 can be used instead. 
 ### Current Limitations
-Undo and Redo is unsupported  
-Vector2 does not rotate with parent
## Getting Started
Tag Any Vector2 field with [Marker2D] and when the node is selected a dot will appear in the viewport marking the location of that point in local coords from its parent. which can then be moved around. Tag a Vector2 with [Marker2D] to visualize where the point is in the scene and move it around the viewport. While the Node2D is Selected

*might work with older versions of Godot but it is untested
