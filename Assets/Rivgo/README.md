# 🔦 Flashlight System for Unity

A modular flashlight system for your Unity projects, designed for easy integration and customization. It includes core flashlight functionality, optional blinking effects, audio feedback, support for light cookies.

## ✨ Features

*   **Core Flashlight:** Simple on/off switching.
*   **Modular Blinking:** Customizable blinking/flickering effect component (`FlashlightBlinker.cs`).
*   **Modular Audio:** Customizable sound effects for switching and blinking (`FlashlightAudio.cs`). 🔊
*   **Light Cookies:** Supports Unity's Light Cookies for custom light shapes. Sample cookies provided. 🍪
*   **New Input System Ready:** Integrates with Unity's Input System via the `FlashlightInput.cs` component and a pre-configured `InputActionAsset`. 🎮

## 🚀 Quick Setup Guide

1.  **Install Dependencies:**
    *   If not already installed, open the Package Manager (`Window > Package Manager`).
    *   Find "Input System" in the "Unity Registry" and click "Install".

2.  **Input Action Setup (for `SwitchFlashlight`):**
    The package includes `Assets/Rivgo/FPC/Rivgo_FPC_InputSystem_Actions.inputactions`. This asset pre-defines the `SwitchFlashlight` action (bound to the 'E' key by default).
    To use it with your player:
    *   Select your player GameObject.
    *   Add a `PlayerInput` component to it.
    *   In the `PlayerInput` component:
        *   Assign the `Rivgo_FPC_InputSystem_Actions` asset (from `Assets/Rivgo/FPC/`) to the **Actions** field.
        *   Set **Behavior** to **Invoke Unity Events**.
        *   Expand **Events > Player**.
        *   Find the **SwitchFlashlight** event. Click the `+` to add a listener.
        *   Drag the GameObject that has the `FlashlightInput.cs` script onto the object slot.
            *   *If using the included `Flashlight.prefab` as a child of your player, and `FlashlightInput.cs` is on the player, drag the player GameObject.*
            *   *If `FlashlightInput.cs` is on the Flashlight prefab itself, drag the Flashlight GameObject.*
            *   The included `= Player =.prefab` (see step 4b) already has `FlashlightInput.cs` on the root and is configured.
        *   From the function dropdown, select `FlashlightInput > OnSwitchFlashlight()`.

3.  **Using the Flashlight:**

    *   **Option A: Add Flashlight Prefab to Your Player:**
        *   Locate the `Flashlight.prefab` at `Assets/Rivgo/FlashlightSystem/Flashlight.prefab`.
        *   Drag it into your scene, typically as a child of your player's camera or a "hand" transform.
        *   Ensure your player GameObject (or the Flashlight prefab itself, if preferred) has the `FlashlightInput.cs` script attached and the `PlayerInput` component is configured as described in step 3.

    *   **Option B: Use the Included Player Prefab (Recommended for Quick Start 👍):**
        *   Locate the `= Player =.prefab` at `Assets/Rivgo/FPC/Prefabs/= Player =.prefab`.
        *   Drag this prefab into your scene.
        *   This player prefab comes with:
            *   `SimpleFPC.cs` for movement.
            *   `PlayerInput` component pre-configured with `Rivgo_FPC_InputSystem_Actions`.
            *   The `Flashlight.prefab` already attached as a child of its Main Camera.
            *   `FlashlightInput.cs` on the player root, linked to the `SwitchFlashlight` action.
            *   It's ready to go! Press 'E' to toggle the flashlight.

4.  **Demo Scene:**
    *   Open the scene `Assets/Rivgo/FlashlightSystem/Demo/Scenes/DEMO_Flashlight_System.unity`.
    *   This scene uses the `= Player =.prefab` and demonstrates a fully working setup.

## 🧩 Modularity

The flashlight system is designed to be modular:

