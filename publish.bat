@echo off

msbuild -p:Configuration=Release -p:Platform=x64 MikuMikuLibrary.Native\MikuMikuLibrary.Native.vcxproj
call :publish CliTools DatabaseConverter
call :publish CliTools FarcPack
call :publish . MikuMikuModel
call :7z x86
call :7z x64
exit /b

:publish
call :pub x86 %1 %2
call :pub x64 %1 %2
exit /b

:pub
dotnet publish --runtime win-%~1 --no-self-contained --output publish\%~1 --configuration Release -p:PublishSingleFile=true -p:PublishReadyToRun=true %~2\%~3\%~3.csproj
exit /b

:7z
del publish\%~1\*.pdb
7z a -t7z -m0=lzma2 -mx=9 -mfb=64 -md=1024m -ms=on publish\MikuMikuLibrary-%~1.7z .\publish\%~1\*
exit /b