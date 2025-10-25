# Project Suggestions

- Add lighting to the scenes.
- Have laser pointers always visible to the user to make it easier to tell what controller is pointing at.
    - Simply toggle laser pointers always active when keyboard is active instead? (Pre-warn users)
- Never a good idea to translate player when in VR. Teleport them only to move unless the user is the one in controller.
- Clean project of unused assets (scripts, meshes, textures, etc.).
- Fix scaling issues (especially present in hospital room as bed appears massive).
- Place manager/singleton scripts into a separate scene and load this scene additively instead of adding every object to Unity's `DontDestroyOnLoad`. This will make it so that scripts do not have to use `FindObjectOfType` as much which can be expensive, especially with lots of objects in the loaded scenes.
- Refactor singleton architecture.
- Cleanup project prefabs. For example, there is an excessive amount of Module prefabs. These prefabs could likely be condensed down into one or several prefabs, greatly cleaning the project.

