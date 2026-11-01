#!/bin/sh
# Build with Mono's mcs against the game's own assemblies (adjust paths).
M="/path/to/Valheim/valheim_Data/Managed"; R="/path/to/BepInEx/core"
mcs -noconfig -nostdlib -target:library -out:RecipeScrollFix.dll -nowarn:0436,1701 \
  -r:$M/mscorlib.dll -r:$M/netstandard.dll -r:$M/System.dll -r:$M/System.Core.dll \
  -r:$M/UnityEngine.dll -r:$M/UnityEngine.CoreModule.dll -r:$M/UnityEngine.UI.dll \
  -r:$M/UnityEngine.UIModule.dll -r:$M/Unity.TextMeshPro.dll -r:$M/assembly_valheim.dll \
  -r:$M/assembly_utils.dll -r:$M/assembly_guiutils.dll -r:$M/SoftReferenceableAssets.dll \
  -r:$M/Splatform.dll -r:$R/BepInEx.dll -r:$R/0Harmony.dll RecipeScrollFix.cs
