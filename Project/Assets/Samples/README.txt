How to Create conveyer belt using Spline

Needs Blender for some parts/ can be used without just don't use blender meshes


1. Create new object in spline hierarchy - choose draw tool
	-conveyer can be joined together to make loop (not required)

2. Add Spline Mesh Resolution script to created spline

2.1. Assign Segment Mesh to one of the sample meshes found in the package under "Spline Mesh/ 1.4.1/ Conveyer Belt Sample/ Mesh"

2.2. assign Mesh Name - this will name the whole conveyer, so name appropriately

2.3. assign both Uv Resolutions and Mesh Resolution with a variable - 5 works as a starting point

2.4. hit Generate Mesh button


3. assign Materials in renderer - sample uses the Metal White material in Element 0 and Metal Black in Element 1
				- Sample Materials can be found in the same mesh location as part 2.1

3.1 Changing Uv Resolutions from part 2.3 will scale the moving conveyer part appropriately
				- Bigger number = smaller scale


4. Add Spline Instantiate to the spline object

4.1. Add to the list Items To Instantiate the Conveyer Legs found in "Spline Mesh/ 1.4.1/ Conveyer Belt Sample/ Prefabs"

4.2. set Rotation Offset X to 90

4.3. set Position offset to match conveyer - some tweaking will be required
					   - sample claims x is 0.3 and y is 0.63 to be correct
4.3.1. Hitting Bake Instances will create the legs as GameObjects so you can manually manoeuvre them

4.4. Instantiate Method can be used to set leg count and general position on conveyer
					- linear will instantiate legs at set distances apart
					- Instance count will set a specified leg count total and space accordingly


5. Add Spline Box Collider Generator Script to the spline object


5.1. Positions can be altered within the script

6. add a Rigid Body component to the spline object


7. add Animate Texture Offset script to the spline object

7.1. set Material index to 1 to match the material renderer second slot and offset direction to -1

7.2. adjust speed accordingly with how fast you need it - ONLY AFFECTS VISUALS - choose low decimal value like 1.3/ 1.45


8. Add Conveyer Belt Mover script to the spline object

8.1. assign Spline Container with the Spline Container inside spline object

8.2. Adjust speed accordingly - this affects the actual object move speed

8.3 Enable Snap Rotation to make objects rotate accordingly around conveyer if needed