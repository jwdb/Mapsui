SETLOCAL
SET VERSION=%1
SET NUGET=.\Tools\NuGet\nuget.exe

rmdir obj /s /q
rmdir Release /s /q
msbuild Scripts\updateversionnumber.proj /p:AsmVersion=%VERSION%  
msbuild Scripts\build.proj
%NUGET% pack .\Scripts\Mapsui.nuspec -Version %VERSION% -outputdirectory Release
%NUGET% pack .\Scripts\Mapsui.Forms.nuspec -Version %VERSION% -outputdirectory Release