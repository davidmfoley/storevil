pushd "%~dp0"

msbuild default.build /t:UnInstallResharper %1

popd