rem @echo off
set FRAMEWORK_PATH=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%FRAMEWORK_PATH%;

msbuild /nologo src/embedly-dotnet.sln /p:Configuration=Release /t:Clean
msbuild /nologo src/embedly-dotnet.sln /p:Configuration=Release /p:TargetFrameworkVersion=4.0

pkg\nuget Pack pkg\embedly-dotnet.nuspec
pkg\nuget Pack pkg\embedly-dotnet.mongodb.nuspec