*   **Blinking (`FlashlightBlinker.cs`):**
    *   This component is attached to the `Flashlight.prefab`.
    *   You can customize its extensive parameters in the Inspector to change blinking behavior.
    *   To disable blinking entirely, you can:
        *   Uncheck "Is Blinking Active" on the `FlashlightBlinker` component.
        *   Or, remove the `FlashlightBlinker` component from the `Flashlight.prefab`.
*   **Audio (`FlashlightAudio.cs`):**
    *   This component is also on the `Flashlight.prefab` and uses an `AudioSource` component.
    *   You can assign your own `AudioClip` arrays for switch and blinking sounds.
    *   Adjust `AudioSource` settings (e.g., volume) as needed.
    *   If you don't want audio, simply remove the `FlashlightAudio` component (and the `AudioSource` if not used for anything else).

## 🍪 Light Cookies

*   The `Light` component within the `Flashlight.prefab` (specifically on the child GameObject named "Light") has a **Cookie** slot.
*   Sample light cookies are provided in `Assets/Rivgo/FlashlightSystem/LightCookies/`.
*   Drag any of these (or your own 2D texture, properly configured as a Cookie) into this slot to change the shape and pattern of the flashlight beam.

## 📜 LICENSE
This project is distributed under the MIT license. See the LICENSE file for details.

## ☁️ Getting the Latest Version

Download the latest version from the GitHub releases for this project.

---

## 📚 Detailed Documentation

### 1. Introduction

The Flashlight System provides a comprehensive solution for implementing a flashlight in Unity. It's built with modularity in mind, allowing developers to easily enable, disable, or customize features like blinking and audio. It uses interfaces for core functionality, making it extensible. The system is set up for use with Unity's Universal Render Pipeline (URP) and the New Input System.

### 2. 📂 Directory Structure Overview

*   **`Assets/Rivgo/`**: Root folder for the package.
    *   **`FlashlightSystem/`**: Core components and assets for the flashlight.
        *   **`Demo/`**: Contains assets for the demo scene.
            *   `Materials/`: Materials used in the demo.
            *   `Models/`: Simple models for the demo environment.
            *   `Scenes/`: The `DEMO_Flashlight_System.unity` scene and its lighting data.
            *   `Textures/`: Textures for the demo materials.
        *   **`LightCookies/`**: A collection of sample 2D textures to be used as light cookies.
        *   **`Prefabs/`**:
            *   `Flashlight.prefab`: The main flashlight prefab with all its logic.
        *   **`Scripts/`**: C# scripts for the flashlight functionality.
            *   **`Abstractions/`**: Defines interfaces for core components.
                *   `IFlashlightCore.cs`: Interface for the basic flashlight operations.
                *   `IFlashlightBlinker.cs`: Interface for the flashlight blinking behavior.
            *   `FlashlightCore.cs`: Default implementation of `IFlashlightCore`.
            *   `FlashlightBlinker.cs`: Default implementation of `IFlashlightBlinker`.
            *   `FlashlightAudio.cs`: Handles audio playback for flashlight events.
            *   `FlashlightInput.cs`: Handles input for toggling the flashlight using Unity's New Input System.
        *   **`Sounds/`**: Sample audio clips.
            *   `Blinking/`: Sounds for the blinking effect.
            *   `Switch/`: Sounds for toggling the flashlight on/off.
    *   **`FPC/`**: Components for the First-Person Controller.
        *   **`Prefabs/`**:
            *   `= Player =.prefab`: A ready-to-use FPC prefab that includes the flashlight.
        *   **`Scripts/`**:
            *   `SimpleFPC.cs`: Script for the first-person character movement and camera look.
        *   `Rivgo_FPC_InputSystem_Actions.inputactions`: The `InputActionAsset` used by the FPC and `FlashlightInput`.
*   **`Assets/Settings/`**: Contains URP-related settings assets. These are part of the project setup used to develop the package and are included for the demo scene to work correctly.

### 3. Core Components 🔩

#### 3.1. `FlashlightCore.cs` (implements `IFlashlightCore`)

