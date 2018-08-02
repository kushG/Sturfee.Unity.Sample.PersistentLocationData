# SUMMARY

This sample project saves digital objects placed in the real world by their GPS coordinates.

This means digital objects will remain in the same physical location when you close and reopen the app. For example, you may place a digital object next to a building, then the following day, reopen the app down the street, and notice the same object in the distance next to the same building.

While this sample project saves this data locally on your phone, this data could be saved on a server or made viewable to multiple players by other means if a developer decided to do so.

As an extra, this project also showcases the difference between tier 1, tier 2, and tier 3 detection.

# BUILD REQUIREMENTS

The default project has blank Mapbox and Sturfee keys.  
*You must plug in your own unique keys in order to use this sample game correctly.*

Go to [Mapbox.com](https://mapbox.com) and [Sturfee.com](https://sturfee.com) and create an account to generate your respective keys.

**How to apply Mapbox key:**

1. In the Unity project, on the top menu bar, click Mapbox -> Setup. A new window should appear.
2. Under "Access Token" paste your key value from the Mapbox website and press the "Submit" button

**How to apply Sturfee key:**

1. In the Unity project, on the top menu bar, click Sturfee -> Configure. A new window should appear.
2. Next to "API Key" there is an empty box. Paste your key value from the Sturfee website here and press the "Request Access" button

**Building to Android (With AR Core)**

1. Open the Unity project and click on the 'SturfeeXrSession' object in the Game scene hierarchy
2. Under the 'SturfeeXrSession' script in the Inspector view, click on the 'Provider Set' options. Make sure it is set to Custom -> ArCore Provider Set.
3. Then make sure that the 'Play On Start' option is toggled OFF
4. Scroll down to 'XR Settings' section and make sure  'ArCore Supported' is checked. 
5. Add 'STURFEE_ARCORE'(if not already added) to 'Scripting Define Symbols' in 'Android tab -> Other Settings -> Configuration' section of 'Player Settings' inspector window


**Building to iPhone (With AR Kit)**

1. Open the Unity project and click on the 'SturfeeXrSession' object in the Game scene hierarchy
2. Under the 'SturfeeXrSession' script in the Inspector view, click on the 'Provider Set' options. Make sure it is set to Custom -> ArKit Provider Set
3. Then make sure that the 'Play On Start' option is toggled ON

# HOW TO PLAY:

### Steps:
1. Go outside.
2. Check that your phone has GPS on with a good internet connection.
3. Open the app, and wait for the Sturfee session to initialize.
4. Align your phone roughly perpendicular to the ground in landscape mode as the prompt tells you to hold your phone up.
4. Press the scan button that appears after completing this prompt as you keep the phone camera level.
5. Stand still and move your phone in order to align the center of the screen with the circles placed in the environment.
6. Wait for localization to complete as your location is computed using the pictures just taken.
6. Once localization is complete, take note of where you are located in the mini-map view. You can also press on the 'Map View' button in the top right to get a full screen view, allowing you to move the map camera around as well.
7. On the right hand side there are several buttons. Tap the 'Tier 1 Item Placement' button, then tap on either the ground or a building to place an object at that location. 
8. This action calls the Sturfee server and might take a moment to register placement. A failed call could be the result of connection issues.
9. Now tap the 'Tier 3 Item Placement' button, then drag your finger across the terrain and buildings on screen. Notice that the object can be freely dragged along these environments.
10. This action uses preloaded building and terrain data to determine placement.
11. Determine a desirable location for the object, then press the 'Save Placement' button at the top.
12. Using Tier 2 Placement only takes in preloaded terrain data and does not hold building data. Thus it only enables ground placement.
12. Now press the 'Interact' button.
13. Tap on any of the objects you placed on screen and an option will appear to remove it. You may do so if you wish.
14. Make sure you have several objects placed in the scene, and then close the app.
15. Move to a new location not too far, in range of view of where you placed the objects in the physical world.
16. Reopen the app, you should now see 2 options, to either load your previous game, or start a new game. Choose 'Load Game'
17. Go through the localization process just as you did before, and notice the objects you placed to be where you left them in the environment, despite yourself having moved and restarted the app. 
18. You can open the map view to be able to see where items are placed for several blocks. If you place objects in entirely different areas, this map is not setup to view items too far away from your localization point, but if you localize back near the original location, you will be able to see them on the map again.


___


>NOTES:
> Unity Version: 2017.3.0f3
>
