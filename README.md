# Miku Miku Library

Format library and file editor for Hatsune Miku: Project DIVA games.

# Building

* [Stable (release) builds](https://github.com/blueskythlikesclouds/MikuMikuLibrary/releases)
* [Unstable (development) builds](https://ci.appveyor.com/project/blueskythlikesclouds/mikumikulibrary/build/artifacts)

## Manually building

1. Clone the repository with the `--recursive` option. `git clone --recursive https://github.com/blueskythlikesclouds/MikuMikuLibrary.git`
2. Install FBX SDK. (See instructions [here.](https://github.com/blueskythlikesclouds/MikuMikuLibrary/tree/master/MikuMikuLibrary.Native/Dependencies/FBX))
3. Install the .NET SDK/.NET 7.0 Runtime through Visual Studio Installer.
4. Open the solution in Visual Studio 2022.
5. Restore the missing NuGet packages.
6. Build the solution.

# Projects

## Miku Miku Library

This is the main library of the solution, providing methods and classes to read, edit and write file formats from Hatsune Miku: Project DIVA games.

## Miku Miku Model

A GUI front-end of the library that allows you to work with models, textures, motions and sprites.

## Command line tools

These are command line front-ends for certain functionalities of the library.

### Database Converter

A program that allows you to convert database files to XML or vice versa.

Supported files:

* aet_db.bin/.aei
* bone_data.bin/.bon
* mot_db.bin
* obj_db.bin/.osi
* spr_db.bin/.spi
* stage_data.bin/.stg
* str_array.bin/string_array.bin/.str
* tex_db.bin/.txi

### FARC Pack

A program that allows you to extract or create FARC files. MM+ CPK files are also supported.

# Special thanks

* [ActualMandM](https://github.com/ActualMandM)
* [BroGamer4256](https://github.com/BroGamer4256)
* [Brolijah](https://github.com/Brolijah)
* [Charl-Ep](https://github.com/Charl-Ep)
* [chrrox](https://www.deviantart.com/chrrox)
* [featjinsoul](https://github.com/featjinsoul)
* [keikei14](https://github.com/keikei14)
* [korenkonder](https://github.com/korenkonder)
* [lybxlpsv](https://github.com/lybxlpsv)
* [minmode](https://www.deviantart.com/minmode)
* [nastys](https://github.com/nastys)
* [s117](https://github.com/s117)
* [samyuu](https://github.com/samyuu)
* [Stewie100](https://github.com/Stewie100)
* [thtrandomlurker](https://github.com/thtrandomlurker)
* [Waelwindows](https://github.com/Waelwindows)