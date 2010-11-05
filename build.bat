pushd "%~dp0"

msbuild default.build /t:Package %1

popd