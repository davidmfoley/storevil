ECHO OFF

set dest=%1

if not (%1) == () goto :install
  ECHO Please supply a destination 
  ECHO For example, install_storevil C:\StorEvil

goto :eof

:install 

if not exist %1 mkdir %1\
copy StorEvil.Core.DLL %1\
copy StorEvil.exe %1\
copy Funq.DLL %1\
copy Spark.dll %1\
copy StorEvil.Assertions.DLL %1\

ECHO installed StorEvil to %1
ECHO To uninstall StorEvil, delete this directory