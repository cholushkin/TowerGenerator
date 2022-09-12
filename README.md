
# TowerGenerator Unity submodule
## FBX commands
FBX commands are properties that you can add in Blender to hierarchy elements. After Blender file is exported to Unity as FBX these propreties works as commands that configures **TowerGenerator Chunks**.
Each command could have from zero to several parameters. Parameters could be omitted then the default values take place. [Also see printable cheat sheet version](https://docs.google.com/spreadsheets/d/1sefKKZGdllpTpHPTX2AZMRrZCT5wT7QiN8GhPCoiGW4/edit?usp=sharing).

![This is an image](Images/Screenshot_1.png)
### GroupSet
-------------------
Adds GroupSet component to the node which makes the node be able to randomly enable some subset of its children. 

| Parameters | Description |
| --- | --- |
| int MinObjectsSelected = 0 | Minimum amount of items this GroupSet could activate |
| int MaxObjectsSelected = ChildrenNumber | Maximum amount of items this GroupSet could activate |

```
Examples:
GroupSet( ) // Enable from 0 to maximum children contained in this node.
GroupSet( 0, 2 ) // Enable from 0 to 2 children.
GroupSet( 1 ) // Enable from 1 child to maximum children amount in this node.
GroupSet( 1, 1 ) // Very common usecase - enable only one child of all available children in this node.
```

### GroupStack
-------------------
Adds GroupStack component to the node which makes nested nodes elements act as a stack elements. The stack could turn on contained elements sequentially one after another.

| Parameters | Description |
| --- | --- |
| int MinIndexSelected = -1 | Index of element which represents minimum possible stack level.|

Let's assume that the stack node contains 3 elements with corresponding indexes 0,1,2.
```
Examples:
GroupStack( ) // Enable randomly one of the stack state: xxx(all disabled), 0xx, 01x, 012(all enabled) 
GroupStack( 0 ) // MinimumIndexSelected = 0. Possible states are: 0xx, 01x, 012
GroupStack( 1 ) // Possible states are: 01x, 012
```

### CollisionDependent
------------------------
Add CollisionDependent component to the node which makes node/chunk auto disabled if collision accures.
| Parameters | Description |
| --- | --- |
| FragmentRelation FragmentDomination = Submissive| Possible values: Submissive/Dominant. Submissive nodes get disabled when collision happened during instantiation of neighbor chunk. Dominant node will not allow neighbur/current chunk to spawn if collision happened in neigbor/current chunk.
| CollisionCheck CollisionCheck = AABB | Possible values: AABB/ConvexHull/Mesh. Specifies collision check method. |

```
Examples:
CollisionDependent( ) // by default FragmentDomination will be Submissive
CollisionDependent( Dominant ) // make fragment disable other collided segments"
```


### AddCollider
-------------------
ChunkImportSource.AddColliders specifies should chunk importer add mesh colliders or not to all objects in the chunk. AddCollider FBX command indicates to add collider to specified node in the chunk even if ChunkImportSource.AddColliders equals to 'false'.

No parameters

```
Examples:
AddCollider()
```

### IgnoreCollider
-------------------
ChunkImportSource.AddColliders specifies should chunk importer add mesh colliders or not to all objects in the chunk. IgnoreCollider FBX command indicates to ignire adding collider to specified node in the chunk even if ChunkImportSource.AddColliders equals to 'true'.

No parameters

```
Examples:
IgnoreCollider()
```


### ToDo FBX commands
* Collider - make this node's geometry be a collider for mesh collider. Ignore import but take geometry for collider mesh.
* IgnoreImport - ignore the node and its children. Could be useful to delete some proxy-auxiliary geometry in the model.
* RandomScaleRange - scale node in specified range on instantiation. float FromScale | float ToScale
* RandomScaleValues - scale node by one of the value from the list. For example RandomScaleValues( 1.0, 2.0, 4.0 )
* AddAABBSizeMultiplier(2.0) "if (ExpandAABBMargin) bounds.Expand(Vector3.one * Meta.ChunkMargin * 2f);"
* AddAABBMargin(2.0) "if (ExpandAABBMargin) bounds.Expand(Vector3.one * Meta.ChunkMargin + 2f);"