*   **Location:** `Assets/Rivgo/FlashlightSystem/Scripts/FlashlightCore.cs`
*   **Purpose:** Manages the fundamental state (on/off) of the flashlight and controls the actual Unity `Light` component.
*   **Inspector Fields:**
    *   `_lightSource` (Light): Reference to the Unity `Light` component that acts as the flashlight beam. If not assigned, it attempts to find one in its children during `Awake()`.
    *   `_startsOn` (bool): If checked, the flashlight will be turned on when the game starts.
*   **Public API:**
    *   `bool IsOn { get; }`: Returns `true` if the flashlight is currently on, `false` otherwise.
    *   `Light LightSource { get; }`: Provides access to the Unity `Light` component.
    *   `Transform FlashlightTransform { get; }`: Returns the `Transform` of the GameObject this script is attached to.
    *   `void Switch()`: Toggles the flashlight state (on to off, or off to on).
    *   `void TurnOn()`: Turns the flashlight on.
    *   `void TurnOff()`: Turns the flashlight off.
*   **Events:**
    *   `event Action<bool> OnStateChanged`: Invoked when the flashlight state changes. The `bool` parameter is `true` if turned on, `false` if turned off.
    *   `event Action OnTurnedOn`: Invoked specifically when the flashlight is turned on.
    *   `event Action OnTurnedOff`: Invoked specifically when the flashlight is turned off.
*   **Customization:**
    *   Change `_startsOn` in the Inspector.
    *   Manually assign a different `Light` component to `_lightSource`. Ensure this `Light` component is configured as desired (e.g., Type: Spot, Color, Intensity, Range, Spot Angle, Cookie).
*   **Extensibility:** If you need more complex core logic (e.g., battery life, different light modes), you can create your own script that implements `IFlashlightCore` and replace `FlashlightCore.cs` on the prefab.

#### 3.2. `FlashlightBlinker.cs` (implements `IFlashlightBlinker`)

*   **Location:** `Assets/Rivgo/FlashlightSystem/Scripts/FlashlightBlinker.cs`
*   **Purpose:** Adds a configurable blinking or flickering effect to the flashlight when it's on and blinking is active.
*   **Inspector Fields:**
    *   **Blinking Activation:**
        *   `Is Blinking Active` (bool): Master toggle for the blinking behavior. If true, blinking can occur. If false, it's disabled. (This is the public property, serialized for editor convenience).
    *   **Overall Cycle Timing:**
        *   `_minIntervalBetweenBlinkBursts` (float): Minimum time (seconds) the light stays steady before a new blinking burst can start.
        *   `_maxIntervalBetweenBlinkBursts` (float): Maximum time (seconds) the light stays steady.
        *   `_minBlinkingBurstDuration` (float): Minimum total duration (seconds) of a single blinking burst.
        *   `_maxBlinkingBurstDuration` (float): Maximum total duration (seconds) of a single blinking burst.
    *   **Individual Blink Settings (During a Burst):**
        *   `_minIndividualBlinkOffDuration` (float): Minimum time (seconds) the light is dimmed/off during one flicker within a burst.
        *   `_maxIndividualBlinkOffDuration` (float): Maximum time (seconds) the light is dimmed/off.
        *   `_minIndividualBlinkOnDuration` (float): Minimum time (seconds) the light is at full intensity during one flicker within a burst.
        *   `_maxIndividualBlinkOnDuration` (float): Maximum time (seconds) the light is at full intensity.
    *   **Intensity Modulation (During a Burst):**
        *   `_minIntensityFactorDuringBlink` (float, 0-1): Minimum intensity multiplier when the light "blinks off" (0 means completely off).
        *   `_maxIntensityFactorDuringBlink` (float, 0-1): Maximum intensity multiplier when the light "blinks off".
        *   `_restoreOriginalIntensityOnStop` (bool): If true, the light's intensity reverts to its pre-blink value when blinking stops or the component is disabled.
*   **Public API:**
    *   `void SetBlinkingBehavior(bool active)`: Programmatically enables or disables the blinking behavior.
    *   `void TriggerBlinkingBurst()`: Manually starts a blinking burst if conditions allow (blinking active, flashlight on).
