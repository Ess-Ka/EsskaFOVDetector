# EsskaFOVDetector

Detects the vertical FOV of the main camera for VRChat.

![grafik](https://github.com/Ess-Ka/EsskaFOVDetector/assets/84975839/2ced1ee8-fedb-4fde-8b17-6ff11e926f44)

## Usage ##

Add the prefab to your scene and select your preferred detection mode. 

`On Start` Detection runs once on start for VR and desktop.

`Interval Desktop` Detection runs once on start for VR and desktop and than intervalled for desktop.

`Manually` Detection has to be started manually with the `StartDetction` method.

## Get the FOV ##

Use the `DetectedFOV` property on the component.


## React to changes ##

Register your component to FOVDetector and add the OnFOVChange method to react to changes. 

Example:
```
public class YourComponent : UdonSharpBehaviour {
    public FOVDetector FOVDetector;

    void Start() {
        FOVDetector.Register(this);
    }

    public void OnFOVChanged() {
        Debug.Log($"New FOV is {FOVDetector.DetectedFOV}°");
    }
}
```


