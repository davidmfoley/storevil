pushd "%~dp0"

set RSRoot=%USERPROFILE%\AppData\Local\JetBrains\ReSharper\v5.1

set StorEvilDest10=%RSRoot%\vs10.0\Plugins\StorEvil
if exist "%StorEvilDest10%" rd /S /Q "%StorEvilDest10%"

set StorEvilDest9=%RSRoot%\vs9.0\Plugins\StorEvil
if exist "%StorEvilDest9%\*.*" rd /S /Q "%StorEvilDest9%"

popd