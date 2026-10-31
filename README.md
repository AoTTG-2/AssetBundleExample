# AoTTG2 Asset Bundle Example

This Unity project builds **custom asset bundles** for the AoTTG2 map editor. Custom assets let you add your own prefabs (meshes, textures, materials, sounds, animations, particles, lights, and so on) to a map, and other players load them automatically when they join.

The project is already set up to match the game: same Unity version, same Universal Render Pipeline (URP) package and render settings. Use it as your starting point instead of a blank project, otherwise your assets may render differently or not at all in-game.

Upgrading from the old (Unity 2023.1) version of this project? See [Upgrading from the old project](#upgrading-from-the-old-project).

## Requirements

- **Unity 6000.3.27f2**, installed through [Unity Hub](https://unity.com/download) (or with the Install button on its [release page](https://unity.com/releases/editor/whats-new/6000.3.27f2)). Use this exact version: asset bundles built with a newer Unity than the game can't be loaded.
- In Unity Hub, add these modules to the editor for each platform you want to support:
  - **Mac Build Support**
  - **Linux Build Support**
  - **Android Build Support**

  Windows support is included by default. Platforms without their module are skipped when building.

  Either the Mono or the IL2CPP version of a module works. Asset bundles contain no code, so the scripting backend doesn't matter. Unity Hub on Windows only offers Mac Build Support (Mono), and that's fine.

## Your first asset bundle

1. Download this project: <https://github.com/AoTTG-2/AssetBundleExample>
2. In Unity Hub, click **Add** and select the project folder, then open it with Unity 6000.3.27f2.
3. In the menu bar, click **Assets → Build AssetBundles** and choose a platform, for example **Windows**. You can also right-click the Assets folder in the Project window and use the same menu.

   Building can take a while, especially the first time, because every platform compiles its own shaders. Build only the platforms you're targeting. **All Platforms**, at the bottom of the menu, builds every platform in one go.
4. When the build finishes, the output folder opens automatically. The bundles are in the `AssetBundles` folder inside the project folder (next to `Assets`, not inside it), with one folder per platform:

   ```
   AssetBundles/
     Windows/example
     Mac/example
     Linux/example
     Android/example
   ```

The file named `example`, with no extension, is your asset bundle. Unity's extra build files (`.manifest` files and a file named after the platform folder) aren't used by the game, so they're deleted automatically and only your bundles are left.

## Using asset bundles in the map editor

1. Copy the bundle for **your own platform**, for example `AssetBundles/Windows/example`, to your custom assets folder:
   - Windows and Mac: `Documents/Aottg2/CustomAssets`
   - Linux: `~/.local/share/Aottg2/CustomAssets`
2. Open the map editor and select **Options → Custom Assets**.
3. Type the bundle name (`example`) and click **Add**.
4. The bundle's prefabs (here, ExampleCube and ExampleSphere) now show up in **Add Object → Custom**, and you can place them like any other object.

The map now depends on this bundle. To open or edit the map later, a file with the same name has to be in your CustomAssets folder (or available through a URL, see below).

## Letting other players load your assets

Other players need the bundle too. There are two ways to provide it.

### Share the file

Send players the bundle for their platform and have them put it in their CustomAssets folder. This works, but every player has to do it manually.

### Host it online (recommended)

Upload the bundle files to a web host, then paste the links into the map editor:

1. In the map editor, open **Options → Custom Assets**.
2. Choose a platform in the **Platform** dropdown and paste the link for that platform's bundle into the field next to the bundle name:
   - **Default**: used by any platform that doesn't have its own link. Put the **Windows** bundle here, since most players are on Windows.
   - **Mac**, **Linux**, **Android**: the bundles from the matching `AssetBundles` folders.
3. Players download the bundle when they join and load the map. They're asked to confirm the download first.

A bundle only works on the platform it was built for. A Mac player who downloads the Windows bundle can't load it, so give each platform you support its own link.

Download links must:

- Start with `https://`.
- **Download the file directly, without redirecting.** Many file hosts send share links through one or more redirects (for example Dropbox `?dl=1` links), and those fail. To check a link, run `curl -I <link>`: the first line should be `200`, not `301` or `302`.
- Download the raw bundle file, **not** a `.zip` or any other archive.
- Point to a file no larger than 1 GB.

The name of the uploaded file doesn't matter. The game saves the download under the bundle name from the map editor.

## Adding your own prefabs

All your content goes in the `Assets/AssetBundle` folder. `Assets/AssetBundle/Prefabs` contains the two example prefabs, ExampleCube and ExampleSphere.

1. Create your prefab (drag a GameObject from the scene into the Prefabs folder).
2. Select the prefab. At the bottom of the Inspector there's an **AssetBundle** dropdown: set it to your bundle name.
3. Build again with **Assets → Build AssetBundles** for the platforms you need.

Only **prefabs** show up in the map editor. Textures, materials, meshes, sounds and animations are included automatically when a prefab uses them.

**Meshes must have Read/Write enabled.** Select the model (`.fbx`, `.obj`, ...) and in the Inspector's **Model** tab, enable **Read/Write**, then click **Apply**. The game combines meshes when loading maps and can't do that with unreadable meshes.

## Naming your asset bundle

Every bundle in a map needs a unique name, so don't release your content as `example`. Pick something specific to you, such as `myusernamecity1assets`, and avoid generic names like `cityassets` or `wagons`.

To name your bundle:

1. Select one of your prefabs.
2. In the **AssetBundle** dropdown at the bottom of the Inspector, click **New** and type the name.
3. Assign the same name to all the prefabs that belong in the bundle.

Unity always makes bundle names lowercase, and the built file uses that name. Type it in the map editor exactly as the file is named.

**Name bundles in Unity instead of renaming the built file.** A bundle keeps its original name internally. Two bundles that were both built as `example` can conflict when a map loads them together, even if the files were renamed.

**Names are permanent once used in a map.** Maps store the bundle name and each prefab's name. Renaming either later breaks every map that uses them, so choose your names before placing objects.

## Materials and shaders

The game renders with the Universal Render Pipeline (URP), and this project uses the same URP setup, so what you see in the Scene view matches the game.

- Use URP shaders: **Universal Render Pipeline/Lit**, **Simple Lit**, **Unlit**, the **Particles** shaders, or your own **Shader Graph** shaders.
- Shaders from Unity's old built-in pipeline (Standard, Legacy Shaders, Mobile, and older custom surface shaders) don't work in URP. When the game loads a bundle that uses them, it swaps them for the closest basic URP material. Only the color, main texture, tiling and cutout are kept, so convert them yourself for the best result (see [Upgrading from the old project](#upgrading-from-the-old-project)).
- In the map editor, the **Default** material keeps your prefab's own materials and only tints their main color. The other map materials (Basic, Transparent, and so on) replace your materials entirely.

## Allowed components

For security, custom prefabs can only contain the components below. Prefabs with any other component, including **all scripts**, fail to load. Animation events are removed when the bundle loads.

| Category | Components |
| --- | --- |
| Basics | Transform, MeshFilter, Renderer (MeshRenderer, SkinnedMeshRenderer, SpriteRenderer, ...), LODGroup, SortingGroup, TextMesh |
| Physics | Collider (all types), Rigidbody, ArticulationBody, CharacterController, Joint (all types), ConstantForce, Cloth |
| Animation | Animation, Animator |
| Audio | AudioSource, AudioListener, AudioReverbZone, audio filters (Chorus, Distortion, Echo, High Pass, Low Pass, Reverb) |
| Effects | ParticleSystem, ParticleSystemForceField, LineRenderer, TrailRenderer, Lens Flare (SRP), WindZone |
| Lighting | Light (with URP's Universal Additional Light Data), ReflectionProbe, LightProbeGroup, Volume |
| Other | Terrain, VideoPlayer, Skybox, OcclusionArea, OcclusionPortal |

The legacy **Lens Flare**, **Projector** and **Light Probe Proxy Volume** components are allowed, but URP doesn't render them. Use **Lens Flare (SRP)** instead of the legacy lens flare.

If a VideoPlayer streams from a URL, players are asked to confirm the URL before it plays.

## Building from the command line

You can also build without opening the editor, for example in a script:

```bash
"<path to Unity 6000.3.27f2>/Unity.exe" -batchmode -quit -projectPath "<path to this project>" -executeMethod AssetBundleBuilder.BuildAll
```

To build a single platform, replace `BuildAll` with `BuildWindows`, `BuildMac`, `BuildLinux` or `BuildAndroid`. If a build fails, Unity exits with an error code.

## Upgrading from the old project

AoTTG2 moved from Unity 2023.1 and the built-in render pipeline to **Unity 6000.3.27f2** with the **Universal Render Pipeline (URP)**. This section explains what that means for content you made with the old version of this project, and how to bring it over.

### Do I have to migrate?

Your existing maps and bundles keep working: the game still loads asset bundles built with the old project. But materials using the old built-in shaders (Standard, Legacy Shaders, Mobile, and older custom surface shaders) can't render in URP, so the game automatically swaps them for the closest basic URP material:

| Old material | Becomes |
| --- | --- |
| Opaque (Standard, Diffuse, ...) | Basic lit material |
| Cutout (Standard Cutout, Transparent/Cutout/...) | Basic lit cutout material |
| Fade / Transparent | Basic lit transparent material |
| Particles | Unlit particle material (alpha blended or additive) |
| Terrain | The game's terrain material |

Only the **color**, **main texture** (with its tiling and offset) and **cutout threshold** carry over. Normal maps, metallic and smoothness, emission, specular, detail maps and custom shader effects are lost. Built-in unlit shaders and the legacy particle shaders already work in URP and are left as they are.

Old bundles are also **Windows only**, so players on Mac, Linux and Android can't load them.

Migrating gives you back the full look of your materials and lets you build for every platform.

### Step by step

#### 1. Install Unity 6000.3.27f2

Install **6000.3.27f2** through [Unity Hub](https://unity.com/download), and add **Mac Build Support**, **Linux Build Support** and **Android Build Support** for the platforms you want to support (see [Requirements](#requirements)).

#### 2. Back up your old project

Copy your whole old project folder somewhere safe before changing anything.

#### 3. Get the new project and copy your content into it

Don't open your old project in Unity 6. The upgrade wouldn't set up URP to match the game. Instead:

1. Download the new project: <https://github.com/AoTTG-2/AssetBundleExample>
2. Copy **your own folders** from the old project's `Assets` folder into the new project's `Assets` folder, together with their `.meta` files. The `.meta` files keep the links between your prefabs, materials and textures.
3. Don't copy the old `ProjectSettings`, `Packages` or `Library` folders. The new project's settings match the game and must stay as they are.
4. Open the new project in Unity 6000.3.27f2 and wait for the import to finish.

Your prefabs keep their **AssetBundle** names, so they still build into bundles with the same names.

#### 4. Convert your materials

Anything that renders magenta is still using a built-in shader.

1. Open **Window → Rendering → Render Pipeline Converter**.
2. Select **Built-in to URP** in the dropdown.
3. Enable **Material Upgrade** and **Read-only Material Converter**.
4. Click **Initialize Converters**, review the list, then click **Convert Assets**.

This converts Standard, Legacy, Mobile and particle materials to their URP equivalents while keeping textures, normal maps, emission and other settings.

#### 5. Fix what the converter can't

- **Custom shaders.** Surface shaders and lit shaders written for the built-in pipeline don't work in URP. Recreate them in **Shader Graph** (Create → Shader Graph → URP), or switch their materials to a URP shader. Simple unlit shaders without lighting usually still work.
- **Lens flares.** Replace legacy **Lens Flare** components with **Lens Flare (SRP)**.
- **Projectors.** URP doesn't render the legacy **Projector** component, so remove it.
- **Lights.** Point and spot lights fall off the same way as before, but intensity is now applied in linear space, like in the game. Lights with an intensity other than 1 can look brighter or dimmer, so check them and adjust if needed.
- **Terrain.** Assign the URP terrain material (Universal Render Pipeline/Terrain/Lit) in the terrain's settings if the converter didn't.

Check every prefab in the Scene view: it looks the same as it will in-game.

#### 6. Build for all platforms

Click **Assets → Build AssetBundles** and choose the platforms you're targeting, or **All Platforms** at the bottom of the menu. The bundles are now in the `AssetBundles` folder inside your project folder, with one folder per platform. The old project used to put a single Windows bundle in `Assets/Build` instead. See [Your first asset bundle](#your-first-asset-bundle) for details.

#### 7. Update your maps

Your bundle and prefab names haven't changed, so your maps still find the right objects. You only need to replace the bundle files:

- **Local files:** replace the old bundle in your CustomAssets folder with the new one for your platform.
- **Hosted files:** upload the new bundles and update the links in the map editor under **Options → Custom Assets**. Put the Windows bundle in **Default** and add the Mac, Linux and Android links in their **Platform** fields. If you upload the new Windows bundle to the same link as before, maps using it pick it up without being edited.

Players download hosted bundles again every time they start the game, so the new version reaches them right away.
