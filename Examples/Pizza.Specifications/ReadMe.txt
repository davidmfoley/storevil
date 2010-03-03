This is an example set of StorEvil specifications for an imaginary Pizza business.
 
In order to execute the specs, make sure you have StorEvil built and in your path. Then, build the Pizza.TestContext project (you can just use VS to do this).

Once that is built, you can run the following command...
  storevil execute 

... which should run all of the specs. 

It will use the settings in the storevil.config file in this folder, as long as you are in this folder or a sub folder... StorEvil searches up from the current directory until it finds a storevil.config file. (So you can navigate to a subfolder and execute only the specs in that folder).

The storevil.config in this folder points to the Pizza.TestContext.dll in the Pizza.TestContext\bin\Debug folder.

You can also use StorEvil to generate NUnit fixtures:
  storevil nunit Generated.cs
... this will generate NUnit fixtures in the file Generated.cs, in this folder.