*   **Events:**
    *   `event Action OnBlinkBurstStarted`: Invoked when a blinking burst begins.
    *   `event Action OnBlinkBurstEnded`: Invoked when a blinking burst finishes.
    *   `event Action OnBlinkingBehaviorChanged`: Invoked when `IsBlinkingActive` state changes (e.g., via `SetBlinkingBehavior` or Inspector).
*   **Dependencies:** Requires an `IFlashlightCore` component on the same GameObject or a parent.
*   **Modularity:** This component can be removed from the `Flashlight.prefab` if blinking is not needed. To create a different blinking pattern, you can implement `IFlashlightBlinker` in a new script.

#### 3.3. `FlashlightAudio.cs` 🎧

*   **Location:** `Assets/Rivgo/FlashlightSystem/Scripts/FlashlightAudio.cs`
*   **Purpose:** Plays sound effects associated with flashlight actions (switching on/off, blinking).
*   **Inspector Fields:**
    *   `_audioSource` (AudioSource): Reference to the `AudioSource` component used to play sounds. One is already on the `Flashlight.prefab`.
    *   `_switchSounds` (AudioClip[]): An array of audio clips. One will be chosen randomly to play when the flashlight is switched on or off.
    *   `_blinkingSounds` (AudioClip[]): An array of audio clips. One will be chosen randomly to play (and loop) when a blinking burst starts.
*   **Dependencies:** Requires an `AudioSource` component. It listens to events from `IFlashlightCore` and optionally `IFlashlightBlinker` (searches in parent or children for these).
*   **Modularity:**
    *   Customize sounds by assigning different `AudioClip`s to the arrays.
    *   Adjust the `AudioSource` component's properties (e.g., Volume, Spatial Blend, Output Mixer Group).
    *   If no audio is desired, remove this component from the `Flashlight.prefab`.

#### 3.4. `FlashlightInput.cs` ⌨️

*   **Location:** `Assets/Rivgo/FlashlightSystem/Scripts/FlashlightInput.cs`
*   **Purpose:** Acts as a bridge between Unity's New Input System and the flashlight's core logic. It listens for a specific input action and calls the `Switch()` method on the `IFlashlightCore`.
*   **Inspector Fields:** None directly. Configuration happens via the `PlayerInput` component.
*   **Method for Input System:**
    *   `public void OnSwitchFlashlight()`: This method should be linked to the "SwitchFlashlight" `InputAction` event in the `PlayerInput` component.
*   **Dependencies:**
    *   Requires a `PlayerInput` component on the same GameObject.
    *   Requires an `IFlashlightCore` implementation (it searches for one on the same GameObject or its children during `Awake`).
*   **Setup:**
    1.  Ensure the GameObject has a `PlayerInput` component.
    2.  Assign an `InputActionAsset` (like the provided `Rivgo_FPC_InputSystem_Actions.inputactions`) to the `PlayerInput`'s "Actions" field.
    3.  Set `PlayerInput` "Behavior" to "Invoke Unity Events".
    4.  Under "Events > [YourActionMapName, e.g., Player]", find the "SwitchFlashlight" event.
    5.  Add a listener (+) and drag the GameObject with `FlashlightInput.cs` to the object slot.
    6.  Select `FlashlightInput.OnSwitchFlashlight` from the function dropdown.

### 4. Prefabs 🧊

#### 4.1. `Flashlight.prefab`

*   **Location:** `Assets/Rivgo/FlashlightSystem/Flashlight.prefab`
*   **Structure:**
    *   **Root (Flashlight):**
        *   `FlashlightCore.cs`
        *   `FlashlightBlinker.cs`
        *   `FlashlightAudio.cs`
        *   `AudioSource` component
    *   **Child GameObject ("Light"):**
        *   `Light` component (Unity Spotlight, pre-configured for URP)
        *   `UniversalAdditionalLightData` (URP specific light data)
