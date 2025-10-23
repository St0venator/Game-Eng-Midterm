# Game Eng Midterm

The group consists of only me, Steven Smith, 100816723 The strategy behind forming this group was that no one in the class was willing to do the project with me, so I decided to complete it alone. This was also to help ensure every part of this project was completed to a level that I was satisfied with, as working alone gives me complete and total control over every aspect of the project.

This project represents a natural terrain generator. This project, using a small amount of presets and more confurable settings in order to generate random terrain complete with foliage and solid terrain, as well as a water layer at a configurable height.

Singleton: My singleton implementation will come in the form of a deployer for the script that creates the mesh, so that the user can specify where, how large, and other aesthetic factors.

Factory: The factory implementation will come in the form of the object that creates the mesh for the terrain, which will move around the scene and scale itself according to the controller's inputs, and create child objects which contain the terrain and colliders.

Command: The command pattern comes in the form of a second pass foliage shader, which will wait to recieve a command from the terrain factory to indicate the terrain is finalized, before it uses raycasting and prefabs to populate the terrain with a preset variety of foliage.
