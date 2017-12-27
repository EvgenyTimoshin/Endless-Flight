# sumPause v1.0

- Unity Asset Store: *Submitted, Pending Approval* <<< **Current stable release**
- Project Homepage: http://jerrydenton.github.io/sumPause/
- Developer Contact: https://cyberlogical.com/sumpause/

## OVERVIEW
sumPause is a simple, lightweight, and open-source, Unity Asset for managing a paused/unpaused state. Drag-and-drop the prefab into your scene and instantly have a working pause button. Uses the native Unity UI so you can easily change the look and feel of everything right in the editor. Licensed under MIT and CC0 so there are no worries on usage rights.

## SETUP
- Create a Canvas if there is not one in your scene already. [Create > UI > Canvas] in Hierarchy
- Copy the included sumPause prefab into your Canvas.
- Your game now has a pause button! Try it out.
- (Optional) Adjust position on screen in the RectTransform component
- (Optional) Select different icons from the 'Sprites' folder and attach it to the prefab (Don't forget the 
    default sprite on the Image component)

**Check 'SampleScene' for example of extended setup options**

## OPTIONS
The following options are available on the 'SumPause' component
- *Use Event* : Whether to trigger an event for other objects to listen for [bool, def: true]
- *Detect Escape Key* : Whether to pause on Escape Key press [bool, def: true]
- *Paused Sprite* : Sprite to display when game is paused (Additional sprites included in 'Sprites folder)
- *Playing Sprite* : Sprite to display when game is playing (Additional sprites included in 'Sprites' folder)

## EXTENDED FEATURES

### Notifications/Events
If you need to notify other objects of a pause/resume state make sure the *Use Event* option is checked on the SumPause component. This will cause the **SumPause.pauseEvent** event to trigger. Now any script can react to changes by adding a listener (C# Example) - 

```csharp
    // Add the event listener
    void OnEnable() {
        SumPause.pauseEvent += OnPause;
    }

    // Remove the event listener
    void OnDisable() {
        SumPause.pauseEvent -= OnPause;
    }

    /// <summary>What to do when the pause button is pressed.</summary>
    /// <param name="paused">New pause state</param>
    void OnPause(bool paused) {
        if (paused) {
            // Code to execute when the game is paused
			Debug.Log("Pause");
        }
        else {
            // Code to execute when the game is resumed
			Debug.Log("Resume");
        }
    }
```

### Changing the pause action
The only thing happening by default on a pause/resume is a stop/start, of game time. This is fine for most cases, but if you need to do something extra look for, and edit, the following code in SumPause.cs -

```csharp
    /// <summary>This is what we want to do when the game is paused or unpaused.</summary>
    static void OnChange() {
        if(status) {
            // What to do when paused
            Time.timeScale = 0; // Set game speed to 0
        }
        else {
            // What to do when unpaused
            Time.timeScale = 1; // Resume normal game speed
        }
    }
```

## EXAMPLE
'SampleScene' contains working examples of usage with well commented code.

## PROJECT LICENSE
- The MIT License (MIT) - https://opensource.org/licenses/MIT
- Copyright (c) 2016 Jerry Denton

## OTHER ASSETS
- Icons are from the awesome CC0 collection by asset creator Kenney - https://kenney.itch.io/
- License (Creative Commons Zero, CC0) - http://creativecommons.org/publicdomain/zero/1.0/

## CREATED BY
- Jerry Denton
- http://www.cyberlogical.com

### CHANGE NOTES
----------------------------------------------------------

- v 1.0
- Initial version

----------------------------------------------------------

