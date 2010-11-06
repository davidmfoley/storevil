pushd "%~dp0"

set RSRoot=%USERPROFILE%\AppData\Local\JetBrains\ReSharper\v5.1

set RS10=%RSRoot%\vs10.0
if not exist "%RS10%\" goto :SkipVS10

set StorEvilDest10=%RSRoot%\vs10.0\Plugins\StorEvil

if exist "%StorEvilDest10%" del /Q "%StorEvilDest10%\*.*"
if not exist "%StorEvilDest10%" mkdir "%StorEvilDest10%"

copy StorEvil.ReSharper.dll "%StorEvilDest10%\"
copy StorEvil.Core.dll "%StorEvilDest10%\"
copy Spark.dll "%StorEvilDest10%\"
copy Funq.dll "%StorEvilDest10%\"

:SkipVS10

if not exist "%RSRoot%\vs9.0" goto :eof

set StorEvilDest9=%RSRoot%\vs9.0\Plugins\StorEvil

if exist "%StorEvilDest9%\*.*" del /Q "%StorEvilDest9%\*.*"
if not exist "%StorEvilDest9%" mkdir "%StorEvilDest9%"

copy StorEvil.ReSharper.dll "%StorEvilDest9%\"
copy StorEvil.Core.dll "%StorEvilDest9%\"
copy Spark.dll "%StorEvilDest9%\"
copy Funq.dll "%StorEvilDest9%\"

popd