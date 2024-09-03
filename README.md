# Slime Hunter

![](https://img.itch.zone/aW1nLzE3NTExMTE2LnBuZw==/original/MIHxmP.png)

## Getting Started

### Installation

1. Clone the Repository:

   ```bash
   git clone https://github.com/Studio-Bounce/slime-hunter.git
   ```

2. Open the project in `Unity 2022.3.27f1`.

### Adding FMOD Plugin

> **Note:** While FMOD is not strictly required to run the game, the absence of FMOD will result in the game lacking sound. For development purposes, we **highly** recommend installing FMOD. Without it, references to FMOD components will be hidden, which can lead to unintended changes in existing audio configurations.

To add FMOD, follow these steps:

1. Download FMOD from [FMOD for Unity](https://www.fmod.com/download#fmodforunity), selecting version `2.02.22 (Unity Verified)`.
2. With the Unity project open, execute the plugin package and import FMOD to the project. The FMOD files should be added to `Assets/Plugins/FMOD` by default.
3. After importing the files, the **FMOD Setup Wizard** should open.

> **Note:** Make sure to refresh any open scenes in Unity to ensure all existing references are linked with the plugin.

#### Setup Wizard

You can skip all the steps in the setup wizard except for `Linking`.

> **Tip:** If you don't see the setup wizard, there should be a new menu item at the top labeled `FMOD`. From there, go to `Setup Wizard`.

- **Linking:**
  - Select `Single Platform Build` and open the `Desktop` build folder located at `/Assets/Game/Audio/FMOD/Build/Desktop/`.

---

And you're done!

## Contributing

1. Create a new branch off of `develop`:

   ```bash
   git checkout develop
   git checkout -b feat-new-change
   ```

2. Make your changes.
3. Add, commit, and push your changes.
4. Squash and merge to `develop`.

### Naming Conventions

Branches should follow this naming convention: `<branch-type>-<description>`

The first word should indicate the type of branch (fix, feature, docs, test, etc.). Use lower-case and kebab-case for clarity (separate each word with a hyphen).