*   **Configuration:** This prefab is ready to use. The `Light` component has settings for intensity, color, range, spot angle, and a slot for a "Cookie" texture. The default cookie `Flashlight_Cookie_05` is assigned.
*   **Usage:** Drag this prefab into your scene, usually as a child of a player's camera or hand. Then, set up input via `FlashlightInput.cs` as described above if not using the Player prefab.

#### 4.2. `= Player =.prefab`

*   **Location:** `Assets/Rivgo/FPC/Prefabs/= Player =.prefab`
*   **Purpose:** A fully functional First-Person Controller ready for use, demonstrating the flashlight integration.
*   **Structure:**
    *   **Root ("= Player ="):**
        *   `CharacterController` component
        *   `PlayerInput` component (Actions: `Rivgo_FPC_InputSystem_Actions`, Behavior: Invoke Unity Events, Default Map: Player)
        *   `SimpleFPC.cs` (handles movement and look)
        *   `FlashlightInput.cs` (linked to `PlayerInput` events for "SwitchFlashlight")
    *   **Child ("Main Camera"):**
        *   `Camera` component (Tag: MainCamera)
        *   `AudioListener` component
        *   `UniversalAdditionalCameraData` (URP specific camera data)
        *   **Grandchild ("Flashlight"):** An instance of the `Flashlight.prefab`, positioned relative to the camera.
*   **Usage:** Drag into your scene for an immediate FPC with a working flashlight.

### 5. Input System Setup (`Rivgo_FPC_InputSystem_Actions.inputactions`) 🕹️

*   **Location:** `Assets/Rivgo/FPC/Rivgo_FPC_InputSystem_Actions.inputactions`
*   **Purpose:** Defines the input actions used by the `SimpleFPC` and `FlashlightInput`.
*   **Key Actions (Player Map):**
    *   `Move` (Value, Vector2): Bound to WASD, arrow keys, gamepad left stick. Used by `SimpleFPC.OnMove()`.
    *   `Look` (Value, Vector2): Bound to mouse delta, gamepad right stick. Used by `SimpleFPC.OnLook()`.
    *   `Jump` (Button): Bound to Spacebar, gamepad south button. Used by `SimpleFPC.OnJump()`.
    *   `SwitchFlashlight` (Button): Bound to 'E' key. Used by `FlashlightInput.OnSwitchFlashlight()`.
*   **UI Map:** Contains standard actions for UI navigation, not directly used by the flashlight system but included for completeness with `PlayerInput`.
*   **Customization:** You can open this asset in the Input Action editor (double-click it) to change bindings, add new actions, or create new control schemes.

### 6. Light Cookies 🍪

*   **Location:** `Assets/Rivgo/FlashlightSystem/LightCookies/`
*   **Files:** `Flashlight_Cookie_01.png` to `Flashlight_Cookie_05.png`.
*   **Usage:** These are 2D textures that can be applied to the "Cookie" slot of a `Light` component (Spotlight type). They shape the light's projection, creating more detailed and realistic light patterns instead of a simple circular cone.
*   **To apply:**
    1.  Select the "Light" child GameObject of the `Flashlight.prefab`.
    2.  In the Inspector, find the `Light` component.
    3.  Drag one of the cookie textures (or your own) into the "Cookie" slot.
    4.  Ensure your texture is imported with "Texture Type" set to "Cookie" and "Alpha Source" set to "From Gray Scale" (or as appropriate for your cookie texture). The provided cookies are already configured.

### 7. Demo Scene (`DEMO_Flashlight_System.unity`) 🎬

*   **Location:** `Assets/Rivgo/FlashlightSystem/Demo/Scenes/DEMO_Flashlight_System.unity`
*   **Contents:** This scene includes:
    *   An instance of the `= Player =.prefab`.
    *   A simple environment made of cubes (`= Map =` GameObject).
    *   A Directional Light.
    *   An EventSystem for UI (though UI elements related to controls are simple world-space text).
    *   Basic lighting setup for URP.
*   **Purpose:** To provide a working example of the flashlight system integrated with the FPC. You can play this scene to test the flashlight ('E' key), movement (WASD), jump (Space), and camera look (mouse).

