# Slime Hunter

## Getting Started

### Installation

1. Clone the Repo

`git clone https://github.com/Studio-Bounce/slime-hunter.git`

2. Open the project in `Unity 2022.3.27f1`

### Adding FMOD Plugin

Goto [FMOD for Unity](https://www.fmod.com/download#fmodforunity) and download version `2.02.22` (Unity Verified).
With the Unity Project open, execute the plugin package and import FMOD to the project.

FMOD files should be added to `Assets/Plugins/FMOD`

After Importing the files, the **FMOD Setup Wizard** should open.

#### Setup Wizard

You can skip through all the steps in the setup wizard except for `Linking`.
> If you don't see the setup wizard, there should be a new menu item at the top labelled `FMOD`. From there goto `Setup Wizard`.

- Linking
  - Select `FMOD Studio Project` and open up the file in `Assets/Game/Audio/FMOD/slime-hunter-audio.fspro`

And you're done!

## Contributing

1. Create a new branch off of `develop`

`git checkout develop`
`git checkout -b feat-new-change`

2. Make your changes

3. Add-Commit-Push your changes

4. Squash and merge to develop

### Naming Conventions

`<branch-type>-<description>`

The first word should indicate the type of branch this will be (fix, feature, docs, test, etc.).  Use lower-case and kebab-case for clarity (separate each word with a hyphen - )
