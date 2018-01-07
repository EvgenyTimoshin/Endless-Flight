The Draw Call Minimizer is the next step in optimizing your environments.

What Draw Call Minimizer does is create a texture atlas and as few 
meshes as possible (Unity has a 65000 vertice limit on objects)
no matter how many materials are childed under the game object.

New in Version 2.0:
-Complete overhaul of entire project:

	-Editor batching tool now complete, users can batch meshes together in the editor, and choose whether or not they want textures atlased together or if they just want regular batching
	-Editor batcher also exports the objects into the project so the user can view, edit and manipulate them at any time
	-Code split up into namespaces so that similar named scripts in user project will no longer interfere and cause issues
	-DrawCallMinimizer script is much easier to understand and is much easier to manipulate
	-Now has helpful links in the menu to the wiki for documentation

New in Version 1.3:

-Support for multiple shaders!
-Now Draw Call Minimizer will create objects based on their shader type, and creates texture atlases for each shader.
-It can now also support custom shaders that you have programmed yourself, all you need to do is enter what you named
the shader properties into the script.