# MCUnity

MechCommander Gold Mission Viewer made in Unity.

## Installation

* Download this repository as Zip or git clone it.
* Open it as a new Project in Unity.
* Open Scene Scenes/MissionLoader
* Select 'MechCommanderUnity' Object in Hierarchy
* Click Browse button on Inspector.
* Select one original MechCommanderGold exe file (with a valid instalattion folder).
* If it´s valid folder it will Copy and decompress a number of files from the MCG Install dir. (it will take 2-3minutes)
* When it´s over, select a Map from 'Select a Map to Scene Load' and hopefully it will Load the Map/Mission

![Editor Preview](https://i.imgur.com/KNZNKM0.png)


### Prerequisites:
* Unity Editor, i´m using 2019.3 , probably it will work with any version post 2017.*
* Valid Mechcommander Gold folder or CD

### What´s working:
* Maps with Tiles and OverlayTiles for the Mission.
* Terrain Objects: (trees, lamps) static and with animation if the basic state has animation
* Buildings: static and with animations if the basic state has animation. Some Buildings "Shadow" is not corretly positioned.
* Turrets: static and with animations if the basic state has animation. In the Turrets i´ve put a rotation change code every X seconds.

### Included:
* API folder with most of the MechCommander Gold file formats included (Readers)


## License
[MIT](https://choosealicense.com/licenses/mit/)