### 8. Extensibility and Customization 🎨

*   **Changing Light Properties:** Directly modify the `Light` component on the "Light" child of `Flashlight.prefab`. Adjust intensity, color, range, spot angle, shadow settings, or assign a new cookie.
*   **Custom Blinking Logic:** Create a new C# script that implements the `IFlashlightBlinker` interface. You can then replace the `FlashlightBlinker.cs` component on the `Flashlight.prefab` with your custom script.
*   **Custom Core Flashlight Logic:** If you need features like battery consumption, multiple light modes, or different on/off mechanisms, create a new C# script implementing `IFlashlightCore`. Replace `FlashlightCore.cs` on the prefab with your script. Ensure other components like `FlashlightBlinker` and `FlashlightAudio` can still find your new core component (they search on the same GameObject, then parents, then children).
*   **Modifying Input:**
    *   Change keybindings by editing the `Rivgo_FPC_InputSystem_Actions.inputactions` asset.
    *   For a completely different input scheme, you can remove `FlashlightInput.cs` and trigger the `_flashlightService.Switch()` (or `TurnOn()`/`TurnOff()`) methods from your own input handling scripts.
*   **Disabling Modules:**
    *   **No Blinking:** Remove the `FlashlightBlinker` component from `Flashlight.prefab`.
    *   **No Audio:** Remove the `FlashlightAudio` and `AudioSource` components from `Flashlight.prefab`.
*   **Using Without Provided FPC:** The `Flashlight.prefab` is self-contained. You can attach it to your own character controller. You will need to:
    1.  Add the `FlashlightInput.cs` script to your player (or the flashlight prefab itself).
    2.  Set up a `PlayerInput` component on your player, configure it with an `InputActionAsset` that includes an action for toggling the flashlight (like "SwitchFlashlight").
    3.  Link this action's event to the `FlashlightInput.OnSwitchFlashlight()` method.

### 9. Troubleshooting / FAQ 🤔

*   **Flashlight not turning on/off with 'E' key:**
    *   Ensure the **Input System package** is installed.
    *   Verify the `PlayerInput` component on your player is correctly configured:
        *   "Actions" field has `Rivgo_FPC_InputSystem_Actions` assigned.
        *   "Behavior" is "Invoke Unity Events".
        *   The "SwitchFlashlight" event under "Events > Player" is linked to `FlashlightInput.OnSwitchFlashlight()`.
    *   Check that the `FlashlightInput` script can find an `IFlashlightCore` (usually `FlashlightCore.cs`) on itself or a child (like the Flashlight prefab).
    *   Make sure the `Light` component within the `Flashlight.prefab` is not disabled by other means.
*   **No flashlight sounds:**
    *   Check if `FlashlightAudio.cs` and an `AudioSource` component are present on `Flashlight.prefab`.
    *   Ensure `AudioClip`s are assigned to `_switchSounds` and `_blinkingSounds` in `FlashlightAudio`.
    *   Verify the `AudioSource` volume is not zero and it's not muted.
    *   Check if there's an `AudioListener` in your scene (the demo player prefab has one on its camera).
*   **Blinking effect not working:**
    *   Ensure `FlashlightBlinker.cs` is on `Flashlight.prefab`.
    *   Check that "Is Blinking Active" is true on `FlashlightBlinker`.
    *   The flashlight must be ON for blinking to occur.
    *   Review the interval and duration settings; they might be too long/short.
*   **Light looks like a simple circle:**
    *   Ensure a "Cookie" texture is assigned to the `Light` component on `Flashlight.prefab -> Light`.
    *   Make sure the cookie texture is imported with "Texture Type: Cookie".
*   **FPC not moving or looking:**
    *   Verify `PlayerInput` bindings for "Move" and "Look" actions are correct for your input devices (Keyboard/Mouse, Gamepad).
    *   Ensure `SimpleFPC.cs` is enabled.
    *   Check `_cameraTransform` is assigned in `SimpleFPC`.
    *   Make sure no other script or component is interfering with character control or camera rotation.