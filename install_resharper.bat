pushd "%~dp0"

msbuild default.build /t:InstallResharper %1

popd