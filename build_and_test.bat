pushd "%~dp0"

msbuild default.build /t:All %1

popd