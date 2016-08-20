Simple component to sync the JawBone of your character with batch of AudioClips.


How this works:
1. Add the component on a game object, it’s better to select your character and add this component.

2. Drag and Drop the jaw bone of your character to this component JAW.

3. Set the default audio volume of the sounds, you can customize the jaw position range by this. (default: 100)
4. Set the initial position of the jaw. (Default: 0)

Features:
- You can also use AutoPlay toggle to play the lip sync when scene starts.
- You can call the lipsysnc and customize which audio plays from the script.
How to call the lipsync in c#:<BR>
GetComponent<simpleLipSync>().isTalking = true;
GetComponent<simpleLipSync>().SetVolume(Volume Value Float);
GetComponent<simpleLipSync>().setSound(index of the sound you add on your component);

More:
- The Demo scene is also available in this package.