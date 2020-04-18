
copy ..\bin\Release\vk\netstandard1.4\vk.dll .\lib\netstandard1.4
copy ..\bin\Release\vk\netstandard1.4\vk.pdb .\lib\netstandard1.4

copy ..\bin\Release\vk.uwp\netstandard1.4\vk.dll .\lib\uap10.0
copy ..\bin\Release\vk.uwp\netstandard1.4\vk.pdb .\lib\uap10.0

del Vk.*.nupkg

nuget pack Vk.nuspec

