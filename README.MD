# MUPS
Created by Wampa842

## Contents
- [Overview](#overview)
- [Features](#features)
- [Supported file types](#supported-file-types)
- [Planned features](#planned-features)
- [Credits and license](#credits-and-license)

## Overview
MUPS tries to replicate what MMD does in Unity's environment.

## Features
- Skeleton-based posing
- Morphs
- PBR and toon-shaded rendering
- Multiple stored frames

## Supported file types
Model files:
|File|Support|Features
|---|---|---
|PMX 2.0|Yes|Everything
|PMX 2.1|Partial|Only the features present in PMX 2.0
|PMD|No|
|X|Planned|
|OBJ|Planned|
|FBX|Planned|Static mesh only

Image files:
|File|Support|Features
|---|---|---
|PNG|Yes|
|JPEG|Yes|
|DDS|Yes|
|TGA|Planned|
|EXR|No|
|HDR|Planned|RGBE format

Other files
|File|Support|Features
|---|---|---
|MUPS|Native|Saved scene data, JSON format
|Unity asset bundle|Planned|Additional content that cannot be loaded/compiled in Unity at runtime (materials, shaders, etc)

## Planned features
Near future:
- More morph types
- Support for planned file types

Distant future:
- Animation
- VPD read/write
- VMD read/write

## Credits and license
- 