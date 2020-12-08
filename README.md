# Miku Miku Library
Format library and file editor for Hatsune Miku: Project DIVA games.

# Building
* [Stable (release) builds](https://github.com/blueskythlikesclouds/MikuMikuLibrary/releases)
* [Unstable (development) builds](https://ci.appveyor.com/project/blueskythlikesclouds/mikumikulibrary/build/artifacts)

## Manually building
1. Open the solution in Visual Studio 2019 (or later).
2. Restore missing NuGet packages.
3. Install FBX SDK. (See instructions [here.](https://github.com/blueskythlikesclouds/MikuMikuLibrary/tree/master/MikuMikuLibrary.Native/Dependencies/FBX))
4. Build the solution.

# Projects
## Miku Miku Library
This is the main library of the solution, providing methods and classes to read, edit and write file formats from Hatsune Miku: Project DIVA games.

## Miku Miku Model
A GUI front-end of the library that allows you to work with models, textures, motions and sprites.

## Command line tools
These are command line front-ends for certain functionalities of the library.

### Database Converter
A program that allows you to convert database files to .xml or vice versa.

Supported files:
* aet_db.bin/.aei
* bone_data.bin/.bon
* mot_db.bin
* obj_db.bin/.osi
* spr_db.bin/.spi
* stage_data.bin
* str_array.bin/string_array.bin/.str
* tex_db.bin/.txi

### FARC Pack
A program that allows you to unpack or repack .farc files.

# Special thanks
* [Brolijah](https://github.com/Brolijah)
* [chrrox](https://www.deviantart.com/chrrox)
* [keikei14](https://github.com/keikei14)
* [korenkonder](https://github.com/korenkonder)
* [lybxlpsv](https://github.com/lybxlpsv)
* [minmode](https://www.deviantart.com/minmode)
* [nastys](https://github.com/nastys)
* [s117](https://github.com/s117)
* [samyuu](https://github.com/samyuu)
* [Stewie1.0](https://github.com/Stewie100)
* [Waelwindows](https://github.com/Waelwindows)