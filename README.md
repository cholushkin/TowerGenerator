
# TowerGenerator Unity submodule
## FBX commands
FBX commands are properties that you can add in Blender to hierarchy elements. After Blender file is exported to Unity as FBX these propreties works as commands that configures **TowerGenerator Chunks**.
Each command could have from zero to several parameters. Parameters could be omitted then the default values take place. 

![This is an image](Images/Screenshot_1.png)
### GroupSet
Adds GroupSet component to the node which makes the node to be able to randomly enable some subset of its children. 

| Parameters | Description |
| --- | --- |
| int MinObjectsSelected = 0 | Minimum amount of items this GroupSet could activate |
| int MaxObjectsSelected = ChildrenNumber | Maximum amount of items this GroupSet could activate |

**Examples**
```
GroupSet( ) // Enable from 0 to maximum children contained in this node.
GroupSet( 0, 2 ) // Enable from 0 to 2 children.
GroupSet( 1 ) // Enable from 1 child to maximum children amount in this node.
```
